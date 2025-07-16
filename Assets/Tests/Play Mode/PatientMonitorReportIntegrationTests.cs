using UnityEngine;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Tests.PlayMode
{
    public class PatientMonitorReportIntegrationTests
    {
        //Path to scene
        private readonly string scenePath = "Assets/Scenes/POITestScene.unity";

        private PatientMonitorReport patientMonitorReportController;
        private VisualElement patientMonitorReportRoot;

        private Button backButton;
        private Button reportButton;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            //Wait for scene to load
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));

            //Assign values
            patientMonitorReportController = Object.FindFirstObjectByType<PatientMonitorReport>();
            patientMonitorReportRoot = patientMonitorReportController.GetComponent<UIDocument>().rootVisualElement;

            backButton = patientMonitorReportRoot.Q<Button>("BackButton");
            reportButton = patientMonitorReportRoot.Q<Button>("ReportButton");
        }

        [TearDown]
        public void TearDown()
        {
            patientMonitorReportController = null;
            patientMonitorReportRoot = null;

            backButton = null;
            reportButton = null;

            UIManager.Instance = null;
        }
        
        [UnityTest]
        public IEnumerator Show_ShouldDisplayPatientMonitorReport()
        {

            //Act
            patientMonitorReportController.Show();

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.IsTrue(patientMonitorReportRoot.style.display == DisplayStyle.Flex);
        }

        [UnityTest]
        public IEnumerator Hide_ShouldHidePatientMonitorReport()
        {
            //Arrange
            patientMonitorReportRoot.style.display = DisplayStyle.Flex;

            //Act
            patientMonitorReportController.Hide();

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.IsTrue(patientMonitorReportRoot.style.display == DisplayStyle.None);
        }

        [UnityTest]
        public IEnumerator OnBackButtonClick_ShouldTriggerOnBackBtnClickedEvent()
        {
            //Arrange
            bool listener = false;
            patientMonitorReportController.OnBackBtnClicked.AddListener(() => listener = true);

            //Act
            yield return TestUtils.ClickOnButton(backButton);

            //Assert
            Assert.IsTrue(listener == true);
        }

        [UnityTest]
        public IEnumerator OnReportButtonClick_ShouldTriggerOnReportBtnClickedEvent()
        {
            //Arrange
            bool listener = false;
            patientMonitorReportController.OnReportBtnClicked.AddListener((List<PatientMonitorVitals> list) => listener = true);

            //Act
            yield return TestUtils.ClickOnButton(reportButton);

            //Assert
            Assert.IsTrue(listener == true);
        }
    }
}
