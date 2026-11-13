using UnityEngine;

namespace BioTrilha.Board
{
    /// <summary>
    /// Câmera ortográfica de cima do tabuleiro.
    ///
    /// - Visão geral (padrão): mostra a área jogável inteira, qualquer que seja a proporção da tela.
    /// - Seguindo um peão: dá zoom e mantém o peão centralizado enquanto ele anda de casa em casa.
    /// - Quando o peão para: afasta suavemente e volta para a visão geral.
    ///
    /// Uso (a lógica de movimento chama estes métodos):
    ///   cameraController.FollowPawn(peao);   // o peão começou a andar
    ///   cameraController.ShowOverview();     // o peão parou
    ///
    /// O plano do tabuleiro é XZ (a câmera olha para baixo); a altura da câmera (Y) não é alterada.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class BoardCameraController : MonoBehaviour
    {
        [Header("Tabuleiro (plano XZ, em unidades do mundo)")]
        [Tooltip("Centro do tabuleiro (x, z).")]
        [SerializeField] private Vector2 boardCenter = Vector2.zero;

        [Tooltip("Metade da largura e da altura do tabuleiro inteiro. A câmera nunca mostra além dessas bordas.")]
        [SerializeField] private Vector2 boardHalfSize = new Vector2(10.24f, 5.12f);

        [Tooltip("Metade da largura e da altura da área que PRECISA ficar visível na visão geral " +
                 "(casas, INÍCIO, FIM e Penitenciária, com uma folga).")]
        [SerializeField] private Vector2 playAreaHalfSize = new Vector2(9.85f, 4.5f);

        [Header("Zoom no peão")]
        [Tooltip("Tamanho ortográfico ao seguir o peão (menor = mais perto).")]
        [SerializeField] private float followSize = 1.8f;

        [Tooltip("Limite de aproximação. Abaixo disso a imagem do tabuleiro (8192x4096) perde nitidez.")]
        [SerializeField] private float minSize = 1.35f;

        [Header("Transições (segundos)")]
        [SerializeField] private float zoomInTime = 0.6f;
        [SerializeField] private float zoomOutTime = 0.9f;
        [Tooltip("Suavização ao acompanhar o peão já com zoom (quanto menor, mais colada ao peão).")]
        [SerializeField] private float followSmoothTime = 0.15f;

        [Header("Opções")]
        [Tooltip("Impede que a câmera mostre a área fora do tabuleiro. Perto das bordas o peão deixa de ficar " +
                 "exatamente no centro, mas nunca sai da tela.")]
        [SerializeField] private bool clampToBoard = true;

        [Tooltip("Peão usado pelos comandos de teste do menu de contexto do componente.")]
        [SerializeField] private Transform debugTarget;

        private Camera cam;
        private Transform target;
        private Vector3 positionVelocity;
        private float sizeVelocity;

        /// <summary>true enquanto a câmera acompanha um peão.</summary>
        public bool IsFollowing => target != null;

        /// <summary>Peão acompanhado no momento (ou null na visão geral).</summary>
        public Transform Target => target;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            cam.orthographic = true;
        }

        private void Start()
        {
            SnapToOverview();
        }

        /// <summary>Tamanho ortográfico que enquadra a área jogável inteira para a proporção atual da tela.</summary>
        public float FitSize()
        {
            float aspect = Mathf.Max(0.1f, cam != null ? cam.aspect : (float)Screen.width / Mathf.Max(1, Screen.height));
            // largura visível = size * aspect * 2; altura visível = size * 2
            return Mathf.Max(playAreaHalfSize.y, playAreaHalfSize.x / aspect);
        }

        /// <summary>Começa a acompanhar o peão: zoom nele e câmera centralizada nele.</summary>
        public void FollowPawn(Transform pawn)
        {
            target = pawn;
        }

        /// <summary>Para de acompanhar e volta, suavemente, para a visão geral do tabuleiro.</summary>
        public void ShowOverview()
        {
            target = null;
        }

        /// <summary>Vai imediatamente para a visão geral, sem animação.</summary>
        public void SnapToOverview()
        {
            if (cam == null) cam = GetComponent<Camera>();
            target = null;
            cam.orthographicSize = FitSize();
            Vector3 p = transform.position;
            transform.position = new Vector3(boardCenter.x, p.y, boardCenter.y);
            positionVelocity = Vector3.zero;
            sizeVelocity = 0f;
        }

        private void LateUpdate()
        {
            float desiredSize;
            Vector2 desiredCenter;
            float smoothTime;

            if (target != null)
            {
                desiredSize = Mathf.Max(followSize, minSize);
                desiredCenter = new Vector2(target.position.x, target.position.z);
                // enquanto ainda está aproximando, usa o tempo de zoom; depois só acompanha de perto
                smoothTime = Mathf.Abs(cam.orthographicSize - desiredSize) > 0.05f ? zoomInTime : followSmoothTime;
            }
            else
            {
                desiredSize = FitSize();
                desiredCenter = boardCenter;
                smoothTime = zoomOutTime;
            }

            // zoom
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, desiredSize, ref sizeVelocity, smoothTime);

            // posição (o Y da câmera não muda)
            Vector3 current = transform.position;
            Vector3 goal = new Vector3(desiredCenter.x, current.y, desiredCenter.y);
            Vector3 next = Vector3.SmoothDamp(current, goal, ref positionVelocity, smoothTime);

            if (clampToBoard) next = ClampToBoard(next);
            transform.position = next;
        }

        private Vector3 ClampToBoard(Vector3 position)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            // quanto a câmera pode se afastar do centro sem mostrar fora do tabuleiro
            float maxOffsetX = boardHalfSize.x - halfW;
            float maxOffsetZ = boardHalfSize.y - halfH;

            // se a visão é maior que o tabuleiro nesse eixo, fica centralizada
            float x = maxOffsetX > 0f
                ? Mathf.Clamp(position.x, boardCenter.x - maxOffsetX, boardCenter.x + maxOffsetX)
                : boardCenter.x;
            float z = maxOffsetZ > 0f
                ? Mathf.Clamp(position.z, boardCenter.y - maxOffsetZ, boardCenter.y + maxOffsetZ)
                : boardCenter.y;

            return new Vector3(x, position.y, z);
        }

        // ----- comandos de teste (botão direito no componente, no Inspector) -----

        [ContextMenu("Teste: seguir o peão de teste")]
        private void DebugFollow()
        {
            if (debugTarget != null) FollowPawn(debugTarget);
        }

        [ContextMenu("Teste: voltar à visão geral")]
        private void DebugOverview()
        {
            ShowOverview();
        }

        private void OnValidate()
        {
            minSize = Mathf.Max(0.1f, minSize);
            followSize = Mathf.Max(minSize, followSize);
            zoomInTime = Mathf.Max(0.01f, zoomInTime);
            zoomOutTime = Mathf.Max(0.01f, zoomOutTime);
            followSmoothTime = Mathf.Max(0.01f, followSmoothTime);
        }

        private void OnDrawGizmosSelected()
        {
            // verde = tabuleiro inteiro; amarelo = área jogável que precisa caber na visão geral
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(new Vector3(boardCenter.x, 0f, boardCenter.y),
                new Vector3(boardHalfSize.x * 2f, 0.01f, boardHalfSize.y * 2f));
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector3(boardCenter.x, 0f, boardCenter.y),
                new Vector3(playAreaHalfSize.x * 2f, 0.01f, playAreaHalfSize.y * 2f));
        }
    }
}
