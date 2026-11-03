using UnityEngine;

namespace BioTrilha.Cards
{
    /// <summary>
    /// Dados de uma carta de ação. As artes (frente/verso) já contêm todo o texto da carta;
    /// os campos numéricos existem para a lógica do jogo (compra/venda).
    /// </summary>
    [CreateAssetMenu(fileName = "ActionCard", menuName = "BioTrilha/Cards/Action Card", order = 0)]
    public class ActionCardData : ScriptableObject
    {
        [Tooltip("Número da carta (1 a 36), na ordem do Canva.")]
        public int id;

        [Tooltip("Título exibido da carta (com acentos).")]
        public string displayName;

        [Header("Arte")]
        public Sprite frontSprite;
        public Sprite backSprite;

        [Header("Custo")]
        [Tooltip("Preço de compra da carta.")]
        public int buyPrice;

        [Tooltip("Preço de venda da carta.")]
        public int sellPrice;

        [Header("Regras (opcional)")]
        [TextArea(2, 6)]
        [Tooltip("Descrição do benefício para uso futuro na lógica do jogo.")]
        public string benefitText;
    }
}
