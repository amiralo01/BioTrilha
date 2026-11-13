#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using BioTrilha.Cards;

namespace BioTrilha.EditorTools
{
    /// <summary>
    /// Menu: BioTrilha/Cards/Gerar Cartas de Questoes
    /// Configura os sprites importados, cria os 47 ScriptableObjects e o prefab QuestionCard.
    /// É idempotente: rodar de novo atualiza os assets existentes.
    /// </summary>
    public static class QuestionCardGenerator
    {
        private const string ArtRoot = "Assets/BioTrilha/Art/2D/Cards/Questions";
        private const string BackPath = ArtRoot + "/Back/Questoes_Back.png";
        private const string DataFolder = "Assets/BioTrilha/SciptableObjects/Cards/Questions";
        private const string PrefabPath = "Assets/BioTrilha/Prefabs/Cards/QuestionCard.prefab";

        // Proporção real da arte exportada do Canva: ~817 x 1568.
        private const float CardAspect = 817f / 1568f; // ~0.521
        private const float PrefabHeight = 400f;

        // O painel que cobre a resposta e os resultados começa logo abaixo da linha divisória
        // (que termina em y ~ 677 de 1568 px) e vai até a base da carta.
        private const float CoverTopFromTop = 680f / 1568f;
        private static readonly Color32 CardYellow = new Color32(255, 205, 5, 255);

        // Confirmado pelo autor: só a carta 2 (Sobre a Biopirataria) é guardada ao acertar.
        private const int KeepOnCorrectId = 2;

