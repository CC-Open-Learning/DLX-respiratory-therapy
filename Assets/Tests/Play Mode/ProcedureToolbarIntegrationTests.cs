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
    public class ProcedureToolbarIntegrationTests
    {
        //Path to scene
        private readonly string scenePath = "Assets/Scenes/POITestScene.unity";

        private ProcedurePOIToolbar procedureToolbarController;
        
        private VisualElement procedureToolbarRoot;
        private Button patientMonitorButton;
        private Button frontViewButton;
        private Button bronchMonitorButton;
        private Button bronchoscopeButton;
        private Button patientViewButton;
        private Button procedureTableButton;

        private POI patientMonitorPOI;
        
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            //Wait for scene to load
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));

            //Assign values
            patientMonitorPOI = GameObject.Find("PatientMonitorPOI").GetComponent<POI>();

            procedureToolbarController = GameObject.FindFirstObjectByType<ProcedurePOIToolbar>();
            procedureToolbarRoot = procedureToolbarController.GetComponent<UIDocument>().rootVisualElement;

            patientMonitorButton = procedureToolbarRoot.Q("PatientMonitorButton").Q<Button>();
            frontViewButton = procedureToolbarRoot.Q("FrontViewButton").Q<Button>();
            bronchMonitorButton = procedureToolbarRoot.Q("BronchMonitorButton").Q<Button>();
            bronchoscopeButton = procedureToolbarRoot.Q("BronchoscopeButton").Q<Button>();
            patientViewButton = procedureToolbarRoot.Q("PatientViewButton").Q<Button>();
            procedureTableButton = procedureToolbarRoot.Q("ProcedureTableButton").Q<Button>();

            //Setup
            UIManager.Instance.SetSimulationStatus(SimulationStatus.Biopsy);
        }

        [TearDown]
        public void TearDown()
        {
            patientMonitorButton = null;
            frontViewButton = null;
            bronchMonitorButton = null;
            bronchoscopeButton = null;
            patientViewButton = null;
            procedureTableButton = null;
            
            patientMonitorPOI = null;

            UIManager.Instance = null;
            POIManager.Instance = null;
        }
        
        [UnityTest]
        public IEnumerator Show_WhenInitalized_ShouldDisplayProcedureToolbar()
        {
            //Arrange
            procedureToolbarController.ToggleProcedureToolbarStatus(true);

            //Act
            procedureToolbarController.Show();

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.IsTrue(procedureToolbarRoot.style.display == DisplayStyle.Flex);
        }

        [UnityTest]
        public IEnumerator Hide_WhenInitalized_ShouldHideProcedureToolbar()
        {
            //Arrange
            procedureToolbarController.ToggleProcedureToolbarStatus(true);
            procedureToolbarRoot.style.display = DisplayStyle.Flex;

            //Act
            procedureToolbarController.Hide();

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.IsTrue(procedureToolbarRoot.style.display == DisplayStyle.None);
        }

        [UnityTest]
        public IEnumerator Show_WhenNotInitalized_ShouldNotDisplayProcedureToolbar()
        {
            //Arrange
            procedureToolbarController.ToggleProcedureToolbarStatus(false);

            //Act
            procedureToolbarController.Show();

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.IsTrue(procedureToolbarRoot.style.display == DisplayStyle.None);
        }

        [UnityTest]
        public IEnumerator Hide_WhenNotInitalized_ShouldNotHideProcedureToolbar()
        {
            //Arrange
            procedureToolbarController.ToggleProcedureToolbarStatus(false);
            procedureToolbarRoot.style.display = DisplayStyle.Flex;

            //Act
            procedureToolbarController.Hide();

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.IsTrue(procedureToolbarRoot.style.display == DisplayStyle.Flex);
        }

        [UnityTest]
        public IEnumerator OnPatientMonitorButtonClick_ShouldTriggerOnPOIClickEvent()
        {
            //Arrange
            bool listener = false;
            procedureToolbarController.OnPatientMonitorBtnClicked.AddListener(() => listener = true);

            //Act
            yield return TestUtils.ClickOnButton(patientMonitorButton);

            //Assert
            Assert.IsTrue(listener == true);
        }

        [UnityTest]
        public IEnumerator OnPatientViewButtonClick_ShouldTriggerOnPOIClickEvent()
        {
            //Arrange
            bool listener = false;
            procedureToolbarController.OnPatientViewBtnClicked.AddListener(() => listener = true);

            //Act
            yield return TestUtils.ClickOnButton(patientViewButton);

            //Assert
            Assert.IsTrue(listener == true);
        }

        [UnityTest]
        public IEnumerator OnBronchMonitorButtonClick_ShouldTriggerOnPOIClickEvent()
        {
            //Arrange
            bool listener = false;
            procedureToolbarController.OnBronchMonitorBtnClicked.AddListener(() => listener = true);

            //Act
            yield return TestUtils.ClickOnButton(bronchMonitorButton);

            //Assert
            Assert.IsTrue(listener == true);
        }

        [UnityTest]
        public IEnumerator OnBronchoscopeButtonClick_ShouldTriggerOnPOIClickEvent()
        {
            //Arrange
            bool listener = false;
            procedureToolbarController.OnBronchoscopeBtnClicked.AddListener(() => listener = true);

            //Act
            yield return TestUtils.ClickOnButton(bronchoscopeButton);

            //Assert
            Assert.IsTrue(listener == true);
        }

        [UnityTest]
        public IEnumerator OnProcedureTableButtonClick_ShouldTriggerOnPOIClickEvent()
        {
            //Arrange
            bool listener = false;
            procedureToolbarController.OnProcedureTableBtnClicked.AddListener(() => listener = true);

            //Act
            yield return TestUtils.ClickOnButton(procedureTableButton);

            //Assert
            Assert.IsTrue(listener == true);
        }
        
        [UnityTest]
        public IEnumerator OnPOIButtonClick_DifferenPOIAlreadyOpen_ShouldTriggerOnPOIClickEvent()
        {
            //Arrange
            bool listener = false;
            procedureToolbarController.OnFrontViewBtnClicked.AddListener(() => listener = true);

            //Act
            POIManager.Instance.HandlePOIOpened(patientMonitorPOI);
            yield return TestUtils.ClickOnButton(frontViewButton);

            //Assert
            Assert.IsTrue(listener == true);
        }

        [UnityTest]
        public IEnumerator OnPOIButtonClick_SamePOIAlreadyOpen_ShouldNotTriggerOnPOIClickEvent()
        {
            //Arrange
            bool listener = false;
            procedureToolbarController.OnPatientMonitorBtnClicked.AddListener(() => listener = true);

            //Act
            POIManager.Instance.HandlePOIOpened(patientMonitorPOI);
            yield return TestUtils.ClickOnButton(patientMonitorButton);

            //Assert
            Assert.IsTrue(listener == false);
        }

        [UnityTest]
        public IEnumerator OnFrontViewButtonClick_InDefaultView_ShouldNotTriggerOnPOIClickEvent()
        {
            //Arrange
            bool listener = false;
            procedureToolbarController.OnFrontViewBtnClicked.AddListener(() => listener = true);

            //Act
            yield return TestUtils.ClickOnButton(frontViewButton);

            //Assert
            Assert.IsTrue(listener == false);
        }
    }
}
