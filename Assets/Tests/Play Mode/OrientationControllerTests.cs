using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.Analytics;
using VARLab.RespiratoryTherapy;
using VARLab.Velcro;

namespace Tests.PlayMode
{
    public class OrientationControllerTests
    {
        //Path to scene
        private readonly string scenePath = "Assets/Scenes/OrientationControllerTestScene.unity";

        //Variable setup
        private OrientationController orientationController;
        private BiopsyController biopsyController;

        private ProgressIndicatorV1 progressBar;
        private UIDocument uIDocument;
        private VisualElement root;
        private VisualElement checklist;

        private GameObject testExplorable;

        private GameObject conclusionPanel;

        private UIManager uiManager;

        private AnalyticsManager analyticsManager;

        /// <summary>
        /// Runs before every test, sets up the scene and variables
        /// </summary>
        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            //Wait for scene to load
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath,
                new LoadSceneParameters(LoadSceneMode.Single));

            //Manually set analytics references - this is necessary because the CoreAnalytics.Initalize() method is called before the scene is fully loaded
            analyticsManager = Object.FindAnyObjectByType<AnalyticsManager>();
            analyticsManager.SetUpReferences();
            CoreAnalytics.SetAnalyticsManager(analyticsManager);
            
            //Assign everything
            orientationController = Object.FindAnyObjectByType<OrientationController>();
            biopsyController = Object.FindAnyObjectByType<BiopsyController>();

            progressBar = Object.FindAnyObjectByType<ProgressIndicatorV1>();
            orientationController.medicalCart = GameObject.Find("MedicalCart");
            orientationController.bronchTower = GameObject.Find("BronchTower");

            uIDocument = progressBar.GetComponent<UIDocument>();

            testExplorable = Object.FindFirstObjectByType<Explorable>().gameObject;
            conclusionPanel = Object.FindFirstObjectByType<MessageDisplayPanelController>().gameObject;

            uiManager = UIManager.Instance;

