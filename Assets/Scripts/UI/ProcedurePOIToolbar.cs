using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    [RequireComponent(typeof(UIDocument))]
    public class ProcedurePOIToolbar : MonoBehaviour, IUserInterface
    {
        [Header("Event Hook-Ins")]
        public UnityEvent OnToolbarShown;
        public UnityEvent OnToolbarHidden;
        /// Invokes <see cref="POI.Open"/>
        public UnityEvent OnPatientMonitorBtnClicked;
        /// Invokes <see cref="POI.Open"/>
        public UnityEvent OnPatientViewBtnClicked;
        /// Invokes <see cref="ToolbarController.Hide"/>
        /// Invokes <see cref="POIManager.HandlePOICameraReset"/>
        public UnityEvent OnFrontViewBtnClicked;
        /// Invokes <see cref="POI.Open"/>
        public UnityEvent OnBronchoscopeBtnClicked;
        /// Invokes <see cref="POI.Open"/>
        public UnityEvent OnBronchMonitorBtnClicked;
        /// Invokes <see cref="POI.Open"/>
        public UnityEvent OnProcedureTableBtnClicked;

        public VisualElement Root { get; private set; }
        private Button patientMonitorButton;
        private Button patientViewButton;
        private Button frontViewButton;
        private Button bronchoscopeButton;
        private Button bronchMonitorButton;
        private Button procedureTableButton;

        private bool isInitalized;

        private void Start()
        {
            Root = GetComponent<UIDocument>().rootVisualElement;

            patientMonitorButton = Root.Q("PatientMonitorButton").Q<Button>();
            patientViewButton = Root.Q("PatientViewButton").Q<Button>();
            frontViewButton = Root.Q("FrontViewButton").Q<Button>();
            bronchoscopeButton = Root.Q("BronchoscopeButton").Q<Button>();
            bronchMonitorButton = Root.Q("BronchMonitorButton").Q<Button>();
            procedureTableButton = Root.Q("ProcedureTableButton").Q<Button>();

            OnToolbarShown ??= new UnityEvent();
            OnToolbarHidden ??= new UnityEvent();
            OnPatientMonitorBtnClicked ??= new UnityEvent();
            OnPatientViewBtnClicked ??= new UnityEvent();
            OnFrontViewBtnClicked ??= new UnityEvent();
            OnBronchoscopeBtnClicked ??= new UnityEvent();
            OnBronchMonitorBtnClicked ??= new UnityEvent();

            SetupPOIButtons();

            isInitalized = false;
            Root.style.display = DisplayStyle.None;
        }

        public void SetupPOIButtons()
        {
            patientMonitorButton.clicked += () =>
            {
                POIButtonCallback(POIType.PatientMonitor, OnPatientMonitorBtnClicked);
            };
            patientViewButton.clicked += () =>
            {
                POIButtonCallback(POIType.PatientView, OnPatientViewBtnClicked);
            };
            frontViewButton.clicked += () =>
            {
                POIButtonCallback(POIType.FrontView, OnFrontViewBtnClicked);
            };
            bronchoscopeButton.clicked += () =>
            {
                POIButtonCallback(POIType.BronchoscopeProcedure, OnBronchoscopeBtnClicked);
            };
            bronchMonitorButton.clicked += () =>
            {
                POIButtonCallback(POIType.BronchMonitor, OnBronchMonitorBtnClicked);
            };
            procedureTableButton.clicked += () =>
            {
                POIButtonCallback(POIType.ProcedureTable, OnProcedureTableBtnClicked);
            };
        }

        private void POIButtonCallback(POIType poi, UnityEvent poiAction)
        {
            POI currentPOI = POIManager.Instance.CurrentPOI;
            if (currentPOI != null && currentPOI.POIName == poi || currentPOI == null && poi == POIType.FrontView)
            {
                return;
            }

            poiAction?.Invoke();
            Hide();
        }

        public void ToggleProcedureToolbarStatus(bool toggle)
        {
            isInitalized = toggle;
        }

        /// <summary>
        /// Shows the root of the procedure toolbar and triggers OnToolbarShown
        /// </summary>
        public void Show()
        {
            if (isInitalized)
            {
                Root.Show();
                OnToolbarShown?.Invoke();
            }
        }

        /// <summary>
        /// Hides the root of the procedure toolbar and triggers OnToolbarHidden
        /// </summary>
        public void Hide()
        {
            if (isInitalized)
            {
                Root.Hide();
                OnToolbarHidden?.Invoke();
            }
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
            patientMonitorButton.SetEnabled(enabled);
            patientViewButton.SetEnabled(enabled);
            frontViewButton.SetEnabled(enabled);
            bronchoscopeButton.SetEnabled(enabled);
            bronchMonitorButton.SetEnabled(enabled);
            procedureTableButton.SetEnabled(enabled);
        }
    }
}
