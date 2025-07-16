using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    public class ProgressTableController : MonoBehaviour, IUserInterface
    {
        public List<ScenarioTasksSO> BiopsyScenarios = new List<ScenarioTasksSO>();

        [SerializeField] private VisualTreeAsset categoryTemplate;
        [SerializeField] private VisualTreeAsset entryTemplate;
        [SerializeField] private VisualTreeAsset elementTemplate;
        [SerializeField] private VisualTreeAsset headerLabelTemplate;
        [SerializeField] private VisualTreeAsset nodeCompletedTemplate;
        [SerializeField] private VisualTreeAsset nodeInProgressTemplate;
        [SerializeField] private VisualTreeAsset nodeNotStartedTemplate;
        [SerializeField] private VisualTreeAsset nodeStepperInProgressTemplate;
        [SerializeField] private VisualTreeAsset nodeStepperCompletedTemplate;
        [SerializeField] private VisualTreeAsset nodeStepperNotStartedTemplate;

        private VisualElement handbookRoot;
        private VisualElement table;
        private ScrollView categoryHolder;
        private float scrollViewOriginalHeight;
        private VisualElement currentlyOpenEntryHolder;
        private VisualElement completeIndicator;
        private const string ExpandButtonClickedClass = "table-category-exapnd-button-clicked";
        private VisualElement expandedButton;
        
        private void Start()
        {
            handbookRoot = gameObject.GetComponentInParent<UIDocument>().rootVisualElement;

            table = handbookRoot.Q("Table");
            categoryHolder = table.Q<ScrollView>("CategoryHolder");
            ResetProgress();
        }

        public void GenerateProgress()
        {
            categoryHolder?.Clear();
            int scenarioIndex = 0;

            //populate scenarios
            foreach (ScenarioTasksSO scenario in BiopsyScenarios)
            {
                if (scenario.ShowInHandbook)
                {
                    VisualElement newScenario = categoryTemplate.CloneTree();
                    VisualElement categoryNameHolder = newScenario.Q<VisualElement>("CategoryNameHolder");
                    Label categoryLabel = newScenario.Q<Label>("CategoryNameLabel");
                    categoryLabel.text = scenario.ScenarioName;
                    
                    VisualElement background = newScenario.Q<VisualElement>("Background");
                    Color backgroundColor = ColourBackground(scenarioIndex);
                    background.style.backgroundColor = backgroundColor;
                    categoryNameHolder.style.backgroundColor = backgroundColor;
                    categoryLabel.style.backgroundColor = background.style.backgroundColor;

                    bool allTasksCompleted = scenario.ScenarioTaskList
                        .Where(task => task is not POIHandlerTaskSO && task is not PromptTaskSO && task is not LockPOITaskSO)
                        .All(task => task.IsExecuted);

                    if (allTasksCompleted)
                    {
                        Label completeLabel = new Label("Complete");
                        completeLabel.AddToClassList("scenario-complete-label");
                        background.Add(completeLabel);
                    }
                    
                    VisualElement entryHolder = newScenario.Q("EntryHolder");
                    Button expandButton = newScenario.Q<Button>("ExpandButton");
                    expandButton.EnableInClassList(ExpandButtonClickedClass, true);
                    expandButton.RegisterCallback<ClickEvent, VisualElement>(PlayExpandAnimation, expandButton);
                    expandButton.RegisterCallback<ClickEvent, VisualElement>(ShowTasks, newScenario);

                    //populate tasks
                    GetTaskProgress(scenario, entryHolder, backgroundColor);

                    newScenario.AddToClassList("table-scenario");
                    categoryHolder?.Add(newScenario);
                    entryHolder.Hide();
                    
                    scenarioIndex++;
                }
            }
        }

        private void GetTaskProgress(ScenarioTasksSO scenario, VisualElement entryHolder, Color backgroundColor)
        {
            int rowIndex = 0;
            
            foreach (var task in scenario.ScenarioTaskList)
            {
                if (task is not POIHandlerTaskSO && task is not PromptTaskSO && task is not LockPOITaskSO)
                {
                    VisualElement tableElementInstance = elementTemplate.Instantiate();
                    VisualElement container = tableElementInstance.Q<VisualElement>("Container");
                    container.RemoveFromClassList("justify-center");
                    tableElementInstance.style.backgroundColor = backgroundColor;
                    
                    Label elementLabel = tableElementInstance.Q<Label>("Label");
                    elementLabel.text = task.Description;
                    elementLabel.AddToClassList("table-element-label");

                    if (task.IsExecuted)
                    {
                        completeIndicator = nodeCompletedTemplate.Instantiate();
                        completeIndicator.Q<Label>("NodeLabel").text = "";
                        
                        VisualElement checkContainer = completeIndicator.Q<VisualElement>("Node");
                        checkContainer.AddToClassList("check-container");
                        checkContainer.RemoveFromClassList("node");
                        tableElementInstance.Add(completeIndicator);
                    }

                    tableElementInstance.AddToClassList("table-element");
                    entryHolder.Add(tableElementInstance);
                    
                    rowIndex++;
                }
            }
        }

        public void UpdateNodeStepper(string nodeContainer, string stepContainer, string status)
        {
            VisualElement replacementNode = null;
            VisualElement replacementStepper = null;
            
            VisualElement parentContainer = handbookRoot.Q<VisualElement>(nodeContainer);
            VisualElement nodeToReplace = parentContainer.Q<VisualElement>("Node");

            VisualElement stepperContainer = handbookRoot.Q<VisualElement>(stepContainer);
            VisualElement stepToReplace = stepperContainer.Q<VisualElement>("Step");

            if (status == "InProgress")
            {
                VisualElement inProgressTemplate = nodeInProgressTemplate.CloneTree();
                VisualElement inProgressStepTemplate = nodeStepperInProgressTemplate.CloneTree();
                replacementNode = inProgressTemplate.Q<VisualElement>("Node");
                replacementStepper = inProgressStepTemplate.Q<VisualElement>("Step");
            }
            else if (status == "Complete")
            {
                VisualElement completeIndicator = nodeCompletedTemplate.CloneTree();
                VisualElement completeStepTemplate = nodeStepperCompletedTemplate.CloneTree();
                replacementNode = completeIndicator.Q<VisualElement>("Node");
                replacementStepper = completeStepTemplate.Q<VisualElement>("Step");
            } 
            else if (status == "NotStarted")
            {
                VisualElement notStartedIndicator = nodeNotStartedTemplate.CloneTree();
                VisualElement notStartedStepTemplate = nodeStepperNotStartedTemplate.CloneTree();
                replacementNode = notStartedIndicator.Q<VisualElement>("Node");
                replacementStepper = notStartedStepTemplate.Q<VisualElement>("Step");
            }

            int index = parentContainer.IndexOf(nodeToReplace);
            parentContainer.RemoveAt(index);
            parentContainer.Insert(index, replacementNode);
            stepperContainer.Remove(stepToReplace);
            stepperContainer.Add(replacementStepper);
        }

        private void ShowTasks(ClickEvent evt, VisualElement category)
        {
            var entryHolder = category.Q("EntryHolder");
            scrollViewOriginalHeight = categoryHolder.resolvedStyle.height;

            if (currentlyOpenEntryHolder == entryHolder)
            {
                entryHolder.Hide();
                currentlyOpenEntryHolder = null;
                return;
            }

            currentlyOpenEntryHolder?.Hide();
            entryHolder.Show();
            currentlyOpenEntryHolder = entryHolder;
            categoryHolder.style.height = new StyleLength(category.resolvedStyle.height + scrollViewOriginalHeight);
        }

        private void PlayExpandAnimation(ClickEvent evt, VisualElement expandButton)
        {
            if (expandedButton != null && expandedButton != expandButton)
                expandedButton.ToggleInClassList(ExpandButtonClickedClass);
            
            expandButton.ToggleInClassList(ExpandButtonClickedClass);
            expandedButton = expandButton;
            categoryHolder.style.height = new StyleLength(scrollViewOriginalHeight);
        }

        private static Color ColourBackground(int scenarioIndex)
        {
            Color bgColor = (scenarioIndex % 2 == 0) 
                ? Color.black 
                : new Color(45f / 255f, 45f / 255f, 45f / 255f);
            return bgColor;
        }

        public void ResetProgress()
        {
            foreach (ScenarioTasksSO scenario in BiopsyScenarios)
            {
                foreach (var task in scenario.ScenarioTaskList)
                {
                    task.IsExecuted = false;
                }
            }
        }
        
        public void Show()
        {
            GenerateProgress();
        }

        public void Hide()
        {
        }
    }
}