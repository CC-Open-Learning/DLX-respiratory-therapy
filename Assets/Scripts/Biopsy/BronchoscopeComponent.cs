namespace VARLab.RespiratoryTherapy
{
    public class BronchoscopeComponent : BiopsyProcedureComponentBase
    {
        private BiopsyController biopsyController;

        private void Start()
        {
            ComponentAccessed = false;
            biopsyController = FindObjectOfType<BiopsyController>();
        }

        /// <summary>
        /// This will method will be called by the Interactable.cs, Mouse Click event.
        /// On the object which contains BronchoscopeComponent in the scene.
        /// </summary>
        public override void ClickOnComponent()
        {
            if (ComponentAccessed) return;
            
            base.ClickOnComponent();
            biopsyController.UpdateScenarioIndex();
        }
    }
}
