using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    /// <summary>
    /// The stethoscope component manages the breathing checking scenario
    /// </summary>
    public class StethoscopeComponent : BiopsyProcedureComponentBase
    {
        private BiopsyController biopsyController;
        private Notification notificationInstance;
        private Prompt promptInstance;
        private SoundManager soundManager;
        private bool hasCheckedBreathing;
        private SoundPlayOptions breathingSoundPlayOptions;
        private Coroutine stethoscopeCoroutine;
        private const float CoroutineDelay = 0.01f;
        
        // Step scenario or task variables after breathing check is complete.
        [Space] [SerializeField]  private bool stepTask;
        [SerializeField] private bool stepScenario;
        
        // Game objects used to control the movement of the stethoscope.
        [Space] [SerializeField] [Tooltip("Stethoscope game object that will check the patients breathing")] private GameObject stethoscopeCursor;
        [SerializeField] private Transform stethoscopeTransformOrigin;
        
        // Notification and Prompt assets used during the breathing check sequence that are not related to a task.
        [Space] [Header("Stethoscope Notification Assets")] 
        [SerializeField] private NotificationSO breathingMissedNotification;
        [SerializeField] private NotificationSO breathingNotification;
        [SerializeField] private Sprite breathingNotificationIcon;
        [Header("Breathing Check Incorrect Choice Prompt")]
        [SerializeField] private PromptSO incorrectChoicePrompt;
        
        // Breathing sound effect.
        [Space] [SerializeField] AudioClip breathingSound;
        
        /// Invokes <see cref="BreathingControl.Show"/>
        [Space] public UnityEvent OnStethoscopeEnabled;
        /// Invokes <see cref="BreathingControl.Hide"/>
        public UnityEvent OnBreathingCheckComplete;
        
        private void Start()
        {
            OnStethoscopeEnabled ??= new UnityEvent();
            OnBreathingCheckComplete ??= new UnityEvent();
            
            // Setup sound options for breathing sound effect.
            breathingSoundPlayOptions = new SoundPlayOptions();
            breathingSoundPlayOptions.Loop = true;
            
            biopsyController = FindFirstObjectByType<BiopsyController>();
            notificationInstance = FindFirstObjectByType<Notification>();
            promptInstance = FindFirstObjectByType<Prompt>();
            soundManager = FindFirstObjectByType<SoundManager>();

            if (biopsyController == null || promptInstance == null || soundManager == null || notificationInstance == null)
            {
                Debug.LogWarning("Error: StethoscopeComponent.cs couldn't find a reference to a script");
            }

            stethoscopeCoroutine = null;
            stethoscopeCursor.transform.position = stethoscopeTransformOrigin.position;
        }

        /// <summary>
        /// Invoke event when the ItemHandlerTask requests the stethoscope as the active biopsy procedure equipment. 
        /// </summary>
        private void OnEnable()
        {
            OnStethoscopeEnabled?.Invoke();
        }
        
        /// <summary>
        /// Handles mouse enter events for breathing hotspots.
        /// Gets called from the Interactable events on each breathing hotspot.
        /// </summary>
        /// <param name="hotspot"> Breathing hotspot the mouse has entered. </param>
        public void OnMouseEnterBreathingHotspot(GameObject hotspot)
        {
            SetStethoscopeTransform(hotspot.transform);
        }
        
        /// <summary>
        /// Handles mouse exit events for breathing hotspots.
        /// Gets called from the Interactable events on each breathing hotspot.
        /// </summary>
        public void OnMouseExitBreathingHotspot()
        {
            soundManager.StopSound();
            SetStethoscopeTransform(stethoscopeTransformOrigin);
        }
        
        /// <summary>
        /// Displays breathing sound notification and uses the sounds manager to play the stethoscope breath audio effect.
        /// Called when the stethoscope reaches a breathing hotspot.
        /// </summary>
        private void OnStethoscopeEnterBreathingHotSpot()
        {
            if(!hasCheckedBreathing) { hasCheckedBreathing = true; }
            
            promptInstance.Hide();
            notificationInstance.SetCustomNotification(Color.gray, Color.white, breathingNotificationIcon);
            notificationInstance.HandleDisplayUI(breathingNotification);
            
            soundManager.PlaySound(breathingSound, breathingSoundPlayOptions);
        }
        
        /// <summary>
        /// Manages the coroutine that moves the stethoscope from its origin to each breathing hotspot.
        /// </summary>
        /// <param name="targetPosition"> The target position of the stethoscope </param>
        private void SetStethoscopeTransform(Transform targetPosition)
        {
            if (stethoscopeCoroutine != null)
            {
                StopCoroutine(stethoscopeCoroutine);
                stethoscopeCoroutine = null;
            }
            
            stethoscopeCoroutine = StartCoroutine(TranslateStethoscopeTransform(targetPosition));
        }

        /// <summary>
        /// Moves the stethoscope to a desired position over time.
        /// </summary>
        /// <param name="targetPosition"> The target position of the stethoscope </param>
        private IEnumerator TranslateStethoscopeTransform(Transform targetPosition)
        {
            Vector3 targetRotation = targetPosition.rotation.eulerAngles;
            
            while (stethoscopeCursor.transform.position != targetPosition.position || stethoscopeCursor.transform.rotation != Quaternion.Euler(targetRotation))
            {
                stethoscopeCursor.transform.position = Vector3.MoveTowards(stethoscopeCursor.transform.position, targetPosition.position, CoroutineDelay);
                stethoscopeCursor.transform.rotation = Quaternion.RotateTowards(stethoscopeCursor.transform.rotation, Quaternion.Euler(targetRotation), 1f);
                
                yield return new WaitForSeconds(CoroutineDelay);
            }
            
            if (targetPosition != stethoscopeTransformOrigin)
            {
                OnStethoscopeEnterBreathingHotSpot();
            }
        }

        /// <summary>
        /// Validates if the correct option for the breathing check is selected.
        /// If no hotspots have been checked, a notification with instructions will appear.
        /// </summary>
        /// <param name="isProceed"> If the correct 'proceed' button was selected </param>
        public void ValidateProceedOrPause(bool isProceed)
        {
            if (!hasCheckedBreathing)
            {
                promptInstance.Hide();
                notificationInstance.HandleDisplayUI(breathingMissedNotification);
                return;
            }
            
            if (isProceed)
            {
                OnBreathingCheckComplete?.Invoke();
                if (stepTask)
                {
                    biopsyController.UpdateScenarioTaskIndex();
                }
                if (stepScenario)
                {
                    biopsyController.UpdateScenarioIndex();
                }
            }
            else
            {
                promptInstance.HandleDisplayUI(incorrectChoicePrompt);
            }
        }
    }
}