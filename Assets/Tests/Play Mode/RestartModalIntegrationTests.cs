using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEditor;
using NUnit.Framework;
using VARLab.Velcro;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class RestartModalIntegrationTests : MonoBehaviour
    {
        private int sceneCounter = 0;

        private GameObject restartModalObj;
        private GameObject uiManagerObj;

        private RestartModalController restartModal;
        private UIManager uiManager;

        private UIDocument restartModalDoc;
        private VisualElement rootContainer;

        [UnitySetUp]
        [Category("BuildServer")]
        public IEnumerator SetUp()
        {
            sceneCounter = TestUtils.ClearScene(sceneCounter, "RestartModalScene");

            //Set up <UIManager>
            uiManagerObj = new GameObject("UIManager");
            uiManager = uiManagerObj.AddComponent<UIManager>();

            //Set up <RestartModal> UI
            restartModalObj = new GameObject("RestartModal");
            restartModalDoc = restartModalObj.AddComponent<UIDocument>();

            //Load required assets from project files
            VisualTreeAsset restartModalUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Main Menu/RestartModal.uxml");
            PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Common/RT Panel Settings.asset");
            restartModalDoc.panelSettings = panelSettings;

            rootContainer = restartModalDoc.rootVisualElement;
            restartModalUXML.CloneTree(restartModalDoc.rootVisualElement);

            restartModal = restartModalObj.AddComponent<RestartModalController>();
            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            restartModalObj = null;
            uiManagerObj = null;

            restartModal = null;
            uiManager = null;

            restartModalDoc = null;
            rootContainer = null;

            UIManager.Instance = null;
        }

        [Test, Order(1)]
        [Category("BuildServer")]
        public void Show_ShouldSetDisplayFlex()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

            //Act
            restartModal.Show();

            //Assert
            Assert.AreEqual(expectedStyle, restartModalDoc.rootVisualElement.style.display);
        }

        [Test, Order(2)]
        [Category("BuildServer")]
        public void Hide_ShouldSetDisplayNone()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            //Act
            restartModal.Show();
            restartModal.Hide();

            //Assert
            Assert.AreEqual(expectedStyle, restartModalDoc.rootVisualElement.style.display);
        }

        [UnityTest]
        public IEnumerator RestartButton_ShouldTriggerOnInitializeMainMenu()
        {
            //Arrange
            bool eventTriggered = false;
            Button restartButton = rootContainer.Q<VisualElement>("RestartButtonContainer").Q<Button>();
            restartModal.OnInitializeMainMenu.AddListener(() => eventTriggered = true);

            //Act
            yield return TestUtils.ClickOnButton(restartButton);

            //Assert
            Assert.IsTrue(eventTriggered, "Restart button did not trigger OnInitializeMainMenu");
        }

        [UnityTest]
        public IEnumerator RestartButton_OrientationMode_ShouldTriggerOnOrientationRestart()
        {
            //Arrange
            bool eventTriggered = false;
            Button restartButton = rootContainer.Q<VisualElement>("RestartButtonContainer").Q<Button>();
            restartModal.OnOrientationRestart.AddListener(() => eventTriggered = true);
            uiManager.SetSimulationStatus(SimulationStatus.Orientation);

            //Act
            yield return TestUtils.ClickOnButton(restartButton);

            //Assert
            Assert.IsTrue(eventTriggered, "Restart button did not trigger OnOrientationRestart");
        }

        [UnityTest]
        public IEnumerator RestartButton_BiopsyMode_ShouldTriggerOnBiopsyRestart()
        {
            //Arrange
            bool eventTriggered = false;
            Button restartButton = rootContainer.Q<VisualElement>("RestartButtonContainer").Q<Button>();
            restartModal.OnBiopsyRestart.AddListener(() => eventTriggered = true);
            uiManager.SetSimulationStatus(SimulationStatus.Biopsy);

            //Act
            yield return TestUtils.ClickOnButton(restartButton);

            //Assert
            Assert.IsTrue(eventTriggered, "Restart button did not trigger OnBiopsyRestart");
        }

        [UnityTest]
        public IEnumerator CancelButton_ShouldSetDisplayNone()
        {
            //Arrange
            Button cancelButton = rootContainer.Q<VisualElement>("CancelButtonContainer").Q<Button>();
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            //Act
            yield return TestUtils.ClickOnButton(cancelButton);

            //Assert
            Assert.AreEqual(expectedStyle, restartModalDoc.rootVisualElement.style.display);
        }
    }
}