using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class HandbookControllerIntegrationTests
    {
        private HandbookController handbookController;
        
        private VisualElement handbookRoot;
        private VisualElement progressContainer;
        private VisualElement glossaryContainer;
        private VisualElement tableContainer;
        private VisualElement monitorContainer;
        private Button progressButton;
        private Button glossaryButton;
        private Button tableButton;
        private Button monitorButton;

        private List<VisualElement> categoryContainers;

        private int sceneCounter;
        
        [SetUp]
        public void SetUp()
        {
            sceneCounter = TestUtils.ClearScene(sceneCounter, "HandbookScene");

            GameObject handbookGameObject = new GameObject("Handbook");

            //Setup controllers
            GameObject referenceObject = new GameObject("ReferenceObject");
            referenceObject.transform.parent = handbookGameObject.transform;
            BiopsyController biopsyController = referenceObject.AddComponent<BiopsyController>();
            ProgressTableController progressTableController = referenceObject.AddComponent<ProgressTableController>();
            GlossaryController glossaryController = referenceObject.AddComponent<GlossaryController>();

            //Setup Handbook UI
            UIDocument handbookUIDocument = handbookGameObject.AddComponent<UIDocument>();
            VisualTreeAsset handbookUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Handbook/Handbook.uxml");
            handbookUIDocument.panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Common/RT Panel Settings.asset");
            handbookUIDocument.visualTreeAsset = handbookUXML;
            handbookUXML.CloneTree(handbookUIDocument.rootVisualElement);
            handbookController = handbookGameObject.AddComponent<HandbookController>();

            //Reference BiopsyController, ProgressTableController, and GlossaryController as SerializedFields
            SerializedObject so = new SerializedObject(handbookController);
            so.FindProperty("biopsyController").objectReferenceValue = biopsyController;
            so.FindProperty("progressTableController").objectReferenceValue = progressTableController;
            so.FindProperty("glossaryController").objectReferenceValue = glossaryController;
            so.ApplyModifiedProperties();

            //Find UI element references
            handbookRoot = handbookUIDocument.rootVisualElement;

            categoryContainers = new List<VisualElement>();
            progressContainer = handbookRoot.Q<VisualElement>("ProgressContainer");
            categoryContainers.Add(progressContainer);
            glossaryContainer = handbookRoot.Q<VisualElement>("GlossaryContainer");
            categoryContainers.Add(glossaryContainer);
            tableContainer = handbookRoot.Q<VisualElement>("TableContainer");
            categoryContainers.Add(tableContainer);
            monitorContainer = handbookRoot.Q<VisualElement>("MonitorContainer");
            categoryContainers.Add(monitorContainer);

            progressButton = handbookRoot.Q<Button>("ProgressButton");
            glossaryButton = handbookRoot.Q<Button>("GlossaryButton");
            tableButton = handbookRoot.Q<Button>("TableButton");
            monitorButton = handbookRoot.Q<Button>("MonitorButton");
        }

        [TearDown]
        public void TearDown()
        {
            handbookController = null;

            handbookRoot = null;
            progressContainer = null;
            glossaryContainer = null;
            tableContainer = null;
            monitorContainer = null;
            progressButton = null;
            glossaryButton = null;
            tableButton = null;
            monitorButton = null;

            categoryContainers = null;

            UIManager.Instance = null;
        }
        
        [UnityTest]
        public IEnumerator Show_ShouldDisplayHandbook()
        {
            //Arrange
            handbookRoot.style.display = DisplayStyle.None;

            //Act
            handbookController.Show();
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(handbookRoot.style.display == DisplayStyle.Flex);
        }

        [UnityTest]
        public IEnumerator Hide_ShouldHideHandbook()
        {
            //Arrange
            handbookRoot.style.display = DisplayStyle.Flex;

            //Act
            handbookController.Hide();
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(handbookRoot.style.display == DisplayStyle.None);
        }

        [UnityTest]
        public IEnumerator OnButtonClick_ShouldHighlightButton()
        {
            //Act
            yield return TestUtils.ClickOnButton(monitorButton);

            //Assert
            Assert.IsTrue(monitorButton.ClassListContains("handbook-button-category-selected") &&
                !monitorButton.ClassListContains("handbook-button-category-unselected"));
        }

        [UnityTest]
        public IEnumerator OnProgressButtonClick_ShouldChangeContentToProgress()
        {
            //Arrange
            categoryContainers.ForEach(container => container.style.display = DisplayStyle.Flex);
            categoryContainers.Remove(progressContainer);

            //Act
            yield return TestUtils.ClickOnButton(progressButton);

            //Assert
            Assert.IsTrue(progressContainer.style.display == DisplayStyle.Flex &&
                categoryContainers.All(container => container.style.display == DisplayStyle.None));
        }

        [UnityTest]
        public IEnumerator OnGlossaryButtonClick_ShouldChangeContentToGlossary()
        {
            //Arrange
            categoryContainers.ForEach(container => container.style.display = DisplayStyle.Flex);
            categoryContainers.Remove(glossaryContainer);

            //Act
            yield return TestUtils.ClickOnButton(glossaryButton);

            //Assert
            Assert.IsTrue(glossaryContainer.style.display == DisplayStyle.Flex &&
                categoryContainers.All(container => container.style.display == DisplayStyle.None));
        }

        [UnityTest]
        public IEnumerator OnTableButtonClick_ShouldChangeContentToTable()
        {
            //Arrange
            categoryContainers.ForEach(container => container.style.display = DisplayStyle.Flex);
            categoryContainers.Remove(tableContainer);

            //Act
            yield return TestUtils.ClickOnButton(tableButton);

            //Assert
            Assert.IsTrue(tableContainer.style.display == DisplayStyle.Flex &&
                categoryContainers.All(container => container.style.display == DisplayStyle.None));
        }

        [UnityTest]
        public IEnumerator OnMonitorButtonClick_ShouldChangeContentToMonitor()
        {
            //Arrange
            categoryContainers.ForEach(container => container.style.display = DisplayStyle.Flex);
            categoryContainers.Remove(monitorContainer);

            //Act
            yield return TestUtils.ClickOnButton(monitorButton);

            //Assert
            Assert.IsTrue(monitorContainer.style.display == DisplayStyle.Flex &&
                categoryContainers.All(container => container.style.display == DisplayStyle.None));
        }
    }
}
