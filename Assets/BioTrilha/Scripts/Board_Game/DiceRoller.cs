using System;

namespace BioTrilha.Board
{
    /// <summary>
    /// Sorteio do dado de 6 faces. O número é decidido aqui, não pela física do dado 3D.
    /// Com semente fixa o resultado se repete (usado nos testes).
    /// </summary>
    public class DiceRoller
    {
        public const int Faces = 6;
        private readonly Random random;

        public DiceRoller() : this(Environment.TickCount) { }

        public DiceRoller(int seed)
        {
            random = new Random(seed);
        }

        /// <summary>Número de 1 a 6.</summary>
        public int Roll() => random.Next(1, Faces + 1);
    }
}
