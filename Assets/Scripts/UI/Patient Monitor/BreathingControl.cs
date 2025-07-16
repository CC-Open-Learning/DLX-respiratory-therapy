using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    /// <summary>
    /// Displays the patient controls UI for the breathing check scenario.
    /// It reuses the patient control UI meant for the Vitals control monitor, but has custom functionality for the
    /// breathing check scenario.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class BreathingControl : MonoBehaviour, IUserInterface
    {
        /// Invokes <see cref="StethoscopeComponent.ValidateProceedOrPause"/>
        public UnityEvent OnProceedBtnClicked;
        /// Invokes <see cref="StethoscopeComponent.ValidateProceedOrPause"/>
        public UnityEvent OnPauseBtnClicked;
        
        public VisualElement Root {  get; private set; }
        private Button proceedButton;
        private Button pauseButton;
        private Label breathingCheckLabel;
        private const string BreathingCheckLabelText = "Proceed if breathing is normal";
        
        private void Start()
        {
            OnProceedBtnClicked ??= new UnityEvent();
            OnPauseBtnClicked ??= new UnityEvent();
            
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            proceedButton = Root.Q<Button>("ProceedButton");
            pauseButton = Root.Q<Button>("PauseButton");
            breathingCheckLabel = Root.Q<Label>("PatientMonitorLabel");
            
            // Button click events
            proceedButton.clicked += () => { OnProceedBtnClicked?.Invoke(); };
            pauseButton.clicked += () => { OnPauseBtnClicked?.Invoke(); };

            // Set label text
            breathingCheckLabel.text = BreathingCheckLabelText;
            
            Root.Hide();
        }
        
        public void Show()
        {
            if (Root == null) { Root = gameObject.GetComponent<UIDocument>().rootVisualElement; }
            Root.Show();
        }
        
        public void Hide()
        {
            if (Root == null) { Root = gameObject.GetComponent<UIDocument>().rootVisualElement; }
            Root.Hide();
        }
    }
}
