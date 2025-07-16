using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class ItemHandlerTaskSOTests
    {
        private ItemHandlerTaskSO itemHandlerTask;
        private GameObject itemHandlerTaskObject;
        private GameObject physicianAnimationObject;
        private BiopsyProcedureComponentHandler componentHandler;
        private List<BiopsyProcedureComponentBase> components = new();

        private GameObject bronchoscopeObject;
        private BiopsyProcedureComponentBase bronchoscopeComp;
        private GameObject suctionTubeObject;
        private BiopsyProcedureComponentBase suctionTubeComp;
        private GameObject bronchConnectorObject;
        private BiopsyProcedureComponentBase bronchConnectorComp;

        [SetUp]
        [Category("BuildServer")]
        public void Setup()
        {
            itemHandlerTask = ScriptableObject.CreateInstance<ItemHandlerTaskSO>();
            itemHandlerTaskObject = new GameObject();
            componentHandler = itemHandlerTaskObject.AddComponent<BiopsyProcedureComponentHandler>();

            bronchoscopeObject = new GameObject();
            bronchoscopeComp = bronchoscopeObject.AddComponent<BronchoscopeComponent>();
            bronchoscopeComp.procedureEquipment = BiopsyProcedureEquipment.Bronchoscope;
            components.Add(bronchoscopeComp);

            //objects targeted by RemoveConnections
            physicianAnimationObject = new GameObject();
            physicianAnimationObject.AddComponent<PhysicianAnimationController>();
            physicianAnimationObject.AddComponent<Animator>();
            physicianAnimationObject.GetComponent<Animator>().runtimeAnimatorController  = 
                AssetDatabase.LoadAssetAtPath<AnimatorController>(
                "Assets/Animations/Biopsy Procedure/Female_Physician/Female_Physician.controller");
            suctionTubeObject = new GameObject();
            suctionTubeObject.AddComponent<Animator>();
            suctionTubeComp = suctionTubeObject.AddComponent<SuctionTubeComponent>();
            bronchConnectorObject = new GameObject();
            
            
            bronchConnectorComp = bronchConnectorObject.AddComponent<ConnectorComponent>();
            suctionTubeComp.procedureEquipment = BiopsyProcedureEquipment.SuctionTubeConnector;
            bronchConnectorComp.procedureEquipment = BiopsyProcedureEquipment.BronchoscopeConnector;
            components.Add(suctionTubeComp);
            components.Add(bronchConnectorComp);

            componentHandler.components = components;
            itemHandlerTask.biopsyProcedureComponentHandler = componentHandler;
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(itemHandlerTask);
            Object.DestroyImmediate(itemHandlerTaskObject);
            Object.DestroyImmediate(bronchoscopeObject);
            Object.DestroyImmediate(suctionTubeObject);
            Object.DestroyImmediate(bronchConnectorObject);
            components.Clear();
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator ItemHandlerTask_SetProcedureEquipment_ShouldSetTheProcedureEquipment()
        {
            BiopsyProcedureEquipment expectedResult = BiopsyProcedureEquipment.Bronchoscope;

            itemHandlerTask.ProcedureEquipment = expectedResult;
            yield return null;

            Assert.AreEqual(expectedResult, BiopsyProcedureEquipment.Bronchoscope);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator ItemHandlerTask_Execute_ShouldEnableEquipment()
        {
            itemHandlerTask.ProcedureEquipment = BiopsyProcedureEquipment.Bronchoscope;
            bronchoscopeObject.SetActive(false);

            itemHandlerTask.Execute();
            yield return null;

            Assert.IsTrue(bronchoscopeObject.activeSelf);
        }
        
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator ItemHandlerTask_EnableAllItems_ShouldEnableAllEquipment()
        {
            itemHandlerTask.EnableAllTableItems = true;
            bronchoscopeObject.SetActive(false);

            itemHandlerTask.Execute();
            yield return null;

            Assert.IsTrue(bronchoscopeObject.activeSelf);
        }
        
        [UnityTest]
        public IEnumerator Execute_With_Null_ComponentHandler_Instantiates_ComponentHandler()
        {
            itemHandlerTask.biopsyProcedureComponentHandler = null;
            Assert.IsNull(itemHandlerTask.biopsyProcedureComponentHandler);
        
            itemHandlerTask.Execute();
            yield return null;
        
            Assert.NotNull(componentHandler);
        }
    }
}