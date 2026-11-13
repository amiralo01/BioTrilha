using UnityEngine;

namespace BioTrilha.Cards
{
    /// <summary>
    /// Dados de uma carta de Questões. O jogador responde Sim ou Não à pergunta e, conforme
    /// acerta ou erra, aplica-se o resultado impresso na carta. Todas as cartas compartilham
    /// o mesmo verso ("Questões").
    ///
    /// A arte (frente) traz a pergunta, a resposta e os dois resultados. No jogo, a parte de baixo
    /// (resposta e resultados) fica coberta até o jogador responder (ver QuestionCardView).
    /// </summary>
    [CreateAssetMenu(fileName = "Questao", menuName = "BioTrilha/Cards/Question Card", order = 2)]
    public class QuestionCardData : ScriptableObject
    {
        [Tooltip("Número da carta (1 a 47), na ordem do Canva.")]
        public int id;

        [Tooltip("Título impresso na carta (ex.: 'Sobre o Açaí').")]
        public string displayName;

        [Header("Arte")]
        public Sprite frontSprite;

        [Tooltip("Verso comum a todas as cartas deste baralho.")]
        public Sprite backSprite;

        [Header("Pergunta")]
        [TextArea(2, 6)]
        [Tooltip("Texto da pergunta, para exibir em UI ou ler em voz alta.")]
        public string questionText;

        [Tooltip("true = a resposta correta é 'Sim'; false = 'Não'.")]
        public bool answerIsYes;

        [Header("Resultado")]
        [TextArea(2, 4)]
        [Tooltip("Texto do resultado ao ACERTAR (só exibição; nunca é interpretado pelo código).")]
        public string correctOutcomeText;

        [TextArea(2, 4)]
        [Tooltip("Texto do resultado ao ERRAR (só exibição; nunca é interpretado pelo código).")]
        public string wrongOutcomeText;

        [Tooltip("Ao acertar, a carta fica guardada com o jogador em vez de voltar ao baralho " +
                 "(só 'Sobre a Biopirataria': permite sair da Penitenciária).")]
        public bool keepCardOnCorrect;

        /// <summary>true se a resposta do jogador (Sim = true, Não = false) está correta.</summary>
        public bool IsCorrectAnswer(bool playerSaidYes) => playerSaidYes == answerIsYes;
    }
}
