using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;
using VARLab.Velcro;

namespace Tests.PlayMode
{
    public class StethoscopeComponentIntegrationTests
    {
        //Path to scene
        private readonly string scenePath = "Assets/Scenes/BiopsyComponentTestScene.unity";
        private const float MovementDelay = 0.5f;

        private AudioSource audioSource;

        private VisualElement breathingControl;
        private Label promptMessage;
        private Label notificationMessage;

        private StethoscopeComponent stethoscopeComponent;
        private GameObject stethoscopeCursor;
        private GameObject stethoscopeHotspot;

        private Vector3 initialPosition;
        private Vector3 breathingHotspotPosition;

        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            //Wait for scene to load
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));

            //Assign everything
            audioSource = Object.FindAnyObjectByType<AudioSource>();

            breathingControl = Object.FindAnyObjectByType<BreathingControl>().GetComponent<UIDocument>().rootVisualElement;
            promptMessage = Object.FindAnyObjectByType<Prompt>().GetComponent<UIDocument>().rootVisualElement.Q<Label>("MessageLabel");
            notificationMessage = Object.FindAnyObjectByType<Notification>().GetComponent<UIDocument>().rootVisualElement.Q<Label>("Text");

            stethoscopeComponent = Object.FindAnyObjectByType<StethoscopeComponent>();
            stethoscopeCursor = GameObject.Find("Cursor");
            stethoscopeHotspot = Object.FindAnyObjectByType<StethoscopeBreathingHotspot>().gameObject;    

            initialPosition = stethoscopeCursor.transform.position;
            breathingHotspotPosition = stethoscopeHotspot.transform.position;

            //Hide breathing control, so StethoscopeComponent is treated the same way it is if it's disabled on scene start
            breathingControl.style.display = DisplayStyle.None;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            audioSource = null;

            breathingControl = null;
            promptMessage = null;
            notificationMessage = null;

            stethoscopeComponent = null;
            stethoscopeCursor = null;
            stethoscopeHotspot = null;

            yield return null;
        }

        [UnityTest]
        public IEnumerator OnEnable_ShouldShowBreathingControl()
        {
            // Arrange 
            stethoscopeComponent.gameObject.SetActive(false);

            // Act
            stethoscopeComponent.gameObject.SetActive(true);
            yield return null;

            // Assert
            Assert.IsTrue(DisplayStyle.Flex == breathingControl.style.display);
        }

        [UnityTest]
        public IEnumerator OnMouseEnterBreathingHotspot_ShouldMoveStethoscopeToBreathingHotspot()
        {
            // Act
            stethoscopeComponent.OnMouseEnterBreathingHotspot(stethoscopeHotspot);
            yield return new WaitForSecondsRealtime(MovementDelay); //Wait for movement to finish

            // Assert
            Assert.AreEqual(breathingHotspotPosition, stethoscopeCursor.transform.position);
        }

        [UnityTest]
        public IEnumerator OnMouseEnterBreathingHotspot_ShouldShowBreathingNotification()
        {
            // Arrange
            const string ExpectedMessage = "*Patient breathing patterns sound*";

            // Act
            stethoscopeComponent.OnMouseEnterBreathingHotspot(stethoscopeHotspot);
            yield return new WaitForSecondsRealtime(MovementDelay); //Wait for movement to finish

            // Assert
            Assert.AreEqual(ExpectedMessage, notificationMessage.text);
        }

        [UnityTest]
        public IEnumerator OnMouseEnterBreathingHotspot_ShouldPlayBreathingSound()
        {
            // Act
            stethoscopeComponent.OnMouseEnterBreathingHotspot(stethoscopeHotspot);
            yield return new WaitForSecondsRealtime(MovementDelay); //Wait for movement to finish

            // Assert
            Assert.IsTrue(audioSource.isPlaying);
        }

        [UnityTest]
        public IEnumerator OnMouseExitBreathingHotspot_ShouldMoveStethoscopeToInitialPosition()
        {
            // Act
            stethoscopeComponent.OnMouseExitBreathingHotspot();
            yield return new WaitForSecondsRealtime(MovementDelay); //Wait for movement to finish

            // Assert
            Assert.IsTrue(stethoscopeCursor.transform.position == initialPosition);
        }

        [UnityTest]
        public IEnumerator OnMouseExitBreathingHotspot_ShouldStopBreathingSound()
        {
            // Act
            stethoscopeComponent.OnMouseExitBreathingHotspot();
            yield return new WaitForSecondsRealtime(MovementDelay); //Wait for movement to finish

            // Assert
            Assert.IsTrue(!audioSource.isPlaying);
        }

        [UnityTest]
        public IEnumerator ValidateProceedOrPause_Proceed_ShouldHideBreathingControl()
        {
            // Arrange 
            stethoscopeComponent.OnMouseEnterBreathingHotspot(stethoscopeHotspot);
            yield return new WaitForSecondsRealtime(MovementDelay); //Wait for movement to finish

            // Act
            stethoscopeComponent.ValidateProceedOrPause(true);
            yield return null; //Wait for one frame so UI can update

            // Assert
            Assert.IsTrue(DisplayStyle.None == breathingControl.style.display);
        }

        [UnityTest]
        public IEnumerator ValidateProceedOrPause_Pause_ShouldShowIncorrectChoicePrompt()
        {
            // Arrange 
            const string ExpectedMessage = "Try checking again.";
            stethoscopeComponent.OnMouseEnterBreathingHotspot(stethoscopeHotspot);
            yield return new WaitForSecondsRealtime(MovementDelay); //Wait for movement to finish

            // Act
            stethoscopeComponent.ValidateProceedOrPause(false);
            yield return null; //Wait for one frame so UI can update

            // Assert
            Assert.AreEqual(ExpectedMessage, promptMessage.text);
        }

        [UnityTest]
        public IEnumerator ValidateProceedOrPause_HasNotCheckedBreathing_ShouldShowBreathingMissedNotification()
        {
            // Arrange 
            const string ExpectedMessage = "Hover your mouse over the patients chest to check their breathing";

            // Act
            stethoscopeComponent.ValidateProceedOrPause(true);
            yield return null; //Wait for one frame so UI can update

            // Assert
            Assert.AreEqual(ExpectedMessage, notificationMessage.text);
        }
    }
}
