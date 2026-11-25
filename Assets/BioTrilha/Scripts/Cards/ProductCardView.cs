using UnityEngine;
using UnityEngine.UI;

namespace BioTrilha.Cards
{
    /// <summary>
    /// Mostra uma carta de Produto na UI (Canvas). Alterna entre frente (nome, estrelas, imagem)
    /// e verso (descrição e subprodutos).
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class ProductCardView : MonoBehaviour
    {
        [SerializeField] private Image faceImage;

        public ProductCardData Data { get; private set; }
        public bool IsFaceUp { get; private set; }

        private void Reset()
        {
            faceImage = GetComponent<Image>();
        }

        private void Awake()
        {
            if (faceImage == null) faceImage = GetComponent<Image>();
        }

        public void Setup(ProductCardData data, bool faceUp = true)
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
