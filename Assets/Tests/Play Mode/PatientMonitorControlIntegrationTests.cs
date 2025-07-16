using UnityEngine;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Tests.PlayMode
{
    public class PatientMonitorControlIntegrationTests
    {
        //Path to scene
        private readonly string scenePath = "Assets/Scenes/POITestScene.unity";

        private PatientMonitorControl patientMonitorControlController;
        private VisualElement patientMonitorControllerRoot;

        private Button proceedButton;
        private Button pauseButton;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            //Wait for scene to load
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));

            //Assign values
            patientMonitorControlController = Object.FindFirstObjectByType<PatientMonitorControl>();
            patientMonitorControllerRoot = patientMonitorControlController.GetComponent<UIDocument>().rootVisualElement;

            proceedButton = patientMonitorControllerRoot.Q<Button>("ProceedButton");
            pauseButton = patientMonitorControllerRoot.Q<Button>("PauseButton");
        }

        [TearDown]
        public void TearDown()
        {
            patientMonitorControlController = null;
            patientMonitorControllerRoot = null;

            proceedButton = null;
            pauseButton = null;

            UIManager.Instance = null;
        }
        
        [UnityTest]
        public IEnumerator Show_ShouldDisplayPatientMonitorControl()
        {
            //Act
            patientMonitorControlController.Show();

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.IsTrue(patientMonitorControllerRoot.style.display == DisplayStyle.Flex);
        }

        [UnityTest]
        public IEnumerator Hide_ShouldHidePatientMonitorControl()
        {
            //Arrange
            patientMonitorControllerRoot.style.display = DisplayStyle.Flex;

            //Act
            patientMonitorControlController.Hide();

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.IsTrue(patientMonitorControllerRoot.style.display == DisplayStyle.None);
        }

        [UnityTest]
        public IEnumerator OnProceedButtonClick_ShouldTriggerOnProceedBtnClickedEvent()
        {
            //Arrange
            bool listener = false;
            patientMonitorControlController.OnProceedBtnClicked.AddListener(() => listener = true);

            //Act
            yield return TestUtils.ClickOnButton(proceedButton);

            //Assert
            Assert.IsTrue(listener == true);
        }

        [UnityTest]
        public IEnumerator OnPauseButtonClick_ShouldTriggerOnPauseBtnClickedEvent()
        {
            //Arrange
            bool listener = false;
            patientMonitorControlController.OnPauseBtnClicked.AddListener(() => listener = true);

            //Act
            yield return TestUtils.ClickOnButton(pauseButton);

            //Assert
            Assert.IsTrue(listener == true);
        }
    }
}
