using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    public class HandbookController : MonoBehaviour, IUserInterface
    {
        private VisualElement handbookRoot;
        private const string DimmedBackgroundClass = "information-dialog-canvas";
        
        private Button closeBtn;
        private Button currentlyHighlightedButton;

        private VisualElement progressContainer;
        private VisualElement glossaryContainer;
        private VisualElement tableContainer;
        private VisualElement monitorContainer;
        private List<VisualElement> categoryContainers;

        [SerializeField] private ProgressTableController progressTableController;
        [SerializeField] private GlossaryController glossaryController;
        [SerializeField] private BiopsyController biopsyController;
        
        [Header("Events")]
        /// Invokes <see cref="ToolbarController.Hide"/>
        public UnityEvent<bool> OnHandbookShown;
        /// Invokes <see cref="ProcedurePOIToolbar.Show"/>
        /// Invokes <see cref="ToolbarController.Show"/>
        public UnityEvent<bool> OnHandbookHidden;

        private void Awake()
        {
            handbookRoot = GetComponent<UIDocument>().rootVisualElement;

            categoryContainers = new List<VisualElement>();
            progressContainer = handbookRoot.Q<VisualElement>("ProgressContainer");
            categoryContainers.Add(progressContainer);
            glossaryContainer = handbookRoot.Q<VisualElement>("GlossaryContainer");
            categoryContainers.Add(glossaryContainer);
            tableContainer = handbookRoot.Q<VisualElement>("TableContainer");
            categoryContainers.Add(tableContainer);
            monitorContainer = handbookRoot.Q<VisualElement>("MonitorContainer");
            categoryContainers.Add(monitorContainer);
            
            closeBtn = handbookRoot.Q<Button>("CloseBtn");
            closeBtn.clicked += Hide;
            
            handbookRoot.Hide();
        }

        private void Start()
        {
            OnHandbookShown ??= new UnityEvent<bool>();
            OnHandbookHidden ??= new UnityEvent<bool>();

            handbookRoot.Q<VisualElement>("TabVertical").Query<Button>().ForEach(button =>
            {
                button.clicked += () => HandleCategorySwitch(button);
            });

            progressTableController.BiopsyScenarios = biopsyController.ScenarioList;
        }

        private void HandleCategorySwitch(Button button)
        {
            ChangeHighlightedCategory(button);
            ChangeContentCategory(button.name);
        }

        public void Show()
        {
            if (currentlyHighlightedButton == null || currentlyHighlightedButton.name == "ProgressButton")
            {
                currentlyHighlightedButton = handbookRoot.Q<Button>("ProgressButton");
                HandleCategorySwitch(currentlyHighlightedButton);
            }
            OnHandbookShown?.Invoke(true);
            handbookRoot.Show();
            handbookRoot.EnableInClassList(DimmedBackgroundClass, true);
        }

        public void Hide()
        {
            OnHandbookHidden?.Invoke(false);
            handbookRoot.Hide();
        }
        
        private void ChangeHighlightedCategory(Button buttonToHighlight)
        {
            currentlyHighlightedButton?.AddToClassList("handbook-button-category-unselected");
            currentlyHighlightedButton?.RemoveFromClassList("handbook-button-category-selected");
            
            buttonToHighlight?.RemoveFromClassList("handbook-button-category-unselected");
            buttonToHighlight?.AddToClassList("handbook-button-category-selected");

            currentlyHighlightedButton = buttonToHighlight;
        }

        private void ChangeContentCategory(string categoryButtonName)
        {
            VisualElement newCategoryContainer;
            switch (categoryButtonName)
            {
                case "ProgressButton":
                    newCategoryContainer = progressContainer;
                    progressTableController.Show();
                    break;
                case "GlossaryButton":
                    newCategoryContainer = glossaryContainer;
                    glossaryController.Show();
                    break;
                case "TableButton":
                    newCategoryContainer = tableContainer;
                    break;
                case "MonitorButton":
                    newCategoryContainer = monitorContainer;
                    break;
                default:
                    newCategoryContainer = progressContainer;
                    break;
            }

            foreach (VisualElement container in categoryContainers)
            {
                container.style.display = container == newCategoryContainer ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }
}