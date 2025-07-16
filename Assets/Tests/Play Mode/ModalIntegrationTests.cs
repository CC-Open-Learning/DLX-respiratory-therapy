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
    public class ModalIntegrationTests : MonoBehaviour
    {
        private int sceneCounter = 0;

        private GameObject modalObj;
        private UIDocument modalDoc;
        private Modal modal;
        private ModalSO modalSO;

        [UnitySetUp]
        [Category("BuildServer")]
        public IEnumerator SetUp()
        {
            sceneCounter = TestUtils.ClearScene(sceneCounter, "ModalScene");

            //Set up <Modal> UI
            modalObj = new GameObject("Modal");
            modalDoc = modalObj.AddComponent<UIDocument>();
            modal = modalObj.AddComponent<Modal>();

            //Load required assets from project files
            VisualTreeAsset modalUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Modals/Modal.uxml");
            PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Common/RT Panel Settings.asset");
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/Modals/ModalNPC_1_Sprite.png");
            Sprite noteSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VELCRO UI/Sprites/Icons/Info_Sprite.png");

            //Reference panel settings and source asset as SerializedFields
            SerializedObject so = new SerializedObject(modalDoc);
            so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
            so.FindProperty("sourceAsset").objectReferenceValue = modalUXML;
            so.ApplyModifiedProperties();

            modalSO = ScriptableObject.CreateInstance<ModalSO>();
            modalSO.Name = "Name";
            modalSO.Title = "Title";
            modalSO.Description = "Description";
            modalSO.SubDescription = "Sub Description";
            modalSO.Note = "Note";
            modalSO.ButtonText = "Button";
            modalSO.Image = sprite;
            modalSO.NoteImage = noteSprite;
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

        [Test, Order(3)]
        [Category("BuildServer")]
        public void HandleDisplayUI_WithSO_ShouldPopulateStrings()
        {
            //Arrange
            string expectedName = "Name";
            string expectedTitle = "Title";
            string expectedDescription = "Description";
            string expectedSubDescription = "Sub Description";
            string expectedNote = "Note";
            string expectedBtnText = "Button";

            //Act
            modal.HandleDisplayUI(modalSO);

            //Assert
            Assert.AreEqual(expectedName, modalDoc.rootVisualElement.Q<TemplateContainer>().Q<Label>().text);
            Assert.AreEqual(expectedTitle, modalDoc.rootVisualElement.Q<Label>("TitleLabel").text);
            Assert.AreEqual(expectedDescription, modalDoc.rootVisualElement.Q<Label>("DescriptionLabel").text);
            Assert.AreEqual(expectedSubDescription, modalDoc.rootVisualElement.Q<Label>("SubDescriptionLabel").text);
            Assert.AreEqual(expectedNote, modalDoc.rootVisualElement.Q<Label>("NoteLabel").text);
            Assert.AreEqual(expectedBtnText, modalDoc.rootVisualElement.Q<TemplateContainer>().Q<Button>().text);
        }

        [Test, Order(4)]
        [Category("BuildServer")]
        public void SetContent_NoteEmpty_SetsLabelDisplayNone()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            ModalSO tempModalSO = modalSO;
            tempModalSO.Note = "";

            //Act
            modal.SetContent(tempModalSO);

            //Assert
            Assert.AreEqual(expectedStyle, modalDoc.rootVisualElement.Q<Label>("NoteLabel").style.display);
        }

        [Test, Order(5)]
        [Category("BuildServer")]
        public void SetContent_SubDescriptionEmpty_SetsLabelDisplayNone()
        {
            //Arrange
            StyleEnum<DisplayStyle> expectedStyle = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            ModalSO tempModalSO = modalSO;
            tempModalSO.SubDescription = "";

            //Act
            modal.SetContent(tempModalSO);

            //Assert
            Assert.AreEqual(expectedStyle, modalDoc.rootVisualElement.Q<Label>("SubDescriptionLabel").style.display);
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
    }
}