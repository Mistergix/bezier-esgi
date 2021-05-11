using GameTroopers.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Arcadia.Sample
{
    public class ShopMenu : Menu, IOnMenuLoaded
    {
        public void OnMenuLoaded()
        {
            closeButton.onClick.AddListener(OnCloseButton);
        }

        private void OnCloseButton()
        {
            GameManager.Instance.BackEvent();
        }

        protected internal override void HandleGoBack()
        {
            GameManager.Instance.ShowMainMenu();
        }
    
        [SerializeField] Button closeButton;
    }
}

