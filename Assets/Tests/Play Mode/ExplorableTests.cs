using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class ExplorableTests
    {
        ExplorableInformationSO explorableInformation;
        ExplorableTransformDetails explorableTransformDetails;
        
        private GameObject testObject;
        private Explorable explorable;
        private Renderer[] renderers;

        [SetUp]
        [Category("BuildServer")]
        public void Setup()
        {
            explorableInformation = ScriptableObject.CreateInstance<ExplorableInformationSO>();
            explorableTransformDetails = ScriptableObject.CreateInstance<ExplorableTransformDetails>();

            explorableTransformDetails.rotationValues = Quaternion.Euler(90, 90, 90);
            explorableTransformDetails.scaleValues = new Vector3(3, 3, 3);
            
            testObject = new GameObject("TestExplorable");
            explorable = testObject.AddComponent<Explorable>();

            // Add a dummy ExplorableInformationSO scriptable object
            explorable.explorableInformation = explorableInformation;

            // Add renderers for testing
            var child = GameObject.CreatePrimitive(PrimitiveType.Cube);
            child.transform.SetParent(testObject.transform);
            renderers = child.GetComponentsInChildren<Renderer>();
        }

        [TearDown]
        [Category("BuildServer")]
        public void Teardown()
        {
            explorableInformation = null;
            explorableTransformDetails = null;
        }

        /// <summary>
        /// Test ensures the InteractedWith property of the Explorable class can be set through the SetInteractionStatus method
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Explorable_ExploredStatusChanged_ValueProperlySet()
        {
            //Arrange
            bool expectedValue = true;
            GameObject gameObject;

            //Act
            gameObject = new GameObject();
            gameObject.AddComponent<Explorable>();
            gameObject.GetComponent<Explorable>().IsExplored = expectedValue;
            yield return null;

            //Assert
            Assert.IsTrue(gameObject.GetComponent<Explorable>().IsExplored);
        }

        /// <summary>
        /// Test ensures the proper warning is thrown when a Explorable script is started without having a proper reference to a Explorabe
        /// information Scriptable Object
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Explorable_NullExplorableSO_WarningIsThrown()
        {
            //Arrange
            string expectedResult = "No Explorable Information associated with Explorable";
            GameObject gameObject;

            //Act
            gameObject = new GameObject();
            gameObject.AddComponent<Explorable>();
            yield return null;

            //Assert
            LogAssert.Expect(LogType.Warning, expectedResult);
        }

        /// <summary>
        /// Test ensures that the rotation and scale of a gameobject can be set through a function that utilizes a scriptable object
        /// for its data. This is done within a range due to floating point rounding errors
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Explorable_UsingCustomSOValues_VariablesCorrectlyChange()
        {
            //Arrange
            GameObject gameObject;
            Quaternion expectedRotation = Quaternion.Euler(90f, 90f, 90f);
            float range = 0.5f;

            //Act
            gameObject = new GameObject();
            gameObject.AddComponent<Explorable>();
            gameObject.GetComponent<Explorable>().explorableInformation = explorableInformation;
            gameObject.GetComponent<Explorable>().transformDetails = explorableTransformDetails;
            yield return null;

            gameObject.GetComponent<Explorable>().SetExplorableTransformValues();
            yield return null;

            //Assert
            Assert.AreEqual(expectedRotation.eulerAngles.x, gameObject.transform.rotation.eulerAngles.x, range, "The actual X value didnt match the expected X value");
            Assert.AreEqual(expectedRotation.eulerAngles.y, gameObject.transform.rotation.eulerAngles.y, range, "The actual Y value didnt match the expected Y value");
            Assert.AreEqual(expectedRotation.eulerAngles.z, gameObject.transform.rotation.eulerAngles.z, range, "The actual Z value didnt match the expected Z value");
            Assert.AreEqual(new Vector3(3, 3, 3), gameObject.transform.localScale);
        }


        /// <summary>
        /// Test ensures that the rotation and scale of a gameobject can be set through a function that utilizes a scriptable object
        /// for its data. This is done within a range due to floating point rounding errors
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Explorable_UsingDefaultValues_VariablesCorrectlyChange()
        {
            //Arrange
            GameObject gameObject;
            Quaternion expectedRotation = Quaternion.Euler(0f, 0f, 0f);
            float range = 0.5f;

            //Act
            gameObject = new GameObject();
            gameObject.AddComponent<Explorable>();
            gameObject.GetComponent<Explorable>().explorableInformation = explorableInformation;
            yield return null;

            gameObject.GetComponent<Explorable>().SetExplorableTransformValues();
            yield return null;

            //Assert
            Assert.AreEqual(expectedRotation.eulerAngles.x, gameObject.transform.rotation.eulerAngles.x, range, "The actual X value didnt match the expected X value");
            Assert.AreEqual(expectedRotation.eulerAngles.y, gameObject.transform.rotation.eulerAngles.y, range, "The actual Y value didnt match the expected Y value");
            Assert.AreEqual(expectedRotation.eulerAngles.z, gameObject.transform.rotation.eulerAngles.z, range, "The actual Z value didnt match the expected Z value");
            Assert.AreEqual(new Vector3(1, 1, 1), gameObject.transform.localScale);
        }
        
        [UnityTest]
        public IEnumerator HighlightExplorable_Should_InvokeOnHighlightExplorableEvent()
        {
            // Arrange
            bool eventInvoked = false;
            explorable.OnHighlightExplorable.AddListener(() => eventInvoked = true);

            // Act
            explorable.HighlightExplorable();
            yield return null; // Wait for the frame to process the event

            // Assert
            Assert.IsTrue(eventInvoked, "OnHighlightExplorable event was not invoked.");
        }
        
        [UnityTest]
        public IEnumerator UnHighlightExplorable_Should_InvokeOnUnHighlightExplorableEvent()
        {
            // Arrange
            bool eventInvoked = false;
            explorable.OnUnHighlightExplorable.AddListener(() => eventInvoked = true);

            // Act
            explorable.UnHighlightExplorable();
            yield return null; // Wait for the frame to process the event

            // Assert
            Assert.IsTrue(eventInvoked, "OnUnHighlightExplorable event was not invoked.");
        }

    }
}