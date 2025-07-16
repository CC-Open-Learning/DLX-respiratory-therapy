using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "Item Request Task", menuName = "RT Scriptable Objects/Biopsy/Scenario Tasks/Item Request Task")]
    public class ItemRequestTaskSO : BiopsyBaseTaskSO
    {
        private TableController tableController;
        private BiopsyController biopsyController;
        
        public BiopsyProcedureEquipment requestedItem;
        [Header("Select the action to take after finding the requested item")]
        public bool GoToNextTask;
        public bool GoToNextScenario;
        public PromptSO RequestPrompt;
        
        public override void Execute()
        {
            if (tableController == null)
            {
                tableController = FindFirstObjectByType<TableController>();
            }

            if (biopsyController == null)
            {
                biopsyController = FindFirstObjectByType<BiopsyController>();
            }

            tableController.SetRequestedItem(requestedItem, this);
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
