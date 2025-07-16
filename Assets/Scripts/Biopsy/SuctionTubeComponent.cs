namespace VARLab.RespiratoryTherapy
{
    public class SuctionTubeComponent : BiopsyProcedureComponentBase
    {
        private OutlineHelper outlineHelper;
        
        private void Start()
        {
            PhysicianAnimationController = FindFirstObjectByType<PhysicianAnimationController>();
            outlineHelper = GetComponent<OutlineHelper>();
        }
        
        private void OnEnable()
        {
            if (outlineHelper != null)
            {
                outlineHelper.SetOutlineActive(true);
            }
        }

        /// <summary>
        /// This will method will be called by the Interactable.cs, Mouse Click event.
        /// On the object which contains BronchoscopeComponent in the scene.
        /// </summary>
        public override void ClickOnComponent()
        {
            if (!ComponentAccessed)
            {
                base.ClickOnComponent(); 
                PhysicianAnimationController.SetActionTrigger();
            }
            else
            {
                PhysicianAnimationController.RemoveSuctionTube();
            }
        }
    }
}
