using System.Collections;
using Cinemachine;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class AnimationEndBlockerIntegrationTests : MonoBehaviour
    {
        //Path to scene
        private readonly string scenePath = "Assets/Scenes/AnimationEndBlockerTestScene.unity";

        private AnimationEndBlocker animationEndBlocker;
        private GameObject interactionBlocker;

        private BiopsyController biopsyController;
        private CameraManager cameraManager;
        private UIManager uiManager;

        private CinemachineBrain mainCamera;
        private GameObject frontViewCameraObject;
        private GameObject patientMonitorCameraObject;
        private CinemachineVirtualCamera patientMonitorCamera;
        private GameObject procedureTableCameraObject;
        private CinemachineVirtualCamera procedureTableCamera;

        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            //Wait for scene to load
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));

            //Assign everything
            animationEndBlocker = FindFirstObjectByType<AnimationEndBlocker>();
            interactionBlocker = GameObject.Find("InteractionBlocker");
            animationEndBlocker.InteractionBlocker = interactionBlocker;

            biopsyController = FindFirstObjectByType<BiopsyController>();
            cameraManager = FindFirstObjectByType<CameraManager>();
            uiManager = FindFirstObjectByType<UIManager>();

            mainCamera = FindFirstObjectByType<CinemachineBrain>();
            frontViewCameraObject = GameObject.Find("Default Biopsy Camera");
            patientMonitorCameraObject = GameObject.Find("Patient Monitor POI Camera");
            patientMonitorCamera = patientMonitorCameraObject.GetComponent<CinemachineVirtualCamera>();
            procedureTableCameraObject = GameObject.Find("Procedure Table POI Camera");
            procedureTableCamera = procedureTableCameraObject.GetComponent<CinemachineVirtualCamera>();

            //These objects should be disabled by default, but need to be enabled so they can be found using GameObject.Find
            interactionBlocker.SetActive(false);
            patientMonitorCameraObject.SetActive(false);
            procedureTableCameraObject.SetActive(false);

            

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            animationEndBlocker = null;
            interactionBlocker = null;

            //biopsyController = null;
            Destroy(biopsyController);
            cameraManager = null;
            uiManager = null;

            mainCamera = null;
            frontViewCameraObject = null;
            patientMonitorCameraObject= null;
            patientMonitorCamera = null;
            procedureTableCameraObject = null;
            procedureTableCamera = null;

            POIManager.Instance = null;
            UIManager.Instance = null;

            yield return null;
        }

        [UnityTest]
        public IEnumerator HandleAnimationCompleted_ShouldDisableInteractionBlocker()
        {
            //Arrange
            interactionBlocker.SetActive(true);
            biopsyController.StartBiopsy();

            //Act
            animationEndBlocker.HandleAnimationCompletedTask();
            yield return null;

            //Assert
            Assert.IsTrue(!interactionBlocker.activeSelf);
        }

        [UnityTest]
        public IEnumerator HandleAnimationCompleted_StepTask_ShouldActivatePatientMonitorCamera()
        {
            //Arrange
            uiManager.SetSimulationStatus(SimulationStatus.Biopsy);
            cameraManager.SetDefaultCamera(frontViewCameraObject);
            //Let the camera transition finish
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);
            biopsyController.StartBiopsy();

            //Act
            animationEndBlocker.HandleAnimationCompletedTask();
            //Let the camera transition finish
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.AreEqual(patientMonitorCamera, mainCamera.ActiveVirtualCamera);
        }

        [UnityTest]
        public IEnumerator HandleAnimationCompleted_StepScenario_ShouldActivateProcedureTableCamera()
        {
            //Arrange
            uiManager.SetSimulationStatus(SimulationStatus.Biopsy);
            cameraManager.SetDefaultCamera(frontViewCameraObject);
            //Let the camera transition finish
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);
            biopsyController.StartBiopsy();

            //Act
            animationEndBlocker.HandleAnimationCompletedScenario();
            //Let the camera transition finish
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            //Assert
            Assert.AreEqual(procedureTableCamera, mainCamera.ActiveVirtualCamera);
        }
    }
}
