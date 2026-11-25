#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using BioTrilha.Cards;

namespace BioTrilha.EditorTools
{
    /// <summary>
    /// Menu: BioTrilha/Cards/Gerar Cartas de Produto
    /// Configura os sprites importados, cria os 75 ScriptableObjects e o prefab ProductCard.
    /// É idempotente: rodar de novo atualiza os assets existentes.
    /// Arquivo gerado a partir da página 5 do Canva ("CARTAS DE PRODUTO"); categorias pela
    /// lista "Produtos bioeconômicos de cada tipo".
    /// </summary>
    public static class ProductCardGenerator
    {
        private const string ArtRoot = "Assets/BioTrilha/Art/2D/Cards/Products";
        private const string DataFolder = "Assets/BioTrilha/SciptableObjects/Cards/Products";
        private const string PrefabPath = "Assets/BioTrilha/Prefabs/Cards/ProductCard.prefab";

        // Arte renderizada do PDF do Canva em 960 x 1840.
        private const float CardAspect = 960f / 1840f; // ~0.522
        private const float PrefabHeight = 400f;

        private struct Row
        {
            public int id; public string file, title; public int stars; public ProductCategory category;
            public string description, subproducts;
            public Row(int id, string file, string title, int stars, ProductCategory category, string description, string subproducts)
            {
                this.id = id; this.file = file; this.title = title; this.stars = stars; this.category = category;
                this.description = description; this.subproducts = subproducts;
            }
        }

