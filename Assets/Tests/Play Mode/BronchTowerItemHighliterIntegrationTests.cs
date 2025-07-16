using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class BronchTowerItemHighliterIntegrationTests
    {
         private GameObject testObject;
        private BronchTowerItemHighliter highlighter;
        private GameObject itemToHighlight;
        private Renderer[] childRenderers;
        private Material originalMaterial;
        private Material transparentMaterial;

        [SetUp]
        public void Setup()
        {
            // Create a test GameObject with the BronchTowerItemHighliter component
            testObject = new GameObject("TestObject");
            highlighter = testObject.AddComponent<BronchTowerItemHighliter>();

            // Create and assign the transparent material
            transparentMaterial = new Material(Shader.Find("Standard"));
            highlighter.TransparentMaterial = transparentMaterial;

            // Create a child GameObject to highlight
            itemToHighlight = new GameObject("ItemToHighlight");
            itemToHighlight.transform.SetParent(testObject.transform);
            highlighter.ItemToHighlight = itemToHighlight;

            // Add multiple renderers with materials to the parent GameObject
            for (int i = 0; i < 3; i++)
            {
                GameObject child = GameObject.CreatePrimitive(PrimitiveType.Cube);
                child.transform.SetParent(testObject.transform);
                Renderer renderer = child.GetComponent<Renderer>();

                originalMaterial = new Material(Shader.Find("Standard")) { color = Color.white };
                renderer.material = originalMaterial;
            }

            // Collect child renderers for verification
            childRenderers = testObject.GetComponentsInChildren<Renderer>();
        }

        [TearDown]
        public void Teardown()
        {
            // Clean up after tests
            Object.DestroyImmediate(testObject);
            Object.DestroyImmediate(itemToHighlight);
            Object.DestroyImmediate(transparentMaterial);
            Object.DestroyImmediate(originalMaterial);
        }

        [UnityTest]
        public IEnumerator HighlightExplorable_ShouldReplaceMaterialsWithTransparentMaterial()
        {
            // Act
            highlighter.HighlightExplorable();
            yield return null;

            // Assert
            foreach (Renderer renderer in childRenderers)
            {
                // Skip the item to highlight
                if (renderer.gameObject == itemToHighlight) continue;

                // Verify that all materials are replaced with the transparent material
                foreach (Material material in renderer.materials)
                {
                    Assert.AreEqual(transparentMaterial.shader, material.shader, "Material was not replaced with the transparent material.");
                }
            }
        }

        [UnityTest]
        public IEnumerator RestoreOriginalMaterials_ShouldRestoreOriginalMaterials()
        {
            // Act
            highlighter.HighlightExplorable();
            yield return null;

            highlighter.RestoreOriginalMaterials();
            yield return null;

            // Assert
            foreach (Renderer renderer in childRenderers)
            {
                // Skip the item to highlight
                if (renderer.gameObject == itemToHighlight) continue;

                // Verify that original materials are restored
                foreach (Material material in renderer.materials)
                {
                    Assert.AreEqual(originalMaterial.shader, material.shader, "Material was not restored to the original material.");
                }
            }
        }
    }
}
