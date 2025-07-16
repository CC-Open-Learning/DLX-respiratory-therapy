using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VARLab.Analytics;

namespace VARLab.RespiratoryTherapy
{
    public class BiopsyController : MonoBehaviour
    {
        private int scenarioIndex = 0;
        private int scenarioTaskIndex = 0;
        private bool isBiopsyPlaying;
        private UIManager uiManager;
        private LearnerSessionHandler learnerSessionHandler;
        private List<BiopsyBaseTaskSO> scenarioTasks;
        private POIManager poiManager;
        private bool isDelayCoroutineRunning;

        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject biopsyParentObject;
        [SerializeField] private GameObject defaultBiopsyCamera;

        public OrientationController OrientationController;
        public BronchMonitorController BronchMonitorController;
        public StethoscopeComponent StethoscopeComponent;
        public PromptSO CurrentTaskPrompt;
        public ProgressTableController ProgressTableController;

        [Header("Scenario Sequence")] public List<ScenarioTasksSO> ScenarioList = new();

        [Header("Events")] [Tooltip("CameraManager.SetDefaultCamera(GameObject)")]
        public UnityEvent<GameObject> OnBiopsyStarted;

        [Tooltip("LevelSelectController.SwitchRooms()")]
        public UnityEvent ChangeRoom;

        [Tooltip("BiopsyProcedureComponents.ResetBiopsyProcedureComponents()")]
        public UnityEvent OnResetBiopsy;

        private void Start()
        {
            scenarioTasks = new();
            OnBiopsyStarted ??= new UnityEvent<GameObject>();
            poiManager = FindFirstObjectByType<POIManager>();

            uiManager = UIManager.Instance;
            learnerSessionHandler = LearnerSessionHandler.Instance;

            isBiopsyPlaying = false;
            isDelayCoroutineRunning = false;
        }

        public void StartBiopsy()
        {
            POIManager.Instance.ResetPOIs();
            ProcessScenario();
            HandleBiopsyToolbar();
        }

        public void RestartBiopsy()
        {
            OnResetBiopsy?.Invoke();
            scenarioIndex = 0;
            scenarioTaskIndex = 0;
            isBiopsyPlaying = false;
            ProgressTableController.ResetProgress();
            BronchMonitorController.SetMonitorMaterial();
            learnerSessionHandler.SetUserStatus(false);
        }

        public void UnloadBiopsy()
        {
            POIManager.Instance.ResetPOIs();
            scenarioIndex = 0;
            scenarioTaskIndex = 0;
            UIManager.Instance.HideScreen(UIPanel.Prompt);
        }

        private void HandleBiopsyToolbar()
        {
            uiManager.SetUpToolbar(new ToolbarConfig() { ShowMenuButton = true, ShowHandbookButton = true });
            uiManager.ShowScreen(UIPanel.Toolbar);
            uiManager.ToggleProcedureToolbar(true);
            uiManager.ShowScreen(UIPanel.ProcedureToolbar);
        }

        /// <summary>
        /// Processes the current scenario by executing its tasks 
        /// and then moves to the next scenario in the list.
        /// </summary>
        private void ProcessScenario()
        {
            //trigger restart after last scenario
            if (scenarioIndex == ScenarioList.Count)
            {
                OnResetBiopsy?.Invoke();
                scenarioIndex = 0;
                scenarioTaskIndex = 0;
                BronchMonitorController.SetMonitorMaterial();
                StethoscopeComponent.gameObject.SetActive(false);
                ProgressTableController.ResetProgress();
                StartBiopsy();
                return;
            }
            
            UpdateHandbookProgressStepper();

            scenarioTasks.Clear();
            scenarioTasks.AddRange(ScenarioList[scenarioIndex].ScenarioTaskList);
            ProcessScenarioTasks();
        }

        /// <summary>
        /// Executes the current task in the given list of scenario tasks.
        /// </summary>
        private void ProcessScenarioTasks()
        {
            if (scenarioTasks[scenarioTaskIndex]?.HideSpeakerUI == true) HideToasts();
            if (scenarioTasks[scenarioTaskIndex] is PromptTaskSO currentPromptTask)
                CurrentTaskPrompt = currentPromptTask.PromptSO;

            scenarioTasks[scenarioTaskIndex].Execute();
            scenarioTasks[scenarioTaskIndex].IsExecuted = true;

            if (scenarioTasks[scenarioTaskIndex].TaskIndexIncrement)
            {
                UpdateScenarioTaskIndex();
            }

            var shouldAutoIncrementScenario = scenarioTaskIndex == scenarioTasks.Count - 1 &&
                                              scenarioTasks[scenarioTaskIndex].ScenarioIndexIncrement;
            if (shouldAutoIncrementScenario)
            {
                if (!isDelayCoroutineRunning)
                {
                    StartCoroutine(DelayedScenarioUpdate(2f));
                }
            }
        }

