using UnityEngine;
using VARLab.Velcro;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.RespiratoryTherapy
{
    public class RestartModalController : MonoBehaviour, IUserInterface
    {
        private UIManager uiManager;
        private Button cancelButton;
        private Button restartButton;
        private VisualElement restartModalRoot;
        
        [Header("Events")]
        [Tooltip("Invokes OrientationController.ResetOrientation()")]
        public UnityEvent OnOrientationRestart = new ();
        public UnityEvent OnBiopsyRestart = new ();
        [Tooltip("Invokes MainMenuController.InitializeMainMenu()")]
        public UnityEvent OnInitializeMainMenu = new ();
        
        private void Awake()
        {
            restartModalRoot = GetComponent<UIDocument>().rootVisualElement;
            cancelButton = restartModalRoot.Q<VisualElement>("CancelButtonContainer").Q<Button>();
            restartButton = restartModalRoot.Q<VisualElement>("RestartButtonContainer").Q<Button>();
            
            cancelButton.clicked += () => Cancel();
            restartButton.clicked += () => Restart();
            
            Hide();
        }

        private void Start()
        {
            uiManager = UIManager.Instance;
        }

        /// <summary>
        /// Hide the restart confirmation UI by setting its root element's display style to None,
        /// making it invisible in the UI.
        /// </summary>
        private void Cancel()
        {
            Hide();
        }

        /// <summary>
        /// Hide the restart confirmation UI by setting its root element's display style to None,
        /// Trigger OnOrientationRestart event which will call reset method in MainMenuController.
        /// </summary>
        private void Restart()
        {
            Hide();
            
            if (uiManager.simulationStatus == SimulationStatus.Orientation)
            {
                OnOrientationRestart?.Invoke();
            }
            else
            {
                OnBiopsyRestart?.Invoke();
            }
            
            OnInitializeMainMenu?.Invoke();
        }
        
        /// <summary>
        /// Displays the main menu by setting its root element's display style to Flex,
        /// making it visible in the UI.
        /// </summary>
        public void Show()
        {
            restartModalRoot.Show();
        }

        /// <summary>
        /// Hides the main menu by setting its root element's display style to None, making it invisible in the UI.
        /// </summary>
        public void Hide()
        {
            restartModalRoot.Hide();
        }
    }
}
