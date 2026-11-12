#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using BioTrilha.Cards;

namespace BioTrilha.EditorTools
{
    /// <summary>
    /// Menu: BioTrilha/Cards/Gerar Cartas de Sorte ou Reves
    /// Configura os sprites importados, cria os 26 ScriptableObjects e o prefab LuckSetbackCard.
    /// É idempotente: rodar de novo atualiza os assets existentes.
    /// </summary>
    public static class LuckSetbackCardGenerator
    {
        private const string ArtRoot = "Assets/BioTrilha/Art/2D/Cards/LuckSetback";
        private const string BackPath = ArtRoot + "/Back/SorteOuReves_Back.png";
        private const string DataFolder = "Assets/BioTrilha/SciptableObjects/Cards/LuckSetback";
        private const string PrefabPath = "Assets/BioTrilha/Prefabs/Cards/LuckSetbackCard.prefab";

        // Proporção real da arte exportada do Canva: 871 x 1669.
        private const float CardAspect = 871f / 1669f; // ~0.522
        private const float PrefabHeight = 400f;

        // id, nome do arquivo (sem acento), título exibido, tipo (impresso na carta)
        private static readonly (int id, string file, string title, LuckSetbackKind kind)[] Cards =
        {
            (1,  "ImpactosClimaticos", "Impactos Climáticos", LuckSetbackKind.Setback),
            (2,  "InvestimentoSustentavelBemSucedido", "Investimento Sustentável Bem-sucedido", LuckSetbackKind.Luck),
            (3,  "Biopirataria", "Biopirataria", LuckSetbackKind.Setback),
            (4,  "SubsidioDoGoverno", "Subsídio do Governo", LuckSetbackKind.Luck),
            (5,  "PerdaDeColheitaPorPragas", "Perda de Colheita por Pragas", LuckSetbackKind.Setback),
            (6,  "RastreabilidadeDigitalAbreNovosMercados", "Rastreabilidade Digital Abre Novos Mercados", LuckSetbackKind.Luck),
            (7,  "SeloDeCertificacaoSustentavel", "Selo de Certificação Sustentável", LuckSetbackKind.Luck),
            (8,  "RestauracaoDeAreaDegradada", "Restauração de Área Degradada", LuckSetbackKind.Luck),
            (9,  "MercadoLocalAquecido", "Mercado Local Aquecido", LuckSetbackKind.Luck),
            (10, "ErosaoEDegradacaoDoSolo", "Erosão e Degradação do Solo", LuckSetbackKind.Setback),
            (11, "FaltaDeMaoDeObraQualificada", "Falta de Mão de Obra Qualificada", LuckSetbackKind.Setback),
            (12, "AltaNoCustoDosInsumos", "Alta no Custo dos Insumos", LuckSetbackKind.Setback),
            (13, "QuedaDePrecoNoMercado", "Queda de Preço no Mercado", LuckSetbackKind.Setback),
            (14, "IncidenteAmbiental", "Incidente Ambiental", LuckSetbackKind.Setback),
            (15, "NovaDescobertaCientifica", "Nova descoberta científica", LuckSetbackKind.Luck),
            (16, "CriseHidrica", "Crise Hídrica", LuckSetbackKind.Setback),
            (17, "ExportacaoRecorde", "Exportação Recorde", LuckSetbackKind.Luck),
            (18, "MultaPorDesperdicioDeRecursos", "Multa por Desperdício de Recursos", LuckSetbackKind.Setback),
            (19, "CreditosDeCarbonoVendidos", "Créditos de Carbono Vendidos", LuckSetbackKind.Luck),
            (20, "ParceriaComComunidadeExtrativista", "Parceria com Comunidade Extrativista", LuckSetbackKind.Luck),
            (21, "CapacitacaoTecnicaDaEquipe", "Capacitação Técnica da Equipe", LuckSetbackKind.Luck),
            (22, "ApoioDaRedeDeProdutores", "Apoio da Rede de Produtores", LuckSetbackKind.Luck),
            (23, "RecallDeLoteContaminado", "Recall de Lote Contaminado", LuckSetbackKind.Setback),
            (24, "RompimentoDeContratoComercial", "Rompimento de Contrato Comercial", LuckSetbackKind.Setback),
            (25, "QuebraDoBiodigestor", "Quebra do Biodigestor", LuckSetbackKind.Setback),
            (26, "EmbargoPorDesmatamento", "Embargo por Desmatamento", LuckSetbackKind.Setback),
        };

        // Confirmado pelo autor: só a 15 (Nova descoberta científica) é guardada; só a 16 (Crise Hídrica) atinge todos.
        private const int KeepCardId = 15;
        private const int AllPlayersCardId = 16;

        [MenuItem("BioTrilha/Cards/Gerar Cartas de Sorte ou Reves")]
        public static void Generate()
        {
            EnsureFolder(DataFolder);
            EnsureFolder(Path.GetDirectoryName(PrefabPath).Replace('\\', '/'));

            ConfigureSprite(BackPath);
            var back = AssetDatabase.LoadAssetAtPath<Sprite>(BackPath);
            if (back == null)
            {
                Debug.LogError($"[LuckSetbackCardGenerator] Verso não encontrado em '{BackPath}'.");
                return;
            }

            int created = 0, updated = 0;
            foreach (var c in Cards)
            {
                string frontPath = $"{ArtRoot}/Front/{c.file}_Front.png";
                ConfigureSprite(frontPath);

                var front = AssetDatabase.LoadAssetAtPath<Sprite>(frontPath);
                if (front == null)
                {
                    Debug.LogError($"[LuckSetbackCardGenerator] Sprite não encontrado para '{c.file}'.");
                    continue;
                }

                string assetPath = $"{DataFolder}/SorteReves_{c.id:00}_{c.file}.asset";
                var data = AssetDatabase.LoadAssetAtPath<LuckSetbackCardData>(assetPath);
                bool isNew = data == null;
                if (isNew) data = ScriptableObject.CreateInstance<LuckSetbackCardData>();

                data.id = c.id;
                data.displayName = c.title;
                data.kind = c.kind;
                data.frontSprite = front;
                data.backSprite = back;
                data.keepCard = c.id == KeepCardId;
                data.targetAllPlayers = c.id == AllPlayersCardId;

                if (isNew) { AssetDatabase.CreateAsset(data, assetPath); created++; }
                else { EditorUtility.SetDirty(data); updated++; }
            }

            AssetDatabase.SaveAssets();
            CreatePrefab();
            AssetDatabase.Refresh();
            Debug.Log($"[LuckSetbackCardGenerator] ScriptableObjects criados: {created}, atualizados: {updated}. Prefab: {PrefabPath}");
        }

        private static void ConfigureSprite(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.maxTextureSize = 2048;
            importer.spritePixelsPerUnit = 100;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            importer.SetTextureSettings(settings);

            importer.SaveAndReimport();
        }

        private static void CreatePrefab()
        {
            var root = new GameObject("LuckSetbackCard", typeof(RectTransform), typeof(Image),
                typeof(AspectRatioFitter), typeof(LuckSetbackCardView));

            var rect = (RectTransform)root.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f); // âncoras fixas: não deforma
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(PrefabHeight * CardAspect, PrefabHeight);

            var image = root.GetComponent<Image>();
            image.preserveAspect = true;
            image.raycastTarget = true;

            var fitter = root.GetComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            fitter.aspectRatio = CardAspect;

            // Pré-visualiza o verso comum no prefab.
            image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackPath);

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}
#endif
