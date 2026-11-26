using System.Collections;
using UnityEngine;

namespace BioTrilha.Pawns
{
    /// <summary>
    /// Faz o peão pular de casa em casa. Vai no objeto raiz do peão (Peao_*), que tem o pivô na base.
    /// Durante a partida o Rigidbody fica cinemático: a posição é sempre a da casa (y = 0 é a superfície)
    /// e peões que dividem uma casa não se empurram pela física.
    /// </summary>
    public class PawnMover : MonoBehaviour
    {
        [SerializeField] private float hopDuration = 0.28f;
        [SerializeField] private float hopHeight = 0.35f;
        [SerializeField] private float settleDuration = 0.15f;

        private Rigidbody body;
        private Vector3 baseScale;

        public bool IsMoving { get; private set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            baseScale = transform.localScale;
            if (body != null) body.isKinematic = true;
        }

        /// <summary>Pula para <paramref name="target"/> num arco (uma casa).</summary>
        public IEnumerator HopTo(Vector3 target)
        {
            BeginMove();
            Vector3 from = transform.position;
            for (float t = 0f; t < hopDuration; t += Time.deltaTime)
            {
                float k = t / hopDuration;
                Vector3 p = Vector3.Lerp(from, target, k);
                p.y += 4f * hopHeight * k * (1f - k);
                transform.position = p;
                yield return null;
            }
            transform.position = target;
        }

        /// <summary>Desliza até a vaga final na casa e ajusta a escala (peões que dividem a casa ficam menores).</summary>
        public IEnumerator SettleAt(Vector3 target, float scaleFactor)
        {
            BeginMove();
            Vector3 from = transform.position;
            Vector3 fromScale = transform.localScale;
            Vector3 toScale = baseScale * scaleFactor;
            for (float t = 0f; t < settleDuration; t += Time.deltaTime)
            {
                float k = Mathf.SmoothStep(0f, 1f, t / settleDuration);
                transform.position = Vector3.Lerp(from, target, k);
                transform.localScale = Vector3.Lerp(fromScale, toScale, k);
                yield return null;
            }
            transform.position = target;
            transform.localScale = toScale;
            EndMove();
        }

        /// <summary>Coloca o peão direto na posição, sem animação (preparação da partida).</summary>
        public void PlaceImmediately(Vector3 target, float scaleFactor)
        {
            if (body == null) body = GetComponent<Rigidbody>();
            if (baseScale == Vector3.zero) baseScale = transform.localScale;
            transform.position = target;
            transform.localScale = baseScale * scaleFactor;
            if (body != null)
            {
                body.isKinematic = true;
                body.position = target;
            }
        }

        private void BeginMove()
        {
            if (IsMoving) return;
            IsMoving = true;
        }

        private void EndMove()
        {
            IsMoving = false;
        }
    }
}
