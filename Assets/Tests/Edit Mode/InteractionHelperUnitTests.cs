using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VARLab.RespiratoryTherapy;

namespace Tests.EditMode
{
    public class InteractionHelperTests
    {
        private GameObject testObject;
        private Collider testObjectCollider;
        private GameObject anotherTestObject;
        private List<Collider> anotherTestObjectColliders;

        [SetUp]
        public void Setup()
        {
            testObject = new GameObject();
            testObjectCollider = testObject.AddComponent<BoxCollider>();

            anotherTestObject = new GameObject();
            anotherTestObjectColliders = new List<Collider>();
            Collider collider = anotherTestObject.AddComponent<BoxCollider>();
            anotherTestObjectColliders.Add(collider);
            collider = anotherTestObject.AddComponent<BoxCollider>();
            anotherTestObjectColliders.Add(collider);
        }

        [TearDown]
        public void TearDown()
        {
            testObject = null;
            anotherTestObject = null;
            anotherTestObjectColliders = null;
        }

        [Test]
        public void ToggleInteractivity_EnableObject_ShouldEnableCollider()
        {
            //Arrange
            testObjectCollider.enabled = false;

            //Act
            InteractionHelper.ToggleInteractivity(new List<GameObject>() { testObject }, true);

            //Assert
            Assert.AreEqual(true, testObjectCollider.enabled);
        }

        [Test]
        public void ToggleInteractivity_DisableObject_ShouldDisableCollider()
        {
            //Arrange
            testObjectCollider.enabled = true;

            //Act
            InteractionHelper.ToggleInteractivity(new List<GameObject>() { testObject }, false);

            //Assert
            Assert.AreEqual(false, testObjectCollider.enabled);
        }

        [Test]
        public void ToggleInteractivity_EnableMultipleObjects_ShouldEnableAllColliders()
        {
            //Arrange
            anotherTestObjectColliders.ForEach(collider => collider.enabled = false);

            //Act
            InteractionHelper.ToggleInteractivity(new List<GameObject>() { testObject, anotherTestObject }, true);

            //Assert
            Assert.AreEqual(true, anotherTestObjectColliders.All(gameObject => gameObject.GetComponents<Collider>().All(collider => collider.enabled == true)));
        }

        [Test]
        public void ToggleInteractivity_DisableMultipleObjects_ShouldDisableAllColliders()
        {
            //Arrange
            anotherTestObjectColliders.ForEach(collider => collider.enabled = true);

            //Act
            InteractionHelper.ToggleInteractivity(new List<GameObject>() { testObject, anotherTestObject }, false);

            //Assert
            Assert.AreEqual(true, anotherTestObjectColliders.All(gameObject => gameObject.GetComponents<Collider>().All(collider => collider.enabled == false)));
        }
    }
}
