using UnityEngine;

namespace BioTrilha.Cards
{
    /// <summary>Tipo da carta do baralho Sorte ou Revés (impresso na própria arte).</summary>
    public enum LuckSetbackKind
    {
        Luck,     // Sorte
        Setback   // Revés
    }

    /// <summary>
    /// Dados de uma carta de Sorte ou Revés. A arte (frente) já contém o título, o texto e o efeito.
    /// Todas as cartas compartilham o mesmo verso ("Sorte ou Revés").
    /// </summary>
    [CreateAssetMenu(fileName = "SorteReves", menuName = "BioTrilha/Cards/Luck or Setback Card", order = 1)]
    public class LuckSetbackCardData : ScriptableObject
    {
        [Tooltip("Número da carta (1 a 26), na ordem do Canva.")]
        public int id;

        [Tooltip("Título exibido da carta (com acentos).")]
        public string displayName;

        [Tooltip("Sorte ou Revés, conforme impresso na carta.")]
        public LuckSetbackKind kind;

        [Header("Arte")]
        public Sprite frontSprite;

        [Tooltip("Verso comum a todas as cartas deste baralho.")]
        public Sprite backSprite;

        [Header("Regras")]
        [Tooltip("A carta fica guardada com o jogador em vez de voltar ao fundo do baralho (só a 'Nova descoberta científica').")]
        public bool keepCard;

        [Tooltip("O efeito atinge todos os jogadores, resolvido em sequência a partir de quem puxou a carta (ex.: Crise Hídrica).")]
        public bool targetAllPlayers;

        [TextArea(2, 6)]
        [Tooltip("Descrição do efeito para uso futuro na lógica do jogo. Nunca é interpretada pelo código.")]
        public string benefitText;
    }
}
