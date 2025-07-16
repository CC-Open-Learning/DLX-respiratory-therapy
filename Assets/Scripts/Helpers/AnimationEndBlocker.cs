using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    public class AnimationEndBlocker : MonoBehaviour
    {
        public GameObject InteractionBlocker;
        private BiopsyController biopsyController;

        private void Start()
        {
           biopsyController = FindFirstObjectByType<BiopsyController>();
        }

        public void HandleAnimationCompletedTask()
        {
            InteractionBlocker.SetActive(false);
            biopsyController.UpdateScenarioTaskIndex();
        }

        public void HandleAnimationCompletedScenario()
        {
            InteractionBlocker.SetActive(false);
            biopsyController.UpdateScenarioIndex();
        }
    }
}