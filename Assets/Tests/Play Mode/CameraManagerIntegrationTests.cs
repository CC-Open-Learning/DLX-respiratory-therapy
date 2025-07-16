using Cinemachine;
using NUnit.Framework;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class CameraManagerIntegrationTests : MonoBehaviour
    {
        //Path to scene
        private readonly string scenePath = "Assets/Scenes/CameraTestScene.unity";

        private CameraManager cameraManager;
        private CinemachineBrain mainCamera;

        private GameObject defaultCameraObject;
        private CinemachineVirtualCamera defaultCamera;
        private GameObject otherCameraObject;
        private CinemachineVirtualCamera otherCamera;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            //Wait for scene to load
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));

            //Assign everything
            cameraManager = FindFirstObjectByType<CameraManager>();
            mainCamera = FindFirstObjectByType<CinemachineBrain>();

            defaultCameraObject = GameObject.Find("Default Orientation Camera");
            defaultCamera = defaultCameraObject.GetComponent<CinemachineVirtualCamera>();

            otherCameraObject = GameObject.Find("Bronchtower POI Camera");
            otherCamera = otherCameraObject.GetComponent<CinemachineVirtualCamera>();
            //The camera should be disabled by default, but it has to enabled in the test scene so it can be found using GameObject.Find
            otherCameraObject.SetActive(false);

            POIManager.Instance.HandlePOIOpened(GameObject.Find("Bronchtower").GetComponent<POI>());

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            cameraManager = null;
            mainCamera = null;

            defaultCameraObject = null;
            defaultCamera = null;

            otherCameraObject = null;
            otherCamera = null;

            POIManager.Instance = null;

            yield return null;
        }

        [UnityTest]
        public IEnumerator ActivateCamera_ShouldChangeActiveCamera()
        {
            //Act
            cameraManager.ActivateCamera(otherCameraObject, POICameraTransition.In);
            //Let the camera transition finish
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.AreEqual(otherCamera, mainCamera.ActiveVirtualCamera);
        }

        [UnityTest]
        public IEnumerator ActivateCamera_ShouldDisablePreviousCamera()
        {
            //Act
            cameraManager.ActivateCamera(otherCameraObject, POICameraTransition.In);
            //Let the camera transition finish
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.IsTrue(!defaultCameraObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator ActivateDefaultCamera_ShouldSetActiveCameraToDefault()
        {
            //Arrange
            cameraManager.ActivateCamera(otherCameraObject, POICameraTransition.In);
            //Let the camera transition finish
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Act
            cameraManager.ActivateDefaultCamera(false);
            //Let the camera transition finish
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.AreEqual(defaultCamera, mainCamera.ActiveVirtualCamera);
        }

        [UnityTest]
        public IEnumerator ActivateDefaultCamera_ShouldDisablePreviousCamera()
        {
            //Arrange
            cameraManager.ActivateCamera(otherCameraObject, POICameraTransition.In);
            //Let the camera transition finish
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Act
            cameraManager.ActivateDefaultCamera(false);
            //Let the camera transition finish
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.IsTrue(!otherCameraObject.activeSelf);
        }
    }
}