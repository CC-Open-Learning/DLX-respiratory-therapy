using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "Patient Status", menuName = "RT Scriptable Objects/Biopsy/Scenario Tasks/Patient Status")]
    public class PatientStatusTaskSO : BiopsyBaseTaskSO
    {
        public string Name;
        [SerializeField] private PatientStatusSO patientStatusSO;
        private PatientStatusController patientStatusController;

        public override void Execute()
        {
            if (patientStatusController == null)
            {
                patientStatusController = FindFirstObjectByType<PatientStatusController>();
            }
            
            patientStatusController.HandleSetStatus(patientStatusSO);
        }
    }
}
