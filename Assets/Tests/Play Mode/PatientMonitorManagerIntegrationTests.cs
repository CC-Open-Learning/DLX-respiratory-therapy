using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class PatientMonitorManagerIntegrationTests : MonoBehaviour
    {
        //Path to scene
        private readonly string scenePath = "Assets/Scenes/PatientMonitorTestScene.unity";

        private TextMeshProUGUI ecg;
        private TextMeshProUGUI resp;
        private TextMeshProUGUI spo2;
        private TextMeshProUGUI co2;
        private TextMeshProUGUI nibpPressure;
        private TextMeshProUGUI nibpPulse;

        private Prompt prompt;
        private Label promptMessage;
        private PromptSO basePatientMonitorPromptSO;

        private Modal modal;
        private Label modalMessage;
        private ModalSO basePatientMonitorModalSO;

        private VisualElement patientMonitorControl;
        private VisualElement patientMonitorReport;

        private BiopsyController biopsyController;

        private PatientMonitorManager patientMonitorManager;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            //Wait for scene to load
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));

            //Assign everything
            ecg = GameObject.Find("TMP ECG").GetComponent<TextMeshProUGUI>();
            resp = GameObject.Find("TMP RESP").GetComponent<TextMeshProUGUI>();
            spo2 = GameObject.Find("TMP Sp02").GetComponent<TextMeshProUGUI>();
            co2 = GameObject.Find("TMP C02").GetComponent<TextMeshProUGUI>();
            nibpPressure = GameObject.Find("TMP NIBP Pressure").GetComponent<TextMeshProUGUI>();
            nibpPulse = GameObject.Find("TMP NIBP Pulse").GetComponent<TextMeshProUGUI>();

            prompt = FindFirstObjectByType<Prompt>();
            promptMessage = prompt.GetComponent<UIDocument>().rootVisualElement.Q<Label>("MessageLabel");
            basePatientMonitorPromptSO = AssetDatabase.LoadAssetAtPath<PromptSO>("Assets/Scriptable Objects/Biopsy/Message Data/Patient Monitor/Vitals Check Prompt SO.asset");

            modal = FindFirstObjectByType<Modal>();
            modalMessage = modal.GetComponent<UIDocument>().rootVisualElement.Q<Label>("DescriptionLabel");
            basePatientMonitorModalSO = AssetDatabase.LoadAssetAtPath<ModalSO>("Assets/Scriptable Objects/Biopsy/Message Data/Patient Monitor/Vitals Problem B Modal SO.asset");

            patientMonitorControl = FindFirstObjectByType<PatientMonitorControl>().GetComponent<UIDocument>().rootVisualElement;
            patientMonitorReport = FindFirstObjectByType<PatientMonitorReport>().GetComponent<UIDocument>().rootVisualElement;

            biopsyController = FindFirstObjectByType<BiopsyController>();

            patientMonitorManager = FindFirstObjectByType<PatientMonitorManager>();

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ecg = null;
            resp = null;
            spo2 = null;
            co2 = null;
            nibpPressure = null;
            nibpPulse = null;

            prompt = null;
            promptMessage = null;

            modal = null;
            modalMessage = null;

            patientMonitorControl = null;
            patientMonitorReport = null;

            biopsyController = null;

            patientMonitorManager = null;

            UIManager.Instance = null;

            yield return null;
        }

        [UnityTest]
        public IEnumerator SetupVitalsCheck_ShouldShowPatientMonitorControl()
        {
            //Arrange
            PatientMonitorTaskSO vitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            vitalsCheckTask.VitalsCheckType = VitalsType.Stable;
            patientMonitorControl.style.display = DisplayStyle.None;

            //Act
            patientMonitorManager.SetupVitalsCheck(vitalsCheckTask);
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(patientMonitorControl.style.display == DisplayStyle.Flex);
        }

        [UnityTest]
        public IEnumerator SetupVitalsCheck_Stable_ShouldSetStableWaveforms()
        {
            //Act
            PatientMonitorTaskSO stableVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            stableVitalsCheckTask.VitalsCheckType = VitalsType.Stable;
            patientMonitorManager.SetupVitalsCheck(stableVitalsCheckTask);
            yield return null; //Wait for one frame so UI can update

            //Assert
            bool ecgCheck = CheckStableWaveformRange(PatientMonitorVitals.ECG, int.Parse(ecg.text));
            bool respCheck = CheckStableWaveformRange(PatientMonitorVitals.RESP, int.Parse(resp.text));
            bool spo2Check = CheckStableWaveformRange(PatientMonitorVitals.SpO2, int.Parse(spo2.text));
            bool co2Check = CheckStableWaveformRange(PatientMonitorVitals.CO2, int.Parse(co2.text));
            string[] nibpPressureValues = nibpPressure.text.Split('/');
            bool nibpCheck = CheckStableWaveformRange(PatientMonitorVitals.NIBP, int.Parse(nibpPressureValues[0]),
                int.Parse(nibpPressureValues[1]), int.Parse(nibpPulse.text.Substring(1, 2)));
            Assert.IsTrue(ecgCheck && respCheck && spo2Check && co2Check && nibpCheck);
        }


        [UnityTest]
        public IEnumerator SetupVitalsCheck_ProblemA_ShouldSetSpecificPoorWaveforms()
        {
            //Act
            PatientMonitorTaskSO problemAVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            problemAVitalsCheckTask.VitalsCheckType = VitalsType.ProblemA;
            patientMonitorManager.SetupVitalsCheck(problemAVitalsCheckTask);
            yield return null; //Wait for one frame so UI can update

            //Assert
            bool ecgCheck = CheckStableWaveformRange(PatientMonitorVitals.ECG, int.Parse(ecg.text));
            bool respCheck = CheckStableWaveformRange(PatientMonitorVitals.RESP, int.Parse(resp.text));
            bool spo2Check = CheckPoorWaveformRange(PatientMonitorVitals.SpO2, int.Parse(spo2.text));
            bool co2Check = CheckStableWaveformRange(PatientMonitorVitals.CO2, int.Parse(co2.text));
            string[] nibpPressureValues = nibpPressure.text.Split('/');
            bool nibpCheck = CheckStableWaveformRange(PatientMonitorVitals.NIBP, int.Parse(nibpPressureValues[0]),
                int.Parse(nibpPressureValues[1]), int.Parse(nibpPulse.text.Substring(1, 2)));
            Assert.IsTrue(ecgCheck && respCheck && spo2Check && co2Check && nibpCheck);
        }

        [UnityTest]
        public IEnumerator SetupVitalsCheck_ProblemB_ShouldSetSpecificPoorWaveforms()
        {
            //Act
            PatientMonitorTaskSO problemBVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            problemBVitalsCheckTask.VitalsCheckType = VitalsType.ProblemB;
            patientMonitorManager.SetupVitalsCheck(problemBVitalsCheckTask);
            yield return null; //Wait for one frame so UI can update

            //Assert
            bool ecgCheck = CheckPoorWaveformRange(PatientMonitorVitals.ECG, int.Parse(ecg.text));
            bool respCheck = CheckPoorWaveformRange(PatientMonitorVitals.RESP, int.Parse(resp.text));
            bool spo2Check = CheckStableWaveformRange(PatientMonitorVitals.SpO2, int.Parse(spo2.text));
            bool co2Check = CheckStableWaveformRange(PatientMonitorVitals.CO2, int.Parse(co2.text));
            string[] nibpPressureValues = nibpPressure.text.Split('/');
            bool nibpCheck = CheckPoorWaveformRange(PatientMonitorVitals.NIBP, int.Parse(nibpPressureValues[0]),
                int.Parse(nibpPressureValues[1]), int.Parse(nibpPulse.text.Substring(1, 2)));
            Assert.IsTrue(ecgCheck && respCheck && spo2Check && co2Check && nibpCheck);
        }

        [UnityTest]
        public IEnumerator SetupVitalsCheck_ProblemC_ShouldSetSpecificPoorWaveforms()
        {
            //Act
            PatientMonitorTaskSO problemCVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            problemCVitalsCheckTask.VitalsCheckType = VitalsType.ProblemC;
            patientMonitorManager.SetupVitalsCheck(problemCVitalsCheckTask);
            yield return null; //Wait for one frame so UI can update

            //Assert
            bool ecgCheck = CheckStableWaveformRange(PatientMonitorVitals.ECG, int.Parse(ecg.text));
            bool respCheck = CheckPoorWaveformRange(PatientMonitorVitals.RESP, int.Parse(resp.text));
            bool spo2Check = CheckPoorWaveformRange(PatientMonitorVitals.SpO2, int.Parse(spo2.text));
            bool co2Check = CheckPoorWaveformRange(PatientMonitorVitals.CO2, int.Parse(co2.text));
            string[] nibpPressureValues = nibpPressure.text.Split('/');
            bool nibpCheck = CheckStableWaveformRange(PatientMonitorVitals.NIBP, int.Parse(nibpPressureValues[0]),
                int.Parse(nibpPressureValues[1]), int.Parse(nibpPulse.text.Substring(1, 2)));
            Assert.IsTrue(ecgCheck && respCheck && spo2Check && co2Check && nibpCheck);
        }

        [UnityTest]
        public IEnumerator SetupVitalsCheck_ProblemAC_ShouldSetRandomPoorWaveforms()
        {
            //Act
            PatientMonitorTaskSO problemACVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            problemACVitalsCheckTask.VitalsCheckType = VitalsType.ProblemAC;
            patientMonitorManager.SetupVitalsCheck(problemACVitalsCheckTask);
            yield return null; //Wait for one frame so UI can update

            //Assert
            bool ecgCheck = CheckStableWaveformRange(PatientMonitorVitals.ECG, int.Parse(ecg.text));
            bool respCheck = CheckStableWaveformRange(PatientMonitorVitals.RESP, int.Parse(resp.text));
            bool spo2Check = CheckPoorWaveformRange(PatientMonitorVitals.SpO2, int.Parse(spo2.text));
            bool co2Check = CheckStableWaveformRange(PatientMonitorVitals.CO2, int.Parse(co2.text));
            string[] nibpPressureValues = nibpPressure.text.Split('/');
            bool nibpCheck = CheckStableWaveformRange(PatientMonitorVitals.NIBP, int.Parse(nibpPressureValues[0]),
                int.Parse(nibpPressureValues[1]), int.Parse(nibpPulse.text.Substring(1, 2)));
            bool problemA = ecgCheck && respCheck && spo2Check && co2Check && nibpCheck;

            ecgCheck = CheckStableWaveformRange(PatientMonitorVitals.ECG, int.Parse(ecg.text));
            respCheck = CheckPoorWaveformRange(PatientMonitorVitals.RESP, int.Parse(resp.text));
            spo2Check = CheckPoorWaveformRange(PatientMonitorVitals.SpO2, int.Parse(spo2.text));
            co2Check = CheckPoorWaveformRange(PatientMonitorVitals.CO2, int.Parse(co2.text));
            nibpPressureValues = nibpPressure.text.Split('/');
            nibpCheck = CheckStableWaveformRange(PatientMonitorVitals.NIBP, int.Parse(nibpPressureValues[0]),
                int.Parse(nibpPressureValues[1]), int.Parse(nibpPulse.text.Substring(1, 2)));
            bool problemC = ecgCheck && respCheck && spo2Check && co2Check && nibpCheck;

            Assert.IsTrue(problemA || problemC);
        }

        [UnityTest]
        public IEnumerator SetupVitalsCheck_ProcedureEndStable_ShouldSetStableWaveforms()
        {
            //Act
            PatientMonitorTaskSO procedureEndStableVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            procedureEndStableVitalsCheckTask.VitalsCheckType = VitalsType.ProcedureEndStable;
            patientMonitorManager.SetupVitalsCheck(procedureEndStableVitalsCheckTask);
            yield return null; //Wait for one frame so UI can update

            //Assert
            bool ecgCheck = CheckStableWaveformRange(PatientMonitorVitals.ECG, int.Parse(ecg.text));
            bool respCheck = CheckStableWaveformRange(PatientMonitorVitals.RESP, int.Parse(resp.text));
            bool spo2Check = CheckStableWaveformRange(PatientMonitorVitals.SpO2, int.Parse(spo2.text));
            bool co2Check = CheckStableWaveformRange(PatientMonitorVitals.CO2, int.Parse(co2.text));
            string[] nibpPressureValues = nibpPressure.text.Split('/');
            bool nibpCheck = CheckStableWaveformRange(PatientMonitorVitals.NIBP, int.Parse(nibpPressureValues[0]),
                int.Parse(nibpPressureValues[1]), int.Parse(nibpPulse.text.Substring(1, 2)));
            Assert.IsTrue(ecgCheck && respCheck && spo2Check && co2Check && nibpCheck);
        }

        [UnityTest]
        public IEnumerator OnVitalsCheckProceed_Stable_ShouldHidePatientMonitorControl()
        {
            //Arrange
            PatientMonitorTaskSO stableVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            stableVitalsCheckTask.VitalsCheckType = VitalsType.Stable;
            patientMonitorManager.SetupVitalsCheck(stableVitalsCheckTask);
            patientMonitorControl.style.display = DisplayStyle.Flex;

            //Act
            patientMonitorManager.OnVitalsCheckProceed();
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(patientMonitorControl.style.display == DisplayStyle.None);
        }

        [UnityTest]
        public IEnumerator OnVitalsCheckProceed_Poor_ShouldChangePrompt()
        {
            //Arrange
            PatientMonitorTaskSO poorVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            poorVitalsCheckTask.VitalsCheckType = VitalsType.ProblemB;
            const string ExpectedMessage = "I'm concerned about these vitals. Please check again.";
            patientMonitorManager.SetupVitalsCheck(poorVitalsCheckTask);
            prompt.HandleDisplayUI(basePatientMonitorPromptSO);

            //Act
            patientMonitorManager.OnVitalsCheckProceed();
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(promptMessage.text == ExpectedMessage);
        }

        [UnityTest]
        public IEnumerator OnVitalsCheckReport_Stable_ShouldHidePatientMonitorReport()
        {
            //Arrange
            PatientMonitorTaskSO stableVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            stableVitalsCheckTask.VitalsCheckType = VitalsType.Stable;
            patientMonitorManager.SetupVitalsCheck(stableVitalsCheckTask);
            patientMonitorReport.style.display = DisplayStyle.Flex;

            //Act
            patientMonitorManager.OnVitalsCheckReport(new List<PatientMonitorVitals>());
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(patientMonitorReport.style.display == DisplayStyle.None);
        }

        [UnityTest]
        public IEnumerator OnVitalsCheckReport_Stable_ShouldChangePrompt()
        {
            //Arrange
            PatientMonitorTaskSO stableVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            stableVitalsCheckTask.VitalsCheckType = VitalsType.Stable;
            const string ExpectedMessage = "That's not correct. These vitals are stable. We can continue.";
            patientMonitorManager.SetupVitalsCheck(stableVitalsCheckTask);
            prompt.HandleDisplayUI(basePatientMonitorPromptSO);

            //Act
            patientMonitorManager.OnVitalsCheckReport(new List<PatientMonitorVitals>());
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(promptMessage.text == ExpectedMessage);
        }

        [UnityTest]
        public IEnumerator OnVitalsCheckReport_ProcedureEndStable_ShouldHidePatientMonitorReport()
        {
            //Arrange
            PatientMonitorTaskSO procedureEndStableVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            procedureEndStableVitalsCheckTask.VitalsCheckType = VitalsType.ProcedureEndStable;
            patientMonitorManager.SetupVitalsCheck(procedureEndStableVitalsCheckTask);
            patientMonitorReport.style.display = DisplayStyle.Flex;

            //Act
            patientMonitorManager.OnVitalsCheckReport(new List<PatientMonitorVitals>());
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(patientMonitorReport.style.display == DisplayStyle.None);
        }

        [UnityTest]
        public IEnumerator OnVitalsCheckReport_ProblemA_Incorrect_ShouldChangePrompt()
        {
            //Arrange
            PatientMonitorTaskSO problemAVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            problemAVitalsCheckTask.VitalsCheckType = VitalsType.ProblemA;
            const string ExpectedMessage = "Check again. There is one concern that needs to be reported to the physician.";
            patientMonitorManager.SetupVitalsCheck(problemAVitalsCheckTask);
            prompt.HandleDisplayUI(basePatientMonitorPromptSO);

            //Act
            patientMonitorManager.OnVitalsCheckReport(new List<PatientMonitorVitals>());
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(promptMessage.text == ExpectedMessage);
        }

        [UnityTest]
        public IEnumerator OnVitalsCheckReport_ProblemB_Incorrect_ShouldChangePrompt()
        {
            //Arrange
            PatientMonitorTaskSO problemBVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            problemBVitalsCheckTask.VitalsCheckType = VitalsType.ProblemB;
            const string ExpectedMessage = "Check again. There are three concerns that need to be reported to the physician.";
            patientMonitorManager.SetupVitalsCheck(problemBVitalsCheckTask);
            prompt.HandleDisplayUI(basePatientMonitorPromptSO);

            //Act
            patientMonitorManager.OnVitalsCheckReport(new List<PatientMonitorVitals>());
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(promptMessage.text == ExpectedMessage);
        }

        [UnityTest]
        public IEnumerator OnVitalsCheckReport_ProblemC_Incorrect_ShouldChangePrompt()
        {
            //Arrange
            PatientMonitorTaskSO problemCVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            problemCVitalsCheckTask.VitalsCheckType = VitalsType.ProblemC;
            const string ExpectedMessage = "Check again. There are three concerns that need to be reported to the physician.";
            patientMonitorManager.SetupVitalsCheck(problemCVitalsCheckTask);
            prompt.HandleDisplayUI(basePatientMonitorPromptSO);

            //Act
            patientMonitorManager.OnVitalsCheckReport(new List<PatientMonitorVitals>());
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(promptMessage.text == ExpectedMessage);
        }

        [UnityTest]
        public IEnumerator OnVitalsCheckReport_Poor_Correct_ShouldHidePatientMonitorReport()
        {
            //Arrange
            PatientMonitorTaskSO problemAVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            problemAVitalsCheckTask.VitalsCheckType = VitalsType.ProblemA;
            patientMonitorManager.SetupVitalsCheck(problemAVitalsCheckTask);
            patientMonitorReport.style.display = DisplayStyle.Flex;

            //Act
            patientMonitorManager.OnVitalsCheckReport(new List<PatientMonitorVitals> { PatientMonitorVitals.SpO2 });
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(patientMonitorReport.style.display == DisplayStyle.None);
        }

        [UnityTest]
        public IEnumerator OnVitalsCheckReport_ProblemA_Correct_ShouldChangeModal()
        {
            //Arrange
            PatientMonitorTaskSO problemAVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            problemAVitalsCheckTask.VitalsCheckType = VitalsType.ProblemA;
            const string ExpectedMessage = "Prolonged procedure has resulted in mild-moderate hypoxemia.\n\nYou pause the procedure, and " +
                "observe respirations while oxygen is still applied. Over 2 minutes, SpO2 returns to 99% and other vital signs remain " +
                "within defined limits.";
            patientMonitorManager.SetupVitalsCheck(problemAVitalsCheckTask);
            modal.HandleDisplayUI(basePatientMonitorModalSO);

            //Act
            patientMonitorManager.OnVitalsCheckReport(new List<PatientMonitorVitals> { PatientMonitorVitals.SpO2 });
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(modalMessage.text == ExpectedMessage);
        }

        [UnityTest]
        public IEnumerator OnVitalsCheckReport_ProblemC_Correct_ShouldChangeModal()
        {
            //Arrange
            PatientMonitorTaskSO problemCVitalsCheckTask = ScriptableObject.CreateInstance<PatientMonitorTaskSO>();
            problemCVitalsCheckTask.VitalsCheckType = VitalsType.ProblemC;
            const string ExpectedMessage = "Over-sedation resulted in suppressed drive to breathe, slight airway obstruction and soft blood " +
                "pressure.\n\nYou pause the procedure, apply a gentle jaw lift, and observe the patient while oxygen is still applied. " +
                "You notice respirations improve. After 2 minutes you notice airway tone improves, and you discontinue your jaw lift. " +
                "After 4 minutes, vital signs have returned within defined limits.";
            patientMonitorManager.SetupVitalsCheck(problemCVitalsCheckTask);
            modal.HandleDisplayUI(basePatientMonitorModalSO);

            //Act
            patientMonitorManager.OnVitalsCheckReport(new List<PatientMonitorVitals> { PatientMonitorVitals.SpO2, PatientMonitorVitals.RESP, PatientMonitorVitals.CO2 });
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(modalMessage.text == ExpectedMessage);
        }

        /// <summary>
        /// Used to check if a given waveform value is within the stable range
        /// </summary>
        private bool CheckStableWaveformRange(PatientMonitorVitals waveform, int value, int nibpExtra1 = 0, int nibpExtra2 = 0)
        {
            const int ECGNormalMin = 60;
            const int ECGNormalMax = 101;
            const int RESPNormalMin = 12;
            const int RESPNormalMax = 19;
            const int Sp02NormalMin = 95;
            const int Sp02NormalMax = 101;
            const int C02NormalMin = 35;
            const int C02NormalMax = 46;
            const int NIBPNormalMin = 100;
            const int NIBPNormalMax = 121;
            const int NIBPmmHGNormalMin = 70;
            const int NIBPmmHGNormalMax = 81;
            //TODO: figure out if pulse rate is needed/pulse rate values
            const int PulseRate = 93;

            switch (waveform)
            {
                case PatientMonitorVitals.ECG:
                    return value >= ECGNormalMin && value < ECGNormalMax;
                case PatientMonitorVitals.RESP:
                    return value >= RESPNormalMin && value < RESPNormalMax;
                case PatientMonitorVitals.SpO2:
                    return value >= Sp02NormalMin && value < Sp02NormalMax;
                case PatientMonitorVitals.CO2:
                    return value >= C02NormalMin && value < C02NormalMax;
                case PatientMonitorVitals.NIBP:
                    bool pressure1 = value >= NIBPNormalMin && value < NIBPNormalMax;
                    bool pressure2 = nibpExtra1 >= NIBPmmHGNormalMin && nibpExtra1 < NIBPmmHGNormalMax;
                    bool pulse = nibpExtra2 == PulseRate;
                    return pressure1 && pressure2 && pulse;
            }

            return false;
        }

        /// <summary>
        /// Used to check if a given waveform value is within the poor range
        /// </summary>
        private bool CheckPoorWaveformRange(PatientMonitorVitals waveform, int value, int nibpExtra1 = 0, int nibpExtra2 = 0)
        {
            const int ECGLowMin = 30;
            const int ECGLowMax = 58;
            const int ECGHighMin = 141;
            const int ECGHighMax = 160;
            const int RESPLowMin = 6;
            const int RESPLowMax = 10;
            const int RESPHighMin = 25;
            const int RESPHighMax = 30;
            const int Sp02LowMin = 86;
            const int Sp02LowMax = 90;
            const int C02LowMin = 25;
            const int C02LowMax = 35;
            const int C02HighMin = 56;
            const int C02HighMax = 65;
            const int NIBPLowMin = 80;
            const int NIBPLowMax = 90;
            const int NIBPHighMin = 141;
            const int NIBPHighMax = 180;
            const int NIBPmmHGLowMin = 50;
            const int NIBPmmHGLowMax = 65;
            const int NIBPmmHGHighMin = 101;
            const int NIBPmmHGHighMax = 110;
            //TODO: figure out if pulse rate is needed/pulse rate values
            const int PulseRate = 93;

            bool isLow = false;
            bool isHigh = false;

            switch (waveform)
            {
                case PatientMonitorVitals.ECG:
                    isLow = value >= ECGLowMin && value < ECGLowMax;
                    isHigh = value >= ECGHighMin && value < ECGHighMax;
                    return isLow || isHigh;
                case PatientMonitorVitals.RESP:
                    isLow = value >= RESPLowMin && value < RESPLowMax;
                    isHigh = value >= RESPHighMin && value < RESPHighMax;
                    return isLow || isHigh;
                case PatientMonitorVitals.SpO2:
                    return value >= Sp02LowMin && value < Sp02LowMax;
                case PatientMonitorVitals.CO2:
                    isLow = value >= C02LowMin && value < C02LowMax;
                    isHigh = value >= C02HighMin && value < C02HighMax;
                    return isLow || isHigh;
                case PatientMonitorVitals.NIBP:
                    if (isLow)
                    {
                        bool pressure1 = value >= NIBPLowMin && value < NIBPLowMax;
                        bool pressure2 = nibpExtra1 >= NIBPmmHGLowMin && nibpExtra1 < NIBPmmHGLowMax;
                        bool pulse = nibpExtra2 == PulseRate;
                        return pressure1 && pressure2 && pulse;
                    }
                    else
                    {
                        bool pressure1 = value >= NIBPHighMin && value < NIBPHighMax;
                        bool pressure2 = nibpExtra1 >= NIBPmmHGHighMin && nibpExtra1 < NIBPmmHGHighMax;
                        bool pulse = nibpExtra2 == PulseRate;
                        return pressure1 && pressure2 && pulse;
                    }
            }

            return false;
        }
    }
}