            root = uIDocument.rootVisualElement;
            checklist = root.Q<VisualElement>("ProgressIndicator");
        }

        /// <summary>
        /// Runs after every test, reset the scene and necessary variables
        /// </summary>
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return SceneManager.UnloadSceneAsync(scenePath);
            orientationController = null;
            progressBar = null;
            root = null;
            checklist = null;
            testExplorable = null;
            analyticsManager = null;
            Object.Destroy(uiManager);
        }


        [UnityTest]
        public IEnumerator InitializeOrientation_With_WelcomeScreen_False_Should_Start_Orientation()
        {
            //Arrange          
            orientationController.ShowWelcome = false;
        
            //Act
            orientationController.InitializeOrientation();
            yield return null;
        
            //Assert
            Assert.IsTrue(uiManager.MessageDisplayPanelController.GetComponent<UIDocument>()
                .rootVisualElement.style.display == DisplayStyle.None);
        }

        [UnityTest]
        public IEnumerator InitializeOrientation_With_WelcomeScreen_True_Should_Show_welcome_Screen()
        {
            //Arrange          
            orientationController.ShowWelcome = true;
            orientationController.SetOrientationPlayStatus(true);

            //Act
            orientationController.InitializeOrientation();
            yield return null;

            //Assert
            Assert.IsTrue(uiManager.MessageDisplayPanelController.GetComponent<UIDocument>()
                .rootVisualElement.style.display == DisplayStyle.Flex);
        }

        /// <summary>
        /// Test ensures the correct error is thrown when passing in a task that doesnt exist to UpdateProgress()
        /// </summary>
        [UnityTest]
        public IEnumerator OrientationController_AddProgress_ErrorOnInvalidTask()
        {
            //Arrange
            string expectedResult = $"Could not find the task: Test";
            LogAssert.Expect(LogType.Error, expectedResult);

            //Act
            progressBar.AddCategory(NameFormatter.ToDescription(ExplorableCategory.BronchTower));
            orientationController.GetComponent<OrientationController>()
                .UpdateProgress(ExplorableCategory.BronchTower, "Test", 1);
            yield return null;

            //Assert
            //LogExpect needs to be before function call
        }

        /// <summary>
        /// Test ensures the correct error is thrown when passing in a number below 1 to UpdateProgress()
        /// </summary>
        [UnityTest]
        public IEnumerator OrientationController_AddProgress_ErrorOnNegativeProgress()
        {
            //Arrange
            string expectedResult = $"Must add at least 1 point of progression";
            LogAssert.Expect(LogType.Error, expectedResult);

            //Act
            progressBar.AddCategory(NameFormatter.ToDescription(ExplorableCategory.BronchTower));
            progressBar.AddTask(0, "Test", 1);
            orientationController.GetComponent<OrientationController>()
                .UpdateProgress(ExplorableCategory.BronchTower, "Test", -1);
            yield return null;

            //Assert
            //LogExpect needs to be before function call
        }

        [UnityTest]
        public IEnumerator OrientationController_CheckIfFinished_FinishedOrientationConclusionPanelAppears()
        {
            // Get their GameObjects
            List<GameObject> allExplorableGameObjects = new();
            Explorable[] allExplorables = Object.FindObjectsByType<Explorable>(FindObjectsSortMode.None);

            foreach (var exp in allExplorables)
            {
                allExplorableGameObjects.Add(exp.gameObject);
            }

            //Act
            foreach (GameObject gameobject in allExplorableGameObjects)
            {
                if (gameobject!=null)
                    gameobject.GetComponent<Explorable>().IsExplored = true;

            }
            orientationController.GetComponent<OrientationController>().CheckForOrientationCompletion();

            //Assert
            Assert.IsTrue(conclusionPanel.GetComponent<UIDocument>().rootVisualElement.style.display.value ==
                          DisplayStyle.Flex);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OrientationController_ShowOrientationChecklist_ChecklistIsShown()
        {
            orientationController.GetComponent<OrientationController>().Hide();
            yield return null;

            orientationController.GetComponent<OrientationController>().Show();
            yield return null;

            Assert.IsTrue(checklist.style.display.value == DisplayStyle.Flex);
        }

        [UnityTest]
        public IEnumerator OrientationController_HideOrientationChecklist_ChecklistIsHidden()
        {
            orientationController.GetComponent<OrientationController>().Show();
            yield return null;

            orientationController.GetComponent<OrientationController>().Hide();
            yield return null;

            Assert.IsTrue(checklist.style.display.value == DisplayStyle.None);
        }

        [UnityTest]
        public IEnumerator HandleCategoryFocused_ShouldCollapseOtherCategories()
        {
            //Arrange  
            orientationController.SetOrientationPlayStatus(true);
            orientationController.InitializeOrientation();
            List<Button> otherCategoryArrows = root.Query<Button>("ArrowHolder").ToList();
            List<VisualElement> otherCategoryTaskHolders = new List<VisualElement>();
            Button currentCategoryArrow = otherCategoryArrows[0];

            otherCategoryArrows.Remove(currentCategoryArrow);
            otherCategoryArrows.ForEach(arrow =>
                otherCategoryTaskHolders.Add(arrow.parent.parent.Q<VisualElement>("TaskHolder")));

            //Act
            orientationController.HandleCategoryFocused("Medications");
            yield return null;

            //Assert
            Assert.IsTrue(otherCategoryTaskHolders.All(taskHolder => taskHolder.style.display == DisplayStyle.None));
        }

        [UnityTest]
        public IEnumerator ToggleOrientationModuleItems_EnablesGameObjects_WhenInOrientation()
        {
            orientationController.medicalCart.SetActive(false);
            orientationController.bronchTower.SetActive(false);
            uiManager.SetSimulationStatus(SimulationStatus.Orientation);

            orientationController.ToggleOrientationModuleItems();
            yield return null;

            Assert.IsTrue(orientationController.medicalCart.activeSelf, "MedicalCart should be active.");
            Assert.IsTrue(orientationController.bronchTower.activeSelf, "BronchTower should be active.");
        }

        [UnityTest]
        public IEnumerator ToggleOrientationModuleItems_DisablesGameObjects_WhenInBiopsy()
        {
            orientationController.medicalCart.SetActive(true);
            orientationController.bronchTower.SetActive(true);
            uiManager.SetSimulationStatus(SimulationStatus.Biopsy);

            orientationController.ToggleOrientationModuleItems();
            yield return null;

            Assert.IsFalse(orientationController.medicalCart.activeSelf, "MedicalCart should be inactive.");
            Assert.IsFalse(orientationController.bronchTower.activeSelf, "BronchTower should be inactive.");
        }

        [UnityTest]
        public IEnumerator ShowLoadingScreen_EventTriggered_WhenChangingToOrientation()
        {
            //arrange - ensure simulation status starts as Biopsy
            uiManager.SetSimulationStatus(SimulationStatus.Biopsy);
            bool loadingEventTriggered = false;
            uiManager.ShowLoadingScreen.AddListener(obj => loadingEventTriggered = true);
            orientationController.SetOrientationPlayStatus(false);
            //act - switch to Orientation
            orientationController.InitializeOrientation();
            yield return null;

            //assert event was invoked
            Assert.IsTrue(loadingEventTriggered);
        }
        
        [UnityTest]
        public IEnumerator ShowLoadingScreen_EventNotTriggered_WhenStayingInOrientation()
        {
            //arrange - ensure simulation status starts as orientation
            uiManager.SetSimulationStatus(SimulationStatus.Orientation);
            bool loadingEventTriggered = false;
            uiManager.ShowLoadingScreen.AddListener(obj => loadingEventTriggered = true);

            //act - Switch to Orientation
            orientationController.SetOrientationPlayStatus(true);
            orientationController.InitializeOrientation();
            yield return null;

            //assert - event was Not invoked
            Assert.IsFalse(loadingEventTriggered);
        }
    }
}