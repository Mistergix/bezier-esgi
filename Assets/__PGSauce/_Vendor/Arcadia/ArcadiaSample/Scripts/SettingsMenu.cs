using GameTroopers.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Arcadia.Sample
{
    public class SettingsMenu : Menu, IOnMenuLoaded
    {
        public void OnMenuLoaded()
        {
            closeButton.onClick.AddListener(OnCloseButton);
            popupButton.onClick.AddListener(OnPopupButton);
        }

        private void OnCloseButton()
        {
            GameManager.Instance.BackEvent();
        }

        private void OnPopupButton()
        {
            GameManager.Instance.ShowPopup();
        }

        protected internal override void HandleGoBack()
        {
            GameManager.Instance.ShowMainMenu();
        }

        [SerializeField] Button closeButton;
        [SerializeField] Button popupButton;
    }
}


