using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class UIManagerIntegrationTests
    {
        private UIManager uiManager;
        private GameObject mainMenuGameObject;
        private GameObject uiManagerGameObject;
        private GameObject welcomeScreenGameObject;
        private UIDocument mainmUIDocument;
        private UIDocument welcomeScreenUIDocument;
        private LearnerSessionHandler learnerSessionHandler;

        private VisualElement rootContainer;
        private MainMenuController mockmMainMenuController;
        private MessageDisplayPanelController _mockMessageDisplayPanelController;

        [SetUp]
        [Category("BuildServer")]
        public void SetUp()
        {
            // Create a new GameObject and add the UIManager component
            uiManagerGameObject = new GameObject();
            uiManager = uiManagerGameObject.AddComponent<UIManager>();
            //Set up welcome screen
            welcomeScreenGameObject = new GameObject();
            welcomeScreenUIDocument = welcomeScreenGameObject.AddComponent<UIDocument>();
            VisualTreeAsset WelcomeScreenUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/MessageDisplayPanel.uxml");
            WelcomeScreenUXML.CloneTree(welcomeScreenUIDocument.rootVisualElement);
            _mockMessageDisplayPanelController = welcomeScreenGameObject.AddComponent<MessageDisplayPanelController>();
            
            //Set up main menu
            mainMenuGameObject = new GameObject();
            mainmUIDocument = mainMenuGameObject.AddComponent<UIDocument>();
            VisualTreeAsset MainMenuUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Main Menu/MainMenu.uxml");
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Common/RT Panel Settings.asset");
            mainmUIDocument.visualTreeAsset = MainMenuUXML;
            mainmUIDocument.panelSettings = panelSettings;
            rootContainer = mainmUIDocument.rootVisualElement;
            mockmMainMenuController = mainMenuGameObject.AddComponent<MainMenuController>();
            
            learnerSessionHandler = mainMenuGameObject.AddComponent<LearnerSessionHandler>();
            mockmMainMenuController.LearnerSessionHandler = learnerSessionHandler;
            
            //Set UImanager as parent
            welcomeScreenGameObject.transform.SetParent(uiManagerGameObject.transform);
            mainMenuGameObject.transform.SetParent(uiManagerGameObject.transform);
            
            // Assign the controllers to the UIManager instance
            uiManager.MessageDisplayPanelController = _mockMessageDisplayPanelController;
            uiManager.MainMenuController = mockmMainMenuController;

            // Create mock UIPanelDataSO and assign it
            UIPanelDataSO mockUIPanelData = ScriptableObject.CreateInstance<UIPanelDataSO>();
            mockUIPanelData.PanelContents = new List<PanelContent>
            {
                new PanelContent { Type = ContentInfo.SimulationWelcome, Title = "Welcome", Message = "Welcome to the simulation!" },
                new PanelContent { Type = ContentInfo.SimulationWelcome, Title = "Main Menu", Message = "Select an option." }
            };
            uiManager.UIPanelData = mockUIPanelData;

            // Initialize the Singleton instance
            UIManager.Instance = uiManager;
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            Object.Destroy(uiManagerGameObject);
        }

        //TODO: Need to re write ethe test for the Initialize method as we have a new main menu.
        // [UnityTest]
        // [Category("BuildServer")]
        // public IEnumerator Initialize_WithWelcomeMessage_Shows_WelcomeScreen()
        // {
        //     // Arrange
        //     uiManager.ShowWelcome = true;
        //     
        //     // Act
        //     uiManager.Initialize();
        //     
        //     // Assert
        //     yield return null; // Wait for one frame
        //
        //     Assert.IsTrue(welcomeScreenUIDocument.rootVisualElement.style.display == DisplayStyle.Flex);
        // }

        
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Initialize_WithoutWelcomeMessage_Shows_MainMenuScreen()
        {
            // Arrange
            uiManager.ShowWelcome = false;
            
            // Act
            uiManager.Initialize();
            
            // Assert
            yield return null; // Wait for one frame
        
            Assert.IsTrue(rootContainer.style.display == DisplayStyle.Flex);
        }
        
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator ShowScreen_When_Pass_MainMenu_Should_Shows_MainMenuScreen()
        {
            // Arrange
            UIPanel panel = UIPanel.MainMenu;
            
            // Act
            uiManager.ShowScreen(panel);
            
            // Assert
            yield return null; // Wait for one frame
        
            Assert.IsTrue(mainmUIDocument.rootVisualElement.style.display == DisplayStyle.Flex);
        }
        
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator HideScreen_When_Pass_MainMenu_Should_Hide_MainMenuScreen()
        {
            // Arrange
            UIPanel panel = UIPanel.MainMenu;
            
            // Act
            uiManager.HideScreen(panel);
            
            // Assert
            yield return null; // Wait for one frame
        
            Assert.IsTrue(mainmUIDocument.rootVisualElement.style.display == DisplayStyle.None);
        }
        
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator ShowMainMenu_Should_Show_MainMenuScreen()
        {
            // Act
            uiManager.ShowMainMenu();
            
            // Assert
            yield return null; // Wait for one frame
        
            Assert.IsTrue(mainmUIDocument.rootVisualElement.style.display == DisplayStyle.Flex);
        }
        
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator SetSimulationStatus_When_Pass_Orientation_Should_SetSimulationStatus_To_Orientation()
        {
            //Arrange
            SimulationStatus status = SimulationStatus.Orientation;
            
            // Act
            uiManager.SetSimulationStatus(status);
            
            // Assert
            yield return null; // Wait for one frame
        
            Assert.IsTrue(uiManager.simulationStatus == status);
        }
    }
}
