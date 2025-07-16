using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class NetworkControllerTests
    {
        private GameObject gameObject;
        private MockNetworkController mockNetworkController;

        /// <summary>
        /// Essential test setup, runs before every test
        /// </summary>
        [SetUp]
        [Category("BuildServer")]
        public void TestSetup()
        {
            gameObject = new("Network Object");
            mockNetworkController = gameObject.AddComponent<MockNetworkController>();
        }

        /// <summary>
        /// Cleans up the GameObject after the test
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Object.Destroy(gameObject);
        }

        /// <summary>
        /// Waits for one fram with yield return null
        /// Then test confirms that the property isConnected is false initially
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator NetworkController_InitialState_ShouldBeDisconnected()
        {
            //Act
            yield return null;

            //Assert
            Assert.IsFalse(mockNetworkController.IsConnected, "isConnected should be false initially.");
        }

        /// <summary>
        /// Tests when there is a confirmed connection
        /// Simulates a scenario where internet connectivity is available
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator NetworkController_WhenConnected_ShouldReturnTrue()
        {
            //Arrange
            mockNetworkController.SimulateNetworkConnection = true;

            //Act
            mockNetworkController.IsNetworkAvailable();
            yield return null;

            //Assert
            Assert.IsTrue(mockNetworkController.IsConnected, "isConnected should be true when internet is available.");
        }

        /// <summary>
        /// Tests when there is no internet connection by setting an environment where it is unavailable
        /// Uses yield return null to simulate a frame
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator NetworkController_WhenDisconnected_ShouldReturnFalse()
        {
            //Arrange
            mockNetworkController.SimulateNetworkConnection = false;

            //Act
            mockNetworkController.IsNetworkAvailable();
            yield return null;

            //Assert
            Assert.IsFalse(mockNetworkController.IsConnected, "isConnected should be false when no internet connection.");
        }
    }

    /// <summary>
    /// Mock class that overrides the network connection behavior
    /// </summary>
    public class MockNetworkController : NetworkController
    {
        public bool SimulateNetworkConnection = false;

        public override bool IsConnectedToNetwork()
        {
            return SimulateNetworkConnection;
        }
    }
}