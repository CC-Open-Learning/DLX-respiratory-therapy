using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;
using VARLab.Velcro;

namespace Tests.PlayMode
{
    public class LockPOITaskSOTests
    {
        private int sceneCounter = 0;

        private ProcedurePOIToolbar poiToolbar;
        private ToolbarController toolbar;

        private LockPOITaskSO lockPOITask;

        private List<Button> allToolbarButtons;

        [UnitySetUp]
        [Category("BuildServer")]
        public IEnumerator Setup()
        {
            sceneCounter = TestUtils.ClearScene(sceneCounter, "LockPOITaskSOScene");

            //Set up <ProcedurePOIToolbar> UI
            GameObject poiToolbarObject = new GameObject("Procedure POI Toolbar");
            UIDocument poiToolbarUIDocument = poiToolbarObject.AddComponent<UIDocument>();

            //Load and reference <ProcedurePOIToolbar> UI assets as SerializedFields
            SerializedObject so = new SerializedObject(poiToolbarUIDocument);
            VisualTreeAsset poiToolbarUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Toolbars/ToolbarProcedure.uxml");
            so.FindProperty("m_PanelSettings").objectReferenceValue = poiToolbarUXML;
            so.FindProperty("sourceAsset").objectReferenceValue = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Common/RT Panel Settings.asset");
            so.ApplyModifiedProperties();

            poiToolbar = poiToolbarObject.AddComponent<ProcedurePOIToolbar>();
            poiToolbarUXML.CloneTree(poiToolbarUIDocument.rootVisualElement);

            //Set up <Toolbar> UI
            GameObject toolbarObject = new GameObject("Toolbar");
            UIDocument toolbarUIDocument = toolbarObject.AddComponent<UIDocument>();

            //Load and reference <Toolbar> UI assets as SerializedFields
            SerializedObject so1 = new SerializedObject(toolbarUIDocument);
            VisualTreeAsset toolbarUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Toolbars/Toolbar.uxml");
            so1.FindProperty("m_PanelSettings").objectReferenceValue = toolbarUXML;
            so1.FindProperty("sourceAsset").objectReferenceValue = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Common/RT Panel Settings.asset");
            so1.ApplyModifiedProperties();

            toolbar = toolbarObject.AddComponent<ToolbarController>();
            toolbarUXML.CloneTree(toolbarUIDocument.rootVisualElement);

            //Create <LockPOITaskSO> SO
            lockPOITask = ScriptableObject.CreateInstance<LockPOITaskSO>();

            allToolbarButtons = new List<Button>();
            allToolbarButtons.AddRange(poiToolbarUIDocument.rootVisualElement.Query<Button>().ToList());
            allToolbarButtons.Add(toolbarUIDocument.rootVisualElement.Q<Button>("HomeButton"));

            yield return null;
        }

        [TearDown]
        public void Teardown()
        {
            poiToolbar = null;
            toolbar = null;

            lockPOITask = null;

            allToolbarButtons = null;
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Execute_True_ShouldDisableToolbarInteraction()
        {
            //Arrange
            poiToolbar.EnableToolbarInteraction();
            toolbar.EnableToolbarInteraction();
            lockPOITask.LockPOI = true;

            //Act
            lockPOITask.Execute();
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(allToolbarButtons.All(button => button.enabledSelf == false));
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Execute_False_ShouldEnableToolbarInteraction()
        {
            //Arrange
            poiToolbar.DisableToolbarInteraction();
            toolbar.DisableToolbarInteraction();
            lockPOITask.LockPOI = false;

            //Act
            lockPOITask.Execute();
            yield return null; //Wait for one frame so UI can update

            //Assert
            Assert.IsTrue(allToolbarButtons.All(button => button.enabledSelf == true));
        }
    }
}
