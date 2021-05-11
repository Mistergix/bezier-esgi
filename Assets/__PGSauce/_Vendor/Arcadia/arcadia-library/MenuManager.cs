using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameTroopers.UI
{
    /// <summary>
    /// UI system initialization and menu management.
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        /// <summary>
        /// Gets the canvas containing all the menus managed by this class.
        /// </summary>
        public Canvas Canvas
        {
            get { return m_canvas; }
            set { m_canvas = value; }
        }

        public EventSystem EventSystem
        {
            get { return m_eventSystem; }
            set { m_eventSystem = value; }
        }

        #region Public methods

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        public void Initialize()
        {
            if (m_menuLoader.IsLoaded)
            {
                throw new InvalidOperationException("Menu manager is already initialized");
            }
            
            DontDestroyOnLoad(gameObject);

            InitMenuLoader();
        }

        /// <summary>
        /// Instantiates all the menus that are part of the specified group.
        /// </summary>
        /// <param name="groupName">Name of the group to load</param>
        /// <exception cref="InvalidOperationException">Thrown if the target group is already loaded</exception>
        /// <exception cref="ArgumentException">Thrown if the target group does not exist or is duplicated</exception>
        public void LoadGroup(string groupName)
        {
            if (!m_menuLoader.IsLoaded)
            {
                throw new InvalidOperationException("You must initialize Menu Manager first");
            }

            m_menuLoader.LoadGroup(groupName, OnLoadMenuCallback);
        }

        /// <summary>
        /// Destroys all the menus that are part of the specified group.
        /// </summary>
        /// <param name="groupName">Name of the group to load</param>
        /// <exception cref="InvalidOperationException">Thrown if the target group is not loaded</exception>
        /// <exception cref="ArgumentException">Thrown if the target group does not exist</exception>
        public void UnloadGroup(string groupName)
        {
            if (!m_menuLoader.IsLoaded)
            {
                throw new InvalidOperationException("You must initialize Menu Manager first");
            }

            m_menuLoader.UnloadGroup(groupName);
        }

        /// <summary>
        /// Gets the loaded instance of the menu of the specified type.
        /// </summary>
        /// <typeparam name="T">The menu type</typeparam>
        /// <returns>Instance of a loaded menu with the target type</returns>
        /// <exception cref="ArgumentException">Thrown if the group to which the menu belongs is not loaded</exception>
        public T GetMenu<T>() where T : Menu
        {
            if (!m_menuLoader.IsLoaded)
            {
                throw new InvalidOperationException("You must initialize Menu Manager first");
            }

            return m_menuLoader.GetLoadedMenu<T>();
        }

        /// <summary>
        /// Shows the menu of the specified type and hides the current active menu in the same layer, if any.
        /// </summary>
        /// <typeparam name="T">The menu type</typeparam>
        /// <param name="shouldPlayAnimation">Used to indicate if the menu should play an animation when it is shows</param>
        /// <returns>Instance of the menu that has been shown</returns>
        /// <exception cref="InvalidOperationException">Thrown if the menu is already active or playing an animation.</exception>
        /// <exception cref="ArgumentException">If a custom animation has to be played but it has not been set</exception>
        public T ShowMenu<T>(bool shouldPlayAnimation = true) where T : Menu
        {
            if (!m_menuLoader.IsLoaded)
            {
                throw new InvalidOperationException("You must initialize Menu Manager first");
            }

            Menu newMenu = GetMenu<T>();

            ShowMenu(newMenu, shouldPlayAnimation);

            return (T)newMenu;
        }

        /// <summary>
        /// Shows the target menu and hides the current active menu in the same layer, if any.
        /// </summary>
        /// <param name="newMenu">Menu instance that will be shown</param>
        /// <param name="shouldPlayAnimation">Used to indicate if the menu should play an animation when it is shows</param>
        /// <returns>Instance of the menu that has been shown</returns>
        /// <exception cref="InvalidOperationException">Thrown if the menu is already active or playing an animation.</exception>
        /// <exception cref="ArgumentException">If a custom animation has to be played but it has not been set</exception>
        public void ShowMenu(Menu newMenu, bool shouldPlayAnimation = true)
        {
            if (newMenu.IsActive)
                throw new InvalidOperationException("Cannot show again a menu that is being shown.");

            string layerName = newMenu.Layer;

            Menu currentMenu = m_menuLoader.GetTopActiveMenuInLayer(layerName);
            if (null != currentMenu)
            {
                if (currentMenu.IsPlayingAnimation)
                {
                    throw new InvalidOperationException("ShowMenu cannot be called again until then current animation ends.");
                }

                currentMenu.Hide(shouldPlayAnimation);
            }

            if (newMenu != null)
            {
                m_menuLoader.SetTopActiveMenuInLayer(layerName, newMenu);
                newMenu.Show(shouldPlayAnimation);
            }
        }

        /// <summary>
        /// Hides the menu of the specified type and shows the last opened menu in the same layer, if any.
        /// </summary>
        /// <typeparam name="T">The menu type</typeparam>
        /// <param name="shouldPlayAnimation">Used to indicate if the menu should play an animation when it hides</param>
        /// <exception cref="InvalidOperationException">Thrown if the menu is already hidden or playing an animation</exception>
        /// <exception cref="ArgumentException">If a custom animation has to be played but it has not been set</exception>
        public void HideMenu<T>(bool shouldPlayAnimation = true) where T : Menu
        {
            if (!m_menuLoader.IsLoaded)
            {
                throw new InvalidOperationException("You must initialize Menu Manager first");
            }

            HideMenu(GetMenu<T>(), shouldPlayAnimation);
        }

        /// <summary>
        /// Hides the target menu and shows the last opened menu in the same layer, if any.
        /// </summary>
        /// <param name="currentMenu">Menu that will be hidden</param>
        /// <param name="shouldPlayAnimation">Used to indicate if the menu should play an animation when it hides</param>
        /// <exception cref="InvalidOperationException">Thrown if the menu is already hidden or playing an animation</exception>
        /// <exception cref="ArgumentException">If a custom animation has to be played but it has not been set</exception>
        public void HideMenu(Menu currentMenu, bool shouldPlayAnimation = true)
        {
            if (!currentMenu.IsActive)
            {
                throw new InvalidOperationException("Can't hide a menu that hasn't been shown");
            }

            if (currentMenu.IsPlayingAnimation)
            {
                throw new InvalidOperationException("HideMenu cannot be called again until then current animation ends.");
            }

            string layerName = currentMenu.Layer;
            Menu nextMenu = m_menuLoader.GetNextActiveMenuInLayer(layerName);

            currentMenu.Hide(shouldPlayAnimation);
            m_menuLoader.SetInactiveMenuInLayer(layerName, currentMenu);
            
            if (nextMenu != null && !nextMenu.IsPlayingAnimation)
            {
                m_menuLoader.SetTopActiveMenuInLayer(nextMenu.Layer, nextMenu);
                nextMenu.Show(shouldPlayAnimation);
            }
        }

        /// <summary>
        /// Hides all the active menus in all the layers.
        /// </summary>
        /// <param name="shouldPlayAnimation">Used to indicate if the menus should play an animation when they hide</param>
        /// <exception cref="ArgumentException">If a custom animation has to be played but it has not been set</exception>
        public void HideAll(bool shouldPlayAnimation = true)
        {
            if (!m_menuLoader.IsLoaded)
            {
                throw new InvalidOperationException("You must initialize Menu Manager first");
            }

            foreach (Menu menu in m_menuLoader.GetLoadedMenus())
            {
                if (menu.IsActive)
                {
                    menu.Hide(shouldPlayAnimation);
                    m_menuLoader.SetInactiveMenuInLayer(menu.Layer, menu);
                }
            }
        }

        /// <summary>
        /// Returns the previous menu to know where you come from.
        /// </summary>
        /// <param name="layerName">Used to indicate the target layer name.</param>
        public Menu GetPreviousMenu(string layerName)
        {
            if (!m_menuLoader.IsLoaded)
            {
                throw new InvalidOperationException("You must initialize Menu Manager first");
            }

            return m_menuLoader.GetNextActiveMenuInLayer(layerName);
        }

        /// <summary>
        /// Used to init the MenuManager using an specific UISettings.
        /// Is not necesary call it manually; will be called automatically
        /// on MenuManager Start() with the default UISettings file.
        /// NOTE: cannot load a new UISettings if another one is already loaded.
        /// </summary>
        /// <param name="settings">Used to indicate the target UI settings.</param>
        public void InitMenus(UISettings settings)
        {
            if (!m_menuLoader.IsLoaded)
            {
                throw new InvalidOperationException("You must initialize Menu Manager first");
            }

            m_menuLoader.Load(settings, Canvas, OnLoadMenuCallback);
        }

        /// <summary>
        /// Propagates an event through the active menus in all the layers to open the last active menu in each layer.
        /// If a menu has the <c>ShouldSwallowBackEvent</c> property active, the event will not continue propagating.
        /// </summary>
        public void GoBack()
        {
            if (!m_menuLoader.IsLoaded)
            {
                throw new InvalidOperationException("You must initialize Menu Manager first");
            }

            foreach (Menu activeMenu in m_menuLoader.GetTopActiveMenus())
            {
                if (activeMenu.IsGoBackEnabled && activeMenu.HasFocus)
                {
                    activeMenu.HandleGoBack();
                }
            }
        }

        #endregion

        #region Protected and private methods

        internal void UpdateFocus()
        {
            bool shouldEnableFocus = true;

            if (IsPlayingAnimation())
            {
                shouldEnableFocus = false;
            }

            foreach (Menu menu in m_menuLoader.GetTopActiveMenus())
            {
                NotifyFocusChange(menu, shouldEnableFocus);

                if (menu.ShouldSwallowFocus)
                {
                    shouldEnableFocus = false;
                }
            }
        }

        private void InitMenuLoader()
        {
            var settings = (UISettings)Resources.Load(UISettings.AssetName);
            m_menuLoader.Load(settings, Canvas, OnLoadMenuCallback);
        }

        private bool IsPlayingAnimation()
        {
            foreach (Menu menu in m_menuLoader.GetTopActiveMenus())
            {
                if (menu.IsPlayingAnimation)
                {
                    return true;
                }
            }

            return false;
        }

        private void NotifyFocusChange(Menu menu, bool shouldEnableFocus)
        {
            if (shouldEnableFocus != menu.HasFocus)
            {
                if (shouldEnableFocus)
                {
                    menu.OnFocusedMenu();
                }
                else
                {
                    menu.OnUnfocusedMenu();
                }
            }
        }
        
        private void OnLoadMenuCallback(Menu menu)
        {
            menu.OnLoadMenu(this);
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_ANDROID
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel"))
            {
                GoBack();
            }
#endif
        }

        #endregion Protected and private methods

        [SerializeField] Canvas m_canvas;
        [SerializeField] EventSystem m_eventSystem;

        MenuLoader m_menuLoader = new MenuLoader();
    }
}
