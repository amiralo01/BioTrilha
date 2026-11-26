namespace BioTrilha.Game
{
    /// <summary>
    /// Estado de um jogador durante a partida. Por enquanto só o que o movimento precisa;
    /// dinheiro, cartas, dívida e Penitenciária entram nas próximas etapas.
    /// </summary>
    public class PlayerState
    {
        public string Name { get; }
        public string Profession { get; }

        /// <summary>Índice da casa na trilha (0 = INÍCIO).</summary>
        public int Position { get; set; }

        /// <summary>Chegou ao FIM: só espera, pulando os próximos turnos.</summary>
        public bool ReachedFinish { get; set; }

        public PlayerState(string name, string profession)
        {
            Name = name;
            Profession = profession;
        }
    }
}
