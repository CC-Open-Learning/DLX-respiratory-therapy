using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VARLab.Interactions;

namespace VARLab.RespiratoryTherapy
{
    public static class InteractionHelper
    {
        public static void ToggleInteractivity(List<GameObject> objects, bool toEnable)
        {
            if (objects.Count > 0)
            {
                foreach (GameObject obj in objects)
                {
                    if (obj.TryGetComponent(out Interactable interactable))
                    {
                        interactable.enabled = toEnable;
                    }
                    if (obj.TryGetComponent(out Collider collider))
                    {
                        obj.GetComponents<Collider>().ToList().ForEach(collider => collider.enabled = toEnable);
                    }
                }
            }
        }
    }
}
