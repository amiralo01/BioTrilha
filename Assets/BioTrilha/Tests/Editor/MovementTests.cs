using System.Collections.Generic;
using System.Linq;
using BioTrilha.Board;
using BioTrilha.EditorTools;
using BioTrilha.Game;
using NUnit.Framework;
using UnityEngine;

namespace BioTrilha.Tests
{
    public class MovementTests
    {
        private const int Finish = 72;

        [Test]
        public void Anda_o_numero_do_dado()
        {
            Assert.AreEqual(5, TurnRules.TargetIndex(0, 5, Finish));
            Assert.AreEqual(66, TurnRules.TargetIndex(60, 6, Finish));
        }

        [Test]
        public void Passando_do_FIM_para_no_FIM()
        {
            Assert.AreEqual(Finish, TurnRules.TargetIndex(70, 6, Finish));
            Assert.AreEqual(Finish, TurnRules.TargetIndex(71, 1, Finish));
            Assert.AreEqual(Finish, TurnRules.TargetIndex(Finish, 3, Finish));
        }

        [Test]
        public void Proximo_jogador_segue_a_ordem_e_pula_quem_chegou_ao_FIM()
        {
            var players = new List<PlayerState> { new PlayerState("A", ""), new PlayerState("B", ""), new PlayerState("C", "") };
            Assert.AreEqual(1, TurnRules.NextPlayer(players, 0));
            Assert.AreEqual(0, TurnRules.NextPlayer(players, 2));

            players[1].ReachedFinish = true;
            Assert.AreEqual(2, TurnRules.NextPlayer(players, 0));

            players[0].ReachedFinish = true;
            Assert.AreEqual(2, TurnRules.NextPlayer(players, 2), "Só resta C: ele joga de novo.");

            players[2].ReachedFinish = true;
            Assert.AreEqual(-1, TurnRules.NextPlayer(players, 2));
            Assert.IsTrue(TurnRules.AllFinished(players));
        }

        [Test]
        public void Maior_numero_comeca()
        {
            var rolls = new Dictionary<int, int> { { 0, 3 }, { 1, 6 }, { 2, 2 } };
            Assert.AreEqual(1, TurnRules.DetermineStartingPlayer(3, p => rolls[p]));
        }

        [Test]
        public void Empate_no_maior_so_os_empatados_lancam_de_novo()
        {
            // Rodada 1: A=5, B=5, C=2 → A e B empatam. Rodada 2: A=1, B=4 → B começa.
            var sequence = new Queue<(int player, int value)>(new[] { (0, 5), (1, 5), (2, 2), (0, 1), (1, 4) });
            int start = TurnRules.DetermineStartingPlayer(3, p =>
            {
                var next = sequence.Dequeue();
                Assert.AreEqual(next.player, p, "Só os empatados lançam de novo, na ordem da lista.");
                return next.value;
            });
            Assert.AreEqual(1, start);
            Assert.AreEqual(0, sequence.Count);
        }

        [Test]
        public void Dado_sempre_entre_1_e_6()
        {
            var roller = new DiceRoller(42);
            var seen = new HashSet<int>();
            for (int i = 0; i < 1000; i++)
            {
                int v = roller.Roll();
                Assert.That(v, Is.InRange(1, 6));
                seen.Add(v);
            }
            Assert.AreEqual(6, seen.Count);
        }

        [Test]
        public void Trilha_tem_INICIO_71_casas_e_FIM()
        {
            var houses = BoardMovementSetup.BuildHouses();
            Assert.AreEqual(BoardMovementSetup.ExpectedHouseCount, houses.Count);
            Assert.AreEqual(HouseType.Start, houses.First().type);
            Assert.AreEqual(HouseType.Finish, houses.Last().type);
            Assert.IsFalse(houses.Skip(1).Take(houses.Count - 2).Any(h => h.type == HouseType.Start || h.type == HouseType.Finish));

            // Contagem por cor lida da arte do tabuleiro.
            Assert.AreEqual(30, houses.Count(h => h.type == HouseType.Product));
            Assert.AreEqual(6, houses.Count(h => h.type == HouseType.Action));
            Assert.AreEqual(18, houses.Count(h => h.type == HouseType.Question));
            Assert.AreEqual(14, houses.Count(h => h.type == HouseType.LuckSetback));
            Assert.AreEqual(3, houses.Count(h => h.type == HouseType.Negotiation));
        }

        [Test]
        public void Casas_vizinhas_estao_proximas_e_dentro_do_tabuleiro()
        {
            var houses = BoardMovementSetup.BuildHouses();
            for (int i = 1; i < houses.Count; i++)
            {
                float d = Vector3.Distance(houses[i - 1].position, houses[i].position);
                Assert.That(d, Is.LessThan(1.2f), $"Casa {i - 1} → {i} longe demais ({d:0.00}).");
            }
            foreach (var h in houses)
            {
                Assert.That(Mathf.Abs(h.position.x), Is.LessThan(10.24f));
                Assert.That(Mathf.Abs(h.position.z), Is.LessThan(5.12f));
            }
        }

        [Test]
        public void Peoes_na_mesma_casa_nao_ficam_no_mesmo_ponto()
        {
            var radius = new Vector2(0.28f, 0.14f);
            for (int n = 2; n <= 7; n++)
            {
                var offsets = Enumerable.Range(0, n).Select(s => PawnLayout.Offset(s, n, radius)).ToList();
                for (int a = 0; a < n; a++)
                for (int b = a + 1; b < n; b++)
                    Assert.That(Vector2.Distance(offsets[a], offsets[b]), Is.GreaterThan(0.05f), $"{n} peões: vagas {a} e {b} coincidem.");
            }
            Assert.AreEqual(Vector2.zero, PawnLayout.Offset(0, 1, radius));
        }
    }
}
