namespace VARLab.RespiratoryTherapy
{
    public class SputumContainerComponent : BiopsyProcedureComponentBase
    {
        private void Start()
        {
            PhysicianAnimationController = FindFirstObjectByType<PhysicianAnimationController>();
        }
        
        public override void ClickOnComponent()
        {
            if (ComponentAccessed) return;
            
            base.ClickOnComponent();
            PhysicianAnimationController.SetActionTrigger();
        }
    }
}
