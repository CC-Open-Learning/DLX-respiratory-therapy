namespace VARLab.RespiratoryTherapy
{
    public class CC20SalineComponent : BiopsyProcedureComponentBase
    {
        private OutlineHelper outlineHelper;
        
        private void Start()
        {
            outlineHelper = GetComponent<OutlineHelper>();
            PhysicianAnimationController = FindFirstObjectByType<PhysicianAnimationController>();
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
            if (!ComponentAccessed) base.ClickOnComponent();
            PhysicianAnimationController.SetActionTrigger();
        }
    }
}