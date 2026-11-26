using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BioTrilha.UI
{
    /// <summary>
    /// Interface provisória do turno: um texto de status e o botão "Lançar dado".
    /// Montada pelo menu BioTrilha > Tabuleiro > Montar Movimento na Cena.
    /// </summary>
    public class TurnHud : MonoBehaviour
    {
        [SerializeField] private Text statusText;
        [SerializeField] private Button rollButton;
        [SerializeField] private Text rollButtonLabel;

        private bool pressed;

        private void Awake()
        {
            if (rollButton != null) rollButton.onClick.AddListener(() => pressed = true);
            SetButton(false, "");
        }

        public void SetStatus(string text)
        {
            if (statusText != null) statusText.text = text;
        }

        /// <summary>Mostra o botão com <paramref name="label"/> e espera o toque.</summary>
        public IEnumerator WaitForPress(string label)
        {
            pressed = false;
            SetButton(true, label);
            while (!pressed) yield return null;
            SetButton(false, label);
        }

        private void SetButton(bool visible, string label)
        {
            if (rollButton != null) rollButton.gameObject.SetActive(visible);
            if (rollButtonLabel != null) rollButtonLabel.text = label;
        }
    }
}
