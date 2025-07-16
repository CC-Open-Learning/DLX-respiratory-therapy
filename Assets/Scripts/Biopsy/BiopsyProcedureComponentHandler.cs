using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    public class BiopsyProcedureComponentHandler : MonoBehaviour
    {
        public List<BiopsyProcedureComponentBase> components = new ();
        private PhysicianAnimationController physicianAnimationController;
        
        /// <summary>
        /// Keeps track of suction object and if the sputum cup is currently connected to the suction tube.
        /// </summary>
        private bool isSputumAttached;

        private void Awake()
        {
           components.AddRange(FindObjectsByType<BiopsyProcedureComponentBase>(FindObjectsInactive.Include, 
               FindObjectsSortMode.None).ToList());
            DisableBiopsyProcedureItem();
            
           
        }

        private void Start()
        {
            physicianAnimationController = FindFirstObjectByType<PhysicianAnimationController>();
        }

        /// <summary>
        /// Disable a biopsy procedure item in the list
        /// </summary>
        private void DisableBiopsyProcedureItem()
        {
            components.OfType<SuctionTubeComponent>().ToList().ForEach(item => item.gameObject.SetActive(false));
            components.OfType<CC20SalineComponent>().ToList().ForEach(item => item.gameObject.SetActive(false));
            components.OfType<BiopsyForcepsComponent>().ToList().ForEach(item => item.gameObject.SetActive(false));
            components.OfType<SpecimenJarComponent>().ToList().ForEach(item => item.gameObject.SetActive(false));
            components.OfType<StethoscopeComponent>().ToList().ForEach(item => item.gameObject.SetActive(false));
            components.OfType<SputumContainerComponent>().ToList().ForEach(item => item.gameObject.SetActive(false));
            components.OfType<CC10SalineComponent>().ToList().ForEach(item => item.gameObject.SetActive(false));
        }
        
        /// <summary>
        /// Enable all the biopsy procedure items in the components list.
        /// </summary>
        public void EnableAllBiopsyProcedureItems()
        {
            //components.ForEach(item => item.gameObject.SetActive(true));

            foreach (var item in components)
            {
                item.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Enables the specified biopsy procedure item
        /// </summary>
        /// <param name="equipmentToEnable">The biopsy procedure item to activate.</param>
        public void EnableBiopsyProcedureItem(BiopsyProcedureEquipment equipmentToEnable)
        {
            DisableBiopsyProcedureItem();

            foreach (var item in components.Where(item => item.procedureEquipment ==  equipmentToEnable))
            {
                switch (item.procedureEquipment)
                {
                    case BiopsyProcedureEquipment.BronchoscopeWithSaline10CC: 
                        // The 10cc saline component is a child of the sputum cup. If the 10cc saline is requested, the sputum cup is also set as active.
                        components.OfType<SputumContainerComponent>().ToList().ForEach(item => item.gameObject.SetActive(true));
                        components.OfType<CC10SalineComponent>().ToList().ForEach(item => item.gameObject.SetActive(true));
                        break;
                    case BiopsyProcedureEquipment.SuctionTubeConnector when !isSputumAttached:
                        // If the sputum cup isn't attached, just set the suction tube connector active.
                        isSputumAttached = true;
                        item.gameObject.SetActive(true);
                        break;
                    case BiopsyProcedureEquipment.SuctionTubeConnector:
                        // If the sputum cup is attached, also enable the suction tube connector, and don't call the .PlayProcedureEquipmentAnimation() method.
                        components.OfType<SputumContainerComponent>().ToList().ForEach(item => item.gameObject.SetActive(true));
                        item.gameObject.SetActive(true);
                        continue;
                    default:
                        item.gameObject.SetActive(true);
                        break;
                }    
                physicianAnimationController.PlayProcedureEquipmentAnimation(item.procedureEquipment);
            }
        }

        public void DisableBiopsyProcedureItem(BiopsyProcedureEquipment equipmentToEnable)
        {
            foreach (var item in components)
            {
                if (item.procedureEquipment == equipmentToEnable)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        public void ResetBiopsyProcedureComponents()
        {
            foreach (var item in components)
            {
                item.ResetComponent();
            }
        }
        
        public GameObject GetProcedureEquipmentObject(BiopsyProcedureEquipment equipment)
        {
            var match = components.Find(x => x.procedureEquipment == equipment);
            return match != null ? match.gameObject : null;
        }
    }
}
