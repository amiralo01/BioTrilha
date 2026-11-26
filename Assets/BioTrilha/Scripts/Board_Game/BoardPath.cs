using System;
using System.Collections.Generic;
using UnityEngine;

namespace BioTrilha.Board
{
    /// <summary>Uma casa da trilha: tipo e posição do centro no mundo (plano XZ, y = 0 é a superfície).</summary>
    [Serializable]
    public class BoardHouse
    {
        public HouseType type;
        public Vector3 position;

        public BoardHouse(HouseType type, Vector3 position)
        {
            this.type = type;
            this.position = position;
        }
    }

    /// <summary>
    /// Trilha do tabuleiro, em ordem: índice 0 = INÍCIO, último índice = FIM.
    /// Preenchida pelo menu BioTrilha > Tabuleiro > Gerar Trilha (posições lidas da arte do tabuleiro).
    /// No editor, cada casa aparece com o número e a cor (Gizmos).
    /// </summary>
    public class BoardPath : MonoBehaviour
    {
        [SerializeField] private List<BoardHouse> houses = new List<BoardHouse>();

        [Header("Peões na mesma casa")]
        [Tooltip("Escala do peão quando divide uma casa comum com outros (1 = tamanho normal).")]
        [SerializeField] private float sharedHouseScale = 0.55f;
        [Tooltip("Raio da elipse de distribuição numa casa comum: x = ao longo da trilha, y = para os lados.")]
        [SerializeField] private Vector2 sharedHouseRadius = new Vector2(0.28f, 0.14f);
        [Tooltip("Escala dos peões no INÍCIO e no FIM, que são casas grandes.")]
        [SerializeField] private float bigSquareScale = 0.85f;
        [SerializeField] private Vector2 bigSquareRadius = new Vector2(0.38f, 0.38f);

        public int Count => houses.Count;
        public int StartIndex => 0;
        public int FinishIndex => houses.Count - 1;
        public IReadOnlyList<BoardHouse> Houses => houses;

        public BoardHouse Get(int index) => houses[index];

        public void SetHouses(List<BoardHouse> newHouses)
        {
            houses = newHouses;
        }

        /// <summary>Direção da trilha na casa (para alinhar os peões que dividem a casa).</summary>
        public Vector3 Tangent(int index)
        {
            if (houses.Count < 2) return Vector3.right;
            int a = Mathf.Max(0, index - 1);
            int b = Mathf.Min(houses.Count - 1, index + 1);
            Vector3 d = houses[b].position - houses[a].position;
            d.y = 0f;
            return d.sqrMagnitude < 1e-6f ? Vector3.right : d.normalized;
        }

        /// <summary>Posição e escala de um peão que ocupa a vaga <paramref name="slot"/> de <paramref name="occupants"/> na casa.</summary>
        public PawnPlacement Placement(int index, int slot, int occupants)
        {
            HouseType type = houses[index].type;
            bool big = type == HouseType.Start || type == HouseType.Finish;
            Vector2 radius = big ? bigSquareRadius : sharedHouseRadius;
            float scale = big ? bigSquareScale : sharedHouseScale;
            Vector2 offset2D = PawnLayout.Offset(slot, occupants, radius);
            if (occupants <= 1) scale = 1f;

            Vector3 t = Tangent(index);
            Vector3 n = new Vector3(-t.z, 0f, t.x);
            Vector3 pos = houses[index].position + t * offset2D.x + n * offset2D.y;
            return new PawnPlacement(pos, scale);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            for (int i = 0; i < houses.Count; i++)
            {
                Gizmos.color = GizmoColor(houses[i].type);
                Gizmos.DrawSphere(houses[i].position + Vector3.up * 0.02f, 0.09f);
                if (i > 0)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(houses[i - 1].position + Vector3.up * 0.02f, houses[i].position + Vector3.up * 0.02f);
                }
                UnityEditor.Handles.Label(houses[i].position + new Vector3(-0.08f, 0.05f, 0.18f), i.ToString());
            }
        }

        private static Color GizmoColor(HouseType type)
        {
            switch (type)
            {
                case HouseType.Product: return new Color(0f, 0.75f, 0.39f);
                case HouseType.Action: return new Color(0f, 0.29f, 0.68f);
                case HouseType.Question: return new Color(1f, 0.8f, 0.02f);
                case HouseType.LuckSetback: return new Color(0.93f, 0.31f, 0f);
                case HouseType.Negotiation: return new Color(0.37f, 0.09f, 0.92f);
                default: return Color.white;
            }
        }
#endif
    }

    public readonly struct PawnPlacement
    {
        public readonly Vector3 position;
        public readonly float scale;

        public PawnPlacement(Vector3 position, float scale)
        {
            this.position = position;
            this.scale = scale;
        }
    }

    /// <summary>
    /// Distribuição dos peões dentro de uma casa. Sozinho: no centro.
    /// Vários: numa elipse (x = ao longo da trilha, y = para os lados), começando pela frente.
    /// </summary>
    public static class PawnLayout
    {
        public static Vector2 Offset(int slot, int occupants, Vector2 radius)
        {
            if (occupants <= 1) return Vector2.zero;
            if (occupants == 2)
                return new Vector2(slot == 0 ? radius.x : -radius.x, 0f);
            // 3 ou mais: pontos igualmente espaçados na elipse, o primeiro de lado (90°).
            float angle = Mathf.PI * 0.5f + slot * (2f * Mathf.PI / occupants);
            return new Vector2(Mathf.Cos(angle) * radius.x, Mathf.Sin(angle) * radius.y);
        }
    }
}
