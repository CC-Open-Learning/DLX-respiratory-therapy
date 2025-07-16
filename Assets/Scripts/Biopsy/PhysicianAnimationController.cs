using System.Collections.Generic;
using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [RequireComponent(typeof(Animator))]
    public class PhysicianAnimationController : MonoBehaviour
    {
        [SerializeField] private GameObject animationInteractionBlocker;

        private Dictionary<BiopsyProcedureEquipment, string> animationMap;
        private bool initial10ccComplete;
        
        private Animator animator;
        private const float CrossFadeTime = 0.2f;
        
        private void Start()
        {
            animator = GetComponent<Animator>();
            InitializeAnimationStatus();
        }
        
        private void InitializeAnimationStatus()
        {
            animationMap = new Dictionary<BiopsyProcedureEquipment, string>
            {
                { BiopsyProcedureEquipment.SuctionTubeConnector, "01BronchoscopeNoTubes" },
                { BiopsyProcedureEquipment.BronchoscopeWithSaline20CC, "03Pending20ccSaline" },
                { BiopsyProcedureEquipment.BronchoscopeWithBiopsyForceps, "05PendingBiopsyForceps" },
                { BiopsyProcedureEquipment.SpecimenJar, "07PendingSpecimenJar" },
                { BiopsyProcedureEquipment.BronchoscopeWithEpinephrine, "09Pending5ccEpinephrine" },
                { BiopsyProcedureEquipment.SterileSputumSpecimenContainer, "11PendingSputumCup" },
                { BiopsyProcedureEquipment.Stethoscope, "Default" }
            };
        }

        public void SetStartBiopsyProcedure()
        {
            initial10ccComplete = false;
            PlayAnimation(animationMap[BiopsyProcedureEquipment.SuctionTubeConnector]);
        }

        public void RemoveSuctionTube()
        {
            PlayAnimation("17SputumCupDetached");
        }

        public void SetActionTrigger()
        {
            animationInteractionBlocker.SetActive(true);
            animator.SetTrigger("Action");
        }
        
        /// <summary>
        /// Gets called from biopsy procedure equipment when an item is set as active.
        /// The items connected animation will also be set to play with a cross-fade.
        /// </summary>
        /// <param name="equipment"></param>
        public void PlayProcedureEquipmentAnimation(BiopsyProcedureEquipment equipment)
        {
            if(equipment == BiopsyProcedureEquipment.Bronchoscope) { return; }
            
            if (equipment == BiopsyProcedureEquipment.BronchoscopeConnector)
            {
                SetActionTrigger();
                return;
            }

            if (equipment == BiopsyProcedureEquipment.BronchoscopeWithSaline10CC)
            {
                var animationState = initial10ccComplete ? "15SputumPending10ccSyringe2" : "13SputumPending10ccSyringe";
                
                initial10ccComplete = true;
                PlayAnimation(animationState);
            }
            else
            {
                PlayAnimation(animationMap[equipment]);
            }
        }
        
        private void PlayAnimation(string animationState)
        {
            AnimationHelper.ActivateCrossFade(animator, animationState, CrossFadeTime, animationInteractionBlocker);
        }
    }
}
