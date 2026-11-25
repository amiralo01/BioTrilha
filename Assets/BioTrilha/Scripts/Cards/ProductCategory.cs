namespace BioTrilha.Cards
{
    /// <summary>
    /// Categoria (tipo) de um produto bioeconômico. O número é o ID da categoria e NÃO deve mudar,
    /// porque as cartas de Ação, Sorte ou Revés e Questões que citam um tipo de produto
    /// (ex.: "produtos relacionados a comidas, bebidas e biocombustíveis") usam esse ID.
    /// O nome da categoria não aparece no nome nem na arte das cartas de Produto.
    /// Fonte: planilha "Produtos bioeconômicos de cada tipo" (76 itens; o Carvão Vegetal conta só em Biomassa).
    /// </summary>
    public enum ProductCategory
    {
        Nenhuma = 0,
        Biocombustiveis = 1,
        Biomassa = 2,
        ProdutosQuimicosBiologicos = 3,
        AlimentosEBebidas = 4,
        FarmaceuticosESaude = 5,
        CosmeticosEHigienePessoal = 6,
        PapelECelulose = 7,
        TexteisEVestuario = 8,
        MateriaisDeConstrucaoBiodegradaveis = 9,
        LimpezaEDetergentesBiodegradaveis = 10,
        FertilizantesOrganicosEBiopesticidas = 11
    }

    public static class ProductCategories
    {
        /// <summary>Nome por extenso da categoria (para menus, depuração e regras).</summary>
        public static string DisplayName(ProductCategory c)
        {
            switch (c)
            {
                case ProductCategory.Biocombustiveis: return "Biocombustíveis";
                case ProductCategory.Biomassa: return "Biomassa";
                case ProductCategory.ProdutosQuimicosBiologicos: return "Produtos Químicos Biológicos";
                case ProductCategory.AlimentosEBebidas: return "Alimentos e Bebidas";
                case ProductCategory.FarmaceuticosESaude: return "Produtos Farmacêuticos e de Saúde";
                case ProductCategory.CosmeticosEHigienePessoal: return "Cosméticos e Produtos de Higiene Pessoal";
                case ProductCategory.PapelECelulose: return "Produtos de Papel e Celulose";
                case ProductCategory.TexteisEVestuario: return "Têxteis e Vestuário";
                case ProductCategory.MateriaisDeConstrucaoBiodegradaveis: return "Materiais de Construção Biodegradáveis";
                case ProductCategory.LimpezaEDetergentesBiodegradaveis: return "Produtos de Limpeza e Detergentes Biodegradáveis";
                case ProductCategory.FertilizantesOrganicosEBiopesticidas: return "Fertilizantes Orgânicos e Biopesticidas";
                default: return "Sem categoria";
            }
        }

        /// <summary>ID numérico da categoria (1 a 11).</summary>
        public static int Id(ProductCategory c) => (int)c;
    }
}
