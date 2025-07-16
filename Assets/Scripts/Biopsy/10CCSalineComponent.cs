using EPOOutline;

namespace VARLab.RespiratoryTherapy
{
    public class CC10SalineComponent : BiopsyProcedureComponentBase
    {
        void Start()
        {
            PhysicianAnimationController = FindFirstObjectByType<PhysicianAnimationController>();
        }

        private void OnEnable()
        {
            if (TryGetComponent(out Outlinable outlinable))
            {
                outlinable.enabled = true;
            }
        }
        
        public override void ClickOnComponent()
        {
            base.ClickOnComponent();
            PhysicianAnimationController.SetActionTrigger();
        }
    }
}
