using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "POI Lock Task", menuName = "RT Scriptable Objects/Biopsy/Scenario Tasks/POI Lock Task")]
    public class LockPOITaskSO : BiopsyBaseTaskSO
    {
        private ProcedurePOIToolbar toolbarPOI;
        private ToolbarController toolbarController;
        
        public bool LockPOI;
        public override void Execute()
        {
            if (toolbarPOI == null)
            {
                toolbarPOI = FindFirstObjectByType<ProcedurePOIToolbar>();
            }

            if (toolbarController == null)
            {
                toolbarController = FindFirstObjectByType<ToolbarController>();
            }

            if (LockPOI)
            {
                toolbarPOI.DisableToolbarInteraction();
                toolbarController.DisableToolbarInteraction();
            }
            else
            {
                toolbarPOI.EnableToolbarInteraction();
                toolbarController.EnableToolbarInteraction();
            }
        }
    }
}
