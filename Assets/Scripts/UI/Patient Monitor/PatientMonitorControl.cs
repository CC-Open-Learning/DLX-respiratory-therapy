using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    [RequireComponent(typeof(UIDocument))]
    public class PatientMonitorControl : MonoBehaviour, IUserInterface
    {
        [Header("Event Hook-Ins")]
        public UnityEvent OnPatientMonitorControlShown;
        public UnityEvent OnPatientMonitorControlHidden;
        /// Invokes <see cref="PatientMonitorManager.OnVitalsCheckProceed"/>
        public UnityEvent OnProceedBtnClicked;
        /// Invokes <see cref="PatientMonitorReport.Show"/>
        public UnityEvent OnPauseBtnClicked;

        public VisualElement Root {  get; private set; }
        private Button proceedButton;
        private Button pauseButton;

        private void Start()
        {
            OnPatientMonitorControlShown ??= new UnityEvent();
            OnPatientMonitorControlHidden ??= new UnityEvent();
            OnProceedBtnClicked ??= new UnityEvent();
            OnPauseBtnClicked ??= new UnityEvent();

            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            proceedButton = Root.Q<Button>("ProceedButton");
            pauseButton = Root.Q<Button>("PauseButton");

            proceedButton.clicked += () =>
            {
                OnProceedBtnClicked?.Invoke();
            };

            pauseButton.clicked += () =>
            {
                OnPauseBtnClicked?.Invoke();
                Hide();
            };

            Root.Hide();
        }

        /// <summary>
        /// Shows the root of the patient monitor control and triggers OnPatientMonitorControlShown
        /// </summary>
        public void Show()
        {
            Root.Show();
            OnPatientMonitorControlShown?.Invoke();
        }

        /// <summary>
        /// Shows the root of the patient monitor control and triggers OnPatientMonitorControlHidden
        /// </summary>
        public void Hide()
        {
            Root.Hide();
            OnPatientMonitorControlHidden?.Invoke();
        }
    }
}