        private static readonly Row[] Cards =
        {
            new Row(1, "Acai", @"Açaí", 4, ProductCategory.AlimentosEBebidas,
                @"O açaí é um fruto da palmeira Euterpe oleracea, nativa da Amazônia. É um alimento nutritivo, rico em vitaminas, minerais e antioxidantes. O açaí é importante para a bioeconomia porque pode ser usado para produzir uma variedade de produtos, como polpa, óleo, farinha e bebidas. O açaí pode ser encontrado na Amazônia, mas também é cultivado em outras regiões do Brasil e do mundo.

Conto: A lenda do açaí
A lenda diz que os frutos do açaí são lágrimas de Iaçã, que chorou de alegria ao ver sua tribo salva da fome, por isso o significado de açaí é a fruta que chora. O açaí é um fruto sagrado para os povos indígenas da Amazônia e é uma importante fonte de alimento e renda para essas comunidades.",
                @"Sorvete
Polpa
Farinha
Casca
Medicamentos
Cosméticos
Óleo
Bebidas"),
            new Row(2, "BiodieselDeSoja", @"Biodiesel de Soja", 4, ProductCategory.Biocombustiveis,
                @"O biodiesel de soja é um biocombustível produzido a partir do óleo da soja por transesterificação. Amplamente utilizado nas regiões Centro-Oeste e Sul do Brasil, ele reduz a dependência de combustíveis fósseis e as emissões de CO₂. Sua importância na bioeconomia inclui o uso sustentável de recursos agrícolas e o aproveitamento de subprodutos.",
                @"Glicerina
Ácidos graxos
Alimentos
Borra de biodiesel"),
            new Row(3, "Bioplasticos", @"Bioplásticos", 4, ProductCategory.ProdutosQuimicosBiologicos,
                @"Os bioplásticos são materiais produzidos a partir de fontes renováveis, como amido de milho, cana-de-açúcar ou óleos vegetais, em vez de petróleo. Eles podem ser obtidos por meio de processos biotecnológicos, como fermentação e polimerização. No Brasil, eles são especialmente importantes nas indústrias agrícolas e embalagens sustentáveis. Na bioeconomia, os bioplásticos ajudam a reduzir a dependência de combustíveis fósseis, diminuindo a pegada de carbono e promovendo um ciclo de vida mais sustentável.",
                @"Sacolas compostáveis
Embalagens biodegradáveis
Filmes plásticos"),
            new Row(4, "Alecrim", @"Alecrim", 3, ProductCategory.CosmeticosEHigienePessoal,
                @"O alecrim é uma planta aromática utilizada tanto na culinária quanto na produção de óleos essenciais e cosméticos. Ele é cultivado em várias regiões do Brasil, especialmente no Sudeste e Nordeste, devido ao clima favorável. Na bioeconomia, é valorizado por ser um produto natural com múltiplos usos, desde alimentos a medicamentos. Seus benefícios incluem propriedades medicinais, antioxidantes, e o baixo impacto ambiental no cultivo.",
                @"Cremes Cosméticos
Sabonetes Artesanais
Produtos Farmacêuticos"),
            new Row(5, "Bambu", @"Bambu", 5, ProductCategory.MateriaisDeConstrucaoBiodegradaveis,
                @"O bambu é uma planta de crescimento rápido, conhecida por suas propriedades sustentáveis e versatilidade. Ele é cultivado em várias regiões do Brasil, especialmente no Nordeste e Sudeste, sendo utilizado em construções, móveis e na produção de papel e tecidos. Na bioeconomia, o bambu se destaca por ser renovável, exigir poucos recursos naturais para crescer e contribuir para a captura de carbono. Seus benefícios incluem a redução do desmatamento e a promoção de alternativas ecológicas.",
                @"Biocombustível
Móveis
Fibras para tecidos
Papel e celulose"),
            new Row(6, "EsponjaDeCelulose", @"Esponja de Celulose", 3, ProductCategory.LimpezaEDetergentesBiodegradaveis,
                @"A esponja de celulose é biodegradável, feita de fibras vegetais extraídas de celulose, com produção nas regiões Sul e Sudeste do Brasil. Importante para a bioeconomia, substitui esponjas sintéticas e reduz o impacto ambiental, sendo uma alternativa sustentável e renovável.",
                @"Esponjas cosméticas
Esponjas industriais
Esponjas de banho"),
            new Row(7, "Camomila", @"Camomila", 4, ProductCategory.FarmaceuticosESaude,
                @"A camomila é uma planta medicinal conhecida por suas propriedades calmantes e anti- inflamatórias. É cultivada em regiões de clima temperado no Brasil, com destaque para os estados do Sul e Sudeste. Na bioeconomia, ela é importante por ser utilizada na fabricação de chás, cosméticos e produtos farmacêuticos naturais, promovendo o uso sustentável de recursos naturais. Seus benefícios incluem o baixo impacto ambiental e o incentivo à produção orgânica.",
                @"Chá de camomila
Produtos fitoterápicos
Cremes cosméticos
Loções calmantes"),
            new Row(8, "Maracuja", @"Maracujá", 5, ProductCategory.AlimentosEBebidas,
                @"O maracujá é uma fruta tropical rica em vitaminas e antioxidantes, cultivada principalmente em regiões quentes do Brasil, como São Paulo e Bahia. Sua produção, realizada a partir de sementes ou mudas, é frequentemente integrada a sistemas agroecológicos que favorecem a biodiversidade. Na bioeconomia, o maracujá é uma alternativa sustentável, promovendo renda para pequenos agricultores e contribuindo para a saúde pública através de seus benefícios nutricionais.",
                @"Farinha de semente
Polpa de maracujá
Extrato de maracujá
Óleo de semente"),
            new Row(9, "EtanolDaCanaDeAcucar", @"Etanol da Cana de Açúcar", 4, ProductCategory.Biocombustiveis,
                @"O etanol de cana-de-açúcar é um produto da bioeconomia devido à sua origem renovável, derivada da cana-de-açúcar. Esse biocombustível sustentável contribui para a redução das emissões de gases de efeito estufa, promovendo a transição para fontes de energia mais limpas. Além disso, a produção de etanol gera oportunidades econômicas, impulsionando o setor agrícola e proporcionando alternativas energéticas viáveis.",
                @"Bebidas Alcoólicas
Biocombustíveis
Alimentos
Fertilizante Orgânico
Cosméticos"),
            new Row(10, "CarneBovina", @"Carne Bovina", 2, ProductCategory.AlimentosEBebidas,
                @"A carne bovina é proveniente da criação de gado, sendo obtida por meio da pecuária, uma das principais atividades econômicas do Brasil, especialmente em estados como Mato Grosso, Goiás e Minas Gerais. Ela é relevante para a bioeconomia por impulsionar o agronegócio, gerar empregos e contribuir para o PIB. Seus benefícios incluem o fornecimento de proteína animal, além de subprodutos utilizados na indústria alimentícia e farmacêutica.",
                @"Sebo (sabão e cosméticos)
Couro
Fertilizantes
Insulina bovina"),
            new Row(11, "EtanolDeBeterraba", @"Etanol de Beterraba", 3, ProductCategory.Biocombustiveis,
                @"O etanol de beterraba é um biocombustível produzido a partir da fermentação do açúcar presente na beterraba. Sua produção ocorre em regiões com clima favorável ao cultivo da beterraba, como no Sul do Brasil. Na bioeconomia, é importante por ser uma fonte renovável de energia, contribuindo para a redução das emissões de carbono e a diversificação da matriz energética. Seus benefícios incluem menor impacto ambiental e a possibilidade de aproveitamento de subprodutos agrícolas.",
                @"Mel residual
Melaço
Fertilizantes
Vinhaça"),
            new Row(12, "Cacau", @"Cacau", 5, ProductCategory.AlimentosEBebidas,
                @"O cacau é uma fruta tropical usada principalmente para a produção de chocolate. Cultivado a partir de árvores em climas úmidos, ele é amplamente produzido na região sul da Bahia e no Pará. Na bioeconomia, o cacau é importante por impulsionar economias locais de forma sustentável, preservando florestas e promovendo o agroflorestamento. Seus benefícios incluem a geração de renda para comunidades agrícolas e o estímulo a práticas sustentáveis de cultivo.",
                @"Chocolate
Manteiga de cacau
Pó de cacau
Licor de cacau"),
            new Row(13, "AcafraoDaTerra", @"Açafrão da Terra", 4, ProductCategory.FarmaceuticosESaude,
                @"O açafrão-da-terra, ou cúrcuma, é uma raiz utilizada como tempero e corante natural, além de possuir propriedades medicinais. Ele é cultivado principalmente em estados como Minas Gerais, Bahia e Pará. Na bioeconomia, o açafrão se destaca por ser uma alternativa sustentável a corantes sintéticos e por promover o desenvolvimento de produtos naturais com valor agregado. Seus benefícios incluem usos na alimentação, na saúde e na indústria cosmética.",
                @"Pomadas medicinais
Óleo essencial
Corante natural
Ingrediente para cosméticos"),
            new Row(14, "Mandioca", @"Mandioca", 4, ProductCategory.AlimentosEBebidas,
                @"A mandioca é uma raiz tuberosa amplamente cultivada no Brasil, especialmente nas regiões Norte e Nordeste. Ela é obtida por meio de plantio em solos bem drenados e seu cultivo é importante tanto para pequenos agricultores quanto para grandes produções. Na bioeconomia, a mandioca é valorizada por sua versatilidade na produção de alimentos e bioprodutos, contribuindo para a segurança alimentar e geração de renda. Seus benefícios incluem baixo custo de produção e ampla aplicação industrial e alimentícia.

Conto: A lenda da mandioca
A história de uma jovem indígena que deu à luz uma menina misteriosa. A criança adoeceu e morreu, sendo enterrada na oca. No lugar do sepultamento, cresceu a planta da mandioca, que se tornou alimento essencial para o povo indígena.",
                @"Etanol
Goma
Amido
Farinha de mandioca"),
            new Row(15, "Penicilina", @"Penicilina", 4, ProductCategory.FarmaceuticosESaude,
                @"A penicilina é um antibiótico produzido a partir do fungo Penicillium e é amplamente utilizada no tratamento de infecções bacterianas. No Brasil, é obtida principalmente através de processos de fermentação industrial, sendo essencial para a medicina moderna, pois ajuda a salvar vidas e reduzir a morbidade associada a doenças infecciosas. Sua produção e uso são fundamentais na bioeconomia, promovendo a saúde pública e diminuindo a necessidade de tratamentos mais complexos e caros.",
                @"Ácido 6-aminopenicilânico
Cristais de penicilina
Ácido Peniciloico
Biomassa de fungos"),
            new Row(16, "Leite", @"Leite", 2, ProductCategory.AlimentosEBebidas,
                @"O leite de vaca é um líquido nutritivo produzido pelas glândulas mamárias dos bovinos e obtido através da ordenha. No Brasil, destaca-se em estados como Minas Gerais e Paraná, principais produtores. Na bioeconomia, o leite é um alimento essencial, rico em proteínas e cálcio, e impulsiona indústrias de laticínios, gerando produtos de valor agregado como queijos e iogurtes.",
                @"Queijo
Iogurte
Manteiga
Doce de Leite"),
            new Row(17, "Batata", @"Batata", 3, ProductCategory.AlimentosEBebidas,
                @"É um tubérculo cultivado principalmente nas regiões Sul e Sudeste do Brasil, com destaque para Minas Gerais e Paraná. Obtida através de plantio de mudas ou sementes, é uma fonte importante de carboidratos e base alimentar em diversos pratos. Na bioeconomia, é valorizada por seu uso diversificado na alimentação e indústria, além de ser matéria-prima para produtos como amido e bioplásticos.",
                @"Fécula de Batata
Batata Palha
Purê Desidratado
Bioplástico de Amido"),
            new Row(18, "Arroz", @"Arroz", 3, ProductCategory.AlimentosEBebidas,
                @"O arroz é um cereal amplamente cultivado no Brasil, especialmente em estados como Rio Grande do Sul e Santa Catarina. Ele é produzido em sistemas de irrigação que demandam grandes quantidades de água. Na bioeconomia, o arroz é importante por ser um alimento básico, gerando emprego e renda para comunidades rurais e possibilitando a criação de subprodutos como farelo e óleo de arroz.",
                @"Etanol de Arroz
Farinha de Arroz
Óleo de Arroz
Sílica da Casca de Arroz"),
            new Row(19, "Cumaru", @"Cumaru", 4, ProductCategory.AlimentosEBebidas,
                @"O cumaru, também chamado de ""fava tonka"", é uma semente extraída de árvores nativas da Amazônia. Amplamente encontrado no Pará e Amazonas, é utilizado nas indústrias de cosméticos e perfumaria devido ao seu aroma intenso. Na bioeconomia, é valorizado pela exploração sustentável e pelo potencial de promover práticas agroflorestais, beneficiando comunidades locais.",
                @"Extrato aromático
Sabonetes
Incensos"),
            new Row(20, "Banana", @"Banana", 4, ProductCategory.AlimentosEBebidas,
                @"A banana é uma fruta cultivada principalmente em regiões tropicais e subtropicais, com grande produção no Brasil, especialmente no Nordeste e no Sudeste. Na bioeconomia, ela é importante por ser uma fonte de alimento nutritivo, com subprodutos que agregam valor, como farinha de banana e biomassa. Seu cultivo sustentável contribui para a economia local, gerando renda e emprego, além de ser uma opção de baixo impacto ambiental.",
                @"Álcool de Banana
Extrato de Banana
Biomassa de Banana Verde"),
            new Row(21, "AlgodaoAmericano", @"Algodão Americano", 3, ProductCategory.TexteisEVestuario,
                @"O algodão americano é uma variedade amplamente cultivada para produção de fibras e óleo, obtido por meio do plantio em regiões com clima quente e manejo agrícola eficiente. No Brasil, é muito utilizado no Cerrado, especialmente em estados como Mato Grosso e Bahia, grandes produtores. Na bioeconomia, destaca-se por sua renovabilidade e aplicação em tecidos, bioplásticos e produtos biodegradáveis, promovendo práticas agrícolas sustentáveis.",
                @"Fibras Têxteis
Óleo de Algodão
Linter"),
            new Row(22, "OleoDePalma", @"Óleo de Palma", 1, ProductCategory.AlimentosEBebidas,
                @"O óleo de palma, ou azeite de dendê, é um óleo vegetal extraído do fruto da palmeira de dendê (Elaeis guineensis), cultivada principalmente no Norte e Nordeste do Brasil. Utilizado na alimentação, cosméticos e biocombustíveis, é um produto versátil na bioeconomia. Apesar de suas aplicações, sua produção pode gerar impactos ambientais, destacando a importância de práticas sustentáveis.",
                @"Torta de Palmiste
Cosméticos
Glicerina
Sabões e Detergentes"),
            new Row(23, "CarvaoVegetal", @"Carvão Vegetal", 1, ProductCategory.Biomassa,
                @"O carvão vegetal é um material obtido pela queima controlada da madeira em fornos, sem oxigênio, um processo chamado carbonização. É amplamente utilizado no Brasil, especialmente em Minas Gerais e no Pará, como fonte de energia para siderúrgicas e na produção de aço. Na bioeconomia, destaca-se como alternativa ao carvão mineral, contribuindo para a redução de emissões de carbono quando produzido de forma sustentável.",
                @"Pó de Carvão
Carvão Ativado
Vinhaça de Carvão"),
            new Row(24, "OleoDeSojaRefinado", @"Óleo de Soja Refinado", 1, ProductCategory.AlimentosEBebidas,
                @"O óleo de soja refinado é um óleo vegetal extraído das sementes de soja, amplamente utilizado na culinária e na indústria alimentícia. É obtido por processos de prensagem e refinamento industrial. No Brasil, sua produção é significativa nas regiões Centro-Oeste e Sul, grandes produtoras de soja. Na bioeconomia, sua importância está na geração de renda e no aproveitamento de subprodutos, embora seu impacto ambiental possa ser alto sem práticas sustentáveis.",
                @"Glicerina
Biodiesel
Lecitina de Soja"),
            new Row(25, "OleoDeCoco", @"Óleo de Coco", 2, ProductCategory.CosmeticosEHigienePessoal,
                @"O Óleo de Coco é extraído da polpa do coco, geralmente por prensagem a frio ou aquecimento. Ele é bastante utilizado em diversas regiões do Brasil, especialmente no nordeste, em alimentos, cosméticos e produtos de higiene. Na bioeconomia, é valorizado por ser um produto renovável, com benefícios para a saúde e para a indústria de cosméticos, mas sua produção também exige cuidados devido aos impactos ambientais associados.",
                @"Ácido Caprílico
Ácido Láurico
Cocosol"),
            new Row(26, "BiodieselDeGirassol", @"Biodiesel de Girassol", 3, ProductCategory.Biocombustiveis,
                @"O biodiesel de girassol é um biocombustível produzido a partir do óleo extraído das sementes de girassol por transesterificação. É produzido principalmente em estados como Rio Grande do Sul e Paraná, onde o girassol é cultivado em larga escala. Na bioeconomia, reduz as emissões do transporte e agrega valor à cadeia do girassol, aproveitando a torta como ração animal.",
                @"Glicerina Vegetal
Torta de Girassol
Ácidos Graxos
Biolubrificantes"),
            new Row(27, "EtanolDeMilho", @"Etanol de Milho", 2, ProductCategory.Biocombustiveis,
                @"O etanol de milho é um biocombustível produzido pela fermentação do amido presente no milho. É obtido em usinas especializadas, principalmente em estados como Mato Grosso e Goiás, onde o cultivo de milho é expressivo. Na bioeconomia, destaca-se como alternativa aos combustíveis fósseis, contribuindo para a redução de emissões de carbono e gerando subprodutos como ração animal e dióxido de carbono para outras indústrias.",
                @"Fibra de Milho
Biogás
Amido Residual"),
            new Row(28, "PapelABaseDeAlgas", @"Papel à base de Algas", 4, ProductCategory.PapelECelulose,
                @"O papel à base de algas é produzido a partir de biomassa de algas, especialmente de espécies invasivas ou cultivadas de forma sustentável. No Brasil, seu uso é emergente, especialmente em projetos ecológicos e educacionais. É importante na bioeconomia por reduzir a dependência de celulose de árvores, combater algas invasivas e oferecer uma alternativa sustentável para a indústria do papel.",
                @"Embalagens Sustentáveis
Filtros Biodegradáveis
Material para impressão
Materiais Escolares"),
            new Row(29, "OleoDeAmendoas", @"Óleo de Amêndoas", 3, ProductCategory.CosmeticosEHigienePessoal,
                @"O óleo de amêndoas é um produto extraído das sementes de amendoeiras, obtido por prensagem. No Brasil, é mais comum em regiões de cultivo de amêndoas, sendo usado em cosméticos e produtos de saúde. Na bioeconomia, destaca-se por ser uma alternativa renovável e com aplicações versáteis, promovendo bem-estar com baixo impacto químico.",
                @"Cremes Hidratantes
Sabonetes Líquidos
Óleos Corporais
Bases para Maquiagem"),
            new Row(30, "Pimenta", @"Pimenta", 3, ProductCategory.FertilizantesOrganicosEBiopesticidas,
                @"A pimenta é uma especiaria obtida dos frutos de plantas do gênero Capsicum ou Piper, cultivada em climas tropicais. No Brasil, ela é amplamente utilizada em estados como Bahia e Pará, tanto na culinária quanto na indústria de alimentos. Na bioeconomia, a pimenta se destaca pelo alto valor agregado, sendo usada em produtos alimentícios, cosméticos e medicinais, além de estimular economias locais e a agricultura familiar.",
                @"Repelentes naturais
Molhos e pastas
Corantes
Medicamentos"),
            new Row(31, "Alho", @"Alho", 3, ProductCategory.FertilizantesOrganicosEBiopesticidas,
                @"O alho é uma planta cultivada principalmente por seus bulbos, utilizados como tempero e em tratamentos naturais. No Brasil, é especialmente cultivado em estados como São Paulo, Minas Gerais e Paraná. Na bioeconomia, o alho possui um papel relevante em sistemas agrícolas sustentáveis, com benefícios como o aumento da biodiversidade e o uso em medicamentos e cosméticos.",
                @"Pasta de Alho
Pó de Alho
Alho Fermentado
Extrato de Alho"),
            new Row(32, "Moringa", @"Moringa", 5, ProductCategory.FarmaceuticosESaude,
                @"A Moringa é uma planta tropical cujas folhas, sementes e raízes são consumidas por suas propriedades nutricionais e medicinais. Ela é cultivada principalmente no Nordeste do Brasil e outras regiões tropicais. Sua importância na bioeconomia está na produção sustentável de alimentos, suplementos naturais e cosméticos, contribuindo para a saúde e geração de emprego.",
                @"Chá de Moringa
Sabonetes e Cremes
Raiz de Moringa
Óleo de Moringa"),
            new Row(33, "Laranja", @"Laranja", 5, ProductCategory.AlimentosEBebidas,
                @"A laranja é uma fruta cítrica cultivada principalmente em regiões tropicais e subtropicais. No Brasil, ela é amplamente cultivada em estados como São Paulo, Minas Gerais e Bahia. Na bioeconomia, a laranja tem importância devido ao seu uso em sucos, óleos essenciais e na indústria alimentícia, além de seus benefícios nutricionais, como vitamina C e antioxidantes.",
                @"Suco de Laranja
Compotas e Geleias
Polpa de Laranja
Casca de Laranja"),
            new Row(34, "Lenha", @"Lenha", 3, ProductCategory.Biomassa,
                @"A lenha é madeira proveniente de árvores, utilizada principalmente como fonte de energia para aquecimento ou em fornos. No Brasil, é comum em áreas rurais e em algumas cidades para fins domésticos e comerciais, como nas cozinhas e nos sistemas de aquecimento. Na bioeconomia, a lenha sustentável é uma alternativa renovável para substituir combustíveis fósseis, reduzindo a emissão de gases poluentes e contribuindo para o manejo florestal responsável.",
                @"Biomassa
Óleo Essencial de Madeira
Carvão Vegetal"),
            new Row(35, "BiodieselDeCanola", @"Biodiesel de Canola", 3, ProductCategory.Biocombustiveis,
                @"O biodiesel de canola é um biocombustível produzido a partir do óleo extraído das sementes da planta de canola. Cultivado principalmente no Sul do Brasil, onde o clima é mais favorável, ele é usado como alternativa sustentável ao diesel fóssil. Na bioeconomia, reduz emissões de poluentes, promove energia renovável e agrega valor à cadeia agrícola, beneficiando o meio ambiente e a economia local.",
                @"Biolubrificantes
Glicerina Vegetal
Sabonetes
Energia Elétrica"),
            new Row(36, "Maca", @"Maçã", 4, ProductCategory.AlimentosEBebidas,
                @"A maçã é uma fruta de clima temperado, cultivada principalmente no sul do Brasil, com destaque para os estados de Santa Catarina e Rio Grande do Sul. Obtida de pomares comerciais, requer manejo agrícola cuidadoso para manter a qualidade. Na bioeconomia, a maçã é importante por gerar subprodutos diversos e promover o agronegócio sustentável quando são usadas práticas de manejo responsáveis, beneficiando a economia e a alimentação saudável.",
                @"Polpa de Maçã
Sidra
Suco de Maçã
Vinagre de maçã"),
            new Row(37, "TrichodermaHarzianum", @"Trichoderma Harzianum", 5, ProductCategory.FertilizantesOrganicosEBiopesticidas,
                @"O Trichoderma harzianum é um fungo benéfico usado como biocontrole contra doenças fúngicas em plantas, sendo amplamente aplicado na agricultura orgânica e sustentável. Ele é facilmente cultivado em substratos agrícolas e é utilizado em diversas regiões do Brasil, principalmente nas culturas de frutas, hortaliças e plantas ornamentais. Sua importância na bioeconomia reside no fato de reduzir o uso de pesticidas químicos, promovendo a saúde do solo e a sustentabilidade agrícola.",
                @"Biofertilizantes
Enzimas
Compostos Antifúngicos"),
            new Row(38, "Estreptomicina", @"Estreptomicina", 1, ProductCategory.FarmaceuticosESaude,
                @"A estreptomicina é um antibiótico produzido pela bactéria Streptomyces griseus, usado principalmente no tratamento de tuberculose e infecções bacterianas. No Brasil, é amplamente utilizado no setor de saúde e também na agricultura, para controle de doenças em plantas. Sua importância na bioeconomia está na produção de medicamentos essenciais, mas deve ser usada com cautela devido ao risco de resistência bacteriana.",
                @"Extratos Biotecnológicos
Endosporos Bacterianos
Resíduos Orgânicos de Fermentação"),
            new Row(39, "Valeriana", @"Valeriana", 5, ProductCategory.FarmaceuticosESaude,
                @"A valeriana é uma planta medicinal conhecida por suas propriedades calmantes e sedativas, amplamente usada na produção de medicamentos fitoterápicos. Cultivada a partir de sementes ou mudas, cresce em climas temperados, com cultivo moderado em estados como São Paulo e Minas Gerais. Na bioeconomia, ela promove a valorização de práticas agrícolas sustentáveis e seus benefícios incluem alívio de insônia e estresse, integrando soluções naturais à saúde.",
                @"Chás Relaxantes
Pomadas e Cremes
Relaxantes Musculares
Suplementos Nutricionais"),
            new Row(40, "Tomate", @"Tomate", 3, ProductCategory.AlimentosEBebidas,
                @"O tomate é um fruto amplamente cultivado em todo o Brasil, especialmente em regiões como São Paulo, Minas Gerais e Goiás, devido ao clima favorável. Obtido por cultivo agrícola em estufas ou campo aberto, é utilizado na produção de molhos, sucos e conservas. Na bioeconomia, é importante por gerar renda e subprodutos com valor agregado, além de ser nutritivo, rico em antioxidantes e contribuir para dietas saudáveis.",
                @"Ketchup
Molho de Tomate
Sucos e Bebidas
Pó de Tomate"),
            new Row(41, "OvosDeGalinha", @"Ovos de Galinha", 4, ProductCategory.AlimentosEBebidas,
                @"O ovo de galinha é um alimento rico em proteínas, produzido a partir da criação de galinhas poedeiras. Ele é obtido em granjas, desde criações familiares até grandes sistemas intensivos. No Brasil, é amplamente consumido em todo o território, destaque para estados como São Paulo, Minas Gerais e Paraná. Na bioeconomia, é valioso como fonte acessível de nutrientes, com baixo custo de produção e potencial para aproveitamento integral, incluindo sua casca como fertilizante natural.",
                @"Proteínas de Ovo
Albumina em Pó
Óleo de Gema de Ovo"),
            new Row(42, "PapelDeFibraDeCelulose", @"Papel de Fibra de Celulose", 4, ProductCategory.PapelECelulose,
                @"O Papel de Fibra de Celulose é um material feito a partir das fibras de celulose extraídas de madeira ou outras fontes vegetais, como bambu e resíduos agrícolas. Ele é produzido por meio do cozimento da matéria-prima, seguido de branqueamento e prensagem para formar folhas. No Brasil, é amplamente utilizado na indústria de papel e embalagens sustentáveis, sendo importante para a bioeconomia por substituir plásticos e promover a reciclagem, além de ser biodegradável e renovável.",
                @"Papel Higiênico
Papel Kraft
Papel Jornal
Papel-Cartão"),
            new Row(43, "CarneDeFrango", @"Carne de Frango", 2, ProductCategory.AlimentosEBebidas,
                @"A carne de frango é uma das proteínas animais mais consumidas no Brasil e no mundo, obtida pela criação intensiva ou caipira de aves. É amplamente produzida em estados como Paraná, Santa Catarina e Rio Grande do Sul, que são grandes exportadores. Na bioeconomia, é relevante por seu ciclo produtivo rápido, menor pegada de carbono em comparação a outras carnes e aproveitamento integral de seus subprodutos.",
                @"Óleo de Frango
Caldo de Frango Concentrado
Colágeno de Frango"),
            new Row(44, "LodoDeEsgoto", @"Lodo de Esgoto", 2, ProductCategory.FertilizantesOrganicosEBiopesticidas,
                @"O lodo de esgoto é um resíduo resultante do tratamento de águas residuais em estações de esgoto, sendo obtido por meio da separação de sólidos da água. No Brasil, é amplamente utilizado em estados com grande produção agrícola, como São Paulo e Paraná, para fertilização do solo e produção de biogás. Na bioeconomia, sua importância está na reciclagem de nutrientes e na redução da poluição, trazendo benefícios como a melhoria da fertilidade do solo, geração de energia renovável e menor impacto ambiental.",
                @"Fertilizantes Orgânicos
Biogás
Briquetes de Biomassa"),
            new Row(45, "OvosDePata", @"Ovos de Pata", 2, ProductCategory.AlimentosEBebidas,
                @"O ovo de pata é um produto de origem avícola, obtido a partir da criação de patos, especialmente em pequenas propriedades e sistemas agroecológicos. No Brasil, é mais comum em regiões rurais do Sul e Sudeste, sendo utilizado na culinária e na produção artesanal. Sua importância na bioeconomia está na diversificação da produção animal, oferecendo alto valor nutricional e podendo ser aproveitado em produtos derivados, reduzindo desperdícios.",
                @"Maionese Artesanal
Clara de Ovo em Pó
Cosméticos Naturais"),
            new Row(46, "CarneDePeixe", @"Carne de Peixe", 2, ProductCategory.AlimentosEBebidas,
                @"A carne de peixe é uma importante fonte de proteína, com menor impacto ambiental em comparação com outras carnes, como a de boi. Sua produção pode ser feita de maneira mais sustentável, utilizando sistemas como a aquicultura, que garante a produção sem degradar os ecossistemas naturais. Além disso, a carne de peixe é rica em nutrientes, como ômega-3, essenciais para a saúde humana. A indústria pesqueira e de aquicultura geram emprego e renda, sendo de grande importância para a bioeconomia, especialmente em regiões costeiras e interioranas do Brasil.",
                @"Farinha de Peixe
Óleo de Peixe
Colágeno de Peixe"),
            new Row(47, "Lavanda", @"Lavanda", 4, ProductCategory.CosmeticosEHigienePessoal,
                @"A lavanda é uma planta aromática, cultivada principalmente por suas flores, que são destiladas para a extração de óleo essencial. No Brasil, ela é cultivada em regiões de clima mais ameno, como no interior de São Paulo e Minas Gerais. Sua importância na bioeconomia está na produção de óleos essenciais para cosméticos, perfumes e terapias alternativas, oferecendo benefícios como relaxamento, alívio de estresse e propriedades antimicrobianas.",
                @"Velas Aromáticas
Produtos Cosméticos
Hidrolato de Lavanda
Sabonetes de Lavanda"),
            new Row(48, "HortelaPimenta", @"Hortelã-Pimenta", 4, ProductCategory.CosmeticosEHigienePessoal,
                @"A hortelã-pimenta é uma planta herbácea cultivada principalmente por suas folhas, ricas em mentol, utilizadas em chás, óleos essenciais e cosméticos. Ela pode ser cultivada em climas temperados e subtropicais, sendo comum no Sul e Sudeste do Brasil. Na bioeconomia, a hortelã-pimenta contribui com o uso sustentável de recursos naturais e seus benefícios incluem propriedades calmantes, digestivas e analgésicas.",
                @"Repelentes Naturais
Cremes e Loções
Óleo Essencial de Hortelã-Pimenta"),
            new Row(49, "OleoDeArgan", @"Óleo de Argan", 4, ProductCategory.CosmeticosEHigienePessoal,
                @"O óleo de argan é extraído das sementes da árvore de argan, que cresce principalmente no Marrocos, mas também pode ser encontrado em algumas regiões do Brasil. Ele é bastante utilizado em cosméticos, produtos de cuidados com a pele e cabelo, devido às suas propriedades hidratantes e antioxidantes. Na bioeconomia, o óleo de argan é importante porque gera renda para comunidades locais, contribuindo para a preservação ambiental e a sustentabilidade.",
                @"Creme Hidratante
Sabonetes Naturais
Sérum Capilar de Argan
Shampoo e Condicionador"),
            new Row(50, "Alface", @"Alface", 4, ProductCategory.AlimentosEBebidas,
                @"A alface é uma hortaliça folhosa cultivada principalmente em solos férteis ou sistemas hidropônicos. Ele é amplamente produzido no Sudeste, Sul e Nordeste do Brasil, sendo essencial na alimentação por sua leveza e valor nutricional. Na bioeconomia, destaca-se pela alta produtividade, rápido ciclo de crescimento e possibilidade de cultivo sustentável com baixo impacto ambiental. Seu cultivo pode ser otimizado em estufas e fazendas verticais, reduzindo o uso de água e defensivos agrícolas.",
                @"Óleo Essencial de Alface
Ração para Animais
Extrato de Alface"),
            new Row(51, "MadeiraDeMogno", @"Madeira de Mogno", 2, ProductCategory.PapelECelulose,
                @"A madeira de mogno vem da árvore Swietenia macrophylla, nativa da Amazônia. Por ser espécie ameaçada, sua extração e comercialização são proibidas no Brasil, e hoje só pode ser obtida de plantios legalizados e registrados. Na bioeconomia, seu manejo sustentável evita o desmatamento ilegal e promove a economia local, enquanto sua alta durabilidade e resistência reduzem a necessidade de substituição, tornando-a um recurso valioso.",
                @"Óleo Essencial de Mogno
Painéis e Tábuas
Esculturas e Entalhes
Compensados"),
            new Row(52, "Melancia", @"Melancia", 3, ProductCategory.AlimentosEBebidas,
                @"A melancia é uma fruta refrescante cultivada em campos abertos e sistemas irrigados, bastante comum nas regiões tropicais do Brasil, como no Centro-Oeste, Norte e Nordeste. Na bioeconomia, seu cultivo sustentável impulsiona a economia rural e gera subprodutos com valor agregado. Seus benefícios incluem alto teor de água, vitaminas e antioxidantes, promovendo hidratação e saúde.",
                @"Polpa Desidratada
Suco de Melancia
Casca Compostada"),
            new Row(53, "TelhasDeCortica", @"Telhas de Cortiça", 5, ProductCategory.MateriaisDeConstrucaoBiodegradaveis,
                @"Telhas de cortiça são revestimentos ecológicos feitos a partir da casca do sobreiro, extraída de forma sustentável. No Brasil, são cada vez mais utilizadas em construções ecológicas e projetos de design sustentável, especialmente em regiões urbanas com forte compromisso ambiental. Na bioeconomia, elas promovem o uso de recursos renováveis e oferecem excelente isolamento térmico e acústico, contribuindo para edificações de baixo impacto ambiental.",
                @"Pisos de Cortiça
Rolhas de Cortiça
Mantas Isolantes de Cortiça"),
            new Row(54, "CarneSuina", @"Carne Suína", 2, ProductCategory.AlimentosEBebidas,
                @"A carne suína é obtida da criação de porcos, praticada tanto em sistemas intensivos quanto em produções familiares, sendo bastante utilizada nas regiões Centro- Oeste e Sul do Brasil. Na bioeconomia, ela se destaca pela alta produtividade e eficiência na conversão alimentar, gerando subprodutos valiosos para ração animal e outras indústrias. Seus benefícios incluem o fornecimento de proteína de alta qualidade a um custo acessível, embora a sustentabilidade dependa de práticas de manejo responsáveis para minimizar impactos ambientais.",
                @"Bacon
Presunto
Linguiça
Torresmo"),
            new Row(55, "VacinaBCG", @"Vacina BCG", 3, ProductCategory.FarmaceuticosESaude,
                @"A vacina BCG (Bacilo de Calmette-Guérin) é usada na prevenção da tuberculose e é obtida a partir de uma cepa enfraquecida da bactéria Mycobacterium bovis. Sua produção ocorre em laboratórios especializados, como os do Instituto Butantan e da Fundação Oswaldo Cruz (Fiocruz). No Brasil, é amplamente utilizada em todo o território nacional, sendo obrigatória para recém-nascidos. Na bioeconomia, sua importância está na promoção da saúde pública, reduzindo custos com tratamentos, e na geração de biotecnologia nacional.",
                @"BCG Liofilizada
BCG Recombinante
BCG Intravesical"),
            new Row(56, "TijolosDeAdobe", @"Tijolos de Adobe", 5, ProductCategory.MateriaisDeConstrucaoBiodegradaveis,
                @"Os tijolos de adobe são blocos de barro cru misturado com palha ou fibras naturais, moldados e secos ao sol, sem necessidade de queima. São produzidos artesanalmente ou em pequena escala, sendo comuns no Nordeste e Centro-Oeste do Brasil, especialmente em construções sustentáveis e históricas. Sua importância na bioeconomia está na baixa pegada de carbono, reutilização de materiais naturais e isolamento térmico eficiente, promovendo construções ecológicas e de baixo custo.",
                @"Argamassa de Adobe
Painéis de Adobe
Blocos de Adobe Prensado"),
            new Row(57, "Papelao", @"Papelão", 4, ProductCategory.PapelECelulose,
                @"O papelão é um material derivado da celulose, obtido a partir do processamento de fibras de madeira ou papel reciclado. Ele é amplamente utilizado em todo o Brasil, principalmente na indústria de embalagens, transporte e logística. Na bioeconomia, destaca-se por ser reciclável e biodegradável, reduzindo o impacto ambiental e promovendo a economia circular, além de ser uma alternativa sustentável ao plástico.",
                @"Embalagens Biodegradáveis
Papelão Ondulado
Papelão Reciclado"),
            new Row(58, "Palha", @"Palha", 5, ProductCategory.MateriaisDeConstrucaoBiodegradaveis,
                @"A palha é um subproduto agrícola obtido a partir de plantas como arroz, trigo, milho e cana-de-açúcar, sendo aproveitada após a colheita. No Brasil, é amplamente utilizada no Nordeste e Centro-Oeste para coberturas de casas, produção de artesanato e como forragem animal. Na bioeconomia, destaca-se por ser um material biodegradável e renovável, reduzindo o desperdício e servindo como matéria-prima para bioenergia, isolamento térmico e compostagem, além de contribuir para a conservação do solo.",
                @"Papel e Celulose
Isolamento Térmico e Acústico
Artesanato e Cestaria"),
            new Row(59, "PapelAntimicrobiano", @"Papel Antimicrobiano", 3, ProductCategory.PapelECelulose,
                @"O papel antimicrobiano é um tipo de papel tratado com substâncias que inibem o crescimento de microrganismos, sendo utilizado em embalagens de alimentos, produtos hospitalares e higiene. Ele é produzido a partir de celulose combinada com agentes antimicrobianos naturais ou sintéticos. No Brasil, é amplamente utilizado na indústria alimentícia e na área da saúde, contribuindo para a conservação de produtos e redução de contaminações, promovendo segurança e sustentabilidade.",
                @"Filtros Antimicrobianos
Embalagens
Guardanapos e Toalhas Descartáveis"),
            new Row(60, "CompostoOrganico", @"Composto Orgânico", 5, ProductCategory.FertilizantesOrganicosEBiopesticidas,
                @"O composto orgânico é um fertilizante natural obtido pela decomposição de resíduos orgânicos, como restos de alimentos, folhas e esterco. Ele pode ser produzido em compostagens caseiras, agrícolas ou industriais. No Brasil, é amplamente utilizado na agricultura sustentável, ajudando a enriquecer o solo, reduzir a dependência de fertilizantes químicos e minimizar resíduos orgânicos, promovendo a economia circular na bioeconomia.",
                @"Adubo para Hortas
Biochar
Condicionador de Solo"),
            new Row(61, "EmbalagensBiodegradaveis", @"Embalagens Biodegradáveis", 5, ProductCategory.PapelECelulose,
                @"As embalagens biodegradáveis são feitas de materiais naturais, como amido de milho, celulose e bioplásticos, que se decompõem rapidamente no meio ambiente. Elas são produzidas a partir de fontes renováveis e amplamente utilizadas em supermercados, restaurantes e indústrias de alimentos no Brasil. Na bioeconomia, ajudam a reduzir a poluição plástica, diminuir a pegada de carbono e incentivar o uso de recursos sustentáveis.",
                @"Sacolas Biodegradáveis
Embalagens de Papel Reciclável
Potes Biodegradáveis"),
            new Row(62, "MadeiraDeEucalipto", @"Madeira de Eucalipto", 4, ProductCategory.PapelECelulose,
                @"A madeira de eucalipto vem do cultivo de árvores de crescimento rápido, sendo amplamente utilizada no Brasil, especialmente no Sudeste e Sul, em indústrias de papel, móveis e construção. Sua importância na bioeconomia está na alta produtividade e no menor impacto ambiental em comparação a madeiras nativas. Seus benefícios incluem reflorestamento sustentável, captura de carbono e uso renovável para diversos setores.",
                @"Óleo de Eucalipto
Celulose e Papel
Madeira Serrada
Carvão Vegetal"),
            new Row(63, "VacinaContraRubeola", @"Vacina contra rubéola", 3, ProductCategory.FarmaceuticosESaude,
                @"A vacina contra a rubéola é produzida a partir do vírus atenuado e faz parte da vacina tríplice viral (sarampo, caxumba e rubéola). Está disponível no SUS e é amplamente utilizada em todo o Brasil, especialmente em campanhas nacionais de imunização. Sua importância na bioeconomia está na prevenção de surtos, reduzindo custos com tratamentos médicos e impactos na produtividade, além de contribuir para a erradicação da doença, promovendo saúde pública sustentável.",
                @"Vacina Tetra Viral
Kits de Diagnóstico para Rubéola
Testes Sorológicos"),
            new Row(64, "PapelTissue", @"Papel Tissue", 3, ProductCategory.PapelECelulose,
                @"O papel tissue é um tipo de papel fino e macio, utilizado na produção de lenços, guardanapos, papel higiênico e toalhas de papel. Ele é fabricado a partir da celulose de madeira de reflorestamento, como eucalipto e pinus, tornando-o um produto sustentável. No Brasil, é amplamente utilizado em setores domésticos e industriais, contribuindo para a bioeconomia ao promover o uso eficiente de recursos renováveis e biodegradáveis.",
                @"Papel Higiênico
Lenços de Papel
Fraldas Descartáveis
Toalhas de Papel"),
            new Row(65, "DesinfetantesNaturais", @"Desinfetantes Naturais", 3, ProductCategory.LimpezaEDetergentesBiodegradaveis,
                @"Os desinfetantes naturais são produtos de limpeza feitos com ingredientes biodegradáveis, como óleos essenciais, vinagre e extratos vegetais, sem compostos químicos agressivos. Eles podem ser produzidos de forma caseira ou industrializada, sendo amplamente utilizados em residências, hospitais e estabelecimentos ecológicos em todo o Brasil. Na bioeconomia, ajudam a reduzir a poluição química e a preservar a saúde humana e ambiental, promovendo uma limpeza eficiente e sustentável.",
                @"Detergente Ecológico
Álcool Herbal
Desodorizador Ambiental
Sabão Líquido Natural"),
            new Row(66, "VacinaContraSarampo", @"Vacina contra Sarampo", 2, ProductCategory.FarmaceuticosESaude,
                @"A vacina contra o sarampo é um imunizante produzido a partir do vírus atenuado, garantindo proteção contra a doença. No Brasil, é amplamente utilizada no Sistema Único de Saúde (SUS), especialmente em campanhas de vacinação infantil e em surtos. Sua importância na bioeconomia está na redução de custos com tratamentos hospitalares e na prevenção de epidemias, além de promover a saúde pública e reduzir a mortalidade infantil.",
                @"Vacina Tríplice Viral
Soros Imunológicos
Anticorpos Monoclonais"),
            new Row(67, "PapelEletronico", @"Papel Eletrônico", 4, ProductCategory.PapelECelulose,
                @"O papel eletrônico (e-paper) é um material reutilizável que simula a aparência do papel tradicional, mas usa tecnologia de tinta eletrônica para exibir informações. Ele é produzido a partir de polímeros e componentes eletrônicos de baixo consumo energético. No Brasil, é utilizado principalmente em etiquetas digitais no varejo, painéis informativos e dispositivos de leitura. Na bioeconomia, reduz o desperdício de papel e eletrônicos, economizando recursos e energia. Seus benefícios incluem durabilidade, baixo impacto ambiental e eficiência energética.",
                @"E-readers
Cartões de Visita Digitais
Etiquetas Digitais de Preço
Painéis Informativos"),
            new Row(68, "MadeiraPinus", @"Madeira Pinus", 4, ProductCategory.PapelECelulose,
                @"A madeira de pinus vem de árvores do gênero Pinus, cultivadas principalmente no Sul e Sudeste do Brasil devido ao clima favorável. É obtida por reflorestamento e amplamente usada na construção civil, móveis e papel. Na bioeconomia, sua rápida renovação e cultivo sustentável reduzem o impacto ambiental, além de ser leve, versátil e de baixo custo.",
                @"Óleo Essencial de Pinus
Serragem
Briquetes de Madeira"),
            new Row(69, "IsolamentoDeFibraDeCelulose", @"Isolamento de Fibra de Celulose", 4, ProductCategory.MateriaisDeConstrucaoBiodegradaveis,
                @"O isolamento de fibra de celulose é produzido a partir de papel e jornal reciclados, desfibrados e tratados com sais minerais que os tornam resistentes ao fogo, a fungos e a insetos. É aplicado em paredes, forros e telhados, e no Brasil começa a ser adotado em construções sustentáveis e obras com certificação ambiental. Na bioeconomia, destaca-se por dar destino a um resíduo abundante e por exigir pouca energia na fabricação, bem menos que a lã de vidro. Seus benefícios incluem a redução do gasto de energia com climatização, o conforto acústico e a diminuição do lixo de papel.",
                @"Painéis Acústicos
Enchimento para Forros
Flocos para Insuflamento"),
            new Row(70, "SabaoNatural", @"Sabão Natural", 4, ProductCategory.LimpezaEDetergentesBiodegradaveis,
                @"O sabão natural é feito a partir de óleos vegetais, gorduras naturais e ingredientes biodegradáveis, sem produtos químicos agressivos. É amplamente utilizado no Brasil, especialmente em áreas rurais e por consumidores que buscam alternativas sustentáveis. Sua importância na bioeconomia está na redução do impacto ambiental, incentivo à agricultura sustentável e valorização de matérias-primas renováveis.",
                @"Glicerina Natural
Sabonete Artesanal
Sabão em Barra
Sabão em Pasta"),
            new Row(71, "DetergenteLiquidoDeOleosVegetais", @"Detergente Líquido de Óleos Vegetais", 4, ProductCategory.LimpezaEDetergentesBiodegradaveis,
                @"O detergente líquido de óleos vegetais é feito a partir de óleos naturais, como coco e palma, combinados com agentes de limpeza biodegradáveis. É amplamente utilizado no Brasil, principalmente em lares e estabelecimentos sustentáveis. Na bioeconomia, reduz o uso de petroquímicos e minimiza impactos ambientais, sendo uma alternativa ecológica aos detergentes convencionais.",
                @"Sabão em Barra Vegetal
Amaciante Ecológico
Sabão Multiuso Líquido"),
            new Row(72, "PanosDeLimpezaDeMicrofibra", @"Panos de Limpeza de Microfibra", 3, ProductCategory.LimpezaEDetergentesBiodegradaveis,
                @"Os panos de limpeza de microfibra são feitos de poliéster e poliamida, obtidos a partir de processos industriais têxteis. São amplamente utilizados em residências, comércios e indústrias em todo o Brasil devido à sua alta eficiência na limpeza. Na bioeconomia, ajudam a reduzir o uso de produtos químicos e descartáveis, sendo reutilizáveis e duráveis, o que diminui o desperdício e o impacto ambiental.",
                @"Filamentos Reciclados
Fibra Sintética Reutilizável
Resíduos têxteis"),
            new Row(73, "CamuCamu", @"Camu-camu", 5, ProductCategory.AlimentosEBebidas,
                @"O Camu-camu é um fruto amazônico rico em vitamina C, obtido de arbustos nativos de áreas alagadas. É cultivado principalmente no Amazonas, Acre e Pará, sendo bastante utilizado na indústria alimentícia, cosmética e farmacêutica. Sua importância na bioeconomia está no alto valor nutricional e comercial, gerando renda para comunidades ribeirinhas e promovendo a conservação da floresta. Além de fortalecer a imunidade, seu cultivo sustentável evita o desmatamento e protege ecossistemas aquáticos.",
                @"Óleo de Camu-camu
Polpa Congelada
Extrato em Pó
Cápsulas de Camu-camu"),
            new Row(74, "GengibreMangarataia", @"Gengibre Mangarataia", 4, ProductCategory.AlimentosEBebidas,
                @"O Gengibre Mangarataia é uma raiz cultivada a partir de rizomas, principalmente em regiões de clima quente e úmido, como o Nordeste e o Sudeste do Brasil. É amplamente utilizado na culinária, na medicina natural e na indústria de bebidas e cosméticos. Na bioeconomia, tem grande importância por ser um produto de alto valor agregado, sustentável e com diversas aplicações industriais. Seus benefícios incluem propriedades anti-inflamatórias, digestivas e antioxidantes, além de impulsionar a agricultura familiar e a exportação de produtos naturais.",
                @"Chá de Gengibre
Farinha de Gengibre
Gengibre Cristalizado"),
            new Row(75, "Buriti", @"Buriti", 5, ProductCategory.AlimentosEBebidas,
                @"O buriti é uma palmeira nativa da região Centro-Oeste e Norte do Brasil, especialmente em áreas de cerrado e florestas. Seu fruto é bastante utilizado para a produção de óleos, sucos e polpas, enquanto suas fibras podem ser usadas para artesanato. Na bioeconomia, o buriti é importante por ser uma planta de baixo impacto ambiental e altamente aproveitável, promovendo a sustentabilidade das comunidades locais. Seus benefícios incluem propriedades antioxidantes, anti-inflamatórias e nutricionais, além de ser uma fonte de renda para agricultores e artesãos.",
                @"Polpa de Buriti
Óleo de Buriti
Semente de Buriti
Fibra de Buriti"),
        };

        [MenuItem("BioTrilha/Cards/Gerar Cartas de Produto")]
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
                    Debug.LogError($"[ProductCardGenerator] Sprite não encontrado para '{c.file}'.");
                    continue;
                }

                string assetPath = $"{DataFolder}/Produto_{c.id:00}_{c.file}.asset";
                var data = AssetDatabase.LoadAssetAtPath<ProductCardData>(assetPath);
                bool isNew = data == null;
                if (isNew) data = ScriptableObject.CreateInstance<ProductCardData>();

                data.id = c.id;
                data.displayName = c.title;
                data.stars = c.stars;
                data.category = c.category;
                data.frontSprite = front;
                data.backSprite = back;
                data.descriptionText = c.description.Replace("\r", "");
                data.subproductsText = c.subproducts.Replace("\r", "");

                if (isNew) { AssetDatabase.CreateAsset(data, assetPath); created++; }
                else { EditorUtility.SetDirty(data); updated++; }
            }

            AssetDatabase.SaveAssets();
            CreatePrefab();
            AssetDatabase.Refresh();
            Debug.Log($"[ProductCardGenerator] ScriptableObjects criados: {created}, atualizados: {updated}. Prefab: {PrefabPath}");
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
            var root = new GameObject("ProductCard", typeof(RectTransform), typeof(Image),
                typeof(AspectRatioFitter), typeof(ProductCardView));

            var rect = (RectTransform)root.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(PrefabHeight * CardAspect, PrefabHeight);

            var image = root.GetComponent<Image>();
            image.preserveAspect = true;
            image.raycastTarget = true;

            var fitter = root.GetComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            fitter.aspectRatio = CardAspect;

            var first = AssetDatabase.LoadAssetAtPath<ProductCardData>(
                $"{DataFolder}/Produto_01_{Cards[0].file}.asset");
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
