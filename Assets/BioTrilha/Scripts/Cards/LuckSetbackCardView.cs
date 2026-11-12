using UnityEngine;
using UnityEngine.UI;

namespace BioTrilha.Cards
{
    /// <summary>
    /// Mostra uma carta de Sorte ou Revés na UI (Canvas). Alterna entre frente e verso.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class LuckSetbackCardView : MonoBehaviour
    {
        [SerializeField] private Image faceImage;

        public LuckSetbackCardData Data { get; private set; }
        public bool IsFaceUp { get; private set; }

        private void Reset()
        {
            faceImage = GetComponent<Image>();
        }

        private void Awake()
        {
            if (faceImage == null) faceImage = GetComponent<Image>();
        }

        /// <param name="faceUp">false mostra o verso "Sorte ou Revés" (carta virada para baixo no baralho).</param>
        public void Setup(LuckSetbackCardData data, bool faceUp = false)
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
    }
}
