using NUnit.Framework;
using UnityEngine;
using VARLab.RespiratoryTherapy;

namespace Tests.EditMode
{

    /// <summary>
    ///     This class should test the <see cref="CustomSaveHandler"/>
    /// </summary>
    public class SaveHandlerUnitTests
    {
        private string TestUsername;
        private GameObject saveHandlerObject;
        private CustomSaveHandler saveHandler;

        [SetUp]
        public void Setup()
        {
            saveHandlerObject = new("Save Handler Object");
            saveHandler = saveHandlerObject.AddComponent<CustomSaveHandler>();
            TestUsername = "TestUsername";
        }

        [TearDown]
        public void TearDown()
        {
            saveHandler = null;
            saveHandlerObject = null;
            TestUsername = string.Empty;
        }

        /// <summary>
        ///     Validates that once the SaveHandler has received a username 
        ///     from a successful login, the 'Blob' (save file) name contains the username
        /// </summary>
        [Test]
        public void SaveHandler_HandleLogin_ShouldUpdateBlobName()
        {
            // Arrange
            string nameExpected = TestUsername;

            // Act
            saveHandler.HandleLogin(TestUsername);

            // Assert
            Assert.That(saveHandler.Blob.Contains(nameExpected));
        }
    }
}