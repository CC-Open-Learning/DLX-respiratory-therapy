using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    public class GlossaryController : MonoBehaviour, IUserInterface
    {
        private VisualElement handbookRoot;
        private VisualElement tabs;
        private Button currentlyHighlightedButton;

        private Dictionary<string, string> medications = new();
        private Dictionary<string, string> generalSupplies = new();
        private Dictionary<string, string> bronchoscopySupplies = new();
        private Dictionary<string, string> bronchTowerSupplies = new();

        [SerializeField] private VisualTreeAsset elementTemplate;
        [SerializeField] private VisualTreeAsset categoryTemplate;
        private const string ExpandButtonClickedClass = "table-category-exapnd-button-clicked";
        private VisualElement currentlyOpenEntryHolder;
        private ScrollView currentScrollView;
        private float currentScrollViewOriginalHeight;
        
        private ScrollView medicationHolder;
        private ScrollView generalHolder;
        private ScrollView bronchSupplyHolder;
        private ScrollView bronchTowerHolder;
        private VisualElement expandedButton;
        private VisualElement table;

        private void Start()
        {
            handbookRoot = gameObject.GetComponentInParent<UIDocument>().rootVisualElement;
            table = handbookRoot.Q("GlossaryTable");
            tabs = handbookRoot.Q<VisualElement>("Tabs");
            currentlyHighlightedButton = tabs.Q<Button>("CategoryMedications");
            
            medicationHolder = table.Q<ScrollView>("MedicationHolder");
            generalHolder = table.Q<ScrollView>("GeneralHolder");
            bronchSupplyHolder = table.Q<ScrollView>("BronchSupplyHolder");
            bronchTowerHolder = table.Q<ScrollView>("BronchTowerHolder");

            tabs.Query<Button>(className: "button-category").ForEach(button =>
            {
                button.clicked += () => { HandleCategorySwitch(button); };
            });

            HandleCategorySwitch(currentlyHighlightedButton);
            SortExplorables();
        }

        public void GenerateProgress(ScrollView holder, Dictionary<string, string> tableItems)
        {
            holder?.Clear();
            int index = 0;
            currentScrollView = holder;

            //get table items from list
            foreach (KeyValuePair<string, string> item in tableItems)
            {
                string name = item.Key;
                string description = item.Value;

                VisualElement newScenario = categoryTemplate.CloneTree();
                VisualElement categoryNameHolder = newScenario.Q<VisualElement>("CategoryNameHolder");
                Label categoryLabel = newScenario.Q<Label>("CategoryNameLabel");
                categoryLabel.text = name;

                VisualElement background = newScenario.Q<VisualElement>("Background");
                Color backgroundColor = ColourBackground(index);
                background.style.backgroundColor = backgroundColor;
                categoryNameHolder.style.backgroundColor = backgroundColor;
                categoryLabel.style.backgroundColor = background.style.backgroundColor;

                VisualElement entryHolder = newScenario.Q("EntryHolder");
                Button expandButton = newScenario.Q<Button>("ExpandButton");
                expandButton.EnableInClassList(ExpandButtonClickedClass, true);
                expandButton.RegisterCallback<ClickEvent, VisualElement>(PlayExpandAnimation, expandButton);
                expandButton.RegisterCallback<ClickEvent, VisualElement>(ShowTasks, newScenario);

                //populate description inside
                VisualElement tableElementInstance = elementTemplate.Instantiate();
                VisualElement container = tableElementInstance.Q<VisualElement>("Container");
                container.RemoveFromClassList("justify-center");
                container.RemoveFromClassList("table-element-container");
                container.AddToClassList("table-element-label-container");
                tableElementInstance.style.backgroundColor = backgroundColor;

                Label elementLabel = tableElementInstance.Q<Label>("Label");
                elementLabel.text = description;
                elementLabel.AddToClassList("table-element-label");

                tableElementInstance.AddToClassList("table-element");
                entryHolder.Add(tableElementInstance);

                elementLabel.style.unityTextAlign = TextAnchor.UpperLeft;
                
                newScenario.AddToClassList("table-scenario");
                holder?.Add(newScenario);
                entryHolder.Hide();

                index++;
            }
        }

        private void ShowTasks(ClickEvent evt, VisualElement category)
        {
            var entryHolder = category.Q("EntryHolder");
            currentScrollViewOriginalHeight = currentScrollView.resolvedStyle.height;
            
            if (currentlyOpenEntryHolder == entryHolder)
            {
                entryHolder.Hide();
                currentlyOpenEntryHolder = null;
                return;
            }

            currentlyOpenEntryHolder?.Hide();
            entryHolder.Show();
            currentlyOpenEntryHolder = entryHolder;
            currentScrollView.style.height = new StyleLength(category.resolvedStyle.height + currentScrollViewOriginalHeight);
        }

        private void PlayExpandAnimation(ClickEvent evt, VisualElement expandButton)
        {
            if (expandedButton != null && expandedButton != expandButton)
                expandedButton.ToggleInClassList(ExpandButtonClickedClass);
            
            expandButton.ToggleInClassList(ExpandButtonClickedClass);
            expandedButton = expandButton;
            currentScrollView.style.height = new StyleLength(currentScrollViewOriginalHeight);
        }

        private static Color ColourBackground(int scenarioIndex)
        {
            Color bgColor = (scenarioIndex % 2 == 0)
                ? Color.black
                : new Color(45f / 255f, 45f / 255f, 45f / 255f);
            return bgColor;
        }
        
        private void SortExplorables()
        {
            var components = Resources.FindObjectsOfTypeAll<ExplorableInformationSO>();

            foreach (var explorableInfo in components)
            {
                var objectName = explorableInfo.ExplorableName;
                var objectDescription = explorableInfo.ExplorableDescription;

                switch (explorableInfo.ExplorableCategory)
                {
                    case ExplorableCategory.Medications:
                        medications[objectName] = objectDescription;
                        break;
                    case ExplorableCategory.GeneralSupplies:
                        generalSupplies[objectName] = objectDescription;
                        break;
                    case ExplorableCategory.BronchoscopySupplies:
                        bronchoscopySupplies[objectName] = objectDescription;
                        break;
                    case ExplorableCategory.BronchTower:
                        bronchTowerSupplies[objectName] = objectDescription;
                        break;
                }
            }
        }

        private void GenerateGlossary(string buttonName)
        {
            switch (buttonName)
            {
                case "CategoryMedications":
                    medicationHolder.style.display = DisplayStyle.Flex;
                    GenerateProgress(medicationHolder, medications);
                    break;
                case "CategoryGenSupplies":
                    generalHolder.style.display = DisplayStyle.Flex;
                    GenerateProgress(generalHolder, generalSupplies);
                    break;
                case "CategoryBronchSupplies":
                    bronchSupplyHolder.style.display = DisplayStyle.Flex;
                    GenerateProgress(bronchSupplyHolder, bronchoscopySupplies);
                    break;
                case "CategoryBronchTowerSupplies":
                    bronchTowerHolder.style.display = DisplayStyle.Flex;
                    GenerateProgress(bronchTowerHolder, bronchTowerSupplies);
                    break;
                default:
                    return;
            }
        }

        private void HandleCategorySwitch(Button button)
        {
            ChangeHighlightedCategory(button);
            
            medicationHolder.style.display = DisplayStyle.None;
            generalHolder.style.display = DisplayStyle.None;
            bronchSupplyHolder.style.display = DisplayStyle.None;
            bronchTowerHolder.style.display = DisplayStyle.None;
            
            GenerateGlossary(button.name);
        }

        private void ChangeHighlightedCategory(Button buttonToHighlight)
        {
            currentlyHighlightedButton?.AddToClassList("button-category");
            currentlyHighlightedButton?.AddToClassList("button-category-unselected");
            currentlyHighlightedButton?.RemoveFromClassList("button-category-selected");

            buttonToHighlight.RemoveFromClassList("button-category");
            buttonToHighlight.RemoveFromClassList("button-category-unselected");
            buttonToHighlight.AddToClassList("button-category-selected");
            currentlyHighlightedButton = buttonToHighlight;
        }

        public void Show()
        {
            GenerateGlossary("CategoryMedications");
        }

        public void Hide()
        {
        }
    }
}