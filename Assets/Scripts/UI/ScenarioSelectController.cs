using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    [RequireComponent(typeof(UIDocument))]
    public class ScenarioSelectController : MonoBehaviour, IUserInterface
    {
        private Button btnBack;
        private Button btnSetup;
        private Button btnPatientProcedure;
        private Button btnProcedure;
        private Button btnPostProcedure;
        private Label labelDescription;
        private VisualElement statusCompleteSetup;
        private VisualElement statusCompletePatientProcedure;
        private VisualElement statusCompleteProcedure;
        private VisualElement statusCompletePostProcedure;

        private VisualElement statusinProgressSetup;
        private VisualElement statusinProgressPatientProcedure;
        private VisualElement statusinProgressProcedure;
        private VisualElement statusinProgressPostProcedure;
        [HideInInspector] public VisualElement ScenarioRoot { get; protected set; }
        private UIManager uiManager;
        [SerializeField] private BiopsyController biopsyController;
        
        public UnityEvent OnProcedureStarted = new ();

        private void Awake()
        {
            uiManager = UIManager.Instance;
            // Initialize the root element
            var uiDocument = GetComponent<UIDocument>();
            ScenarioRoot = uiDocument.rootVisualElement;
            SetUpListeners();
            labelDescription = ScenarioRoot.Q<Label>("DescriptionLabel");
            SetElementText("Select the task you want to complete");
            statusCompleteSetup.Hide();
            statusCompletePatientProcedure.Hide();
            statusCompleteProcedure.Hide();
            statusCompletePostProcedure.Hide();

            statusinProgressSetup.Hide();
            statusinProgressPatientProcedure.Hide();
            statusinProgressProcedure.Hide();
            statusinProgressPostProcedure.Hide();

            Hide();
        }

        private void SetUpListeners()
        {
            btnBack = ScenarioRoot.Q<Button>("Icon");
            btnBack.clicked += () => BackButton();

            var setUpContainer = ScenarioRoot.Q<VisualElement>("SetupContainer");
            btnSetup = setUpContainer.Q<Button>("CardButton");
            // btnSetup.clicked += () => EnterSetupScenario();
            var PatientPrepContainer = ScenarioRoot.Q<VisualElement>("PatientPrepContainer");
            btnPatientProcedure = PatientPrepContainer.Q<Button>("CardButton");
            // btnPatientProcedure.clicked += () => EnterPatientPrepScenario();
            var ProcedureContainer = ScenarioRoot.Q<VisualElement>("ProcedureContainer");
            btnProcedure = ProcedureContainer.Q<Button>("CardButton");
            btnProcedure.clicked += () => EnterProcedureScenario();
            var PostProcedureContainer = ScenarioRoot.Q<VisualElement>("PostProcedureContainer");
            btnPostProcedure = PostProcedureContainer.Q<Button>("CardButton");
            // btnPostProcedure.clicked += () => EnterPostProcedureScenario();

            statusCompleteSetup = setUpContainer.Q<VisualElement>("CompleteStatus");
            statusCompletePatientProcedure = PatientPrepContainer.Q<VisualElement>("CompleteStatus");
            statusCompleteProcedure = ProcedureContainer.Q<VisualElement>("CompleteStatus");
            statusCompletePostProcedure = PostProcedureContainer.Q<VisualElement>("CompleteStatus");

            statusinProgressSetup = setUpContainer.Q<VisualElement>("InProgressStatus");
            statusinProgressPatientProcedure = PatientPrepContainer.Q<VisualElement>("InProgressStatus");
            statusinProgressProcedure = ProcedureContainer.Q<VisualElement>("InProgressStatus");
            statusinProgressPostProcedure = PostProcedureContainer.Q<VisualElement>("InProgressStatus");
        }
        
        private void EnterProcedureScenario()
        {
            Hide();
            if (uiManager == null) { uiManager = UIManager.Instance; }
            uiManager.SetUpToolbar(new ToolbarConfig() { ShowMenuButton = true });
            uiManager.ShowScreen(UIPanel.Toolbar);
            uiManager.ToggleProcedureToolbar(true);
            uiManager.ShowScreen(UIPanel.ProcedureToolbar);

            OnProcedureStarted?.Invoke();
        }
        
        private void SetElementText(string descriptionContent)
        {
            labelDescription.text = descriptionContent;
        }
        
        private void BackButton()
        {
            Hide();
            if (uiManager!=null)// this is here mostly for the test runner
            {
               uiManager.MainMenuController.ShowSelectLevel();
            }
        }
        
        public void Hide()
        {
            ScenarioRoot.Hide();
        }

        public void Show()
        {
            ScenarioRoot.Show();
        }
    }
}