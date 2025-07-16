using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;
using Object = UnityEngine.Object;

namespace Tests.PlayMode
{
    public class ToolbarIntegrationTests
    {
        private GameObject toolbarGameObject;
        private ToolbarController toolbarController;
        private UIDocument toolbarUIDocument;
        
        private Button backButton;
        private Button homeButton;
        
        [SetUp]
        [Category("BuildServer")]
        public void SetUp()
        {
            //Set up tool bar
            toolbarGameObject = new GameObject();
            toolbarUIDocument = toolbarGameObject.AddComponent<UIDocument>();
            VisualTreeAsset toolbarUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Toolbars/Toolbar.uxml");
            toolbarUXML.CloneTree(toolbarUIDocument.rootVisualElement);
            toolbarController = toolbarGameObject.AddComponent<ToolbarController>();
            
            homeButton = toolbarUIDocument.rootVisualElement.Q<Button>("HomeButton");
            backButton = toolbarUIDocument.rootVisualElement.Q<Button>("HandbookButton");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            Object.Destroy(toolbarGameObject);
        }
        
        [UnityTest]
        public IEnumerator ResetToolbar_WhenCalled_Should_Reset_Toolbar()
        {
            //Arrange
            ToolbarConfig config = new ToolbarConfig()
            {
                ShowMenuButton = true,
                ShowHandbookButton = true
            };
            
            // Act
            toolbarController.SetToolBarUI(config);
            toolbarController.ResetToolbar();

            // Assert:
            yield return null; // Wait for one frame

            Assert.IsTrue(homeButton.style.display == DisplayStyle.None);
            Assert.IsTrue(backButton.style.display == DisplayStyle.None);
        }
        
        [UnityTest]
        public IEnumerator SetToolBarUI_WhenCalled_Should_Set_Toolbar()
        {
            //Arrange
            ToolbarConfig config = new ToolbarConfig()
            {
                ShowMenuButton = true,
                ShowHandbookButton = true
            };
            
            // Act
            toolbarController.SetToolBarUI(config);

            // Assert:
            yield return null; // Wait for one frame

            Assert.IsTrue(homeButton.style.display == DisplayStyle.Flex);
            Assert.IsTrue(backButton.style.display == DisplayStyle.Flex);
        }
        
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Show_WhenCalled_Should_Display_Toolbar()
        {
            // Act
            toolbarController.Show();

            // Assert:
            yield return null; // Wait for one frame

            Assert.IsTrue(toolbarUIDocument.rootVisualElement.style.display == DisplayStyle.Flex);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Hide__WhenCalled_Should_Hides_Toolbar()
        {
            // Act
            toolbarController.Hide();

            // Assert
            yield return null; // Wait for one frame

            Assert.IsTrue(toolbarUIDocument.rootVisualElement.style.display == DisplayStyle.None);
        }

        private void HomeButtonCallback()
        {
            Debug.Log("HomeButtonCallback");
        }
        
        private void BackButtonCallback()
        {
            Debug.Log("BackButtonCallback");
        }
        
    }
}
