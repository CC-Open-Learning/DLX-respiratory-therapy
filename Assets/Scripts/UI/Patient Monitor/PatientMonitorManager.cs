using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace VARLab.RespiratoryTherapy
{
    public class PatientMonitorManager : MonoBehaviour
    {
        [SerializeField] private BiopsyController biopsyController;
        [SerializeField] private PatientMonitorControl patientMonitorControl;
        [SerializeField] private PatientMonitorReport patientMonitorReport;

        [Header("Dynamic Patient Monitor UI")]
        [SerializeField] private PromptSO proceedWhenPoorPrompt;
        [FormerlySerializedAs("incorrectReportPrompt")] [SerializeField] private PromptSO incorrectReportBPrompt;
        [SerializeField] private PromptSO incorrectReportAcPrompt;
        [SerializeField] private PromptSO pauseWhenStablePrompt;
        [SerializeField] private ModalSO problemAModal;
        [SerializeField] private ModalSO problemCModal;

        [Header("Events")]
        /// Invokes <see cref="PatientMonitorUI.UpdateHeartRateWaveform(int)"/>
        public UnityEvent<int> OnECGChanged;
        /// Invokes <see cref="PatientMonitorUI.UpdateRESPWaveform(int)"/>
        public UnityEvent<int> OnRESPChanged;
        /// Invokes <see cref="PatientMonitorUI.UpdateSpO2Waveform(int)"/>
        public UnityEvent<int> OnSp02Changed;
        /// Invokes <see cref="PatientMonitorUI.UpdateCO2Waveform(int)"/>
        public UnityEvent<int> OnC02Changed;
        /// Invokes <see cref="PatientMonitorUI.UpdateBloodPressure(int, int, int)"/>
        public UnityEvent<int, int, int> OnNIBPChanged;

        private const int ECGNormalMin = 60;
        private const int ECGNormalMax = 101;
        private const int ECGLowMin = 30;
        private const int ECGLowMax = 58;
        private const int ECGHighMin = 141;
        private const int ECGHighMax = 160;
        private const int RESPNormalMin = 12;
        private const int RESPNormalMax = 19;
        private const int RESPLowMin = 6;
        private const int RESPLowMax = 10;
        private const int RESPHighMin = 25;
        private const int RESPHighMax = 30;
        private const int Sp02NormalMin = 95;
        private const int Sp02NormalMax = 101;
        private const int Sp02LowMin = 86;
        private const int Sp02LowMax = 90;
        private const int C02NormalMin = 35;
        private const int C02NormalMax = 46;
        private const int C02LowMin = 25;
        private const int C02LowMax = 35;
        private const int C02HighMin = 56;
        private const int C02HighMax = 65;
        private const int NIBPNormalMin = 100;
        private const int NIBPNormalMax = 121;
        private const int NIBPLowMin = 80;
        private const int NIBPLowMax = 90;
        private const int NIBPHighMin = 141;
        private const int NIBPHighMax = 180;
        private const int NIBPmmHGNormalMin = 70;
        private const int NIBPmmHGNormalMax = 81;
        private const int NIBPmmHGLowMin = 50;
        private const int NIBPmmHGLowMax = 65;
        private const int NIBPmmHGHighMin = 101;
        private const int NIBPmmHGHighMax = 110;
        //TODO: figure out if pulse rate is needed/pulse rate values
        private const int PulseRate = 93;

        private readonly Dictionary<PatientMonitorVitals, VitalsIssue> ProblemVitalsA = new Dictionary<PatientMonitorVitals, VitalsIssue> { { PatientMonitorVitals.SpO2, VitalsIssue.Low } };
        private readonly Dictionary<PatientMonitorVitals, VitalsIssue> ProblemVitalsB = new Dictionary<PatientMonitorVitals, VitalsIssue> {
            { PatientMonitorVitals.ECG, VitalsIssue.High }, { PatientMonitorVitals.RESP, VitalsIssue.High }, { PatientMonitorVitals.NIBP, VitalsIssue.High } };
        private readonly Dictionary<PatientMonitorVitals, VitalsIssue> ProblemVitalsC = new Dictionary<PatientMonitorVitals, VitalsIssue> {
            { PatientMonitorVitals.RESP, VitalsIssue.Low }, { PatientMonitorVitals.SpO2, VitalsIssue.Low }, { PatientMonitorVitals.CO2, VitalsIssue.High } };
        
        private Prompt prompt;
        private Modal modal;

        private PatientMonitorTaskSO patientMonitorTask;
        private VitalsType vitalsType;

        private Dictionary<PatientMonitorVitals, VitalsIssue> poorVitals;

        private void Start()
        {
            OnECGChanged ??= new UnityEvent<int>();
            OnRESPChanged ??= new UnityEvent<int>();
            OnSp02Changed ??= new UnityEvent<int>();
            OnC02Changed ??= new UnityEvent<int>();
            OnNIBPChanged ??= new UnityEvent<int, int, int>();

            prompt = FindFirstObjectByType<Prompt>();
            modal = FindFirstObjectByType<Modal>();

            patientMonitorTask = null;

            vitalsType = VitalsType.Stable;
            poorVitals = new Dictionary<PatientMonitorVitals, VitalsIssue>();

            UpdateWaveforms();
        }

        /// <summary>
        /// Takes a PatientMonitorTaskSO and sets up all required functionality based on the 
        /// SO's values
        /// </summary>
        public void SetupVitalsCheck(PatientMonitorTaskSO patientMonitorTaskSO)
        {
            if (patientMonitorTaskSO == null) { return; }

            poorVitals.Clear();

            patientMonitorTask = patientMonitorTaskSO;

            switch (patientMonitorTaskSO.VitalsCheckType)
            {
                case VitalsType.Stable:
                    vitalsType = VitalsType.Stable;
                    break;
                case VitalsType.ProblemA:
                    vitalsType = VitalsType.ProblemA;
                    foreach (var problemVital in ProblemVitalsA)
                    {
                        poorVitals.Add(problemVital.Key, problemVital.Value);
                    }
                    break;
                case VitalsType.ProblemB:
                    vitalsType = VitalsType.ProblemB;
                    foreach (var problemVital in ProblemVitalsB)
                    {
                        poorVitals.Add(problemVital.Key, problemVital.Value);
                    }
                    break;
                case VitalsType.ProblemC:
                    vitalsType = VitalsType.ProblemC;
                    foreach (var problemVital in ProblemVitalsC)
                    {
                        poorVitals.Add(problemVital.Key, problemVital.Value);
                    }
                    break;
                case VitalsType.ProblemAC:
                    vitalsType = Random.Range(0, 2) == 0 ? VitalsType.ProblemA : VitalsType.ProblemC;
                    Dictionary<PatientMonitorVitals, VitalsIssue> problem = vitalsType == VitalsType.ProblemA ? ProblemVitalsA : ProblemVitalsC;
                    foreach (var problemVital in problem)
                    {
                        poorVitals.Add(problemVital.Key, problemVital.Value);
                    }
                    break;
                case VitalsType.ProcedureEndStable:
                    vitalsType = VitalsType.ProcedureEndStable;
                    break;
            }

            UpdateWaveforms();
            patientMonitorControl.Show();
        }

        public void OnVitalsCheckProceed()
        {
            if (vitalsType == VitalsType.Stable || vitalsType == VitalsType.ProcedureEndStable)
            {
                patientMonitorControl.Hide();
                patientMonitorTask.ExecuteAction();
            }
            else
            {
                ChangePrompt(proceedWhenPoorPrompt);
            }
        }
        
        public void OnVitalsCheckReport(List<PatientMonitorVitals> reportedVitals)
        {
            if (vitalsType == VitalsType.Stable)
            {
                patientMonitorReport.Hide();
                patientMonitorTask.ExecuteAction();
                ChangePrompt(pauseWhenStablePrompt);
            }
            else if (vitalsType == VitalsType.ProcedureEndStable)
            {
                patientMonitorReport.Hide();
                patientMonitorTask.ExecuteAction();
            }
            else
            {
                if (poorVitals.Count == reportedVitals.Count && !poorVitals.Keys.Except(reportedVitals).Any())
                {
                    patientMonitorReport.Hide();
                    patientMonitorTask.ExecuteAction();
                    if (vitalsType == VitalsType.ProblemA)
                    {
                        ChangeModal(problemAModal);
                    }
                    else if (vitalsType == VitalsType.ProblemC)
                    {
                        ChangeModal(problemCModal);
                    }
                }
                else if (vitalsType == VitalsType.ProblemB)
                {
                    ChangePrompt(incorrectReportBPrompt);
                }
                else
                {
                    if (poorVitals.Count == 1)
                    {
                        ChangePrompt(incorrectReportAcPrompt);   
                    }
                    else
                    {
                        ChangePrompt(incorrectReportBPrompt);   
                    }
                }
            }
        }

        /// <summary>
        /// Updates the waveforms values based on the contents of the poorVitals List<>
        /// </summary>
        private void UpdateWaveforms()
        {
            //Update poor vitals to poor values
            foreach (var vital in poorVitals)
            {
                switch (vital.Key)
                {
                    case PatientMonitorVitals.ECG:
                        if (vital.Value == VitalsIssue.Low)
                        {
                            OnECGChanged?.Invoke(Random.Range(ECGLowMin, ECGLowMax));
                        }
                        else
                        {
                            OnECGChanged?.Invoke(Random.Range(ECGHighMin, ECGHighMax));
                        }
                        break;
                    case PatientMonitorVitals.RESP:
                        if (vital.Value == VitalsIssue.Low)
                        {
                            OnRESPChanged?.Invoke(Random.Range(RESPLowMin, RESPLowMax));
                        }
                        else
                        {
                            OnRESPChanged?.Invoke(Random.Range(RESPHighMin, RESPHighMax));
                        }
                        break;
                    case PatientMonitorVitals.SpO2:
                        OnSp02Changed?.Invoke(Random.Range(Sp02LowMin, Sp02LowMax));
                        break;
                    case PatientMonitorVitals.CO2:
                        if (vital.Value == VitalsIssue.Low)
                        {
                            OnC02Changed?.Invoke(Random.Range(C02LowMin, C02LowMax));
                        }
                        else
                        {
                            OnC02Changed?.Invoke(Random.Range(C02HighMin, C02HighMax));
                        }
                        break;
                    case PatientMonitorVitals.NIBP:
                        if (vital.Value == VitalsIssue.Low)
                        {
                            OnNIBPChanged?.Invoke(Random.Range(NIBPLowMin, NIBPLowMax),
                                Random.Range(NIBPmmHGLowMin, NIBPmmHGLowMax), PulseRate);
                        }
                        else
                        {
                            OnNIBPChanged?.Invoke(Random.Range(NIBPHighMin, NIBPHighMax),
                                Random.Range(NIBPmmHGHighMin, NIBPmmHGHighMax), PulseRate);
                        }
                        break;
                }
            }
            //Update normal vitals to normal values
            foreach (PatientMonitorVitals vital in Enum.GetValues(typeof(PatientMonitorVitals)).Cast<PatientMonitorVitals>().ToList().Except(poorVitals.Keys))
            {
                switch (vital)
                {
                    case PatientMonitorVitals.ECG:
                        OnECGChanged?.Invoke(Random.Range(ECGNormalMin, ECGNormalMax));
                        break;
                    case PatientMonitorVitals.RESP:
                        OnRESPChanged?.Invoke(Random.Range(RESPNormalMin, RESPNormalMax));
                        break;
                    case PatientMonitorVitals.SpO2:
                        OnSp02Changed?.Invoke(Random.Range(Sp02NormalMin, Sp02NormalMax));
                        break;
                    case PatientMonitorVitals.CO2:
                        OnC02Changed?.Invoke(Random.Range(C02NormalMin, C02NormalMax));
                        break;
                    case PatientMonitorVitals.NIBP:
                        OnNIBPChanged?.Invoke(Random.Range(NIBPNormalMin, NIBPNormalMax),
                            Random.Range(NIBPmmHGNormalMin, NIBPmmHGNormalMax), PulseRate);
                        break;
                }
            }
        }

        /// <summary>
        /// Swap the current prompt for a new one
        /// </summary>
        private void ChangePrompt(PromptSO promptSO)
        {
            prompt.HandleDisplayUI(promptSO);
        }

        /// <summary>
        /// Swap the current prompt for a new one
        /// </summary>
        private void ChangeModal(ModalSO modalSO)
        {
            modal.HandleDisplayUI(modalSO);
        }
    }
}