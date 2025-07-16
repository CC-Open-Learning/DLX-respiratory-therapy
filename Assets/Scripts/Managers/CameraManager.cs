using System.Collections;
using Cinemachine;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using VARLab.CloudSave;
using VARLab.ObjectViewer;

namespace VARLab.RespiratoryTherapy
{
    [CloudSaved]
    [JsonObject(MemberSerialization.OptIn)]
    public class CameraManager : MonoBehaviour
    {
        /// Invokes <see cref="POIManager.EnableIntractableAfterTransition(POICameraTransition)"/>
        public UnityEvent<POICameraTransition> OnCameraTransitionCompleted;

        [SerializeField] private CinemachineBrain mainCamera;
        [SerializeField] private GameObject startMenuCamera;
        private ObjectViewerController objectViewerController;

        private GameObject defaultCamera;
        private GameObject currentCamera;

        private bool menuCameraReset;

        [JsonProperty]
        public float mouseSensitivity;
        
        void Awake()
        {
            OnCameraTransitionCompleted ??= new UnityEvent<POICameraTransition>();
            objectViewerController = FindFirstObjectByType<ObjectViewerController>();

            menuCameraReset = false;
        }

        private void Start()
        {
            SetDefaultCamera(startMenuCamera);
            mainCamera.m_CameraActivatedEvent.AddListener((ICinemachineCamera, ICinemachineCamera2) => MenuCameraResetCheck());
        }

        public void ActivateCamera(GameObject camera, POICameraTransition transitionType)
        {
            camera.SetActive(true);
            StartCoroutine("WaitForBlendComplete", transitionType);
            currentCamera.SetActive(false);

            currentCamera = camera;
        }

        public void ActivateDefaultCamera(bool simulatedReset)
        {
            if (defaultCamera != currentCamera)
            {
                defaultCamera.SetActive(true);
                StartCoroutine("WaitForBlendComplete",
                    simulatedReset ? POICameraTransition.SimulatedReset : POICameraTransition.Reset);
                currentCamera.SetActive(false);

                currentCamera = defaultCamera;
            }
        }

        public void SetDefaultCamera(GameObject camera)
        {
            if (defaultCamera != null)
            {
                defaultCamera.SetActive(false);
            }

            if (currentCamera != null)
            {
                currentCamera.SetActive(false);
            }

            defaultCamera = camera;
            currentCamera = defaultCamera;
            defaultCamera.SetActive(true);
        }

        /// <summary>
        /// Manually checks for when a camera transition is completed because Cinemachine 2.10 does
        /// not provide an OnBlendComplete event. This Coroutine assumes that a camera was activated
        /// immediately before itself.
        /// </summary>
        private IEnumerator WaitForBlendComplete(POICameraTransition transitionType)
        {
            //Wait until IsBlending is true because the value change is slightly delayed
            yield return new WaitUntil(() => mainCamera.IsBlending);
            yield return new WaitUntil(() => !mainCamera.IsBlending);

            if (POIManager.Instance.CurrentPOI != null)
            {
                OnCameraTransitionCompleted?.Invoke(transitionType);
            }
        }
        
        /// <summary>
        /// Triggered by the slider in the Settings Menu
        /// </summary>
        public void SetCameraSensitivity(float newSensitivityValue)
        {
            //do not allow sensitivity to be less than 10% of the max
            const float minSensitivity = 0.1f;
            if (newSensitivityValue <= minSensitivity) newSensitivityValue = minSensitivity;
            
            const int sensitivityMultiplier = 1000;
            int updatedValue = (int)(newSensitivityValue * sensitivityMultiplier);

            if (objectViewerController != null)
            {
                objectViewerController.MouseSensitivity = updatedValue;
                mouseSensitivity = newSensitivityValue;
            }
        }

        /// <summary>
        /// Toggled from the menu so the camera manager knows it has to make the reset transition instant
        /// </summary>
        public void ToggleMenuCameraReset(bool toggle)
        {
            menuCameraReset = toggle;
        }

        /// <summary>
        /// Force completes the current blend so the camera cuts immediately to the orientation default camera
        /// </summary>
        private void MenuCameraResetCheck()
        {
            if (menuCameraReset)
            {
                mainCamera.ActiveBlend = null;
                menuCameraReset = false;
            }
        }
    }
}