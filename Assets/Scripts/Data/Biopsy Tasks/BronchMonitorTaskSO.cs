using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "Bronch Monitor Task", menuName = "RT Scriptable Objects/Biopsy/Scenario Tasks/Bronch Monitor Task")]
    public class BronchMonitorTaskSO : BiopsyBaseTaskSO
    {
        private BiopsyController biopsyController;
        private BronchMonitorController bronchMonitorController;
        [Header("Select the action to take on showing the last image in the image set")]
        public bool GoToNextTask;
        public bool GoToNextScenario;
        public bool ShowEmptyAirway;
        public bool AutoCycleImages;
        [Header("Select Bronch Monitor Status")]
        public BronchMonitorStatus MonitorStatus;
        [Header("Image Data")]
        public List<BronchMonitorData> BronchMonitorDataList = new ();

        public override void Execute()
        {
            if (bronchMonitorController == null) 
            {
                bronchMonitorController = FindFirstObjectByType<BronchMonitorController>();
            }
            if (biopsyController == null)
            {
                biopsyController = FindFirstObjectByType<BiopsyController>();
            }
            
            bronchMonitorController.SetBronchMonitorData(this);
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

            if (ShowEmptyAirway)
            {
                bronchMonitorController.BronchMonitorMaterial.mainTexture = bronchMonitorController.EmptyAirwayBronchMonitorMaterial.mainTexture;
            }
        }
        
        [Serializable]
        public class BronchMonitorData
        {
            public Sprite Sprite;
            [CanBeNull] public PromptTaskSO PromptTask;
        }
    }
}
