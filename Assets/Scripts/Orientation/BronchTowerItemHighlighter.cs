using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace VARLab.RespiratoryTherapy
{
    public class BronchTowerItemHighliter : MonoBehaviour
    {
        public GameObject ItemToHighlight;
        public Material TransparentMaterial;
        
        private Renderer[] explorableRenderers;
        private Dictionary<Renderer, Material[]> materialsByRender = new ();
        
        private void Start()
        {
            if (ItemToHighlight == null || TransparentMaterial == null)
            {
                return;
            }

            // Get all child renderers except the one belonging to ItemToHighlight
            explorableRenderers = GetComponentsInChildren<Renderer>()
                .Where(renderer => renderer.gameObject != ItemToHighlight)
                .ToArray();
        }
        
        /// <summary>
        /// Highlights explorable items by replacing their materials with the transparent highlight material.
        /// Ensures the original materials are saved for each renderer before making changes.
        /// Logs an error if no renderers are found to highlight.
        /// </summary>
        public void HighlightExplorable()
        {
            if (explorableRenderers == null || explorableRenderers.Length == 0)
            {
                return;
            }

            foreach (Renderer renderer in explorableRenderers)
            {
                // Save the original materials for this renderer
                if (!materialsByRender.ContainsKey(renderer))
                {
                    materialsByRender[renderer] = renderer.materials;
                }

                // Replace all materials with the highlight material
                Material[] highlightMaterials = renderer.materials.Select(_ => TransparentMaterial).ToArray();
                renderer.materials = highlightMaterials;
            }
        }

        /// <summary>
        /// Restores the original materials for all renderers that were previously modified.
        /// Logs an error if no original materials are saved.
        /// Clears the material mappings after restoration to free memory and prevent reuse.
        /// </summary>
        public void RestoreOriginalMaterials()
        {
            if (materialsByRender.Count == 0)
            {
                return;
            }

            foreach (var keyValuePair in materialsByRender)
            {
                Renderer renderer = keyValuePair.Key;
                Material[] materials = keyValuePair.Value;

                // Restore the saved materials
                if (renderer != null)
                {
                    renderer.materials = materials;
                }
            }

            // Clear the originalMaterials dictionary after restoration
            materialsByRender.Clear();
        }
    }
}
