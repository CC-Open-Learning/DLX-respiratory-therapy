using System;
using UnityEngine;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    public class ToolbarController : MonoBehaviour,IUserInterface
    {
        private VisualElement toolbarRoot;
        private UIManager uiManager;
        private Button handbookButton;
        private Button homeButton;
        
        private Action homeButtonCallback;
        private Action handbookButtonCallback;
        
        private void Start()
        {
            uiManager = UIManager.Instance;
            
            toolbarRoot = GetComponent<UIDocument>().rootVisualElement;
            homeButton = toolbarRoot.Q<Button>("HomeButton");
            handbookButton = toolbarRoot.Q<Button>("HandbookButton");
            
            HideToolbarButtons();
            Hide();
        }

        private void HideToolbarButtons()
        {
            handbookButton.style.display = DisplayStyle.None;
            homeButton.style.display = DisplayStyle.None;
        }

        private void RemoveListners()
        {
            handbookButton.clicked -= OnHandbookButtonClicked;
            homeButton.clicked -= OnHomeButtonClicked;
        }

        private void OnHomeButtonClicked()
        {
            homeButtonCallback?.Invoke();
            uiManager.HandleUIPanels();
            uiManager.ShowMainMenu();
            ResetToolbar();
        }
        
        private void OnHandbookButtonClicked()
        {
            handbookButtonCallback?.Invoke();
            uiManager.HandleUIPanels();
            uiManager.ShowHandbook();
        }

        public void ResetToolbar()
        {
            RemoveListners();
            HideToolbarButtons();
            Hide();
        }

        public void SetToolBarUI(ToolbarConfig config)
        {
            ResetToolbar();
            
            if (config.ShowHandbookButton)
            {
                handbookButton.style.display = DisplayStyle.Flex;
                handbookButtonCallback = config.HandbookButtonCallback;
                handbookButton.clicked += OnHandbookButtonClicked;
            }
            if (config.ShowMenuButton)
            {
                homeButton.style.display = DisplayStyle.Flex;
                homeButtonCallback = config.HomeButtonCallback;
                homeButton.clicked += OnHomeButtonClicked;
            }
        }

        /// <summary>
        /// Displays the toolbar by calling the UI helper to show the toolbar UI element.
        /// </summary>
        public void Show()
        {
            toolbarRoot.Show();
        }

        /// <summary>
        /// Hides the toolbar by calling the UI helper to hide the toolbar UI element.
        /// </summary>
        public void Hide()
        {
            toolbarRoot.Hide();
        }
        
        public void DisableToolbarInteraction()
        {
            SetButtonsEnabled(false);
        }

        public void EnableToolbarInteraction()
        {
            SetButtonsEnabled(true);
        }

        private void SetButtonsEnabled(bool enabled)
        {
            homeButton.SetEnabled(enabled);
            handbookButton.SetEnabled(enabled);
        }
    }
}