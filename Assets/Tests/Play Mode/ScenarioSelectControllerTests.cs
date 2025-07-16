using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class ScenarioSelectControllerTests
    {
        private GameObject scenarioSelectGameObject;
        private ScenarioSelectController scenarioSelectController;
        private UIDocument scenarioSelectUIDocument;
        private VisualElement scenarioRoot;
        private GameObject inventoryObject;
        private ScenarioSelectController inventoryController;

        [SetUp]
        [Category("BuildServer")]
        public void SetUp()
        {
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Common/RT Panel Settings.asset");
            VisualTreeAsset uiDocument = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Menus/ScenarioSelect.uxml");
            scenarioSelectGameObject = new();

            scenarioSelectUIDocument = scenarioSelectGameObject.AddComponent<UIDocument>();
            scenarioSelectUIDocument.panelSettings = panelSettings;
            scenarioSelectUIDocument.visualTreeAsset = uiDocument;
            scenarioRoot = scenarioSelectUIDocument.rootVisualElement;
            scenarioSelectController = scenarioSelectGameObject.AddComponent<ScenarioSelectController>();
        }
        
        [TearDown]
        public void TearDown()
        {
            Object.Destroy(scenarioSelectGameObject);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Show_WhenCalled_Should_Display_MainMenu()
        {
            scenarioSelectController.Show();

            yield return null;

            Assert.IsTrue(scenarioSelectUIDocument.rootVisualElement.style.display == DisplayStyle.Flex);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Hide__WhenCalled_Should_Hide_MainMenu()
        {
            scenarioSelectController.Show();
            scenarioSelectController.Hide();

            yield return null; 

            Assert.IsTrue(scenarioSelectUIDocument.rootVisualElement.style.display == DisplayStyle.None);
        }
        
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Scenario_OnCloseClick_PanelCloses()
        {
            Button closeButton = scenarioRoot.Q<Button>("Icon");
            scenarioSelectController.Show();

            yield return null;
            yield return TestUtils.ClickOnButton(closeButton);
            
            Assert.AreEqual((StyleEnum<DisplayStyle>)DisplayStyle.None, scenarioRoot.style.display);
        }
    }
}
