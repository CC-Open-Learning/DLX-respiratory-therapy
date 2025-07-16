using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    /// <summary>
    /// Abstract base class for biopsy procedure tasks, defining the execution behavior.
    /// </summary>
    public abstract class BiopsyBaseTaskSO : ScriptableObject
    {
        [Header("Select the action to auto-increment")]
        public bool TaskIndexIncrement;
        public bool ScenarioIndexIncrement;
        [Tooltip("Hide any Notifications or prompt ui elements that were previously displayed")]
        public bool HideSpeakerUI;
        public bool IsExecuted;
        public string Description;
        
        public abstract void Execute();
    }
}
