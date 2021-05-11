using GameTroopers.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Arcadia.Sample
{
    public class MainMenu : Menu, IOnMenuLoaded, IOnMenuShowStarted, IOnMenuShowEnded, IOnMenuHideStarted, IOnMenuHideEnded, IOnMenuFocused, IOnMenuUnfocused
    {
        public void OnMenuLoaded()
        {
            inventoryButton.onClick.AddListener(OnInventoryButton);
        }

        private void OnInventoryButton()
        {
            GameManager.Instance.ShowInventory();
        }

        public void OnMenuShowStarted()
        {
            stateText.text = "OnMenuShowStarted";
        }

        public void OnMenuShowEnded()
        {
            stateText.text = "OnMenuShowEnded";
        }

        public void OnMenuHideStarted()
        {
            stateText.text = "OnMenuHideStarted";
        }

        public void OnMenuHideEnded()
        {
            stateText.text = "OnMenuHideEnded";
        }

        public void OnMenuFocused()
        {
            stateText.text = "OnMenuFocused";
        }

        public void OnMenuUnfocused()
        {
            stateText.text = "OnMenuUnfocused";
        }

        [SerializeField] Button inventoryButton;
        [SerializeField] Text stateText;
    }
}
