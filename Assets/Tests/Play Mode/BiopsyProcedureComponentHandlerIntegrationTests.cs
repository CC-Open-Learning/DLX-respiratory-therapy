using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class BiopsyProcedureComponentHandlerIntegrationTests
    {
        private readonly string _scenePath = "Assets/Scenes/BiopsyComponentHandlerTestScene.unity";
        private BiopsyProcedureComponentHandler biopsyProcedureComponentHandler;
        
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(_scenePath,
                new LoadSceneParameters(LoadSceneMode.Single));
            
            biopsyProcedureComponentHandler = Object.FindAnyObjectByType<BiopsyProcedureComponentHandler>();
        }
        
        [UnityTest]
        public IEnumerator EnableAllBiopsyProcedureItems_SetsAllActiveTrue()
        {
            biopsyProcedureComponentHandler.EnableAllBiopsyProcedureItems();
            yield return null;
            
            foreach (var component in biopsyProcedureComponentHandler.components)
            {
                Debug.Log(component.gameObject.name);
                Debug.Log(component.gameObject.activeSelf);
                Assert.IsTrue(component.gameObject.activeSelf);
            }
        }

        [UnityTest]
        public IEnumerator EnableBiopsyProcedureItem_OnlyEnablesSpecified()
        {
            biopsyProcedureComponentHandler.EnableBiopsyProcedureItem(BiopsyProcedureEquipment.SpecimenJar);
            
            yield return null;

            foreach (var component in biopsyProcedureComponentHandler.components
                         .Where(component => component.procedureEquipment == BiopsyProcedureEquipment.SpecimenJar))
            {
                Assert.IsTrue(component.gameObject.activeSelf);
            }
        }
        
        [UnityTest]
        public IEnumerator DisableBiopsyProcedureItem_OnlyDisablesSpecified()
        {
            biopsyProcedureComponentHandler.DisableBiopsyProcedureItem(BiopsyProcedureEquipment.BiopsyForceps);
            
            yield return null;

            foreach (var component in biopsyProcedureComponentHandler.components
                         .Where(component => component.procedureEquipment == BiopsyProcedureEquipment.BiopsyForceps))
            {
                Assert.IsFalse(component.gameObject.activeSelf);
            }
        }
        
        [UnityTest]
        public IEnumerator GetProcedureEquipmentObject_ReturnsCorrectGameObject()
        {
            var obj = biopsyProcedureComponentHandler.GetProcedureEquipmentObject(BiopsyProcedureEquipment.BiopsyForceps);
            yield return null;
            Assert.IsNotNull(obj);
            Assert.AreEqual("BiopsyForceps Component", obj.name);
        }
    }
}
