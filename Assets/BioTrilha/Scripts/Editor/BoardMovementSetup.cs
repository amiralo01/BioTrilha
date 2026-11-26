using System.Collections.Generic;
using BioTrilha.Board;
using BioTrilha.Game;
using BioTrilha.Pawns;
using BioTrilha.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BioTrilha.EditorTools
{
    /// <summary>
    /// Menu BioTrilha > Tabuleiro > Montar Movimento na Cena.
    /// Preenche a trilha (71 casas + INÍCIO e FIM) no objeto "game board" e liga peões, dado, câmera,
    /// interface e TurnManager. Pode ser executado de novo: atualiza o que já existe.
    ///
    /// As posições foram lidas da arte do tabuleiro (game board.png, 8192x4096, 400 px/unidade):
    /// centro de cada casa colorida, em ordem do INÍCIO ao FIM. Mundo: x = (px - 4096) / 400, z = (2048 - py) / 400.
    /// </summary>
    public static class BoardMovementSetup
    {
        private readonly struct Row
        {
            public readonly HouseType type;
            public readonly float x, z;
            public Row(HouseType type, float x, float z) { this.type = type; this.x = x; this.z = z; }
        }

        public const int ExpectedHouseCount = 73; // INÍCIO + 71 casas + FIM

        private static readonly Row[] Houses =
        {
            new Row(HouseType.Start, -8.603f, 2.084f), // 0
            new Row(HouseType.Product, -7.746f, 1.441f), // 1
            new Row(HouseType.LuckSetback, -6.934f, 1.513f), // 2
            new Row(HouseType.Product, -6.522f, 2.202f), // 3
            new Row(HouseType.Product, -6.460f, 3.050f), // 4
            new Row(HouseType.Question, -5.805f, 3.384f), // 5
            new Row(HouseType.Product, -5.419f, 2.835f), // 6
            new Row(HouseType.Action, -5.115f, 2.260f), // 7
            new Row(HouseType.LuckSetback, -4.809f, 2.952f), // 8
            new Row(HouseType.Product, -4.505f, 3.686f), // 9
            new Row(HouseType.Question, -3.697f, 3.836f), // 10
            new Row(HouseType.Product, -2.835f, 3.835f), // 11
            new Row(HouseType.Product, -1.975f, 3.835f), // 12
            new Row(HouseType.Negotiation, -1.125f, 3.835f), // 13
            new Row(HouseType.Product, -0.270f, 3.835f), // 14
            new Row(HouseType.Question, 0.585f, 3.835f), // 15
            new Row(HouseType.LuckSetback, 1.435f, 3.835f), // 16
            new Row(HouseType.Action, 2.285f, 3.835f), // 17
            new Row(HouseType.Product, 3.135f, 3.835f), // 18
            new Row(HouseType.Product, 3.985f, 3.835f), // 19
            new Row(HouseType.Question, 4.845f, 3.835f), // 20
            new Row(HouseType.Product, 5.695f, 3.835f), // 21
            new Row(HouseType.Question, 6.539f, 3.837f), // 22
            new Row(HouseType.LuckSetback, 7.295f, 3.540f), // 23
            new Row(HouseType.Product, 7.382f, 2.780f), // 24
            new Row(HouseType.LuckSetback, 6.664f, 2.464f), // 25
            new Row(HouseType.Action, 5.805f, 2.465f), // 26
            new Row(HouseType.Product, 4.955f, 2.465f), // 27
            new Row(HouseType.Question, 4.100f, 2.465f), // 28
            new Row(HouseType.Product, 3.250f, 2.465f), // 29
            new Row(HouseType.LuckSetback, 2.400f, 2.465f), // 30
            new Row(HouseType.Action, 1.550f, 2.465f), // 31
            new Row(HouseType.Question, 0.695f, 2.465f), // 32
            new Row(HouseType.Product, -0.155f, 2.465f), // 33
            new Row(HouseType.Product, -1.015f, 2.465f), // 34
            new Row(HouseType.Question, -1.865f, 2.465f), // 35
            new Row(HouseType.Product, -2.712f, 2.465f), // 36
            new Row(HouseType.LuckSetback, -3.470f, 2.166f), // 37
            new Row(HouseType.Negotiation, -3.671f, 1.361f), // 38
            new Row(HouseType.Product, -3.171f, 0.792f), // 39
            new Row(HouseType.Product, -2.373f, 1.049f), // 40
            new Row(HouseType.LuckSetback, -1.510f, 1.100f), // 41
            new Row(HouseType.Action, -0.660f, 1.100f), // 42
            new Row(HouseType.Product, 0.195f, 1.100f), // 43
            new Row(HouseType.Question, 1.050f, 1.100f), // 44
            new Row(HouseType.LuckSetback, 1.900f, 1.100f), // 45
            new Row(HouseType.LuckSetback, 2.750f, 1.100f), // 46
            new Row(HouseType.Question, 3.600f, 1.100f), // 47
            new Row(HouseType.Product, 4.450f, 1.100f), // 48
            new Row(HouseType.LuckSetback, 5.305f, 1.100f), // 49
            new Row(HouseType.Product, 6.150f, 1.100f), // 50
            new Row(HouseType.Product, 6.994f, 1.100f), // 51
            new Row(HouseType.Question, 7.759f, 0.799f), // 52
            new Row(HouseType.Product, 7.850f, 0.041f), // 53
            new Row(HouseType.Question, 7.124f, -0.280f), // 54
            new Row(HouseType.Product, 6.275f, -0.280f), // 55
            new Row(HouseType.Product, 5.425f, -0.280f), // 56
            new Row(HouseType.Question, 4.570f, -0.280f), // 57
            new Row(HouseType.LuckSetback, 3.715f, -0.280f), // 58
            new Row(HouseType.Action, 2.860f, -0.280f), // 59
            new Row(HouseType.Question, 2.005f, -0.280f), // 60
            new Row(HouseType.Product, 1.155f, -0.280f), // 61
            new Row(HouseType.Negotiation, 0.305f, -0.280f), // 62
            new Row(HouseType.Question, -0.599f, -0.280f), // 63
            new Row(HouseType.Product, -1.535f, -0.280f), // 64
            new Row(HouseType.Question, -2.480f, -0.280f), // 65
            new Row(HouseType.Question, -3.238f, -0.427f), // 66
            new Row(HouseType.LuckSetback, -3.238f, -1.031f), // 67
            new Row(HouseType.Product, -2.480f, -1.175f), // 68
            new Row(HouseType.Product, -1.530f, -1.175f), // 69
            new Row(HouseType.Question, -0.586f, -1.186f), // 70
            new Row(HouseType.LuckSetback, -0.020f, -1.764f), // 71
            new Row(HouseType.Finish, -0.175f, -2.833f), // 72
        };

        private static readonly (string name, string profession, string pawn)[] Players =
        {
            ("Alysson", "Engenheiro Agrônomo", "Peao_Agronomo"),
            ("José", "Engenheiro Químico", "Peao_Quimico"),
            ("Joana", "Cientista", "Peao_Cientista"),
        };

        /// <summary>Tabela da trilha, também usada pelos testes.</summary>
        public static List<BoardHouse> BuildHouses()
        {
            var list = new List<BoardHouse>();
            foreach (var r in Houses) list.Add(new BoardHouse(r.type, new Vector3(r.x, 0f, r.z)));
            return list;
        }

        [MenuItem("BioTrilha/Tabuleiro/Montar Movimento na Cena")]
        public static void Setup()
        {
            var board = Require("game board");
            var dice = Require("Dice");
            var cam = Object.FindObjectOfType<BoardCameraController>();
            if (board == null || dice == null) return;

            // Trilha
            var path = GetOrAdd<BoardPath>(board);
            Undo.RecordObject(path, "Gerar trilha");
            path.SetHouses(BuildHouses());
            EditorUtility.SetDirty(path);

            // Dado
            var diceView = GetOrAdd<DiceView>(dice);

            // Interface
            var hud = BuildHud();

            // TurnManager
            var tmGo = GameObject.Find("TurnManager");
            if (tmGo == null)
            {
                tmGo = new GameObject("TurnManager");
                Undo.RegisterCreatedObjectUndo(tmGo, "TurnManager");
            }
            var tm = GetOrAdd<TurnManager>(tmGo);
            var so = new SerializedObject(tm);
            so.FindProperty("path").objectReferenceValue = path;
            so.FindProperty("dice").objectReferenceValue = diceView;
            so.FindProperty("hud").objectReferenceValue = hud;
            so.FindProperty("boardCamera").objectReferenceValue = cam;
            var setups = so.FindProperty("playerSetups");
            setups.arraySize = Players.Length;
            for (int i = 0; i < Players.Length; i++)
            {
                var pawnGo = Require(Players[i].pawn);
                var e = setups.GetArrayElementAtIndex(i);
                e.FindPropertyRelative("name").stringValue = Players[i].name;
                e.FindPropertyRelative("profession").stringValue = Players[i].profession;
                e.FindPropertyRelative("pawn").objectReferenceValue = pawnGo != null ? GetOrAdd<PawnMover>(pawnGo) : null;
            }
            so.ApplyModifiedProperties();

            // Peões no INÍCIO (mesma distribuição usada no jogo)
            for (int i = 0; i < Players.Length; i++)
            {
                var pawnGo = Require(Players[i].pawn);
                if (pawnGo == null) continue;
                var place = path.Placement(path.StartIndex, i, Players.Length);
                Undo.RecordObject(pawnGo.transform, "Peões no INÍCIO");
                pawnGo.transform.position = place.position;
            }

            EditorSceneManager.MarkSceneDirty(board.scene);
            Debug.Log($"BioTrilha: trilha com {path.Count} casas (0 = INÍCIO, {path.FinishIndex} = FIM); peões, dado, interface e TurnManager configurados.");
        }

        private static TurnHud BuildHud()
        {
            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                Undo.RegisterCreatedObjectUndo(es, "EventSystem");
            }

            var existing = Object.FindObjectOfType<TurnHud>();
            if (existing != null) Undo.DestroyObjectImmediate(existing.gameObject);

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var canvasGo = new GameObject("TurnHud", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasGo, "TurnHud");
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(2400, 1080);
            scaler.matchWidthOrHeight = 1f;

            // Faixa de status no topo
            var panel = NewUi("StatusPanel", canvasGo.transform);
            var prt = panel.GetComponent<RectTransform>();
            prt.anchorMin = new Vector2(0.5f, 1f); prt.anchorMax = new Vector2(0.5f, 1f); prt.pivot = new Vector2(0.5f, 1f);
            prt.anchoredPosition = new Vector2(0, -20); prt.sizeDelta = new Vector2(1400, 150);
            panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);

            var status = NewUi("StatusText", panel.transform).AddComponent<Text>();
            Stretch(status.rectTransform, 24);
            status.font = font; status.fontSize = 40; status.color = Color.white;
            status.alignment = TextAnchor.MiddleCenter;
            status.text = "BioTrilha";

            // Botão "Lançar dado" no canto inferior direito
            var btnGo = NewUi("RollButton", canvasGo.transform);
            var brt = btnGo.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(1f, 0f); brt.anchorMax = new Vector2(1f, 0f); brt.pivot = new Vector2(1f, 0f);
            brt.anchoredPosition = new Vector2(-40, 40); brt.sizeDelta = new Vector2(560, 150);
            var img = btnGo.AddComponent<Image>(); img.color = new Color(1f, 0.8f, 0.02f);
            var button = btnGo.AddComponent<Button>(); button.targetGraphic = img;
            var label = NewUi("Label", btnGo.transform).AddComponent<Text>();
            Stretch(label.rectTransform, 10);
            label.font = font; label.fontSize = 44; label.fontStyle = FontStyle.Bold;
            label.color = new Color(0.04f, 0.27f, 0.1f); label.alignment = TextAnchor.MiddleCenter;
            label.text = "Lançar dado";

            var hud = canvasGo.AddComponent<TurnHud>();
            var so = new SerializedObject(hud);
            so.FindProperty("statusText").objectReferenceValue = status;
            so.FindProperty("rollButton").objectReferenceValue = button;
            so.FindProperty("rollButtonLabel").objectReferenceValue = label;
            so.ApplyModifiedProperties();
            return hud;
        }

        private static GameObject NewUi(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void Stretch(RectTransform rt, float padding)
        {
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(padding, padding); rt.offsetMax = new Vector2(-padding, -padding);
        }

        private static GameObject Require(string name)
        {
            var go = GameObject.Find(name);
            if (go == null) Debug.LogError($"BioTrilha: objeto '{name}' não encontrado na cena.");
            return go;
        }

        private static T GetOrAdd<T>(GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            return c != null ? c : Undo.AddComponent<T>(go);
        }
    }
}
