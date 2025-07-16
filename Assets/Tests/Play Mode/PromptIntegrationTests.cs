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
    public class PromptIntegrationTests
    {
        private int sceneCounter = 0;

        private GameObject promptObj;
        private UIDocument promptDoc;
        private Prompt prompt;
        private PromptSO promptSO;

        [UnitySetUp]
        [Category("BuildServer")]
        public IEnumerator SetUp()
        {
            sceneCounter = TestUtils.ClearScene(sceneCounter, "PromptScene");

            //Set up <Modal> UI
            promptObj = new GameObject("Prompt");
            promptDoc = promptObj.AddComponent<UIDocument>();
            prompt = promptObj.AddComponent<Prompt>();

            //Load required assets from project files
            VisualTreeAsset promptUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Prompts/Prompt.uxml");
            PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Common/RT Panel Settings.asset");
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/Prompt/PromptNPC_1_Sprite.png");

            //Reference panel settings and source asset as SerializedFields
            SerializedObject so = new SerializedObject(promptDoc);
            so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
            so.FindProperty("sourceAsset").objectReferenceValue = promptUXML;
            so.ApplyModifiedProperties();

            promptSO = ScriptableObject.CreateInstance<PromptSO>();
            promptSO.Speaker = "Speaker";
            promptSO.Message = "Message";
            promptSO.Image = sprite;
            promptSO.BarColour = Color.red;
            promptSO.ImageBorderColour = Color.blue;
            promptSO.IsBtnEnabled = true;

            promptUXML.CloneTree(promptDoc.rootVisualElement);
            yield return null;
        }

        [Test, Order(1)]
        [Category("BuildServer")]
        public void Start_SetsRootToDisplayNone()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            //Assert
            Assert.AreEqual(expectedStyle, promptDoc.rootVisualElement.style.display);
        }

        [Test, Order(2)]
        [Category("BuildServer")]
        public void HandleDisplayUI_SetsRootToDisplayFlex()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

            //Act
            prompt.HandleDisplayUI(promptSO);

            //Assert
            Assert.AreEqual(expectedStyle, promptDoc.rootVisualElement.style.display);
        }

        [Test, Order(3)]
        [Category("BuildServer")]
        public void HandleDisplayUI_WithSO_ShouldPopulateContent()
        {
            //Arrange
            string expectedSpeaker = "Speaker";
            string expectedMessage = "Message";
            StyleColor expectedBarColour = new StyleColor(Color.red);
            StyleColor expectedBorderColour = new StyleColor(Color.blue);

            //Act
            prompt.HandleDisplayUI(promptSO);

            //Assert
            Assert.AreEqual(expectedSpeaker, promptDoc.rootVisualElement.Q<Label>("SpeakerLabel").text);
            Assert.AreEqual(expectedMessage, promptDoc.rootVisualElement.Q<Label>("MessageLabel").text);
            Assert.AreEqual(expectedBarColour, promptDoc.rootVisualElement.Q<VisualElement>("Bar").style.backgroundColor);
            Assert.AreEqual(expectedBorderColour, promptDoc.rootVisualElement.Q<VisualElement>("ImageContainer").style.borderBottomColor);
            Assert.AreEqual(expectedBorderColour, promptDoc.rootVisualElement.Q<VisualElement>("ImageContainer").style.borderTopColor);
            Assert.AreEqual(expectedBorderColour, promptDoc.rootVisualElement.Q<VisualElement>("ImageContainer").style.borderLeftColor);
            Assert.AreEqual(expectedBorderColour, promptDoc.rootVisualElement.Q<VisualElement>("ImageContainer").style.borderRightColor);
        }

        [Test, Order(4)]
        [Category("BuildServer")]
        public void SetContent_WithBtnEnabledFalse_SetsDisplayNone()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            PromptSO tempPromptSO = promptSO;
            tempPromptSO.IsBtnEnabled = false;

            //Act
            prompt.SetContent(tempPromptSO);

            //Assert
            Assert.AreEqual(expectedStyle, promptDoc.rootVisualElement.Q<Button>("CloseBtn").style.display);
        }

        [Test, Order(5)]
        [Category("BuildServer")]
        public void SetContent_WithBtnEnabledTrue_SetsDisplayFlex()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            PromptSO tempPromptSO = promptSO;
            tempPromptSO.IsBtnEnabled = true;

            //Act
            prompt.SetContent(tempPromptSO);

            //Assert
            Assert.AreEqual(expectedStyle, promptDoc.rootVisualElement.Q<Button>("CloseBtn").style.display);
        }

        [Test, Order(6)]
        [Category("BuildServer")]
        public void Show_SetsDisplayFlex()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

            //Act
            prompt.Show();

            //Assert
            Assert.AreEqual(expectedStyle, promptDoc.rootVisualElement.style.display);
        }

        [Test, Order(7)]
        [Category("BuildServer")]
        public void Hide_SetsDisplayNone()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            //Act
            prompt.Show();
            prompt.Hide();

            //Assert
            Assert.AreEqual(expectedStyle, promptDoc.rootVisualElement.style.display);
        }
    }
}
