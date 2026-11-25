using UnityEngine;

namespace BioTrilha.Cards
{
    /// <summary>
    /// Dados de uma carta de Produto bioeconômico. A arte já contém o título, as estrelas,
    /// a descrição e os subprodutos; os campos abaixo existem para a lógica do jogo.
    /// </summary>
    [CreateAssetMenu(fileName = "Produto", menuName = "BioTrilha/Cards/Product Card", order = 3)]
    public class ProductCardData : ScriptableObject
    {
        [Tooltip("Número da carta, na ordem do Canva.")]
        public int id;

        [Tooltip("Nome do produto impresso na carta.")]
        public string displayName;

        [Tooltip("Nível de sustentabilidade (estrelas): conta pontos no fim do jogo. 0 = a arte não mostra estrelas.")]
        [Range(0, 5)] public int stars = 1;

        [Tooltip("Tipo do produto. O ID (1 a 11) é usado pelas cartas que citam um tipo de produto.")]
        public ProductCategory category;

        [Header("Arte")]
        public Sprite frontSprite;
        public Sprite backSprite;

        [Header("Textos (só exibição; nunca interpretados pelo código)")]
        [TextArea(2, 6)] public string descriptionText;
        [TextArea(2, 6)] public string subproductsText;

        /// <summary>ID numérico da categoria (1 a 11; 0 = sem categoria).</summary>
        public int CategoryId => (int)category;
    }
}
