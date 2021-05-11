using GameTroopers.UI;
using UnityEngine;

namespace Arcadia.Sample
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance = null;
        
        private void Start()
        {
            m_menuManager.Initialize();
            m_menuManager.LoadGroup("Group1");
    
            ShowTopBar();
            ShowMainMenu();
        }
    
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
    
            DontDestroyOnLoad(gameObject);
        }
    
        /// <summary>
        /// Main menu
        /// </summary>
        public void ShowMainMenu()
        {
            if (!m_menuManager.GetMenu<TopBarMenu>().IsActive)
            {
                ShowTopBar();
            }
    
            m_menuManager.ShowMenu<MainMenu>();
        }
        
        /// <summary>
        /// Inventory menu
        /// </summary>
        public void ShowInventory()
        {
            HideTopBar();
            m_menuManager.ShowMenu<InventoryMenu>();
        }
        
        /// <summary>
        /// Top bar menu
        /// </summary>
        public void ShowTopBar()
        {
            m_menuManager.ShowMenu<TopBarMenu>();
        }
    
        public void HideTopBar()
        {
            m_menuManager.HideMenu<TopBarMenu>();
        }
    
        /// <summary>
        /// Shop menu
        /// </summary>
        public void ShowShop()
        {
            m_menuManager.ShowMenu<ShopMenu>();
        }
    
        /// <summary>
        /// Settings menu
        /// </summary>
        public void ShowSettings()
        {
            m_menuManager.ShowMenu<SettingsMenu>();
        }
        
        /// <summary>
        /// Popup menu
        /// </summary>
        public void ShowPopup()
        {
            m_menuManager.ShowMenu<PopupMenu>();
        }
    
        /// <summary>
        /// Back event
        /// </summary>
        public void BackEvent()
        {
            m_menuManager.GoBack();
        }
    
        [SerializeField] MenuManager m_menuManager;
    }

}

