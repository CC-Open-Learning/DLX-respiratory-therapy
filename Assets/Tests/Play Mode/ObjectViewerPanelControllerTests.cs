using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class ObjectViewerPanelControllerTests
    {
        private GameObject objectViewerPanelObject;
        private GameObject testObject;

        private VisualElement rootContainer;
        private ObjectViewerPanelController objectViewerPanelController;

        private ExplorableInformationSO explorableInformation;
        private Explorable explorable;

        private Button closeButton;
        private Button playAudio;
        private Label explorableNameLabel;
        private Label explorableDescriptionLabel;

        [UnitySetUp]
        [Category("BuildServer")]
        public IEnumerator Setup()
        {
            //Load things for UI Doc
            var uxmlTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Object Viewer/Object Viewer UI.uxml");
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Common/RT Panel Settings.asset");

            //Setup UI
            objectViewerPanelObject = new("Object Viewer Panel");
            var uiDoc = objectViewerPanelObject.AddComponent<UIDocument>();
            uiDoc.panelSettings = panelSettings;
            uiDoc.visualTreeAsset = uxmlTemplate;
            rootContainer = uiDoc.rootVisualElement;
            objectViewerPanelController = objectViewerPanelObject.AddComponent<ObjectViewerPanelController>();
            yield return null;

            testObject = new("Test Object");

            explorableNameLabel = rootContainer.Q<Label>("ExplorableNameLabel");
            explorableDescriptionLabel = rootContainer.Q<Label>("ExplorableDescriptionLabel");
            closeButton = rootContainer.Q<TemplateContainer>().Q<Button>("CloseBtn");
            playAudio = rootContainer.Q<TemplateContainer>().Q<Button>("Button");

            explorableInformation = ScriptableObject.CreateInstance<ExplorableInformationSO>();
            explorableInformation.ExplorableName = "Test";
            explorableInformation.ExplorableDescription = "Test";

            explorable = testObject.AddComponent<Explorable>();
            explorable.explorableInformation = explorableInformation;
        }

        [TearDown]
        [Category("BuildServer")]
        public void TearDown()
        {
            objectViewerPanelController = null;
            objectViewerPanelObject = null;
            testObject = null;
            explorableInformation = null;
            explorable = null;
        }

        /// <summary>
        /// Test ensures that if we pass an object without an explorable component that the events
        ///that reenable interactions get triggered
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator OVPanel_IncorrectObjectNoExplorable_ReenableInteractionsTriggered()
        {
            //Arrange
            bool listener = false;
            objectViewerPanelController.ReenableInteractions.AddListener(() => listener = true);
            GameObject objectWithNoExplorable;

            //Act
            objectWithNoExplorable = new();
            objectViewerPanelController.HandleObjectClick(objectWithNoExplorable);
            yield return null;

            //Assert
            Assert.IsTrue(listener);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator OVPanel_IncorrectObjectIsNull_ReenableInteractionsTriggered()
        {
            //Arrange
            bool listener = false;
            objectViewerPanelController.ReenableInteractions.AddListener(() => listener = true);

            //Act
            objectViewerPanelController.HandleObjectClick(null);
            yield return null;

            //Assert
            Assert.IsTrue(listener);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator OVPanel_OnClickObject_ObjectViewerIsShown()
        {
            //Arrange

            //Act      
            objectViewerPanelController.HandleObjectClick(testObject);
            yield return null;

            //Assert
            Assert.AreEqual((StyleEnum<DisplayStyle>)DisplayStyle.Flex, rootContainer.style.display);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator OVPanel_OnClickObject_ToolbarHiddenEventTriggered()
        {
            //Arrange
            bool listener = false;
            objectViewerPanelController.HideScreen.AddListener((UIPanel) => listener = true);

            //Act
            objectViewerPanelController.HandleObjectClick(testObject);
            yield return null;

            //Assert
            Assert.IsTrue(listener);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator OVPanel_OnClickObject_ViewingObjectEventsTriggered()
        {
            //Arrange
            bool listener = false;
            objectViewerPanelController.ViewObject.AddListener((GameObject) => listener = true);

            //Act
            objectViewerPanelController.HandleObjectClick(testObject);
            yield return null;

            //Assert
            Assert.IsTrue(listener);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator OVPanel_OnClickObject_ExplorableInformationIsSet()
        {
            //Arrange
            string expectedExplorableName = "Test";
            string expectedExplorableDescription = "Test";

            objectViewerPanelController.HandleObjectClick(testObject);
            yield return null;

            //Assert
            Assert.AreEqual(expectedExplorableDescription, explorableNameLabel.text);
            Assert.AreEqual(expectedExplorableName, explorableDescriptionLabel.text);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator OVPanel_OnClickObject_AddProgressEventTriggered()
        {
            //Arrange
            bool listener = false;
            objectViewerPanelController.AddProgress.AddListener((ExplorableCategory ec, string s, int i) => listener = true);

            //Act
            objectViewerPanelController.HandleObjectClick(testObject);
            yield return null;

            //Assert
            Assert.IsTrue(listener);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator OVPanel_OnClosePanel_PanelIsHidden()
        {
            //Act
            objectViewerPanelController.HandleObjectClick(testObject);
            yield return TestUtils.ClickOnButton(closeButton);

            //Assert
            Assert.AreEqual((StyleEnum<DisplayStyle>)DisplayStyle.None, rootContainer.style.display);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator OVPanel_OnClosePanel_AllEventsTriggered()
        {
            //Arrange
            bool showScreenlistener = false;
            objectViewerPanelController.ShowScreen.AddListener((UIPanel) => showScreenlistener = true);

            bool stopAudioListener = false;
            objectViewerPanelController.StopAudio.AddListener((SoundPlayOptions) => stopAudioListener = true);

            bool onClosedListener = false;
            objectViewerPanelController.OnCloseClicked.AddListener(() => onClosedListener = true);

            bool resetObjectListener = false;
            objectViewerPanelController.ResetObject.AddListener(() => resetObjectListener = true);

            //Act
            objectViewerPanelController.HandleObjectClick(testObject);
            yield return TestUtils.ClickOnButton(closeButton);

            //Assert
            Assert.IsTrue(showScreenlistener, "The show screen event was not triggered");
            Assert.IsTrue(stopAudioListener, "The stop audio event was not triggered");
            Assert.IsTrue(onClosedListener, "The on closed event was not triggered");
            Assert.IsTrue(resetObjectListener, "The reset object event was not triggered");
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator OVPanel_OnPlayAudio_AudioPlayEventTriggered()
        {
            //Arrange
            bool listener = false;
            objectViewerPanelController.PlayAudio.AddListener((AudioClip, SoundPlayOptions) => listener = true);

            //Act
            objectViewerPanelController.HandleObjectClick(testObject);
            yield return TestUtils.ClickOnButton(playAudio);

            //Assert
            Assert.IsTrue(listener);
        }
    }
}