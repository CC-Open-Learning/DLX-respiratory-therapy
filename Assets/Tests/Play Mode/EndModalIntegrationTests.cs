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
    public class EndModalIntegrationTests : MonoBehaviour
    {
        private int sceneCounter = 0;
        private UIDocument modalDoc;
        private EndModal modal;
        private EndModalSO modalSO;

        [UnitySetUp]
        [Category("BuildServer")]
        public IEnumerator SetUp()
        {
            sceneCounter = TestUtils.ClearScene(sceneCounter, "ModalScene");
            GameObject modalObj;

            //Set up <Modal> UI
            modalObj = new GameObject("EndModal");
            modalDoc = modalObj.AddComponent<UIDocument>();
            modal = modalObj.AddComponent<EndModal>();

            //Load required assets from project files
            VisualTreeAsset modalUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Modals/EndModal.uxml");
            PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Common/RT Panel Settings.asset");
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/Modals/ModalNPC_1_Sprite.png");

            //Reference panel settings and source asset as SerializedFields
            SerializedObject so = new SerializedObject(modalDoc);
            so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
            so.FindProperty("sourceAsset").objectReferenceValue = modalUXML;
            so.ApplyModifiedProperties();

            modalSO = ScriptableObject.CreateInstance<EndModalSO>();
            modalSO.Name = "Name";
            modalSO.Title = "Title";
            modalSO.Description = "Description";
            modalSO.NextModuleButtonText = "Button";
            modalSO.ReturnToMenuButtonText = "Button";
            modalSO.LineSeparator = null;

            modalSO.Image = sprite;
            modalSO.IsCanvasDimmed = true;

            modalUXML.CloneTree(modalDoc.rootVisualElement);
            yield return null;
        }

        [Test, Order(1)]
        [Category("BuildServer")]
        public void Start_SetsRootToDisplayNone()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            //Assert
            Assert.AreEqual(expectedStyle, modalDoc.rootVisualElement.style.display);
        }

        [Test, Order(2)]
        [Category("BuildServer")]
        public void HandleDisplayUI_SetsRootToDisplayFlex()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

            //Act
            modal.HandleDisplayUI(modalSO);

            //Assert
            Assert.AreEqual(expectedStyle, modalDoc.rootVisualElement.style.display);
        }

        [UnityTest]
        public IEnumerator HandleDisplayUI_WithSO_ShouldPopulateStrings()
        {
            //Arrange
            string expectedName = "Name";
            string expectedTitle = "Title";
            string expectedDescription = "Description";
            string expectedBtnText = "Button";

            //Act
            modal.HandleDisplayUI(modalSO);
            yield return null;

            //Assert
            Assert.AreEqual(expectedName, modalDoc.rootVisualElement.Q<TemplateContainer>("Header").Q<VisualElement>().Q<Label>("NameLabel").text);
            Assert.AreEqual(expectedTitle, modalDoc.rootVisualElement.Q<Label>("TitleLabel").text);
            Assert.AreEqual(expectedDescription, modalDoc.rootVisualElement.Q<Label>("DescriptionLabel").text);
            Assert.AreEqual(expectedBtnText, modalDoc.rootVisualElement.Q<TemplateContainer>().Q<Button>().text);
        }

        [Test, Order(5)]
        [Category("BuildServer")]
        public void SetContent_SubDescriptionEmpty_SetsLabelDisplayNone()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            EndModalSO tempModalSO = modalSO;
            tempModalSO.Description = "";

            //Act
            modal.SetContent(tempModalSO);

            //Assert
            Assert.AreEqual(expectedStyle, modalDoc.rootVisualElement.Q<Label>("DescriptionLabel").style.display);
        }

        [Test, Order(6)]
        [Category("BuildServer")]
        public void Show_SetsDisplayFlex()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

            //Act
            modal.Show();

            //Assert
            Assert.AreEqual(expectedStyle, modalDoc.rootVisualElement.style.display);
        }

        [Test, Order(7)]
        [Category("BuildServer")]
        public void Hide_SetsDisplayNone()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            //Act
            modal.Show();
            modal.Hide();

            //Assert
            Assert.AreEqual(expectedStyle, modalDoc.rootVisualElement.style.display);
        }

        [UnityTest]
        public IEnumerator Hide_AfterMenuButtonPress()
        {
            //Arrange
            var button = modalDoc.rootVisualElement.Q<Button>("MenuButton");
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            //Act
            yield return TestUtils.ClickOnButton(button);

            //Assert
            Assert.AreEqual(expectedStyle, modalDoc.rootVisualElement.style.display);
        }

        [UnityTest]
        public IEnumerator Hide_AfterNextButtonPress()
        {
            //Arrange

            var button = modalDoc.rootVisualElement.Q<Button>("NextButton");
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            //Act
            yield return TestUtils.ClickOnButton(button);

            //Assert
            Assert.AreEqual(expectedStyle, modalDoc.rootVisualElement.style.display);
        }
    }
}