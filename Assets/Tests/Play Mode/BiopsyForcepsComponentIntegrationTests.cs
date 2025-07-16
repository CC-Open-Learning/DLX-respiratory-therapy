using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class BiopsyForcepsComponentIntegrationTests
    {
        private BiopsyForcepsComponent biopsyForcepsComponent;
        private BiopsyController controller;
        private readonly string _scenePath = "Assets/Scenes/BiopsyComponentTestScene.unity";
        
        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(_scenePath,
                new LoadSceneParameters(LoadSceneMode.Single));

            controller = Object.FindAnyObjectByType<BiopsyController>();
            biopsyForcepsComponent = Object.FindAnyObjectByType<BiopsyForcepsComponent>();
        }

        [UnityTest]
        public IEnumerator OnClickOnBiopsyForceps_FirstClick_SetsComponentAccessedAndCallsUpdate()
        {
            // Act
            biopsyForcepsComponent.ComponentAccessed = false;
            biopsyForcepsComponent.ClickOnComponent();
            yield return null;

            // Assert
            Assert.IsTrue(biopsyForcepsComponent.ComponentAccessed);
        }
    }
}
