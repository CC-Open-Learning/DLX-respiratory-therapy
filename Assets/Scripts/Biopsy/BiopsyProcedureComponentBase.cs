using UnityEngine;
using UnityEngine.Serialization;

namespace VARLab.RespiratoryTherapy
{
    /// <summary>
    /// Base class for biopsy procedure components, linking them to a specific biopsy procedure item.
    /// </summary>
    public class BiopsyProcedureComponentBase : MonoBehaviour
    {
        [FormerlySerializedAs("procedureItem")] public BiopsyProcedureEquipment procedureEquipment;
        public bool ComponentAccessed;

        protected PhysicianAnimationController PhysicianAnimationController;
        
        public virtual void ClickOnComponent()
        {
            ComponentAccessed = true;
        }
        
        public void ResetComponent()
        {
            ComponentAccessed = false;
        }
    }
}
