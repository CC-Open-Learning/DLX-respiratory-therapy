using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class CC10SalineComponentIntegrationTests
    {
        private CC10SalineComponent cC10SalineComponent;
        private BiopsyController controller;
        private readonly string scenePath = "Assets/Scenes/BiopsyComponentTestScene.unity";
        
        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath,
                new LoadSceneParameters(LoadSceneMode.Single));

            controller = Object.FindAnyObjectByType<BiopsyController>();
            cC10SalineComponent = Object.FindAnyObjectByType<CC10SalineComponent>();
        }

        [UnityTest]
        public IEnumerator ClickOnComponent_FirstClick_SetsComponentAccessedAndCallsUpdate()
        {
            // Act
            cC10SalineComponent.ComponentAccessed = false;
            cC10SalineComponent.ClickOnComponent();
            yield return null;

            // Assert
            Assert.IsTrue(cC10SalineComponent.ComponentAccessed);
        }
    }
}
