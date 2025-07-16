using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "Item Handler Task", menuName = "RT Scriptable Objects/Biopsy/Scenario Tasks/Item Handler Task")]
    public class ItemHandlerTaskSO : BiopsyBaseTaskSO
    {
        public BiopsyProcedureComponentHandler biopsyProcedureComponentHandler;
        
        [Header("Biopsy Equipment / Tool")]
        public BiopsyProcedureEquipment ProcedureEquipment;
        public bool EnableAllTableItems;
        
        /// <summary>
        /// Executes the task by enabling the specified biopsy procedure item, 
        /// ensuring the component handler is initialized before activation.
        /// </summary>
        public override void Execute()
        {
            if (biopsyProcedureComponentHandler == null) 
            {
                biopsyProcedureComponentHandler = FindFirstObjectByType<BiopsyProcedureComponentHandler>();
            }

            if (EnableAllTableItems)
            {
                biopsyProcedureComponentHandler.EnableAllBiopsyProcedureItems();
            } 
            
            biopsyProcedureComponentHandler.EnableBiopsyProcedureItem(ProcedureEquipment);
        }
    }
}
