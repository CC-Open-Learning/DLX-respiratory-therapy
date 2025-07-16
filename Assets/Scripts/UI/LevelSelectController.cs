using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    public class LevelSelectController : MonoBehaviour,IUserInterface
    {
        private Button btnBiopsy;
        private Button btnOrientation;
        private VisualElement mainMenuRoot;
        private UIManager uiManager;

        public GameObject OrientationRoom;
        public GameObject BiopsyRoom;

        ///Invokes <see cref="OrientationController.InitializeOrientation"/>
        public UnityEvent OnOrientationStarted = new ();
        
        ///Invokes <see cref="BiopsyController.InitializeBiopsy"/>
        public UnityEvent OnBiopsyStarted = new ();
        
        /// <summary>
        /// Initiates the orientation process by hiding the current screen and invoking the OnOrientationStarted event.
        /// </summary>
        private void StartOrientation()
        {
            uiManager.SetSimulationStatus(SimulationStatus.Orientation);
            OnOrientationStarted?.Invoke();
        }

        private void StartBiopsyProcedure()
        {
            uiManager.SetSimulationStatus(SimulationStatus.Biopsy);
            OnBiopsyStarted?.Invoke();
        }

        private void Awake()
        {
            mainMenuRoot = GetComponent<UIDocument>().rootVisualElement;
            btnOrientation = mainMenuRoot.Q<Button>("Orientation");//Biopsy
            btnOrientation.clicked += () => StartOrientation();
            btnBiopsy = mainMenuRoot.Q<Button>("Biopsy");
            btnBiopsy.clicked += () => StartBiopsyProcedure();
            Hide();
        }

        private void Start()
        {
            uiManager = UIManager.Instance;
        }

        /// <summary>
        /// Displays the main menu by setting its root element's display style to Flex,
        /// making it visible in the UI.
        /// </summary>
        public void Show()
        {
            mainMenuRoot.Show();
        }

        /// <summary>
        /// Hides the main menu by setting its root element's display style to None, making it invisible in the UI.
        /// </summary>
        public void Hide()
        {
            mainMenuRoot.Hide();
        }

        public void SwitchRooms()
        {
            if (uiManager.simulationStatus == SimulationStatus.Biopsy)
            {
                BiopsyRoom.SetActive(true);
                OrientationRoom.SetActive(false);
            }
            else
            {
                BiopsyRoom.SetActive(false);
                OrientationRoom.SetActive(true);
            }
        }
    }
}
