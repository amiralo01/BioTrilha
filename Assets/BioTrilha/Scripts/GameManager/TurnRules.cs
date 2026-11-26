using System;
using System.Collections.Generic;
using System.Linq;

namespace BioTrilha.Game
{
    /// <summary>
    /// Regras de movimento e de turno, em C# puro (sem cena), para poderem ser testadas.
    /// </summary>
    public static class TurnRules
    {
        /// <summary>
        /// Casa de destino depois de andar <paramref name="steps"/> casas.
        /// Passando do FIM, o peão para no FIM (não precisa de número exato).
        /// </summary>
        public static int TargetIndex(int current, int steps, int finishIndex)
        {
            if (steps < 0) throw new ArgumentOutOfRangeException(nameof(steps));
            return Math.Min(current + steps, finishIndex);
        }

        /// <summary>
        /// Próximo jogador na ordem da lista (sentido horário), pulando quem já chegou ao FIM.
        /// Retorna -1 se todos já chegaram.
        /// </summary>
        public static int NextPlayer(IReadOnlyList<PlayerState> players, int current)
        {
            for (int i = 1; i <= players.Count; i++)
            {
                int candidate = (current + i) % players.Count;
                if (!players[candidate].ReachedFinish) return candidate;
            }
            return -1;
        }

        public static bool AllFinished(IReadOnlyList<PlayerState> players) => players.All(p => p.ReachedFinish);

        /// <summary>
        /// Uma rodada da escolha de quem começa: recebe os jogadores que lançaram e os valores tirados.
        /// Retorna quem tirou o maior número. Se for um só, ele começa; se forem vários, só eles lançam de novo.
        /// </summary>
        public static List<int> HighestRollers(IReadOnlyList<int> rollers, IReadOnlyList<int> values)
        {
            if (rollers.Count != values.Count) throw new ArgumentException("Cada jogador precisa de um valor.");
            int max = values.Max();
            var result = new List<int>();
            for (int i = 0; i < rollers.Count; i++)
                if (values[i] == max) result.Add(rollers[i]);
            return result;
        }

        /// <summary>
        /// Escolha completa de quem começa, lançando até não haver empate no maior número.
        /// <paramref name="roll"/> recebe o índice do jogador e devolve o valor do dado.
        /// </summary>
        public static int DetermineStartingPlayer(int playerCount, Func<int, int> roll)
        {
            var rollers = Enumerable.Range(0, playerCount).ToList();
            while (rollers.Count > 1)
            {
                var values = rollers.Select(roll).ToList();
                rollers = HighestRollers(rollers, values);
            }
            return rollers[0];
        }
    }
}
