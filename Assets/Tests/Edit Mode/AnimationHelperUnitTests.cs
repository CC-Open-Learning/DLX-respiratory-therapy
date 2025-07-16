using NUnit.Framework;
using UnityEngine;
using VARLab.RespiratoryTherapy;

namespace Tests.EditMode
{
    public class AnimationHelperTests
    {
        private GameObject testObject;
        private Animator testObjectAnimator;
        private GameObject interactionBlocker;

        [SetUp]
        public void Setup()
        {
            testObject = new GameObject("TestObject");
            testObjectAnimator = testObject.AddComponent<Animator>();

            interactionBlocker = new GameObject("InteractionBlocker");
            interactionBlocker.SetActive(false);
        }

        [TearDown]
        public void TearDown()
        {
            testObject = null;
            testObjectAnimator = null;

            interactionBlocker = null;
        }

        [Test]
        public void ActivateAnimation_ShouldEnableInteractionBlocker()
        {
            //Act
            AnimationHelper.ActivateAnimation(testObjectAnimator, "trigger", interactionBlocker);

            //Assert
            Assert.AreEqual(true, interactionBlocker.activeSelf);
        }

        [Test]
        public void HandleAnimationComplete_ShouldDisableInteractionBlocker()
        {
            //Arrange
            interactionBlocker.SetActive(true);

            //Act
            AnimationHelper.HandleAnimationCompleted(interactionBlocker);

            //Assert
            Assert.AreEqual(false, interactionBlocker.activeSelf);
        }
    }
}
