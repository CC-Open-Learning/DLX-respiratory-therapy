using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class SputumSpecimenContainerComponentIntegrationTests
    {
        private SputumContainerComponent sputumSpecimenContainerComponent;
        private BiopsyController controller;
        private readonly string scenePath = "Assets/Scenes/BiopsyComponentTestScene.unity";
        
        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath,
                new LoadSceneParameters(LoadSceneMode.Single));

            controller = Object.FindAnyObjectByType<BiopsyController>();
            sputumSpecimenContainerComponent = Object.FindAnyObjectByType<SputumContainerComponent>();
        }

        [UnityTest]
        public IEnumerator OnClickOnSputumSpecimenContainer_FirstClick_SetsComponentAccessedAndCallsUpdate()
        {
            // Act
            sputumSpecimenContainerComponent.ComponentAccessed = false;
            sputumSpecimenContainerComponent.ClickOnComponent();
            yield return null;

            // Assert
            Assert.IsTrue(sputumSpecimenContainerComponent.ComponentAccessed);
        }
    }
}
