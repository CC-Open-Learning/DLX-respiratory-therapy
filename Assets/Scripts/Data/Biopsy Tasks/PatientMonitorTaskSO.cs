using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "Patient Monitor Task", menuName = "RT Scriptable Objects/Biopsy/Scenario Tasks/Patient Monitor Task")]
    public class PatientMonitorTaskSO : BiopsyBaseTaskSO
    {
        private BiopsyController biopsyController;
        private PatientMonitorManager patientMonitorManager;
        [Header("Vitals Check Data")]
        public VitalsType VitalsCheckType;
        [TextArea(1, 3), Tooltip("The message to be displayed at the end of the vitals check")]
        public string ResolutionMessage;
        [Header("Select the action to take after completing the vitals check")]
        public bool GoToNextTask;
        public bool GoToNextScenario;

        public override void Execute()
        {
            if (patientMonitorManager == null) 
            {
                patientMonitorManager = FindFirstObjectByType<PatientMonitorManager>();
            }
            if (biopsyController == null)
            {
                biopsyController = FindFirstObjectByType<BiopsyController>();
            }

            patientMonitorManager.SetupVitalsCheck(this);
        }
        
        public void ExecuteAction()
        {
            if (GoToNextTask)
            {
                biopsyController.UpdateScenarioTaskIndex();
            }

            if (GoToNextScenario)
            {
                biopsyController.UpdateScenarioIndex();
            }
        }
    }
}
