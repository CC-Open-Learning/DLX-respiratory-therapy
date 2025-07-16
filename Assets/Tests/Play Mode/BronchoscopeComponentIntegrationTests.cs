using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class BronchoscopeComponentIntegrationTests
    {
        private BronchoscopeComponent bronchoscopeComponent;
        private BiopsyController controller;
        private readonly string scenePath = "Assets/Scenes/BiopsyComponentTestScene.unity";

        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath,
                new LoadSceneParameters(LoadSceneMode.Single));

            controller = Object.FindAnyObjectByType<BiopsyController>();
            bronchoscopeComponent = Object.FindAnyObjectByType<BronchoscopeComponent>();
        }

        [UnityTest]
        public IEnumerator OnClickOnBronchoscope_FirstClick_SetsComponentAccessedAndCallsUpdate()
        {
            // Act
            bronchoscopeComponent.ComponentAccessed = false;
            bronchoscopeComponent.ClickOnComponent();
            yield return null;

            // Assert
            Assert.IsTrue(bronchoscopeComponent.ComponentAccessed);
        }
    }
}
