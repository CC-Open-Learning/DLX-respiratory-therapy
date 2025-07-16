using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    public static class AnimationHelper
    {
        // Note: this might need to be updated (or variations added) depending on how we actually end up
        // triggering the animations
        public static void ActivateAnimation(Animator animator, string trigger, GameObject interactionBlocker)
        {
            interactionBlocker.SetActive(true);
            animator.SetTrigger(trigger);
        }

        public static void ActivateCrossFade(Animator animator, string animationName, float duration, GameObject interactionBlocker, bool doBlockInteraction = false)
        {
            if(doBlockInteraction) { interactionBlocker.SetActive(true); }
            animator.CrossFade(animationName, duration);
        }

        /// <summary>
        /// Enables interaction once an animation is completed. Used as an animation event.
        /// </summary>
        public static void HandleAnimationCompleted(GameObject interactionBlocker)
        {
            interactionBlocker.SetActive(false);
        }
    }
}