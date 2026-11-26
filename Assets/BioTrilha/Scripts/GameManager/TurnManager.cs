using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BioTrilha.Board;
using BioTrilha.Pawns;
using BioTrilha.UI;
using UnityEngine;

namespace BioTrilha.Game
{
    /// <summary>
    /// Controla a partida no protótipo de movimento:
    /// escolha de quem começa → (lança o dado → anda → resolve a casa → próximo jogador) até todos chegarem ao FIM.
    /// A entrega de cartas ainda não existe: ela vai escutar o evento <see cref="HouseLanded"/>.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        [Serializable]
        public class PlayerSetup
        {
            public string name;
            public string profession;
            public PawnMover pawn;
        }

        [SerializeField] private BoardPath path;
        [SerializeField] private DiceView dice;
        [SerializeField] private TurnHud hud;
        [SerializeField] private BoardCameraController boardCamera;
        [SerializeField] private List<PlayerSetup> playerSetups = new List<PlayerSetup>();
        [SerializeField] private float pauseAfterRoll = 0.4f;

        /// <summary>
        /// Disparado quando o peão para numa casa. O inteiro é o nível sorteado nas casas verde e azul
        /// (1 a 6, sendo 6 = escolha livre) e 0 nas outras.
        /// </summary>
        public event Action<PlayerState, BoardHouse, int> HouseLanded;

        private readonly List<PlayerState> players = new List<PlayerState>();
        private readonly DiceRoller roller = new DiceRoller();
        private int current;

        public IReadOnlyList<PlayerState> Players => players;

        private void Start()
        {
            if (path == null || path.Count < 2)
            {
                Debug.LogError("TurnManager: a trilha (BoardPath) está vazia. Use BioTrilha > Tabuleiro > Gerar Trilha.");
                return;
            }

            players.Clear();
            foreach (var s in playerSetups)
                players.Add(new PlayerState(s.name, s.profession) { Position = path.StartIndex });

            for (int i = 0; i < players.Count; i++)
            {
                var place = PlacementFor(i);
                playerSetups[i].pawn.PlaceImmediately(place.position, place.scale);
            }

            if (boardCamera != null) boardCamera.SnapToOverview();
            StartCoroutine(RunGame());
        }

        private IEnumerator RunGame()
        {
            yield return ChooseStartingPlayer();

            while (!TurnRules.AllFinished(players))
            {
                yield return PlayTurn(players[current]);
                int next = TurnRules.NextPlayer(players, current);
                if (next < 0) break;
                current = next;
            }

            hud.SetStatus("Todos chegaram ao FIM! Fim da partida.");
        }

        // Preparação, passo 3: maior número começa; empatados no maior lançam de novo.
        private IEnumerator ChooseStartingPlayer()
        {
            var rollers = Enumerable.Range(0, players.Count).ToList();
            string log = "";
            while (rollers.Count > 1)
            {
                var values = new List<int>();
                foreach (int p in rollers)
                {
                    hud.SetStatus("Quem começa? Maior número no dado.\n" + log);
                    yield return hud.WaitForPress($"{players[p].Name}: lançar dado");
                    int value = roller.Roll();
                    yield return dice.Roll(value);
                    values.Add(value);
                    log += $"{players[p].Name} tirou {value}.  ";
                }

                rollers = TurnRules.HighestRollers(rollers, values);
                if (rollers.Count > 1)
                    log = "Empate entre " + string.Join(" e ", rollers.Select(r => players[r].Name)) + ": lançam de novo.\n";
            }

            current = rollers[0];
            hud.SetStatus($"{log}\n{players[current].Name} começa!");
            yield return new WaitForSeconds(1.2f);
        }

        private IEnumerator PlayTurn(PlayerState player)
        {
            var pawn = PawnOf(player);
            hud.SetStatus($"Vez de {player.Name} ({player.Profession}) — casa {player.Position}");
            yield return hud.WaitForPress("Lançar dado");

            int steps = roller.Roll();
            yield return dice.Roll(steps);
            hud.SetStatus($"{player.Name} tirou {steps}.");
            yield return new WaitForSeconds(pauseAfterRoll);

            // Anda casa por casa.
            int from = player.Position;
            int target = TurnRules.TargetIndex(from, steps, path.FinishIndex);
            player.Position = -1; // saiu da casa: reorganiza quem ficou
            yield return RelayoutHouse(from);

            if (boardCamera != null) boardCamera.FollowPawn(pawn.transform);
            for (int i = from + 1; i <= target; i++)
                yield return pawn.HopTo(path.Get(i).position);

            player.Position = target;
            yield return RelayoutHouse(target);
            if (boardCamera != null) boardCamera.ShowOverview();

            var house = path.Get(target);
            int level = 0;
            if (house.type == HouseType.Finish)
            {
                player.ReachedFinish = true;
                hud.SetStatus($"{player.Name} chegou ao FIM!");
                yield return new WaitForSeconds(1.2f);
            }
            else if (house.type.RollsForLevel())
            {
                hud.SetStatus($"{player.Name} parou na casa {target}: {house.type.DisplayName()}.\nLance o dado para o nível da carta.");
                yield return hud.WaitForPress("Lançar dado (nível)");
                level = roller.Roll();
                yield return dice.Roll(level);
                string levelText = level == 6 ? "6: escolha livre" : $"nível {level}★";
                hud.SetStatus($"{player.Name} parou na casa {target}: {house.type.DisplayName()} — {levelText}.");
                yield return new WaitForSeconds(1.2f);
            }
            else
            {
                hud.SetStatus($"{player.Name} parou na casa {target}: {house.type.DisplayName()}.");
                yield return new WaitForSeconds(1.2f);
            }

            HouseLanded?.Invoke(player, house, level);
        }

        /// <summary>Reorganiza os peões de uma casa (vagas e escala), em paralelo.</summary>
        private IEnumerator RelayoutHouse(int houseIndex)
        {
            var running = new List<Coroutine>();
            var occupants = Occupants(houseIndex);
            for (int slot = 0; slot < occupants.Count; slot++)
            {
                var place = path.Placement(houseIndex, slot, occupants.Count);
                running.Add(StartCoroutine(PawnOf(occupants[slot]).SettleAt(place.position, place.scale)));
            }
            foreach (var c in running) yield return c;
        }

        private PawnPlacement PlacementFor(int playerIndex)
        {
            var p = players[playerIndex];
            var occupants = Occupants(p.Position);
            return path.Placement(p.Position, occupants.IndexOf(p), occupants.Count);
        }

        private List<PlayerState> Occupants(int houseIndex) => players.Where(p => p.Position == houseIndex).ToList();

        private PawnMover PawnOf(PlayerState player) => playerSetups[players.IndexOf(player)].pawn;
    }
}
