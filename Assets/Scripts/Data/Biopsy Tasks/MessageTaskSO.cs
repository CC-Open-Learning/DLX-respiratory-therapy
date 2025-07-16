using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "Message Task", menuName = "RT Scriptable Objects/Biopsy/Scenario Tasks/Message Task")]
    public class MessageTaskSO : BiopsyBaseTaskSO
    {
        public string Name;
        public ModalSO ModalSO;
        public bool PreventScenarioAdvanceOnButtonClick;
        private Modal modal;
        private BiopsyController biopsyController;
        
        public override void Execute()
        {
            if (modal == null)
            {
                modal = FindFirstObjectByType<Modal>();
            }

            if (biopsyController == null)
            {
                biopsyController = FindFirstObjectByType<BiopsyController>();
            }
            
            modal.ToggleScenarioIndexUpdate(!PreventScenarioAdvanceOnButtonClick);
            modal.HandleDisplayUI(ModalSO);
        }
    }
}
