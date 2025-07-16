using UnityEngine;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Tests.PlayMode
{
    public class StethoscopeBreathingHotspotIntegrationTests
    {
        private MeshRenderer meshRenderer;
        private Material highlightedMaterial;
        private Material unhighlightedMaterial;
        private StethoscopeBreathingHotspot hotspot;
        
        private readonly string scenePath = "Assets/Scenes/StethoscopeBreathingHotspotTestScene.unity";
        
        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath,
                new LoadSceneParameters(LoadSceneMode.Single));
            
            hotspot = Object.FindAnyObjectByType<StethoscopeBreathingHotspot>();
            meshRenderer = hotspot.GetComponent<MeshRenderer>();

            // Access the private serialized materials using reflection
            var type = typeof(StethoscopeBreathingHotspot);
            unhighlightedMaterial = (Material)type
                .GetField("unhighlightedMaterial", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(hotspot);
            highlightedMaterial = (Material)type
                .GetField("highlightedMaterial", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(hotspot);
        }

        [UnityTest]
        public IEnumerator Start_SetsUnhighlightedMaterial()
        {
            //Act
            yield return null;
            //Assert
            Assert.AreEqual(unhighlightedMaterial, meshRenderer.sharedMaterial);
        }

        [UnityTest]
        public IEnumerator EnableHotspotHighlight_True_SetsHighlightedMaterial()
        {
            //Act
            yield return null;

            hotspot.EnableHotspotHighlight(true);

            yield return null;
            //Assert
            Assert.AreEqual(highlightedMaterial, meshRenderer.sharedMaterial);
        }

        [UnityTest]
        public IEnumerator EnableHotspotHighlight_False_SetsUnhighlightedMaterial()
        {
            //Act
            yield return null;

            hotspot.EnableHotspotHighlight(false);

            yield return null;
            //Assert
            Assert.AreEqual(unhighlightedMaterial, meshRenderer.sharedMaterial);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(hotspot);
        }
    }
}
