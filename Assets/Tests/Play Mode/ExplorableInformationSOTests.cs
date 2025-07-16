using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    /// <summary>
    /// This file contains all the tests for the Explorable Scriptable Object
    /// </summary>
    public class ExplorableSOTests
    {
        ExplorableInformationSO explorableInformation;

        /// <summary>
        /// Essential test setup, runs before every test
        /// </summary>
        [SetUp]
        [Category("BuildServer")]
        public void Setup()
        {
            explorableInformation = ScriptableObject.CreateInstance<ExplorableInformationSO>();
        }

        [TearDown]
        [Category("BuildServer")]
        public void TearDown()
        {
            Object.Destroy(explorableInformation);
        }

        /// <summary>
        /// Test ensures that the name property of a Explorable SO can be properly set and later retrieved
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator EnsureExplorableNameIsSet()
        {
            //Arrange
            string expectedResult = "Test Explorable Name";

            //Act
            explorableInformation.ExplorableName = expectedResult;
            yield return null;

            //Assert
            Assert.AreEqual(expectedResult, explorableInformation.ExplorableName);
        }


        /// <summary>
        /// Test ensures that the description property of a Explorable SO can be properly set and later retrieved
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator EnsureExplorableDescriptionIsSet()
        {
            //Arrange
            string expectedResult = "Test Explorable Description";

            //Act
            explorableInformation.ExplorableDescription = expectedResult;
            yield return null;

            //Assert
            Assert.AreEqual(expectedResult, explorableInformation.ExplorableDescription);
        }

        /// <summary>
        /// Test ensures that the category status property of a Explorable SO can be properly set and later retrieved
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator EnsureExplorableCategoryIsSet()
        {
            //Arrange
            ExplorableCategory expectedResult = ExplorableCategory.Medications;

            //Act
            explorableInformation.ExplorableCategory = expectedResult;
            yield return null;

            //Assert
            Assert.AreEqual(expectedResult, explorableInformation.ExplorableCategory);
        }
    }
}