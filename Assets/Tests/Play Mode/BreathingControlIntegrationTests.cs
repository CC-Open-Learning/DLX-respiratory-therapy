using UnityEditor;
using UnityEngine;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class BreathingControlIntegrationTests 
    {
        private GameObject testObject;
        private UIDocument uiDocument;
        private BreathingControl breathingControl;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            
            var root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/UI Toolkit/Templates/Patient Monitor/PatientMonitorControl.uxml");
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(
                "Assets/UI Toolkit/Common/RT Panel Settings.asset");
            
            // Create GameObject with UIDocument and BreathingControl
            testObject = new GameObject("BreathingControlTestObject");
            uiDocument = testObject.AddComponent<UIDocument>();
            uiDocument.panelSettings = panelSettings;   
            uiDocument.visualTreeAsset = root; 
            // Add BreathingControl component
            breathingControl = testObject.AddComponent<BreathingControl>();
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator ProceedButtonClick_InvokesOnProceedBtnClicked()
        {
            //Arrange
            bool wasCalled = false;
            breathingControl.OnProceedBtnClicked.AddListener(() => wasCalled = true);

            // Act
            Button button = breathingControl.Root.Q<Button>("ProceedButton");
            yield return TestUtils.ClickOnButton(button);
            
            //Assert
            Assert.IsTrue(wasCalled);
        }

        [UnityTest]
        public IEnumerator PauseButtonClick_InvokesOnPauseBtnClicked()
        {
            //Arrange
            bool wasCalled = false;
            breathingControl.OnPauseBtnClicked.AddListener(() => wasCalled = true);

            // Act
            Button button = breathingControl.Root.Q<Button>("PauseButton");
            yield return TestUtils.ClickOnButton(button);
            
            //Assert
            Assert.IsTrue(wasCalled);
        }

        [UnityTest]
        public IEnumerator Start_SetsBreathingLabelTextCorrectly()
        {
            //Arrange
            string labelText = "PatientMonitorLabel";
            
            //Act
            var label = breathingControl.Root.Q<Label>(labelText);
            yield return null;
            
            //Assert
            Assert.AreEqual("Proceed if breathing is normal", label.text);
        }

        [UnityTest]
        public IEnumerator Show_Should_Open_Breathing_Panel()
        {
            //Arrange
            var expectedResult = (StyleEnum<DisplayStyle>)DisplayStyle.Flex;
            
            //Act
            breathingControl.Show();
            yield return null;
            
            //Assert
            Assert.AreEqual(expectedResult, breathingControl.Root.style.display);
        }
        
        [UnityTest]
        public IEnumerator Hide_Should_Close_Breathing_Panel()
        {
            //Arrange
            var expectedResult = (StyleEnum<DisplayStyle>)DisplayStyle.None;
            
            //Act
            breathingControl.Hide();
            yield return null;
            
            //Assert
            Assert.AreEqual(expectedResult, breathingControl.Root.style.display);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Object.Destroy(testObject);
            yield return null;
        }
    }
}
