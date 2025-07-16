using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    [RequireComponent(typeof(UIDocument))]
    public class PatientMonitorReport : MonoBehaviour, IUserInterface
    {
        [Header("Event Hook-Ins")]
        public UnityEvent OnPatientMonitorReportShown;
        public UnityEvent OnPatientMonitorReportHidden;
        /// Invokes <see cref="PatientMonitorControl.Show"/>
        public UnityEvent OnBackBtnClicked;
        /// Invokes <see cref="PatientMonitorManager.OnVitalsCheckReport(List{PatientMonitorVitals})"/>
        public UnityEvent<List<PatientMonitorVitals>> OnReportBtnClicked;

        public VisualElement Root {  get; private set; }
        private Button backButton;
        private Button reportButton;
        private Dictionary<PatientMonitorVitals, Toggle> vitalsToggles;

        private void Start()
        {
            OnPatientMonitorReportShown ??= new UnityEvent();
            OnPatientMonitorReportHidden ??= new UnityEvent();
            OnBackBtnClicked ??= new UnityEvent();
            OnReportBtnClicked ??= new UnityEvent<List<PatientMonitorVitals>>();

            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            backButton = Root.Q<Button>("BackButton");
            reportButton = Root.Q<Button>("ReportButton");

            backButton.clicked += () =>
            {
                OnBackBtnClicked?.Invoke();
                Hide();
            };

            reportButton.clicked += () =>
            {
                OnReportBtnClicked?.Invoke(GetAllCheckedVitals());
            };

            vitalsToggles = new Dictionary<PatientMonitorVitals, Toggle>
            {
                { PatientMonitorVitals.ECG, Root.Q<Toggle>("ECGCheckbox") },
                { PatientMonitorVitals.RESP, Root.Q<Toggle>("RESPCheckbox") },
                { PatientMonitorVitals.SpO2, Root.Q<Toggle>("SpO2Checkbox") },
                { PatientMonitorVitals.CO2, Root.Q<Toggle>("CO2Checkbox") },
                { PatientMonitorVitals.NIBP, Root.Q<Toggle>("NIBPCheckbox") }
            };

            Root.Hide();
        }

        /// <summary>
        /// Returns a List<string> that contains all of the currently checked vitals.
        /// </summary>
        private List<PatientMonitorVitals> GetAllCheckedVitals()
        {
            List<PatientMonitorVitals> checkedVitals = new List<PatientMonitorVitals>();

            foreach (var vital in vitalsToggles)
            {
                if (vital.Value.value)
                {
                    checkedVitals.Add(vital.Key);
                }
            }

            return checkedVitals;
        }

        /// <summary>
        /// Unchecks any currently checked vitals
        /// </summary>
        private void ClearCheckedVitals()
        {
            foreach (var vital in vitalsToggles)
            {
                vital.Value.value = false;
            }
        }

        /// <summary>
        /// Shows the root of the patient monitor report and triggers OnPatientMonitorReportShown
        /// </summary>
        public void Show()
        {
            ClearCheckedVitals();
            Root.Show();
            OnPatientMonitorReportShown?.Invoke();
        }

        /// <summary>
        /// Shows the root of the patient monitor report and triggers OnPatientMonitorReportHidden
        /// </summary>
        public void Hide()
        {
            Root.Hide();
            OnPatientMonitorReportHidden?.Invoke();
        }
    }
}