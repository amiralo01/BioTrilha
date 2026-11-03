#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using BioTrilha.Cards;

namespace BioTrilha.EditorTools
{
    /// <summary>
    /// Menu: BioTrilha/Cards/Gerar Cartas de Ação
    /// Configura os sprites importados, cria os 36 ScriptableObjects e o prefab ActionCard.
    /// É idempotente: rodar de novo atualiza os assets existentes.
    /// </summary>
    public static class ActionCardGenerator
    {
        private const string ArtRoot = "Assets/BioTrilha/Art/2D/Cards/Actions";
        private const string DataFolder = "Assets/BioTrilha/SciptableObjects/Cards/Actions";
        private const string PrefabPath = "Assets/BioTrilha/Prefabs/Cards/ActionCard.prefab";

        // Proporção real da arte exportada do Canva: 609 x 1168.
        private const float CardAspect = 609f / 1168f; // ~0.521
        private const float PrefabHeight = 400f;

        // id, nome do arquivo (sem acento), título exibido, preço de compra (venda = metade)
        private static readonly (int id, string file, string title, int buy)[] Cards =
        {
            (1,  "Agricultura", "Agricultura", 150000),
            (2,  "Biorremediacao", "Biorremediação", 100000),
            (3,  "ProducaoDeBiocombustiveis", "Produção de Biocombustíveis", 150000),
            (4,  "AdubacaoComBiofertilizantesNaturais", "Adubação com Biofertilizantes Naturais", 100000),
            (5,  "BiogasDeResiduos", "Biogás de Resíduos", 150000),
            (6,  "CultivoDeAlimentosBiofortificados", "Cultivo de Alimentos Biofortificados", 200000),
            (7,  "CompostagemBasica", "Compostagem Básica", 20000),
            (8,  "AdocaoDeIluminacaoDeBaixoConsumo", "Adoção de Iluminação de Baixo Consumo", 20000),
            (9,  "LaboratorioComunitarioBasico", "Laboratório Comunitário Básico", 50000),
            (10, "PoliculturaSimples", "Policultura Simples", 20000),
            (11, "UsoDeBiodigestorDePequenoPorte", "Uso de Biodigestor de Pequeno Porte", 100000),
            (12, "SelecaoParticipativaDeSementes", "Seleção Participativa de Sementes", 50000),
            (13, "FixacaoBiologicaDeNitrogenio", "Fixação Biológica de Nitrogênio", 150000),
            (14, "BioeletricidadeDoBagacoDeCana", "Bioeletricidade do Bagaço de Cana", 200000),
            (15, "ManejoFlorestalSustentavel", "Manejo Florestal Sustentável", 20000),
            (16, "ProducaoDeEnzimasPorFermentacao", "Produção de Enzimas por Fermentação", 150000),
            (17, "BioprospeccaoDaBiodiversidadeAmazonica", "Bioprospecção da Biodiversidade Amazônica", 100000),
            (18, "MonitoramentoComBiossensoresAmbientais", "Monitoramento com Biossensores Ambientais", 50000),
            (19, "Pecuaria", "Pecuária", 200000),
            (20, "IrrigacaoInteligente", "Irrigação Inteligente", 200000),
            (21, "AplicacaoDeBiopesticidas", "Aplicação de Biopesticidas", 150000),
            (22, "SistemaAgroflorestal", "Sistema Agroflorestal", 150000),
            (23, "ProducaoDeEtanolE2G", "Produção de Etanol E2G", 200000),
            (24, "UsoDeBiodefensivosMicrobianos", "Uso de Biodefensivos Microbianos", 100000),
            (25, "UsoControladoDeAgrotoxicos", "Uso Controlado de Agrotóxicos", 50000),
            (26, "ViveiroSimplesDeMudas", "Viveiro Simples de Mudas", 50000),
            (27, "PlantioDiretoNaPalha", "Plantio Direto na Palha", 20000),
            (28, "UsoDeFogaoEcologicoALenha", "Uso de Fogão Ecológico a Lenha", 20000),
            (29, "ProducaoDeBriquetesDeBiomassa", "Produção de Briquetes de Biomassa", 50000),
            (30, "ProducaoCaseiraDeExtratosVegetais", "Produção Caseira de Extratos Vegetais", 50000),
            (31, "IntegracaoLavouraPecuariaFlorestaILPF", "Integração Lavoura-Pecuária-Floresta (ILPF)", 200000),
            (32, "CapturaDeBiometanoDeAterroSanitario", "Captura de Biometano de Aterro Sanitário", 150000),
            (33, "SubstituicaoDePlasticosConvencionais", "Substituição de Plásticos Convencionais", 200000),
            (34, "BiofabricaDeFarmacosEmCelulasVegetais", "Biofábrica de Fármacos em Células Vegetais", 200000),
            (35, "EdicaoGeneticaDeCultivosCRISPR", "Edição Genética de Cultivos (CRISPR)", 100000),
            (36, "ConservacaoDeGermoplasma", "Conservação de Germoplasma", 200000),
        };

        [MenuItem("BioTrilha/Cards/Gerar Cartas de Ação")]
        public static void Generate()
        {
            EnsureFolder(DataFolder);
            EnsureFolder(Path.GetDirectoryName(PrefabPath).Replace('\\', '/'));

            int created = 0, updated = 0;
            foreach (var c in Cards)
            {
                string frontPath = $"{ArtRoot}/Front/{c.file}_Front.png";
                string backPath = $"{ArtRoot}/Back/{c.file}_Back.png";
                ConfigureSprite(frontPath);
                ConfigureSprite(backPath);

                var front = AssetDatabase.LoadAssetAtPath<Sprite>(frontPath);
                var back = AssetDatabase.LoadAssetAtPath<Sprite>(backPath);
                if (front == null || back == null)
                {
                    Debug.LogError($"[ActionCardGenerator] Sprite não encontrado para '{c.file}'.");
                    continue;
                }

                string assetPath = $"{DataFolder}/Acao_{c.id:00}_{c.file}.asset";
                var data = AssetDatabase.LoadAssetAtPath<ActionCardData>(assetPath);
                bool isNew = data == null;
                if (isNew) data = ScriptableObject.CreateInstance<ActionCardData>();

                data.id = c.id;
                data.displayName = c.title;
                data.frontSprite = front;
                data.backSprite = back;
                data.buyPrice = c.buy;
                data.sellPrice = c.buy / 2;

                if (isNew) { AssetDatabase.CreateAsset(data, assetPath); created++; }
                else { EditorUtility.SetDirty(data); updated++; }
            }

            AssetDatabase.SaveAssets();
            CreatePrefab();
            AssetDatabase.Refresh();
            Debug.Log($"[ActionCardGenerator] ScriptableObjects criados: {created}, atualizados: {updated}. Prefab: {PrefabPath}");
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
            var root = new GameObject("ActionCard", typeof(RectTransform), typeof(Image),
                typeof(AspectRatioFitter), typeof(ActionCardView));

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

            // Pré-visualiza a primeira carta no prefab.
            var first = AssetDatabase.LoadAssetAtPath<ActionCardData>(
                $"{DataFolder}/Acao_01_{Cards[0].file}.asset");
            if (first != null) image.sprite = first.frontSprite;

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
