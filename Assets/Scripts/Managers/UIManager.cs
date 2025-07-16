using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using VARLab.CloudSave;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    [CloudSaved]
    [JsonObject(MemberSerialization.OptIn)]
    public class UIManager : MonoBehaviour
    {
        [JsonProperty] public SimulationStatus simulationStatus;

        public static UIManager Instance;
        public bool ShowWelcome;
        
        [Header("UI Controllers")] 
        public MainMenuController MainMenuController;
        public InventoryController InventoryController;
        public OrientationController OrientationController;
        public ObjectViewerPanelController ObjectViewerPanelBuilder;
        public MessageDisplayPanelController MessageDisplayPanelController;
        public ScenarioSelectController ScenarioSelectController;
        public ToolbarController ToolbarController;
        public POIToolbarController POIToolbarController;
        public ProcedurePOIToolbar ProcedureToolbarController;
        public PatientMonitorControl PatientMonitorControlController;
        public Notification NotificationController;
        public PatientMonitorReport PatientMonitorReportController;
        public LevelSelectController LevelSelectController;
        public HandbookController HandbookController;
        public PatientStatusController PatientStatusController;

        public Prompt Prompt;
        public SettingsMenu SettingsMenu;

        [Header("UI Panel Data")]
        public UIPanelDataSO UIPanelData;
        
        ///  Invokes <see cref="LoadingIndicator.HandleDisplayUI()"/>
        public UnityEvent<GameObject> ShowLoadingScreen;
        
        public UnityEvent LoadSettings = new ();
        
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
        }

        /// <summary>
        /// Retrieves the corresponding screen controller based on the specified UIPanel type.
        /// </summary>
        /// <param name="uiPanel">The type of UIPanel to determine which screen controller to return.</param>
        /// <returns>The IUIScreen implementation that matches the specified UIPanel.</returns>
        private IUserInterface GetUIPanelController(UIPanel uiPanel)
        {
            IUserInterface uiScreenController = uiPanel switch
            {
                UIPanel.Welcome => MessageDisplayPanelController,
                UIPanel.MainMenu => MainMenuController,
                UIPanel.Orientation => OrientationController,
                UIPanel.OrientationChecklist => OrientationController.OrientationChecklist,
                UIPanel.Inventory => InventoryController,
                UIPanel.Toolbar => ToolbarController,
                UIPanel.POIToolbar => POIToolbarController,
                UIPanel.ScenarioSelect => ScenarioSelectController,
                UIPanel.ProcedureToolbar => ProcedureToolbarController,
                UIPanel.PatientMonitorControl => PatientMonitorControlController,
                UIPanel.SettingsMenu => SettingsMenu,
                UIPanel.Notification => NotificationController,
                UIPanel.Prompt => Prompt,
                UIPanel.PatientMonitorReport => PatientMonitorReportController,
                UIPanel.LevelSelectController => LevelSelectController,
                UIPanel.Handbook => HandbookController,
                UIPanel.PatientStatus => PatientStatusController
            };

            return uiScreenController;
        }

        /// <summary>
        /// Retrieves the title and message for the specified UI panel by searching through the panel data.
        /// It finds the entry in the panel contents where the panel matches the specified UIPanel and returns its
        /// title and message.
        /// </summary>
        /// <param name="uiPanel">The UIPanel type to search for in the panel data.</param>
        /// <returns>A tuple containing the title and content (message) for the specified UIPanel.</returns>
        private PanelContent GetTitleAndContent(ContentInfo contentInfo)
        {
            return UIPanelData.PanelContents.Find(x => x.Type == contentInfo);
        }

        /// <summary>
        /// Manages the UI elements related to the orientation checklist panel. 
        /// Hides the orientation checklist panel and closes the object viewer panel if it is currently open.
        /// </summary>
        private void HandleOrientationUI()
        {
            GetUIPanelController(UIPanel.OrientationChecklist).Hide();
        }

        private void HandleGuidedProcedureUI()
        {
            HideScreen(UIPanel.ProcedureToolbar);
        }
        
        public void ShowMainMenu()
        {
            ShowScreen(UIPanel.MainMenu);
            MainMenuController.InitializeMainMenu();
        }
        
        public void ShowHandbook()
        {
            ShowScreen(UIPanel.Handbook);
        }
        
        public void ShowSettingsMenu()
        {
            HideScreen(UIPanel.MainMenu);
            ShowScreen(UIPanel.SettingsMenu);
            LoadSettings?.Invoke();
        }

        /// <summary>
        /// Initializes the UI by determining whether to show the welcome message or proceed directly to the main menu.
        /// If the welcome message should be shown, it retrieves the title and content for the welcome screen, 
        /// sets the content on the welcome screen controller, and displays the welcome screen. 
        /// Otherwise, it displays the main menu screen.
        /// </summary>
        public void Initialize()
        {
            LoadSettings?.Invoke(); 
            
            if (ShowWelcome)
            {
                // TODO: Implement start menu
            }
            else
            {
                ShowMainMenu();
            }
        }
        
        /// <summary>
        /// Displays the specified UI panel by retrieving the appropriate screen controller 
        /// and invoking its Show method to make it visible on the screen.
        /// </summary>
        /// <param name="uiPanel">The UIPanel type that indicates which screen to display.</param>
        public void ShowScreen(UIPanel uiPanel)
        {
            GetUIPanelController(uiPanel).Show();
        }

        /// <summary>
        /// Hides the specified UI panel by retrieving the corresponding screen controller 
        /// and invoking its Hide method to remove it from view.
        /// </summary>
        /// <param name="uiPanel">The UIPanel type that indicates which screen to hide.</param>
        public void HideScreen(UIPanel uiPanel)
        {
            GetUIPanelController(uiPanel).Hide();
        }

        public void SetUpPOIToolbar(string poiName, Action poiBackCallback, Sprite poiIcon = null)
        {
            POIToolbarController.SetPOIToolbarUI(poiName, poiBackCallback, poiIcon);
        }

        public void ResetPOIToolbar()
        {
            POIToolbarController.ResetPOIToolbar();
        }

        public void SetUpToolbar(ToolbarConfig toolbarConfig)
        {
            ToolbarController.SetToolBarUI(toolbarConfig);
        }

        public void ResetToolBar()
        {
            ToolbarController.ResetToolbar();
        }

        public void ToggleProcedureToolbar(bool toggle)
        {
            ProcedureToolbarController.ToggleProcedureToolbarStatus(toggle);
        }

        /// <summary>
        /// Displays a welcome message on the welcome screen based on the specified message type.
        /// Sets the content of the welcome screen and allows for an optional callback action 
        /// to be executed upon completion.
        /// </summary>
        /// <param name="contentInfo">The type of welcome message to display.</param>
        /// <param name="onComplete">An optional action to execute after the message is shown.</param>
        public void ShowMessagePanel(MessageType type, ContentInfo contentInfo, [CanBeNull] Action onComplete = null)
        {
            MessageDisplayPanelController.SetContent(type, GetTitleAndContent(contentInfo), onComplete);
            ShowScreen(UIPanel.Welcome);
        }

        /// <summary>
        /// Manages the display and state of UI panels based on the current simulation status.
        /// Calls the appropriate UI handling method depending on whether the simulation 
        /// is in the orientation or guided procedure phase.
        /// </summary>
        public void HandleUIPanels()
        {
            switch (simulationStatus)
            {
                case SimulationStatus.Orientation:
                    HandleOrientationUI();
                    break;
                case SimulationStatus.Biopsy:
                    HandleGuidedProcedureUI();
                    break;
            }
        }

        /// <summary>
        /// Sets the current simulation status to the specified status value.
        /// Updates the simulation state based on the provided status parameter.
        /// </summary>
        /// <param name="status">The new simulation status to set.</param>
        public void SetSimulationStatus(SimulationStatus status)
        {
            simulationStatus = status;
        }
    }
}