using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    /// <summary>
    /// Controls the behaviour of hotspots that appear when the breathing check scenario is active.
    /// </summary>
    public class StethoscopeBreathingHotspot : MonoBehaviour
    {
        [SerializeField] private Material unhighlightedMaterial, highlightedMaterial;
        private MeshRenderer meshRenderer;
        
        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = unhighlightedMaterial;
        }

        /// <summary>
        /// Activates, or deactivates the highlight on the hotspot.
        /// Called from its own game objects interactable events.
        /// </summary>
        /// <param name="toEnable"> To enable highlight </param>
        public void EnableHotspotHighlight(bool toEnable)
        {
            meshRenderer.material = toEnable ? highlightedMaterial : unhighlightedMaterial;
        }
    }
}
