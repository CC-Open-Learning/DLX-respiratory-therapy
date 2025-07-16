using UnityEngine;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class MainMenuControllerIntegrationTests 
    {
        private GameObject mainMenuGameObject;
        private MainMenuController mainMenuController;
        private UIDocument uiDocument;
        private UIDocument mainmUIDocument;
        private LearnerSessionHandler learnerSessionHandler;
        private UIManager uiManager;
        private VisualElement rootContainer;

        [SetUp]
        [Category("BuildServer")]
        public void SetUp()
        {
            //Set up main menu
            mainMenuGameObject = new GameObject();
            mainmUIDocument = mainMenuGameObject.AddComponent<UIDocument>();
            VisualTreeAsset MainMenuUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Main Menu/MainMenu.uxml");
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Common/RT Panel Settings.asset");
            mainmUIDocument.visualTreeAsset = MainMenuUXML;
            mainmUIDocument.panelSettings = panelSettings;
            rootContainer = mainmUIDocument.rootVisualElement;
            learnerSessionHandler = mainMenuGameObject.AddComponent<LearnerSessionHandler>();
            mainMenuController = mainMenuGameObject.AddComponent<MainMenuController>();
            mainMenuGameObject.AddComponent<UIManager>();
            uiManager = UIManager.Instance;
            mainMenuController.LearnerSessionHandler = learnerSessionHandler;
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            Object.Destroy(mainMenuGameObject);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Show_WhenCalled_Should_Display_MainMenu()
        {
            // Act
            mainMenuController.Show();

            // Assert:
            yield return null; // Wait for one frame

            Assert.IsTrue(mainmUIDocument.rootVisualElement.style.display == DisplayStyle.Flex);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Hide__WhenCalled_Should_Hide_MainMenu()
        {
            // Act
            mainMenuController.Hide();

            // Assert
            yield return null; // Wait for one frame

            Assert.IsTrue(mainmUIDocument.rootVisualElement.style.display == DisplayStyle.None);
        }
    
        [UnityTest]
        public IEnumerator RestartButtonTriggersModalDisplay()
        {
            // Arrange
            bool eventTriggered = false;
            VisualElement reStartButtonVE = rootContainer.Q<VisualElement>("Button-Restart");//Button-Continue//Button-Continue
            Button reStartButton = reStartButtonVE.Q<Button>();
            mainMenuController.LearnerSessionHandler.SetUserStatus(true);
            mainMenuController.OnRestartModalDisplay.AddListener(() => eventTriggered = true);
            // Act
            mainMenuController.InitializeMainMenu();
            yield return TestUtils.ClickOnButton(reStartButton);
            // Assert
            Assert.IsTrue(eventTriggered, "Restart button did not trigger OnRestartModalDisplay");
        }
        
        [UnityTest]
        public IEnumerator SettingsButtonTriggersSettingsDisplay()
        {
            // Arrange
            bool eventTriggered = false;
            VisualElement settingsButtonVE = rootContainer.Q<VisualElement>("Button-Settings");
            Button settingsButton = settingsButtonVE.Q<Button>();
            mainMenuController.LearnerSessionHandler.SetUserStatus(true);
            mainMenuController.OnSettingsPanelDisplay.AddListener(() => eventTriggered = true);
            // Act
            mainMenuController.InitializeMainMenu();
            yield return TestUtils.ClickOnButton(settingsButton);
            // Assert
            Assert.IsTrue(eventTriggered, "Settings button did not trigger OnSettingsPanelDisply");
        }
        
        [UnityTest]
         public IEnumerator ContinueButtonTriggersBiopsyContinue_When_SimulationStatus_Biopsy()
         {
             // Arrange
             bool eventTriggered = false;
             VisualElement continueButtonVE = rootContainer.Q<VisualElement>("Button-Continue");
             Button continueButton = continueButtonVE.Q<Button>();
             uiManager.simulationStatus = SimulationStatus.Biopsy;
             mainMenuController.LearnerSessionHandler.SetUserStatus(true);
             mainMenuController.OnBiopsyContinue.AddListener(() => eventTriggered = true);
             // Act
             mainMenuController.InitializeMainMenu();
             yield return TestUtils.ClickOnButton(continueButton);
             // Assert
             Assert.IsTrue(eventTriggered, "Continue button did not trigger OnBiopsyContinue with false");
         }
        
        [UnityTest]
        public IEnumerator ContinueButtonTriggersOrientationContinue_When_SimulationStatus_Orientation()
        {
            // Arrange
            bool eventTriggered = false;
            VisualElement continueButtonVE = rootContainer.Q<VisualElement>("Button-Continue");
            Button continueButton = continueButtonVE.Q<Button>();
            uiManager.simulationStatus = SimulationStatus.Orientation;
            mainMenuController.LearnerSessionHandler.SetUserStatus(true);
            mainMenuController.OnOrientationContinue.AddListener(() => eventTriggered = true);
            // Act
            mainMenuController.InitializeMainMenu();
            yield return TestUtils.ClickOnButton(continueButton);
            // Assert
            Assert.IsTrue(eventTriggered, "Continue button did not trigger OnOrientationContinue with false");
        }
    }
}
