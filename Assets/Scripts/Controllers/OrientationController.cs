using System;
using System.Linq;
using UnityEngine;
using VARLab.Velcro;
using Newtonsoft.Json;
using VARLab.CloudSave;
using VARLab.Analytics;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace VARLab.RespiratoryTherapy
{
    /// <summary>
    /// Controller that handles the orientation section of the simulation
    /// </summary>
    [CloudSaved]
    [JsonObject(MemberSerialization.OptIn)]
    public class OrientationController : MonoBehaviour, IUserInterface
    {
        private const int MinProgressionAmount = 1;
        private const string ArrowClosedClass = "progress-v1-category-arrow-closed";

        [JsonProperty]
        private bool shouldShowConclusionPanel;

        private UIManager uiManager;
        private POIManager poiManager;
        private OrientationControllerLogic orientationLogic;
        private LearnerSessionHandler learnerSessionHandler;

        private Dictionary<string, VisualElement> indicatorArrows;
        private Dictionary<string, VisualElement> indicatorTaskHolders;
        
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private BiopsyController biopsyController;
        [SerializeField] private GameObject defaultOrientationCamera;

        public bool ShowWelcome;
        public GameObject medicalCart;
        public GameObject bronchTower;
        public ProgressIndicatorV1 OrientationChecklist;
        
        [Header("Events")]
        /// Invokes <see cref="ProgressIndicatorV1.Show"/>
        public UnityEvent ShowChecklist;
        /// Invokes <see cref="ProgressIndicatorV1.Hide"/>
        public UnityEvent HideChecklist;
        /// Invokes <see cref="CustomSaveHandler.Save"/>
        public UnityEvent OnSaveItem;
        /// Invokes <see cref="CameraManager.SetDefaultCamera(GameObject)"/>
        public UnityEvent<GameObject> OnOrientationStarted;
        
        public UnityEvent OnDeleteItem;
        
        public UnityEvent ChangeRoom;
        
        private bool isOrientationPlaying;
        
        private void Start()
        {
            if (OnOrientationStarted == null) { OnOrientationStarted = new UnityEvent<GameObject>(); }

            shouldShowConclusionPanel = true;
            isOrientationPlaying = false;
            poiManager = POIManager.Instance;
            uiManager = UIManager.Instance;
            learnerSessionHandler = LearnerSessionHandler.Instance;
            orientationLogic = new OrientationControllerLogic();

            indicatorTaskHolders = new Dictionary<string, VisualElement>();
            indicatorArrows = new Dictionary<string, VisualElement>();
            
            LoadOrientationExplorable();
        }

        /// <summary>
        /// Loads and adds all explorable objects to the orientation logic.
        /// Finds all inactive explorable objects in the scene and adds them to the orientation logic's explorable list.
        /// </summary>
        private void LoadOrientationExplorable()
        {
            orientationLogic.AddExplorable(
                FindObjectsByType<Explorable>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList());
        }

        /// <summary>
        /// Initializes and starts the orientation process.
        /// Configures the orientation toolbar, displays relevant UI elements, 
        /// manages the user session, and updates the simulation status to Orientation.
        /// </summary>
        private void StartOrientation()
        {
            ManageUserSession();
            HandleOrientationToolbar();
            Show();
        }

        /// <summary>
        /// Configures and displays the toolbar with the specified settings, including showing the menu button.
        /// </summary>
        private void HandleOrientationToolbar()
        {
            uiManager.SetUpToolbar(new ToolbarConfig() { ShowMenuButton = true });
            uiManager.ShowScreen(UIPanel.Toolbar);
        }

        /// <summary>
        /// Manages the learner's session based on their status.
        /// If the user is a first-time learner, updates their status accordingly. 
        /// Otherwise, loads the user's previous progress.
        /// </summary>
        private void ManageUserSession()
        {
            if (learnerSessionHandler.IsFirstTimeUser)
            {
                learnerSessionHandler.SetUserStatus(true);
                CoreAnalytics.CustomEvent("new_user_session","orintaion",1);
            }
            else
            {
                LoadProgress();
                CoreAnalytics.CustomEvent("existing_user_session","orintaion",1);
            }
        }

        /// <summary>
        /// Populates the checklist with all the categories and tasks that will need to be found throughout
        /// the Orientation module of the sim.
        /// </summary>
        private void PopulateChecklist()
        {
            if (OrientationChecklist.CategoryCount != 0)
            {
                return;
            }
            AddCategoriesToChecklist();
            AddExplorableToCategories();

            PopulateIndicatorDictionaries();
            SetupIndicatorAutoCollapse();
            IndicatorCollapseAll();
        }

        /// <summary>
        /// Populates the orientation checklist with all categories of explorable items.
        /// Iterates through the available categories in the ExplorableCategory enumeration, 
        /// formats their names, and adds them to the checklist.
        /// </summary>
        private void AddCategoriesToChecklist()
        {
            foreach (ExplorableCategory category in Enum.GetValues(typeof(ExplorableCategory)))
            {
                OrientationChecklist.AddCategory(NameFormatter.ToDescription(category));
            }
        }

        /// <summary>
        /// Iterates through all explorable items in the orientation logic, retrieves their category and name,
        /// and adds them as tasks to the appropriate list to be added to the checklist.
        /// </summary>
        private void AddExplorableToCategories()
        {
            List<Explorable> bronchTowerList = new ();
            List<Explorable> otherExplorableList = new ();
            
            foreach (Explorable explorable in orientationLogic.Explorables)
            {
                if (explorable.explorableInformation.ExplorableCategory == ExplorableCategory.BronchTower)
                {
                   bronchTowerList.Add(explorable);
                }
                else
                {
                    bronchTowerList.Add(explorable);
                }
            }
            
            bronchTowerList = bronchTowerList.OrderBy(x => x.explorableInformation.PoiType).ToList();
            AddToCheckList(bronchTowerList);
            AddToCheckList(otherExplorableList);
        }

        /// <summary>
        /// Adds explorable items to their respective categories in the orientation checklist.
        /// </summary>
        public void AddToCheckList(List<Explorable> explorables)
        {
            foreach (Explorable explorable in explorables)
            {
                int categoryIndex = (int)explorable.explorableInformation.ExplorableCategory;
                OrientationChecklist.AddTask(
                    categoryIndex,
                    explorable.explorableInformation.ExplorableName,
                    MinProgressionAmount);
            }
        }

        /// <summary>
        /// Populates Progress Indicator dictionaries with the necessary VisualElements. These are
        /// used elsewhere to modify the behaviour of the category arrows and allow for category auto-collapsing.
        /// </summary>
        private void PopulateIndicatorDictionaries()
        {
            VisualElement categoryHolder = OrientationChecklist.Root.Q<VisualElement>("CategoryHolder");
            foreach (VisualElement category in categoryHolder.Children())
            {
                string categoryName = category.Q<Label>("TitleLabel").text;
                VisualElement taskHolder = category.Q<VisualElement>("TaskHolder");
                indicatorTaskHolders.Add(categoryName, taskHolder);

                VisualElement arrow = category.Q<VisualElement>("ArrowHolder");
                indicatorArrows.Add(categoryName, arrow);
            }
        }

        /// <summary>
        /// Registers an Orientation-specific callback to the Progress Indicator arrows.
        /// </summary>
        private void SetupIndicatorAutoCollapse()
        {
            foreach (KeyValuePair<string, VisualElement> arrow in indicatorArrows)
            {
                arrow.Value.RegisterCallback<ClickEvent, string>(HideOtherCategories, arrow.Key);
            }
        }

        private void IndicatorCollapseAll()
        {
            foreach (KeyValuePair<string, VisualElement> taskHolder in indicatorTaskHolders)
            {
                taskHolder.Value.Hide();
                indicatorArrows[taskHolder.Key].Q<VisualElement>("Arrow").EnableInClassList(ArrowClosedClass, true);
            }
        }

        /// <summary>
        /// Callback used in Orientation that makes the Progress Indicator arrows collapse all other
        /// categories when clicked on.
        /// </summary>
        private void HideOtherCategories(ClickEvent e, string categoryName)
        {
            foreach (KeyValuePair<string, VisualElement> taskHolder in indicatorTaskHolders)
            {
                if (taskHolder.Key != categoryName)
                {
                    taskHolder.Value.Hide();
                    indicatorArrows[taskHolder.Key].Q<VisualElement>("Arrow").EnableInClassList(ArrowClosedClass, true);
                }
            }
        }

        /// <summary>
        /// Expands the given category and collapses all other categories.
        /// </summary>
        public void HandleCategoryFocused(string categoryName)
        {
            foreach (KeyValuePair<string, VisualElement> taskHolder in indicatorTaskHolders)
            {
                if (taskHolder.Key != categoryName)
                {
                    taskHolder.Value.Hide();
                    indicatorArrows[taskHolder.Key].Q<VisualElement>("Arrow").EnableInClassList(ArrowClosedClass, true);
                }
                else
                {
                    taskHolder.Value.Show();
                    indicatorArrows[taskHolder.Key].Q<VisualElement>("Arrow").EnableInClassList(ArrowClosedClass, false);
                }
            }
        }

        /// <summary>
        /// Displays a welcome message for the orientation process by calling the UI manager, 
        /// and sets the StartOrientation method to execute after the message is shown.
        /// </summary>
        private void ShowOrientationWelcome()
        {
            uiManager.ShowMessagePanel(MessageType.Welcome, ContentInfo.OrientationWelcome, StartOrientation);
        }

        /// <summary>
        /// Displays the conclusion panel at the end of the orientation phase.
        /// Triggers the main menu display after the conclusion message.
        /// </summary>
        private void ShowConclusionPanel()
        {
            poiManager.HandlePOICameraReset();
            Hide();
            uiManager.HideScreen(UIPanel.Toolbar);
            uiManager.HideScreen(UIPanel.POIToolbar);
            uiManager.HideScreen(UIPanel.Inventory);
            uiManager.ShowMessagePanel(MessageType.Completion, ContentInfo.OrientationConclusion, ShowMainMenuAfterConclusion);
            CoreAnalytics.CustomEvent("orintaion_completion","orintaion",1);
        }

        /// <summary>
        /// Transitions to the main menu after the conclusion by showing the orientation item selection,
        /// managing UI panels, and displaying the main menu.
        /// </summary>
        private void ShowMainMenuAfterConclusion()
        {
            poiManager.ResetPOIs();
            uiManager.HandleUIPanels();
            uiManager.ShowMainMenu();
        }

        /// <summary>
        /// Determines and handles the initial view for the orientation process.
        /// Displays the orientation welcome screen if enabled; otherwise, directly starts the orientation.
        /// </summary>
        private void HandleInitialView()
        {
            PopulateChecklist();
            
            poiManager.ResetPOIs();
            
            if (ShowWelcome)
            {
                ShowOrientationWelcome();
            }
            else
            {
                StartOrientation();
            }
        }

        public void InitializeOrientation()
        {
            StartCoroutine(InitializeOrientationSequence());
        }

        private IEnumerator InitializeOrientationSequence()
        {
            const float cameraTransitionDelay = 1.0f;
            const float loadingScreenDelay = 3.0f;
            
            
            //Show load screen.
            if (!isOrientationPlaying)
            {
                isOrientationPlaying = true;
                uiManager.ShowLoadingScreen?.Invoke(loadingScreen);
                yield return new WaitForSeconds(cameraTransitionDelay);
                uiManager.HideScreen(UIPanel.MainMenu);
                uiManager.HideScreen(UIPanel.LevelSelectController);
                ChangeRoom?.Invoke();
                OnOrientationStarted?.Invoke(defaultOrientationCamera);
                yield return new WaitForSeconds(loadingScreenDelay);
            }
            else
            {
                uiManager.HideScreen(UIPanel.MainMenu);
                uiManager.HideScreen(UIPanel.LevelSelectController);
            }

            //Handle Initial View
            HandleInitialView();
            
        }

        /// <summary>
        /// Displays the orientation checklist UI element.
        /// </summary>
        public void Show()
        {
            OrientationChecklist.Show();
        }

        /// <summary>
        /// Hide the orientation checklist UI element.
        /// </summary>
        public void Hide()
        {
            OrientationChecklist.Hide();
        }

        /// <summary>
        /// TODO: Implement this once we have more save & load functionality done to load
        /// the progress of the player from a previous time playing
        /// </summary>
        public void LoadProgress()
        {
            foreach (Explorable explorable in orientationLogic.Explorables.Where(explorable => explorable.IsExplored))
            {
                OrientationChecklist.FindCategoryWithName(NameFormatter.ToDescription(
                    explorable.explorableInformation.ExplorableCategory), out int categoryIndex);
                OrientationChecklist.FindTaskWithName(explorable.explorableInformation.ExplorableName, categoryIndex, out int taskIndex);
                OrientationChecklist.AddProgressToTask(categoryIndex, taskIndex, 1);
            }
        }

        public void ResetOrientation()
        {
            shouldShowConclusionPanel = true;
            SetShowWelcome(true);
            
            foreach (Explorable explorable in orientationLogic.Explorables.Where(explorable => explorable.IsExplored))
            {
                OrientationChecklist.FindCategoryWithName(NameFormatter.ToDescription(
                    explorable.explorableInformation.ExplorableCategory), out int categoryIndex);
                OrientationChecklist.FindTaskWithName(explorable.explorableInformation.ExplorableName, categoryIndex, out int taskIndex);
                OrientationChecklist.RemoveProgressFromTask(categoryIndex, taskIndex, 1);
                explorable.IsExplored = false;
            }
            
            learnerSessionHandler.SetUserStatus(false);
            OnDeleteItem?.Invoke();
        }

        /// <summary>
        /// This method updates the progress of one of the tasks on the orientation checklist
        /// </summary>
        /// <param name="categoryName">The category that the task is in</param>
        /// <param name="taskName">The name of the task to be updated</param>
        /// <param name="amountProgressed">How much steps of progression to advance the task</param>
        public void UpdateProgress(ExplorableCategory categoryName, string taskName, int amountProgressed)
        {
            //Check that category exists
            if (!OrientationChecklist.FindCategoryWithName(NameFormatter.ToDescription(categoryName), out int categoryIndex))
            {
                Debug.LogError($"Could not find the category: {categoryName}");
                return;
            }

            //Check that task exists
            if (!OrientationChecklist.FindTaskWithName(taskName, categoryIndex, out int taskIndex))
            {
                Debug.LogError($"Could not find the task: {taskName}");
                return;
            }

            //Check that amount progression is at least 1
            if (amountProgressed < MinProgressionAmount)
            {
                Debug.LogError("Must add at least 1 point of progression");
                return;
            }

            //Update progress at specified task
            OrientationChecklist.AddProgressToTask(categoryIndex, taskIndex, amountProgressed);
        }

        /// <summary>
        ///Checks if the orientation process is completed. If it is, displays a completion message to the user.
        /// </summary>
        public void CheckForOrientationCompletion()
        {
            if (orientationLogic.IsOrientationCompleted() && shouldShowConclusionPanel)
            {
                shouldShowConclusionPanel = false;
                OnSaveItem?.Invoke();
                ShowConclusionPanel();
            }
        }
        
        /// <summary>
        /// called when user interacts with "Orientation" or "Procedure" module button of main menu
        /// </summary>
        public void ToggleOrientationModuleItems()
        {
            SimulationStatus simulationStatus = uiManager.simulationStatus;
           // biopsyController.ToggleProcedureModuleItems();
            
            if (simulationStatus == SimulationStatus.Biopsy)
            {
                medicalCart.SetActive(false);
                bronchTower.SetActive(false);
            }

            if (simulationStatus == SimulationStatus.Orientation)
            {
                medicalCart.SetActive(true);
                bronchTower.SetActive(true);
            }
        }

        /// <summary>
        /// Sets the value of ShowWelcome to the provided status.
        /// </summary>
        /// <param name="status">The value to set ShowWelcome to (true or false).</param>
        public void SetShowWelcome(bool status)
        {
            ShowWelcome = status;
        }

        public void SetOrientationPlayStatus(bool status)
        {
            isOrientationPlaying = status;
        }
    }
}