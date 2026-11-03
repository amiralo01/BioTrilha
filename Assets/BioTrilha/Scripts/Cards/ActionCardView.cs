using UnityEngine;
using UnityEngine.UI;

namespace BioTrilha.Cards
{
    /// <summary>
    /// Mostra uma carta de ação na UI (Canvas). Alterna entre frente e verso.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class ActionCardView : MonoBehaviour
    {
        [SerializeField] private Image faceImage;
        [SerializeField] private bool startFaceUp = true;

        public ActionCardData Data { get; private set; }
        public bool IsFaceUp { get; private set; }

        private void Reset()
        {
            faceImage = GetComponent<Image>();
        }

        private void Awake()
        {
            if (faceImage == null) faceImage = GetComponent<Image>();
        }

        public void Setup(ActionCardData data, bool faceUp = true)
        {
            Data = data;
            SetFace(faceUp);
        }

        public void SetFace(bool faceUp)
        {
            IsFaceUp = faceUp;
            if (Data == null || faceImage == null) return;
            faceImage.sprite = faceUp ? Data.frontSprite : Data.backSprite;
        }

        /// <summary>Pode ser ligado ao OnClick de um Button.</summary>
        public void Flip() => SetFace(!IsFaceUp);

        private void Start()
        {
            // Prefab colocado direto na cena sem Setup(): mantém a face inicial.
            if (Data == null) IsFaceUp = startFaceUp;
        }
    }
}
