using UnityEngine;
using UnityEngine.UI;

namespace BioTrilha.Cards
{
    /// <summary>Qual face da carta de Questões está sendo mostrada.</summary>
    public enum QuestionCardFace
    {
        /// <summary>Verso "Questões" (carta virada para baixo no baralho).</summary>
        Back,

        /// <summary>Frente só com a pergunta: a resposta e os resultados ficam cobertos.</summary>
        Question,

        /// <summary>Frente completa, com a resposta e os resultados (depois que o jogador responde).</summary>
        Answer
    }

    /// <summary>
    /// Mostra uma carta de Questões na UI (Canvas).
    /// A parte de baixo da arte (resposta e resultados) é coberta por um painel da cor da carta
    /// (AnswerCover) enquanto o jogador ainda não respondeu. O painel é recortado pelo formato
    /// arredondado da carta (componente Mask no objeto raiz do prefab).
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class QuestionCardView : MonoBehaviour
    {
        [SerializeField] private Image faceImage;
        [Tooltip("Painel amarelo que cobre a resposta e os resultados.")]
        [SerializeField] private GameObject answerCover;

        public QuestionCardData Data { get; private set; }
        public QuestionCardFace Face { get; private set; } = QuestionCardFace.Back;

        private void Reset()
        {
            faceImage = GetComponent<Image>();
        }

        private void Awake()
        {
            if (faceImage == null) faceImage = GetComponent<Image>();
        }

        public void Setup(QuestionCardData data, QuestionCardFace face = QuestionCardFace.Back)
        {
            Data = data;
            Show(face);
        }

        public void Show(QuestionCardFace face)
        {
            Face = face;

            if (answerCover != null) answerCover.SetActive(face == QuestionCardFace.Question);
            if (Data == null || faceImage == null) return;

            faceImage.sprite = face == QuestionCardFace.Back ? Data.backSprite : Data.frontSprite;
        }

        public void ShowBack() => Show(QuestionCardFace.Back);

        /// <summary>Vira a carta mostrando só a pergunta.</summary>
        public void ShowQuestion() => Show(QuestionCardFace.Question);

        /// <summary>Descobre a resposta e os resultados.</summary>
        public void RevealAnswer() => Show(QuestionCardFace.Answer);
    }
}
