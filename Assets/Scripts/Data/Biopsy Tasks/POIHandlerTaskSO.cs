using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "POI Handler Task", menuName = "RT Scriptable Objects/Biopsy/Scenario Tasks/POI Handler Task")]
    public class POIHandlerTaskSO : BiopsyBaseTaskSO
    {
        private POIManager poiManager;

        [Header("Biopsy Procedure POI")]
        [SerializeField] private POIType procedurePOI;
        
        /// <summary>
        /// Executes the task by informing the POIManager of the target POI
        /// </summary>
        public override void Execute()
        {
            if (poiManager == null) 
            {
                poiManager = POIManager.Instance;
            }

            poiManager.HandleExecutePOITask(procedurePOI);
        }
    }
}
