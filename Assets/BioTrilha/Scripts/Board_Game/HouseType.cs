namespace BioTrilha.Board
{
    /// <summary>
    /// Tipo de cada casa da trilha. A cor da casa no tabuleiro define o tipo.
    /// </summary>
    public enum HouseType
    {
        Start,        // INÍCIO
        Product,      // verde: carta de Produto
        Action,       // azul: carta de Ação
        Question,     // amarela: carta de Questões
        LuckSetback,  // laranja: carta de Sorte ou Revés
        Negotiation,  // roxa: negociação
        Finish        // FIM
    }

    public static class HouseTypeExtensions
    {
        /// <summary>Nome da casa para mostrar na tela.</summary>
        public static string DisplayName(this HouseType type)
        {
            switch (type)
            {
                case HouseType.Start: return "INÍCIO";
                case HouseType.Product: return "Produto (verde)";
                case HouseType.Action: return "Ação (azul)";
                case HouseType.Question: return "Questões (amarela)";
                case HouseType.LuckSetback: return "Sorte ou Revés (laranja)";
                case HouseType.Negotiation: return "Negociação (roxa)";
                case HouseType.Finish: return "FIM";
                default: return type.ToString();
            }
        }

        /// <summary>Casas verde e azul: o jogador lança o dado de novo para o nível da carta.</summary>
        public static bool RollsForLevel(this HouseType type)
        {
            return type == HouseType.Product || type == HouseType.Action;
        }
    }
}
