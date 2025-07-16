using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class CC20SalineComponentIntegrationTests
    {
        private CC20SalineComponent cC20SalineComponent;
        private BiopsyController controller;
        private readonly string scenePath = "Assets/Scenes/BiopsyComponentTestScene.unity";
        
        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath,
                new LoadSceneParameters(LoadSceneMode.Single));

            controller = Object.FindAnyObjectByType<BiopsyController>();
            cC20SalineComponent = Object.FindAnyObjectByType<CC20SalineComponent>();
        }

        [UnityTest]
        public IEnumerator OnClickOnBiopsyForceps_FirstClick_SetsComponentAccessedAndCallsUpdate()
        {
            // Act
            cC20SalineComponent.ComponentAccessed = false;
            cC20SalineComponent.ClickOnComponent();
            yield return null;

            // Assert
            Assert.IsTrue(cC20SalineComponent.ComponentAccessed);
        }
    }
}
