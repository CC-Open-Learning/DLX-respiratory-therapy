using System;
using UnityEngine;
using VARLab.Velcro;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.RespiratoryTherapy
{
    public class MainMenuController : MonoBehaviour,IUserInterface
    {
        private Button restartButton;
        private Button continueButton;
        private Button settingsButton;
        private Button LevelSelecttButton;
        private Button backToLevelSelectButton;
        
        private VisualElement mainMenuRoot;
        private VisualElement levelSelectButtonVE;
        private VisualElement restartButtonVE;
        private VisualElement continueButtonVE;
        private VisualElement settingsButtonEV;
        private VisualElement backToLevelSelectEV;
        
        private UIManager uiManager;
      
        public LearnerSessionHandler LearnerSessionHandler;
        
        [Header("Events")]
        [Tooltip("Invokes OrientationController.ResetOrientation() | BiopsyController.UnloadBiopsy()")]
        public UnityEvent OnLevelSelectModalDisplay = new ();
        [Tooltip("Invokes OrientationController.InitializeOrientation()")]
        public UnityEvent OnOrientationContinue = new ();
        [Tooltip("Invokes BiopsyController.InitializeBiopsy()")]
        public UnityEvent OnBiopsyContinue = new ();
        [Tooltip("Invokes RestartModalController.Show()")]
        public UnityEvent OnRestartModalDisplay = new ();
        [Tooltip("Invokes UIManager.ShowSettingsMenu()")]
        public UnityEvent OnSettingsPanelDisplay = new ();
        [Tooltip("Invokes OrientationController.SetShowWelcomeScreen()")]
        public UnityEvent<bool> OnWelcomeMessageDisplay = new ();
        [Tooltip("Invokes CameraManager.ToggleMenuCameraReset()")]
        public UnityEvent<bool> OnMainMenuShown;
        [Tooltip("Invokes CameraManager.ToggleMenuCameraReset()")]
        public UnityEvent<bool> OnMainMenuHidden;

        private void Awake()
        {
            mainMenuRoot = GetComponent<UIDocument>().rootVisualElement;
            
            restartButtonVE = mainMenuRoot.Q<VisualElement>("Button-Restart");
            continueButtonVE = mainMenuRoot.Q<VisualElement>("Button-Continue");
            settingsButtonEV = mainMenuRoot.Q<VisualElement>("Button-Settings");
            levelSelectButtonVE = mainMenuRoot.Q<VisualElement>("Button-Level-Select");
            backToLevelSelectEV = mainMenuRoot.Q<VisualElement>("Button-BackTo-Level-Select");
            
            restartButton = restartButtonVE.Q<Button>();
            continueButton = continueButtonVE.Q<Button>();
            settingsButton = settingsButtonEV.Q<Button>();
            LevelSelecttButton = levelSelectButtonVE.Q<Button>();
            backToLevelSelectButton = backToLevelSelectEV.Q<Button>();            
            
            continueButton.clicked += () => Continue();
            settingsButton.clicked += () => ShowSettingsPanel();
			LevelSelecttButton.clicked += () => ShowSelectLevel();
            restartButton.clicked += () => ShowRestartConfirmation();
			backToLevelSelectButton.clicked += ()=>ShowSelectLevel();
            
            Hide();
        }

        private void Start()
        {
            uiManager = UIManager.Instance;
        }

        /// <summary>
        /// Enables the start menu by displaying the start and settings buttons,
        /// while ensuring other menu buttons are disabled.
        /// </summary>
        private void EnableStartMenu()
        {
            DisableMenuButtons();
            
            LevelSelecttButton.style.display = DisplayStyle.Flex;
            settingsButtonEV.style.display = DisplayStyle.Flex;
        }
        
        /// <summary>
        /// Enables the continue menu by displaying the continue, restart and settings buttons,
        /// while ensuring other menu buttons are disabled.
        /// </summary>
        private void EnableContinueMenu()
        {
            DisableMenuButtons();
            
            restartButtonVE.style.display = DisplayStyle.Flex;
            continueButtonVE.style.display = DisplayStyle.Flex;
            settingsButtonEV.style.display = DisplayStyle.Flex;
            backToLevelSelectEV.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// Hides all menu buttons by setting their display style to None.
        /// </summary>
        private void DisableMenuButtons()
        {
            restartButtonVE.style.display = DisplayStyle.None;
            continueButtonVE.style.display = DisplayStyle.None;
            settingsButtonEV.style.display = DisplayStyle.None;
            LevelSelecttButton.style.display = DisplayStyle.None;
            backToLevelSelectEV.style.display = DisplayStyle.None;
        }
        
        /// <summary>
        /// Continues the orientation process by disabling the welcome screen 
        /// and starting the orientation sequence.
        /// </summary>
        private void Continue()
        {
            switch (uiManager.simulationStatus)
            {
                case SimulationStatus.Orientation:
                    OnOrientationContinue?.Invoke();
                    OnWelcomeMessageDisplay?.Invoke(false);
                    break;
                case SimulationStatus.Biopsy:
                    OnBiopsyContinue?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Displays the restart confirmation modal by invoking the corresponding event.
        /// </summary>
        private void ShowRestartConfirmation()
        {
            OnRestartModalDisplay?.Invoke();
        }
        
        /// <summary>
        /// Displays the settings panel by invoking the OnSettingsPanelDisply event.
        /// </summary>
        private void ShowSettingsPanel()
        {
            OnSettingsPanelDisplay?.Invoke();
        }
        
        /// <summary>
        /// Initializes the main menu by determining whether to show the start menu 
        /// for first-time users or the continue menu for returning users, then displays the menu.
        /// </summary>
        public void InitializeMainMenu()
        {
            if (LearnerSessionHandler.IsFirstTimeUser)
            {
                EnableStartMenu();
            }
            else
            {
                EnableContinueMenu();
            }
            
            Show();
        }
        
        /// <summary>
        /// Hides the current UI and displays the level selection modal by invoking the associated event.
        /// </summary>
        public void ShowSelectLevel()
        {
            Hide();
            OnLevelSelectModalDisplay?.Invoke();
        }
        
        /// <summary>
        /// Displays the main menu by setting its root element's display style to Flex,
        /// making it visible in the UI.
        /// </summary>
        public void Show()
        {
            if (uiManager.simulationStatus == SimulationStatus.Orientation)
            {
                OnMainMenuShown?.Invoke(true);
            }
            mainMenuRoot.Show();
        }

        /// <summary>
        /// Hides the main menu by setting its root element's display style to None, making it invisible in the UI.
        /// </summary>
        public void Hide()
        {
            OnMainMenuHidden?.Invoke(false);
            mainMenuRoot.Hide();
        }
    }
}
