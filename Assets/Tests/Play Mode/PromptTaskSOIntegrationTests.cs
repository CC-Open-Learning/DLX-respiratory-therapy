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
    public class PromptSOIntegrationTests
    {
        private int sceneCounter = 0;

        private GameObject promptObj;
        private PromptTaskSO promptTask;

        private UIDocument promptDoc;
        private Prompt prompt;// I just need this to exist so prompttaskSO can grab it
        private PromptSO promptSO;

        [UnitySetUp]
        [Category("BuildServer")]
        public IEnumerator SetUp()
        {
            sceneCounter = TestUtils.ClearScene(sceneCounter, "PromptScene");
            promptTask = new PromptTaskSO();
            //Set up <Modal> UI
            promptObj = new GameObject("Prompt");
            promptDoc = promptObj.AddComponent<UIDocument>();
            prompt = promptObj.AddComponent<Prompt>();

            //Load required assets from project files
            VisualTreeAsset promptUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Prompts/Prompt.uxml");
            PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Common/RT Panel Settings.asset");
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/Prompt/PromptNPC_1_Sprite.png");

            promptSO = ScriptableObject.CreateInstance<PromptSO>();
            promptSO.Speaker = "Speaker";
            promptSO.Message = "Message";
            promptSO.Image = sprite;
            promptSO.BarColour = Color.red;
            promptSO.ImageBorderColour = Color.blue;
            promptSO.IsBtnEnabled = true;

            promptTask.Name = "Test";
            promptTask.PromptSO = promptSO;

            promptUXML.CloneTree(promptDoc.rootVisualElement);
            yield return null;
        }
        [Test, Order(1)]
        [Category("BuildServer")]
        public void HandleDisplayUI_WithSO_ShouldPopulateContent()
        {
            //Arrange
            string expectedSpeaker = "Speaker";
            string expectedMessage = "Message";
            StyleColor expectedBarColour = new StyleColor(Color.red);
            StyleColor expectedBorderColour = new StyleColor(Color.blue);

            //Act
            promptTask.Execute();

            //Assert
            Assert.AreEqual(expectedSpeaker, promptDoc.rootVisualElement.Q<Label>("SpeakerLabel").text);
            Assert.AreEqual(expectedMessage, promptDoc.rootVisualElement.Q<Label>("MessageLabel").text);
            Assert.AreEqual(expectedBarColour, promptDoc.rootVisualElement.Q<VisualElement>("Bar").style.backgroundColor);
            Assert.AreEqual(expectedBorderColour, promptDoc.rootVisualElement.Q<VisualElement>("ImageContainer").style.borderBottomColor);
            Assert.AreEqual(expectedBorderColour, promptDoc.rootVisualElement.Q<VisualElement>("ImageContainer").style.borderTopColor);
            Assert.AreEqual(expectedBorderColour, promptDoc.rootVisualElement.Q<VisualElement>("ImageContainer").style.borderLeftColor);
            Assert.AreEqual(expectedBorderColour, promptDoc.rootVisualElement.Q<VisualElement>("ImageContainer").style.borderRightColor);
        }

    }
}