        // id, nome do arquivo (sem acento), título impresso, resposta é "Sim", pergunta, resultado ao acertar, resultado ao errar
        private static readonly (int id, string file, string title, bool answerIsYes, string question, string correct, string wrong)[] Cards =
        {
            (1, "SobreOAcai", @"Sobre o Açaí", true, @"O açaí é um produto valioso no cenário da bioeconomia por originar subprodutos como medicamentos, cosméticos, óleos e bebidas?", @"O jogador poderá trocar um produto seu com o jogador anterior.", @"Dê 20.000 do seu dinheiro para o próximo jogador."),
            (2, "SobreABiopirataria", @"Sobre a Biopirataria", false, @"A biopirataria envolve apenas o contrabando de animais silvestres, sem relação com plantas ou com conhecimentos tradicionais?", @"Guarde esta carta: quando for para a Penitenciária, você poderá sair imediatamente.", @"Dê um dos seus produtos para o jogador anterior."),
            (3, "SobreOOleoDeSojaRefinado", @"Sobre o Óleo de Soja Refinado", false, @"O óleo de soja refinado é obtido apenas por prensagem mecânica, sem necessidade de refinamento industrial?", @"Receba 15.000 por investir em informações corretas sobre a produção.", @"Pague 15.000 por espalhar desinformação sobre o processo produtivo."),
            (4, "SobreABiorremediacao", @"Sobre a Biorremediação", true, @"A biorremediação utiliza organismos vivos para descontaminar áreas poluídas?", @"Receba 20.000 por apoiar soluções sustentáveis na recuperação ambiental.", @"Pague 15.000 por ignorar práticas sustentáveis de descontaminação."),
            (5, "SobreACamomila", @"Sobre a Camomila", false, @"A produção de camomila no Brasil é concentrada em grandes fazendas, sem participação da agricultura familiar?", @"Você valoriza os pequenos produtores. Receba 20.000.", @"Você desconhece quem produz as ervas medicinais. Pague 15.000."),
            (6, "SobreAsEmbalagensBiodegradaveis", @"Sobre as Embalagens Biodegradáveis", true, @"O uso de embalagens biodegradáveis pode agregar valor aos produtos e aumentar sua aceitação no mercado?", @"Receba 20.000 por atender à demanda dos consumidores por produtos sustentáveis.", @"Pague 15.000 por perder espaço no mercado ao ignorar a procura por embalagens sustentáveis."),
            (7, "SobreACarneBovina", @"Sobre a Carne Bovina", false, @"A pecuária bovina impacta apenas positivamente a economia, sem efeitos ambientais?", @"Receba 20.000 por investir em práticas sustentáveis na pecuária.", @"Pague 30.000 por prejuízos causados por desmatamento irregular."),
            (8, "SobreOCarvaoVegetal", @"Sobre o Carvão Vegetal", false, @"Todo carvão vegetal usado no Brasil vem de florestas plantadas e é obtido de maneira sustentável?", @"Receba 20.000 por investir em fornecedores certificados.", @"Pague 30.000 em multas ambientais devido ao uso de carvão irregular."),
            (9, "SobreOTijoloDeAdobe", @"Sobre o Tijolo de Adobe", true, @"Tijolos de adobe, feitos de terra crua e secos ao sol, têm baixo custo e dispensam a queima em forno durante a fabricação?", @"Receba 20.000 pela economia com materiais sustentáveis na sua construção.", @"Pague 15.000 por desperdício de recursos na obra."),
            (10, "SobreAAgricultura", @"Sobre a Agricultura", false, @"Toda a produção agrícola é considerada sustentável dentro da bioeconomia?", @"Receba um incentivo de 25.000 por práticas agrícolas sustentáveis.", @"Pague 20.000 devido a impactos ambientais negativos na produção."),
            (11, "SobreIncidenteAmbiental", @"Sobre Incidente Ambiental", false, @"Empresas e comunidades que dependem de recursos naturais, como a pesca, ficam livres de prejuízos econômicos quando ocorre um incidente ambiental?", @"Receba um incentivo de 40.000 por implementar medidas preventivas contra desastres ambientais.", @"Pague 50.000 devido a sanções ambientais e danos à reputação da sua empresa."),
            (12, "SobreSistemasAgroflorestais", @"Sobre Sistemas Agroflorestais", true, @"Os sistemas agroflorestais, que combinam cultivos agrícolas com árvores na mesma área, podem ajudar a recuperar áreas degradadas e diversificar a renda do agricultor?", @"Sua produção ficou mais diversificada e o solo foi recuperado. Receba 25.000.", @"Você insistiu na monocultura e seu solo se degradou. Pague 20.000."),
            (13, "SobreOMaracuja", @"Sobre o Maracujá", false, @"O cultivo do maracujá no Brasil é restrito a grandes produtores, sem impacto na agricultura familiar?", @"Você reconhece a importância do maracujá na agricultura familiar. Receba 20.000.", @"Sua falta de conhecimento sobre a produção sustentável afeta seus investimentos. Pague 15.000."),
            (14, "SobreALendaDaMandioca", @"Sobre a Lenda da Mandioca", true, @"Segundo uma lenda indígena tupi, a mandioca nasceu onde foi enterrada a menina Mani, e seu nome viria da junção de ""Mani"" com ""oca""?", @"Você valoriza o conhecimento tradicional e cultural. Receba 15.000.", @"Sua falta de conexão com as raízes culturais impacta sua reputação no mercado. Pague 10.000."),
            (15, "SobreACriseHidrica", @"Sobre a Crise Hídrica", true, @"A crise hídrica pode impactar negativamente a bioeconomia ao reduzir a produção agrícola e elevar custos na agroindústria?", @"Você adotou técnicas de irrigação eficiente e reduziu perdas. Receba 35.000 em incentivos governamentais.", @"Sua produção sofreu grandes prejuízos. Pague 45.000 para cobrir os custos adicionais."),
            (16, "SobreOOleoDeCoco", @"Sobre o Óleo de Coco", true, @"O óleo de coco é rico em ácido láurico, componente usado na fabricação de cosméticos, sabões e detergentes?", @"Você investiu na produção sustentável de derivados do óleo de coco. Avance 2 casas.", @"Você ignorou os benefícios industriais do óleo de coco e perdeu oportunidades de mercado. Devolva ao baralho uma carta de produto."),
            (17, "SobreAFixacaoBiologicaDeNitrogenio", @"Sobre a Fixação Biológica de Nitrogênio", true, @"Bactérias que vivem nas raízes da soja conseguem fornecer o nitrogênio de que a planta precisa, dispensando o adubo nitrogenado?", @"Você economizou com fertilizantes. Receba 30.000.", @"Você gastou com adubo desnecessário. Pague 20.000."),
            (18, "SobreOsBioplasticos", @"Sobre os Bioplásticos", false, @"Todo bioplástico, por ser feito de fontes renováveis, é automaticamente biodegradável?", @"Você entende a diferença entre origem renovável e biodegradação. Receba 20.000.", @"Você comprou embalagens pensando que se decompunham e elas foram para o lixão. Pague 20.000."),
            (19, "SobreOPlasticoPLA", @"Sobre o Plástico PLA", false, @"O plástico PLA, feito de milho ou cana, se decompõe rapidamente quando descartado no oceano?", @"Você sabe que o descarte correto é essencial. Avance 1 casa.", @"O descarte errado gerou poluição marinha. Pague 25.000."),
            (20, "SobreABioprospeccao", @"Sobre a Bioprospecção", true, @"Para pesquisar plantas, animais ou microrganismos da biodiversidade brasileira, a lei exige cadastro e a divisão dos benefícios gerados?", @"Você pesquisou a biodiversidade dentro da lei. Receba 25.000.", @"Sua pesquisa foi feita sem cadastro e acabou suspensa. Pague 25.000."),
            (21, "SobreAProducaoDeAcai", @"Sobre a Produção de Açaí", true, @"O estado do Pará é responsável pela maior parte da produção de açaí do Brasil?", @"Você conhece a força da bioeconomia amazônica. Receba 20.000.", @"Você subestimou a produção paraense. Pague 15.000."),
            (22, "SobreACarneDePeixe", @"Sobre a Carne de Peixe", true, @"O tambaqui, peixe nativo da Amazônia, é a espécie nativa mais criada na piscicultura brasileira?", @"Você conhece o potencial da piscicultura amazônica. Receba 20.000.", @"Você ignorou uma espécie nativa de alto valor. Pague 15.000."),
            (23, "SobreOCacau", @"Sobre o Cacau", true, @"O Pará se destaca como um dos maiores produtores de cacau do Brasil?", @"Você conhece o potencial do cacau amazônico. Receba 25.000.", @"Você perdeu uma oportunidade de negócio. Pague 20.000."),
            (24, "SobreOOleoDePalma", @"Sobre o Óleo de Palma", false, @"A produção de dendê (óleo de palma) no Brasil está distribuída igualmente entre todas as regiões do país?", @"Você sabe onde a bioeconomia do dendê acontece. Receba 20.000.", @"Você errou a região e seu investimento foi mal planejado. Pague 20.000."),
            (25, "SobreADomesticacaoDaMandioca", @"Sobre a Domesticação da Mandioca", true, @"A mandioca foi domesticada pelos povos originários na Amazônia há milhares de anos?", @"Você reconhece o saber dos povos originários. Receba 20.000.", @"Você ignorou a origem amazônica da mandioca. Fique 1 rodada sem jogar."),
            (26, "SobreOControleBiologico", @"Sobre o Controle Biológico", true, @"Alguns fungos podem ser aplicados na lavoura para proteger as plantas contra doenças, reduzindo o uso de agrotóxicos químicos?", @"Você apostou no controle biológico e protegeu sua lavoura. Receba 20.000.", @"Você exagerou nos agrotóxicos e prejudicou o solo. Pague 20.000."),
            (27, "SobreAVacinaBCG", @"Sobre a Vacina BCG", true, @"A vacina BCG, aplicada logo após o nascimento, é produzida a partir de uma bactéria enfraquecida e protege contra formas graves da tuberculose?", @"Você reconhece a biotecnologia na saúde pública. Receba 25.000.", @"Você desconhece uma das vacinas mais antigas do calendário. Pague 20.000."),
            (28, "SobreAMoringa", @"Sobre a Moringa", true, @"As sementes de moringa podem ser usadas para ajudar a limpar água turva, funcionando como um coagulante natural?", @"Você conhece uma solução natural e barata para a água. Avance 2 casas.", @"Você usou produtos químicos sem necessidade. Pague 20.000."),
            (29, "SobreOMetano", @"Sobre o Metano", false, @"O gás metano, liberado por resíduos orgânicos em decomposição, aquece o planeta menos do que o gás carbônico (CO₂)?", @"Você entende por que capturar o metano é importante. Receba 25.000.", @"Você deixou o metano escapar para a atmosfera. Pague 30.000."),
            (30, "SobreOBiogas", @"Sobre o Biogás", true, @"O biogás é formado quando microrganismos decompõem a matéria orgânica na ausência de oxigênio?", @"Você domina o princípio da biodigestão. Receba 20.000.", @"Seu biodigestor foi mal operado. Pague 15.000."),
            (31, "SobreOBiodigestor", @"Sobre o Biodigestor", false, @"O biodigestor produz apenas gás, sem gerar nenhum outro produto aproveitável?", @"Você sabe que ele também gera biofertilizante. Pegue uma carta de produto do monte com o nível que tirar no dado.", @"Você descartou o biofertilizante e desperdiçou recursos. Pague 20.000."),
            (32, "SobreOEtanolDeCanaDeAcucar", @"Sobre o Etanol de Cana-de-Açúcar", true, @"O Brasil criou, em 1975, um programa nacional para substituir combustíveis do petróleo pelo álcool da cana-de-açúcar?", @"Você conhece a história da bioenergia brasileira. Receba 30.000.", @"Você desconhece o pioneirismo do país. Pague 20.000."),
            (33, "SobreOEtanolDeSegundaGeracao", @"Sobre o Etanol de Segunda Geração", false, @"Para produzir etanol de segunda geração (2G) é necessário plantar novas áreas de cana-de-açúcar?", @"Você sabe que ele usa bagaço e palha que sobram da produção. Avance 2 casas.", @"Você propôs desmatar para plantar mais cana. Pague 35.000."),
            (34, "SobreOBiodieselDeSoja", @"Sobre o Biodiesel de Soja", true, @"O óleo de soja é a principal matéria-prima usada na produção de biodiesel no Brasil?", @"Você conhece a cadeia do biodiesel. Receba 25.000.", @"Você errou a matéria-prima e comprou o insumo errado. Pague 20.000."),
            (35, "SobreOBiodieselNosPostos", @"Sobre o Biodiesel nos Postos", true, @"O óleo diesel vendido nos postos brasileiros contém, por lei, uma parte de biodiesel misturada?", @"Você entende as políticas de biocombustíveis. Receba 20.000.", @"Você desconhece a lei e perdeu um contrato. Pague 20.000."),
            (36, "SobreAMadeiraDeMogno", @"Sobre a Madeira de Mogno", false, @"A extração e a venda do mogno nativo são livres na Amazônia brasileira?", @"Você respeita a proteção de espécies ameaçadas. Receba 30.000.", @"Você comprou madeira ilegal e foi autuado. Pague 40.000."),
            (37, "SobreAsTelhasDeCortica", @"Sobre as Telhas de Cortiça", false, @"Para obter a cortiça é necessário derrubar a árvore?", @"Você conhece um dos materiais mais renováveis do mundo. Receba 25.000.", @"Você defendeu uma prática que destruiria a floresta. Pague 20.000."),
            (38, "SobreOPapel", @"Sobre o Papel", true, @"No Brasil, todo o papel produzido vem de árvores plantadas para essa finalidade, e não de florestas nativas?", @"Você entende a cadeia sustentável do papel. Receba 20.000.", @"Você acreditou que o papel vem do desmatamento. Pague 15.000."),
            (39, "SobreAMadeiraDeEucalipto", @"Sobre a Madeira de Eucalipto", true, @"O eucalipto é a árvore mais plantada nas florestas cultivadas do Brasil e pode ser colhido em cerca de 7 a 8 anos?", @"Você conhece o manejo de florestas plantadas. Receba 20.000.", @"Você errou o ciclo da floresta e atrasou a colheita. Fique 1 rodada sem jogar."),
            (40, "SobreOLodoDeEsgoto", @"Sobre o Lodo de Esgoto", false, @"O lodo de esgoto pode ser aplicado na lavoura sem nenhum tratamento ou controle?", @"Você respeita as normas de segurança ambiental. Receba 25.000.", @"Você contaminou o solo com patógenos. Pague 35.000."),
            (41, "SobreOsAlimentosBiofortificados", @"Sobre os Alimentos Biofortificados", true, @"A batata-doce de polpa alaranjada é mais rica em vitamina A do que a batata-doce de polpa branca?", @"Você escolheu o alimento mais nutritivo para a sua produção. Receba 25.000.", @"Você ignorou o valor nutricional do alimento biofortificado. Pague 20.000."),
            (42, "SobreAFomeOculta", @"Sobre a Fome Oculta", true, @"A biofortificação busca combater a falta de ferro, zinco e vitamina A na alimentação da população?", @"Você entende a bioeconomia a serviço da saúde. Receba 30.000.", @"Você ignorou a segurança alimentar. Pague 20.000."),
            (43, "SobreOPlantioDireto", @"Sobre o Plantio Direto", true, @"Manter a palha da colheita anterior sobre o solo, como no plantio direto, ajuda a reduzir a erosão?", @"Seu solo ficou protegido. Receba 25.000.", @"A chuva levou a camada fértil do seu solo. Pague 25.000."),
            (44, "SobreOSabaoNatural", @"Sobre o Sabão Natural", true, @"O óleo de cozinha usado pode ser reciclado e transformado em sabão, evitando a contaminação de grandes quantidades de água?", @"Você reaproveitou um resíduo e protegeu os rios. Receba 20.000.", @"Você jogou óleo no ralo e poluiu a água. Pague 25.000."),
            (45, "SobreOComercioDoCumaru", @"Sobre o Comércio do Cumaru", true, @"Municípios paraenses como Santarém e Alenquer comercializam sementes de cumaru para o Brasil e para outros países?", @"Você conhece a bioeconomia da sua região. Avance 2 casas.", @"Você ignorou um mercado local promissor. Pague 20.000."),
            (46, "SobreARastreabilidade", @"Sobre a Rastreabilidade", true, @"Para exportar alguns produtos à União Europeia, é preciso informar a localização geográfica das áreas onde foram produzidos?", @"Você preparou sua produção para o mercado global. Receba 25.000.", @"Você não tinha os dados de origem e perdeu o comprador. Pague 25.000."),
            (47, "SobreATransparenciaNoConsumo", @"Sobre a Transparência no Consumo", true, @"Pesquisas mostram que muitos consumidores aceitam pagar mais por produtos com origem transparente?", @"Você entende o valor da confiança do consumidor. Receba 20.000.", @"Você escondeu a origem e perdeu clientes. Pague 20.000."),
        };

