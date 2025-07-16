using EPOOutline;
using UnityEngine;
using VARLab.Interactions;

namespace VARLab.RespiratoryTherapy
{
    /// <summary>
    /// This component is used on Biopsy Equipment held in the doctor's hand.
    /// It contains helper methods to manage the interactive hitboxes outlines.
    /// </summary>
    [RequireComponent(typeof(Interactable), typeof(Outlinable))]
    public class OutlineHelper : MonoBehaviour
    {
        private Outlinable outline;
        [SerializeField] private Color baseHighlight, activeHighlight;
        
        private void Start()
        {
            outline = GetComponent<Outlinable>();
            outline.OutlineParameters.Color = baseHighlight;
        }

        /// <summary>
        /// Sets the outline colour to either the base or active highlighted colour.
        /// </summary>
        /// <param name="active"> set active </param>
        public void SetHighlightActive(bool active)
        {
            outline.OutlineParameters.Color = active ? activeHighlight : baseHighlight;
        }
        
        /// <summary>
        /// Disables or enables the outlinable component.
        /// </summary>
        /// <param name="active"> set active </param>
        public void SetOutlineActive(bool active)
        {
            outline.enabled = active;
        }
    }
}
