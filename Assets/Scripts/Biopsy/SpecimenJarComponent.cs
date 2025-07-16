namespace VARLab.RespiratoryTherapy
{
    /// <summary>
    /// This component manages the sputum specimen container (and biopsy forceps) that appears in the doctor's hand.
    /// </summary>
    public class SpecimenJarComponent : BiopsyProcedureComponentBase
    {
        private void Start()
        {
            PhysicianAnimationController = FindFirstObjectByType<PhysicianAnimationController>();
        }
        
        /// <summary>
        /// Triggered by the Interactable scripts 'Mouse Click' event.
        /// It requests the animation associated with the forceps and specimen container to be played. 
        /// </summary>
        public override void ClickOnComponent()
        {
            if (ComponentAccessed) return;
            
            base.ClickOnComponent();
            PhysicianAnimationController.SetActionTrigger();
        }
    }
}