        [MenuItem("BioTrilha/Cards/Gerar Cartas de Questoes")]
        public static void Generate()
        {
            EnsureFolder(DataFolder);
            EnsureFolder(Path.GetDirectoryName(PrefabPath).Replace('\\', '/'));

            ConfigureSprite(BackPath);
            var back = AssetDatabase.LoadAssetAtPath<Sprite>(BackPath);
            if (back == null)
            {
                Debug.LogError($"[QuestionCardGenerator] Verso não encontrado em '{BackPath}'.");
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
                    Debug.LogError($"[QuestionCardGenerator] Sprite não encontrado para '{c.file}'.");
                    continue;
                }

                string assetPath = $"{DataFolder}/Questao_{c.id:00}_{c.file}.asset";
                var data = AssetDatabase.LoadAssetAtPath<QuestionCardData>(assetPath);
                bool isNew = data == null;
                if (isNew) data = ScriptableObject.CreateInstance<QuestionCardData>();

                data.id = c.id;
                data.displayName = c.title;
                data.frontSprite = front;
                data.backSprite = back;
                data.questionText = c.question;
                data.answerIsYes = c.answerIsYes;
                data.correctOutcomeText = c.correct;
                data.wrongOutcomeText = c.wrong;
                data.keepCardOnCorrect = c.id == KeepOnCorrectId;

                if (isNew) { AssetDatabase.CreateAsset(data, assetPath); created++; }
                else { EditorUtility.SetDirty(data); updated++; }
            }

