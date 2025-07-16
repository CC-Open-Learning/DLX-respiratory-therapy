using System.Collections.Generic;
using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "Scenario", menuName = "RT Scriptable Objects/Biopsy/Scenario")]
    public class ScenarioTasksSO : ScriptableObject
    {
        public string ScenarioName;
        public List<BiopsyBaseTaskSO> ScenarioTaskList = new ();
        public bool ShowInHandbook;
    }
}
