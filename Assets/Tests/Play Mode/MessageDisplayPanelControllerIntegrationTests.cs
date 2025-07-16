using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;
using MessageType = VARLab.RespiratoryTherapy.MessageType;

namespace Tests.PlayMode
{
    public class MessageDisplayPanelControllerIntegrationTests
    {
        private GameObject messageDisplayPanelGameObject;
        private UIDocument messageDisplayPanelUIDocument;
        private MessageDisplayPanelController messageDisplayPanelController;

        [SetUp]
        [Category("BuildServer")]
        public void SetUp()
        {
            messageDisplayPanelGameObject = new GameObject();
            messageDisplayPanelUIDocument = messageDisplayPanelGameObject.AddComponent<UIDocument>();
            VisualTreeAsset messagePanelUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/MessageDisplayPanel.uxml");
            messagePanelUXML.CloneTree(messageDisplayPanelUIDocument.rootVisualElement);
            messageDisplayPanelController = messageDisplayPanelGameObject.AddComponent<MessageDisplayPanelController>();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            Object.Destroy(messageDisplayPanelGameObject);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Show_WhenCalled_Should_Display_WelcomeScreen()
        {
            // Act
            messageDisplayPanelController.Show();

            // Assert
            yield return null; // Wait for one frame

            Assert.IsTrue(messageDisplayPanelUIDocument.rootVisualElement.style.display== DisplayStyle.Flex);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Hide_WhenCalled_Should_Hides_WelcomeScreen()
        {
            // Act
            messageDisplayPanelController.Hide();

            // Assert
            yield return null; // Wait for one frame

            Assert.IsTrue(messageDisplayPanelUIDocument.rootVisualElement.style.display== DisplayStyle.None);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator SetContent_WhenCalled_Should_SetsTitleAndMessage()
        {
            // Arrange
            PanelContent panelContent = new ()
            {
                Header = "Warning",
                Message = "",
                Title = "No content found",    
                ButtonText = "OK"
            };
            
        
            // Act
            messageDisplayPanelController.SetContent(MessageType.Welcome, panelContent);
        
            // Assert
            yield return null; // Wait for one frame
        
            var titleLabel = messageDisplayPanelUIDocument.rootVisualElement.Q<Label>("Title");
            var messageLabel = messageDisplayPanelUIDocument.rootVisualElement.Q<Label>("Message");
        
            Assert.AreEqual(panelContent.Title, titleLabel.text);
            Assert.AreEqual(panelContent.Message, messageLabel.text);
        }
    }
}