            AssetDatabase.SaveAssets();
            CreatePrefab();
            AssetDatabase.Refresh();
            Debug.Log($"[QuestionCardGenerator] ScriptableObjects criados: {created}, atualizados: {updated}. Prefab: {PrefabPath}");
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
            var root = new GameObject("QuestionCard", typeof(RectTransform), typeof(Image), typeof(Mask),
                typeof(AspectRatioFitter), typeof(QuestionCardView));

            var rect = (RectTransform)root.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f); // âncoras fixas: não deforma
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(PrefabHeight * CardAspect, PrefabHeight);

            var image = root.GetComponent<Image>();
            image.preserveAspect = true;
            image.raycastTarget = true;
            image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackPath); // começa virada para baixo

            // O Mask recorta o painel pelo formato arredondado da carta.
            root.GetComponent<Mask>().showMaskGraphic = true;

            var fitter = root.GetComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            fitter.aspectRatio = CardAspect;

            // Painel que cobre a resposta e os resultados (da linha divisória até a base).
            var cover = new GameObject("AnswerCover", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            cover.transform.SetParent(root.transform, false);
            var coverRect = (RectTransform)cover.transform;
            coverRect.anchorMin = new Vector2(0f, 0f);
            coverRect.anchorMax = new Vector2(1f, 1f - CoverTopFromTop);
            coverRect.offsetMin = Vector2.zero;
            coverRect.offsetMax = Vector2.zero;
            var coverImage = cover.GetComponent<Image>();
            coverImage.color = CardYellow;
            coverImage.raycastTarget = false;
            cover.SetActive(false);

            var view = root.GetComponent<QuestionCardView>();
            var so = new SerializedObject(view);
            so.FindProperty("faceImage").objectReferenceValue = image;
            so.FindProperty("answerCover").objectReferenceValue = cover;
            so.ApplyModifiedPropertiesWithoutUndo();

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