        /// <summary>
        /// Manages the learner's session based on their status.
        /// If the user is a first-time learner, updates their status accordingly. 
        /// Otherwise, loads the user's previous progress.
        /// </summary>
        private void ManageUserSession()
        {
            if (learnerSessionHandler.IsFirstTimeUser)
            {
                learnerSessionHandler.SetUserStatus(true);
                CoreAnalytics.CustomEvent("new_user_session", "biopsy", 1);
            }
            else
            {
                CoreAnalytics.CustomEvent("existing_user_session", "biopsy", 1);
            }
        }

        private IEnumerator DelayedScenarioUpdate(float delay)
        {
            isDelayCoroutineRunning = true;
            yield return new WaitForSeconds(delay);
            UpdateScenarioIndex();
        }

        /// <summary>
        /// Increments the scenario task index if it has not reached the last valid index.
        /// </summary>
        /// <param name="listItemCount">Total number of items in the scenario task list.</param>
        /// <param name="index">Reference to the current task index, which will be updated.</param>
        private void UpdateIndex(int listItemCount, ref int index)
        {
            if (index < listItemCount)
            {
                index++;
            }
        }

        public void UpdateScenarioIndex()
        {
            scenarioTaskIndex = 0;
            UpdateIndex(ScenarioList.Count, ref scenarioIndex);
            ProcessScenario();
            isDelayCoroutineRunning = false;
        }

        public void UpdateScenarioTaskIndex()
        {
            UpdateIndex(scenarioTasks.Count, ref scenarioTaskIndex);
            ProcessScenarioTasks();
        }

        public void InitializeBiopsy()
        {
            StartCoroutine(InitializeBiopsySequence());
        }

        private IEnumerator InitializeBiopsySequence()
        {
            const float cameraTransitionDelay = 1.0f;
            const float loadingScreenDelay = 3.0f;

            if (!isBiopsyPlaying)
            {
                isBiopsyPlaying = true;
                uiManager.ShowLoadingScreen?.Invoke(loadingScreen);
                yield return new WaitForSeconds(cameraTransitionDelay);
                uiManager.HideScreen(UIPanel.MainMenu);
                uiManager.HideScreen(UIPanel.LevelSelectController);
                ChangeRoom?.Invoke();
                OnBiopsyStarted?.Invoke(defaultBiopsyCamera);
                yield return new WaitForSeconds(loadingScreenDelay);
            }
            else
            {
                uiManager.HideScreen(UIPanel.MainMenu);
                uiManager.HideScreen(UIPanel.LevelSelectController);
            }

            StartBiopsy();
            ManageUserSession();
            poiManager.ShowOutlineableWhenOnBronchoscopePOI(false);
        }

        public void SetBiopsyPlayStatus(bool status)
        {
            isBiopsyPlaying = status;
        }
        
        private void UpdateHandbookProgressStepper()
        {
            var steps = new[]
            {
                new { Node = "NodeContainerOne", Step = "StepContainerOne", InProgressIndex = 0, CompleteIndex = 7 },
                new { Node = "NodeContainerTwo", Step = "StepContainerTwo", InProgressIndex = 7, CompleteIndex = 10 },
                new { Node = "NodeContainerThree", Step = "StepContainerThree", InProgressIndex = 10, CompleteIndex = 13 },
                new { Node = "NodeContainerFour", Step = "StepContainerFour", InProgressIndex = 13, CompleteIndex = 17 },
                new { Node = "NodeContainerFive", Step = "StepContainerFive", InProgressIndex = 17, CompleteIndex = int.MaxValue }
            };

            for (int i = 0; i < steps.Length; i++)
            {
                if (scenarioIndex < steps[i].InProgressIndex)
                {
                    ProgressTableController.UpdateNodeStepper(steps[i].Node, steps[i].Step, "NotStarted");
                }
                else if (scenarioIndex >= steps[i].InProgressIndex && scenarioIndex < steps[i].CompleteIndex)
                {
                    ProgressTableController.UpdateNodeStepper(steps[i].Node, steps[i].Step, "InProgress");
                }
                else
                {
                    ProgressTableController.UpdateNodeStepper(steps[i].Node, steps[i].Step, "Complete");
                }
            }
        } 
        
        /// <summary>
        /// Hide Both UI Elements so one doesnt overlap the other
        /// </summary>
        private void HideToasts()
        {
            uiManager.HideScreen(UIPanel.Notification);
            uiManager.HideScreen(UIPanel.Prompt);
        }
    }
}