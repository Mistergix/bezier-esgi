using GameTroopers.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Arcadia.Sample
{
    public class TopBarMenu : Menu, IOnMenuLoaded
    {
        public void OnMenuLoaded()
        {
            shopButton.onClick.AddListener(OnShopButton);
            settingsButton.onClick.AddListener(OnSettingsButton);
        }

        private void OnShopButton()
        {
            GameManager.Instance.ShowShop();
        }

        private void OnSettingsButton()
        {
            GameManager.Instance.ShowSettings();
        }

        /// <summary>
        /// Custom start and end positions
        /// </summary>
        protected override Vector2 GetMenuOffScreenPosition()
        {
            var canvasSize = m_menuManager.Canvas.GetComponent<RectTransform>().sizeDelta;
            return new Vector2(0, canvasSize.y);
        }

        protected override Vector2 GetMenuOnScreenPosition()
        {
            var canvasSize = m_menuManager.Canvas.GetComponent<RectTransform>().sizeDelta;
            return new Vector2(0, 0);
        }

        [SerializeField] Button shopButton;
        [SerializeField] Button settingsButton;
    }
}

