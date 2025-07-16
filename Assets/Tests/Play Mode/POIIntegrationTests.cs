using Cinemachine;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class POIIntegrationTests : MonoBehaviour
    {
        //Path to scene
        private readonly string scenePath = "Assets/Scenes/POITestScene.unity";

        private List<Collider> mainPOIColliders;
        private List<Collider> bronchTowerChildPOIColliders;
        private Collider bronchTowerMidAreaExplorableCollider;
        private Collider bronchTowerExplorableCollider;

        private List<Collider> nonBronchTowerMidAreaExplorableColliders;
        private List<Collider> nonBronchTowerChildPOIColliders;

        private POIManager poiManager;
        private UIManager uiManager;
        private CameraManager cameraManager;
        private CinemachineBrain mainCamera;

        private VisualElement toolbar;
        private VisualElement poiToolbar;
        private VisualElement procedureToolbar;

        private GameObject bronchTowerObject;
        private POI bronchTowerPOI;
        private GameObject bronchTowerMidAreaObject;
        private POI bronchTowerMidAreaPOI;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            //Wait for scene to load
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));

            //Assign everything
            poiManager = FindFirstObjectByType<POIManager>();
            uiManager = FindFirstObjectByType<UIManager>();
            cameraManager = FindFirstObjectByType<CameraManager>();
            mainCamera = FindFirstObjectByType<CinemachineBrain>();

            bronchTowerObject = GameObject.Find("Bronchtower");
            bronchTowerPOI = bronchTowerObject.GetComponent<POI>();
            bronchTowerMidAreaObject = GameObject.Find("BronchtowerMidAreaPOI");
            bronchTowerMidAreaPOI = bronchTowerMidAreaObject.GetComponent<POI>();

            toolbar = FindFirstObjectByType<ToolbarController>().GetComponent<UIDocument>().rootVisualElement;
            poiToolbar = FindFirstObjectByType<POIToolbarController>().GetComponent<UIDocument>().rootVisualElement;
            procedureToolbar = FindFirstObjectByType<ProcedurePOIToolbar>().GetComponent<UIDocument>().rootVisualElement;

            //Sort relevant collider components
            mainPOIColliders = new List<Collider>
            {
                bronchTowerObject.GetComponent<Collider>(),
                GameObject.Find("MedicalCart").GetComponent<Collider>()
            };

            bronchTowerChildPOIColliders = new List<Collider>
            {
                bronchTowerMidAreaObject.GetComponent<Collider>(),
                GameObject.Find("BronchtowerUpperAreaPOI").GetComponent<Collider>(),
                GameObject.Find("BronchtowerLowerAreaPOI").GetComponent<Collider>(),
                GameObject.Find("BronchoscopePOI").GetComponent<Collider>()
            };

            nonBronchTowerMidAreaExplorableColliders = new List<Collider>
            {
                GameObject.Find("BronchTower_Monitor").GetComponent<Collider>(),
                GameObject.Find("BronchTower_ScopeBrackets").GetComponent<Collider>(),
                GameObject.Find("Bronchscope_Connector").GetComponent<Collider>(),
                GameObject.Find("Bronchscope").GetComponent<Collider>(),
                GameObject.Find("BronchTowerExplorable").GetComponent<Collider>()
            };

            nonBronchTowerChildPOIColliders = new List<Collider>
            {
                bronchTowerObject.GetComponent<Collider>(),
                GameObject.Find("MedicalCart").GetComponent<Collider>(),
                GameObject.Find("MedicalCartChildPOI").GetComponent<Collider>()
            };

            bronchTowerMidAreaExplorableCollider = GameObject.Find("BronchTower_LowerUnit").GetComponent<Collider>();
            bronchTowerExplorableCollider = GameObject.Find("BronchTowerExplorable").GetComponent<Collider>();

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            mainPOIColliders = null;
            bronchTowerChildPOIColliders = null;
            bronchTowerMidAreaExplorableCollider = null;

            nonBronchTowerMidAreaExplorableColliders = null;
            nonBronchTowerChildPOIColliders = null;

            poiManager = null;
            uiManager = null;
            cameraManager = null;
            mainCamera = null;

            bronchTowerObject = null;
            bronchTowerPOI = null;
            bronchTowerMidAreaObject = null;
            bronchTowerMidAreaPOI = null;

            toolbar = null;
            poiToolbar = null;
            procedureToolbar = null;

            UIManager.Instance = null;
            POIManager.Instance = null;

            yield return null;
        }

        [UnityTest]
        public IEnumerator Open_ShouldEnablePOIExplorableColliders()
        {
            //Arrange
            poiManager.ToggleAllExplorables(false);
            bronchTowerPOI.Open();
            //Wait until the camera transition finishes before proceeding
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Act
            bronchTowerMidAreaPOI.Open();
            //Let the camera transition finish, because interactivity is toggled at the end of it
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.IsTrue(bronchTowerMidAreaExplorableCollider.enabled && nonBronchTowerMidAreaExplorableColliders.All(collider => collider.enabled == false));
        }

        [UnityTest]
        public IEnumerator Open_ShouldEnableChildPOIColliders()
        {
            //Arrange
            poiManager.ToggleAllPOIs(false);

            //Act
            bronchTowerPOI.Open();
            //Let the camera transition finish, because interactivity is toggled at the end of it
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.IsTrue(bronchTowerChildPOIColliders.All(collider => collider.enabled == true) 
                && nonBronchTowerChildPOIColliders.All(collider => collider.enabled == false));
        }

        [UnityTest]
        public IEnumerator CloseIntoParent_HasParent_ShouldEnableParentPOIExplorableColliders()
        {
            //Arrange
            poiManager.ToggleAllExplorables(false);
            bronchTowerPOI.Open();
            //Wait until the camera transition finishes before proceeding
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);
            bronchTowerMidAreaPOI.Open();
            //Wait until the camera transition finishes before proceeding
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Act
            bronchTowerMidAreaPOI.CloseIntoParent();
            //Let the camera transition finish, because interactivity is toggled at the end of it
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.IsTrue(bronchTowerExplorableCollider.enabled && !bronchTowerMidAreaExplorableCollider.enabled);
        }

        [UnityTest]
        public IEnumerator CloseIntoParent_HasParent_ShouldEnableChildPOIColliders()
        {
            //Arrange
            poiManager.ToggleAllPOIs(false);
            bronchTowerPOI.Open();
            //Wait until the camera transition finishes before proceeding
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);
            bronchTowerMidAreaPOI.Open();
            //Wait until the camera transition finishes before proceeding
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Act
            bronchTowerMidAreaPOI.CloseIntoParent();
            //Let the camera transition finish, because interactivity is toggled at the end of it
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.IsTrue(bronchTowerChildPOIColliders.All(collider => collider.enabled == true) 
                && nonBronchTowerChildPOIColliders.All(collider => collider.enabled == false));
        }

        [UnityTest]
        public IEnumerator CloseIntoParent_NoParent_ShouldDisableAllExplorableColliders()
        {
            //Arrange
            poiManager.ToggleAllExplorables(false);
            bronchTowerPOI.Open();
            //Wait until the camera transition finishes before proceeding
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Act
            bronchTowerPOI.CloseIntoParent();
            //Let the camera transition finish, because interactivity is toggled at the end of it
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.IsTrue(!bronchTowerExplorableCollider.enabled && !bronchTowerMidAreaExplorableCollider.enabled 
                && nonBronchTowerMidAreaExplorableColliders.All(collider => collider.enabled == false));
        }

        [UnityTest]
        public IEnumerator CloseIntoParent_NoParent_ShouldResetToOrientationStart()
        {
            //Arrange
            poiManager.ToggleAllPOIs(false);
            bronchTowerPOI.Open();
            //Wait until the camera transition finishes before proceeding
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Act
            bronchTowerPOI.CloseIntoParent();
            //Let the camera transition finish, because interactivity is toggled at the end of it
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.IsTrue(mainPOIColliders.All(collider => collider.enabled == true)
                && bronchTowerChildPOIColliders.All(collider => collider.enabled == false));
        }

        [UnityTest]
        public IEnumerator CloseIntoReset_ShouldResetToOrientationStart()
        {
            //Arrange
            poiManager.ToggleAllPOIs(false);
            bronchTowerPOI.Open();
            //Wait until the camera transition finishes before proceeding
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Act
            bronchTowerPOI.CloseIntoReset();
            //Let the camera transition finish, because interactivity is toggled at the end of it
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.IsTrue(mainPOIColliders.All(collider => collider.enabled == true)
                && bronchTowerChildPOIColliders.All(collider => collider.enabled == false));
        }

        [UnityTest]
        public IEnumerator CloseIntoSimulated_ShouldResetToOrientationStart()
        {
            //Arrange
            poiManager.ToggleAllPOIs(false);
            bronchTowerPOI.Open();
            //Wait until the camera transition finishes before proceeding
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Act
            bronchTowerPOI.CloseIntoSimulatedReset();
            //Let the camera transition finish, because interactivity is toggled at the end of it
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.IsTrue(mainPOIColliders.All(collider => collider.enabled == true)
                && bronchTowerChildPOIColliders.All(collider => collider.enabled == false));
        }

        [Test]
        public void EnablePOIInteractions_SimualtedReset_ShouldNotHideToolbar()
        {
            //Arrange
            uiManager.SetUpToolbar(new ToolbarConfig() { ShowMenuButton = true});

            //Act
            bronchTowerPOI.EnablePOIInteractions(POICameraTransition.SimulatedReset);

            //Assert
            Assert.IsTrue(toolbar.style.display == DisplayStyle.Flex);
        }

        [Test]
        public void EnablePOIInteractions_OrientationMode_ShouldEnablePOIToolbar()
        {
            //Arrange
            uiManager.SetSimulationStatus(SimulationStatus.Orientation);
            uiManager.SetUpPOIToolbar("Bronch Tower", null);

            //Act
            bronchTowerPOI.EnablePOIInteractions(POICameraTransition.In);

            //Assert
            Assert.IsTrue(poiToolbar.style.display == DisplayStyle.Flex);
        }

        [Test]
        public void EnablePOIInteractions_ProcedureMode_ShouldEnableProcedureToolbar()
        {
            //Arrange
            uiManager.SetSimulationStatus(SimulationStatus.Biopsy);
            uiManager.ToggleProcedureToolbar(true);

            //Act
            bronchTowerPOI.EnablePOIInteractions(POICameraTransition.In);

            //Assert
            Assert.IsTrue(procedureToolbar.style.display == DisplayStyle.Flex);
        }
    }
}
