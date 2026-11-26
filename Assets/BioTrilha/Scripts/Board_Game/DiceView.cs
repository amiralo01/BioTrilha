using System.Collections;
using UnityEngine;

namespace BioTrilha.Board
{
    /// <summary>
    /// Animação do dado 3D: sobe, gira e cai mostrando a face sorteada para cima.
    /// O número vem do <see cref="DiceRoller"/>; esta classe só mostra.
    /// <c>faceUpRotations[n - 1]</c> é a rotação em que a face n fica para cima (+Y).
    /// </summary>
    public class DiceView : MonoBehaviour
    {
        [Tooltip("Rotação local (Euler) que deixa cada face (1 a 6) virada para cima.")]
        [SerializeField] private Vector3[] faceUpRotations =
        {
            // No modelo Dice.fbx: 1 = +X, 2 = +Y, 3 = -Z, 4 = +Z, 5 = -Y, 6 = -X.
            new Vector3(0f, 0f, 90f),
            new Vector3(0f, 0f, 0f),
            new Vector3(90f, 0f, 0f),
            new Vector3(-90f, 0f, 0f),
            new Vector3(180f, 0f, 0f),
            new Vector3(0f, 0f, -90f),
        };

        [SerializeField] private float rollDuration = 0.9f;
        [SerializeField] private float jumpHeight = 0.6f;
        [SerializeField] private float spinsPerSecond = 2.5f;

        private Vector3 restPosition;
        private Rigidbody body;

        public bool IsRolling { get; private set; }

        private void Awake()
        {
            restPosition = transform.localPosition;
            body = GetComponent<Rigidbody>();
        }

        /// <summary>Gira o dado e termina com <paramref name="value"/> para cima.</summary>
        public IEnumerator Roll(int value)
        {
            IsRolling = true;
            bool wasKinematic = body != null && body.isKinematic;
            if (body != null) body.isKinematic = true;

            // Giro aleatório em torno do eixo vertical do mundo: a face sorteada continua para cima.
            Quaternion final = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                               * Quaternion.Euler(faceUpRotations[Mathf.Clamp(value, 1, 6) - 1]);
            Vector3 spinAxis = Random.onUnitSphere;
            float spinTime = rollDuration * 0.75f;

            for (float t = 0f; t < rollDuration; t += Time.deltaTime)
            {
                float k = t / rollDuration;
                transform.localPosition = restPosition + Vector3.up * (4f * jumpHeight * k * (1f - k));
                if (t < spinTime)
                    transform.Rotate(spinAxis, spinsPerSecond * 360f * Time.deltaTime, Space.World);
                else
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, final, Time.deltaTime * 18f);
                yield return null;
            }

            transform.localPosition = restPosition;
            transform.localRotation = final;
            if (body != null) body.isKinematic = wasKinematic;
            IsRolling = false;
        }

        /// <summary>Mostra a face sem animação (usado para calibrar a tabela no editor).</summary>
        public void ShowFace(int value)
        {
            transform.localRotation = Quaternion.Euler(faceUpRotations[Mathf.Clamp(value, 1, 6) - 1]);
        }
    }
}
