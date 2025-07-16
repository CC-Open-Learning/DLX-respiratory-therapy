namespace VARLab.RespiratoryTherapy
{
    public class BiopsyForcepsComponent : BiopsyProcedureComponentBase
    {
        private void Start()
        {
            PhysicianAnimationController = FindFirstObjectByType<PhysicianAnimationController>();
        }
        
        /// <summary>
        /// This will method will be called by the Interactable.cs, Mouse Click event.
        /// On the object which contains BronchoscopeComponent in the scene.
        /// </summary>
        public override void ClickOnComponent()
        {
            if (ComponentAccessed) return;
            
            base.ClickOnComponent();
            PhysicianAnimationController.SetActionTrigger();
        }
    }
}