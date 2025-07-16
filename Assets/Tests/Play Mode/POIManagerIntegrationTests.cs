using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class POIManagerIntegrationTests : MonoBehaviour
    {
        //Path to scene
        private readonly string scenePath = "Assets/Scenes/POITestScene.unity";

        private List<Collider> explorableColliders;
        private List<Collider> poiColliders;
        private List<Collider> mainPOIColliders;

        private POIManager poiManager;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            //Wait for scene to load
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));

            //Assign everything
            explorableColliders = new List<Collider>();
            Explorable[] explorables = FindObjectsByType<Explorable>(FindObjectsSortMode.None);
            foreach (Explorable explorable in explorables)
            {
                explorableColliders.AddRange(explorable.GetComponents<Collider>());
            }
            BronchoscopeComponent[] bronchoscopeComponents = FindObjectsByType<BronchoscopeComponent>(FindObjectsSortMode.None);
            foreach (BronchoscopeComponent component in bronchoscopeComponents)
            {
                explorableColliders.AddRange(component.gameObject.GetComponents<Collider>());
            }

            poiColliders = new List<Collider>();
            mainPOIColliders = new List<Collider>();
            POI[] pois = FindObjectsByType<POI>(FindObjectsSortMode.None);
            foreach (POI poi in pois)
            {
                poiColliders.AddRange(poi.GetComponents<Collider>());

                if (poi.IsMainPOI)
                {
                    mainPOIColliders.AddRange(poi.GetComponents<Collider>());
                }
            }

            poiManager = FindFirstObjectByType<POIManager>();

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            explorableColliders = null;
            poiColliders = null;
            mainPOIColliders = null;

            poiManager = null;

            UIManager.Instance = null;
            POIManager.Instance = null;

            yield return null;
        }

        [Test]
        public void HandlePOIOpened_ShouldSetCurrentPOI()
        {
            //Arrange
            POI poi = poiColliders[0].GetComponent<POI>();

            //Act
            poiManager.HandlePOIOpened(poi);

            //Assert
            Assert.AreEqual(poi, poiManager.CurrentPOI);
        }

        [Test]
        public void ToggleAllExplorables_Enable_ShouldEnableAllExplorableColliders()
        {
            //Arrange
            explorableColliders.ForEach(collider => collider.enabled = false);

            //Act
            poiManager.ToggleAllExplorables(true);

            //Assert
            Assert.AreEqual(true, explorableColliders.All(collider => collider.enabled == true));
        }

        [Test]
        public void ToggleAllExplorables_Disable_ShouldDisableAllExplorableColliders()
        {
            //Arrange
            explorableColliders.ForEach(collider => collider.enabled = true);

            //Act
            poiManager.ToggleAllExplorables(false);

            //Assert
            Assert.AreEqual(true, explorableColliders.All(collider => collider.enabled == false));
        }

        [Test]
        public void ToggleAllPOIs_Enable_ShouldEnableAllPOIColliders()
        {
            //Arrange
            poiColliders.ForEach(collider => collider.enabled = false);

            //Act
            poiManager.ToggleAllPOIs(true);

            //Assert
            Assert.AreEqual(true, poiColliders.All(collider => collider.enabled == true));
        }
        
        [Test]
        public void ToggleAllPOIs_Disable_ShouldDisableAllPOIColliders()
        {
            //Arrange
            poiColliders.ForEach(collider => collider.enabled = true);

            //Act
            poiManager.ToggleAllPOIs(false);

            //Assert
            Assert.AreEqual(true, poiColliders.All(collider => collider.enabled == false));
        }

        [Test]
        public void EnableDefaultPOIs_ShouldEnableDefaultOrientationPOIColliders()
        {
            //Arrange
            mainPOIColliders.ForEach(collider => collider.enabled = false);

            //Act
            poiManager.EnableDefaultPOIs();

            //Assert
            Assert.AreEqual(true, mainPOIColliders.All(collider => collider.enabled == true));
        }

        [Test]
        public void ResetPOIs_ShouldEnableDefaultOrientationPOIColliders()
        {
            //Arrange
            mainPOIColliders.ForEach(collider => collider.enabled = false);

            //Act
            poiManager.ResetPOIs();

            //Assert
            Assert.AreEqual(true, mainPOIColliders.All(collider => collider.enabled == true));
        }
    }
}