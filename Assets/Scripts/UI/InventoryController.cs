using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    public class InventoryController : MonoBehaviour, IUserInterface
    {
        //Events
        [Header("Events")]
        /// Invokes <see cref="UIManager.ShowScreen(UIPanel)"/>
        public UnityEvent<UIPanel> ShowScreen = new();

        ///Invokes <see cref="UIManager.HideScreen(UIPanel)"/>
        public UnityEvent<UIPanel> HideScreen = new();

        ///Invokes <see cref="ObjectViewerPanelController.HandleObjectClick(GameObject)"/>
        public UnityEvent<GameObject> ShowObjectViewerPanel = new();

        ///Invokes <see cref="CustomSaveHandler.Save"/>
        public UnityEvent OnSaveItem = new();

        /// Invokes <see cref="OrientationController.HandleCategoryFocused(string)"/>
        public UnityEvent<string> OnCategoryChanged = new();

        //Lists of inventory objects
        [Header("Inventory Category Lists")]
        public List<GameObject> Medications = new();
        public List<GameObject> GeneralSupplies = new();
        public List<GameObject> BronchoscopySupplies = new();

        /*I would rather we just use a dictionary for everything and not have the 3 lists separately + a dictionary, but we cannot serialize a dictionary in the editor...
         * this can be changed in the future or we could use the package that lets you serialize dictionaries.
        */
        private readonly Dictionary<string, List<GameObject>> allCategories = new();

        //UI Doc things
        [Header("UI references")]
        public VisualTreeAsset Card;
        private VisualElement inventoryRoot;
        private Button closeButton;
        private Button currentlyHighlightedButton;
        private VisualElement scrollViewContent;
        private ScrollView scrollView;
        private Label categorySupplyAmount;
        private Label totalSuppliesFound;
        private ProgressBar progressBar;

        //Generic Variables
        public Sprite defaultPanelImage;

        private string defaultCardText = "";
        private bool isEmptyScrollView => scrollViewContent.childCount == 0;
        private int amountFound = 0;

        private GameObject currentlyActiveObject;

        void Start()
        {
            allCategories["Medications"] = Medications;
            allCategories["GenSupplies"] = GeneralSupplies;
            allCategories["BronchSupplies"] = BronchoscopySupplies;
            SetupUIReferences();
            SetupListeners();
            progressBar.highValue = GetTotalCount();
            inventoryRoot.Hide();
        }

        private void SetupUIReferences()
        {
            inventoryRoot = GetComponent<UIDocument>().rootVisualElement;
            closeButton = inventoryRoot.Q<TemplateContainer>().Q<Button>("CloseBtn");
            currentlyHighlightedButton = inventoryRoot.Q<Button>("CategoryMedications");
            scrollViewContent = inventoryRoot.Q<VisualElement>("ScrollviewContent");
            scrollView = inventoryRoot.Q<ScrollView>("ScrollView");
            categorySupplyAmount = inventoryRoot.Q<Label>("InventoryAmountOf");
            totalSuppliesFound = inventoryRoot.Q<Label>("TotalFoundText");
            progressBar = inventoryRoot.Q<ProgressBar>("ProgressBar");
        }

        private void SetupListeners()
        {
            inventoryRoot.Query<Button>(className: "button-category").ForEach(button =>
            {
                button.clicked += () =>
                {
                    HandleCategorySwitch(button);
                };
                button.RegisterCallback<PointerLeaveEvent>(OnMouseLeaveButton);
                button.RegisterCallback<PointerDownEvent>(
                    evt => {
                        OnMouseDownButton(evt);
                    },
                    TrickleDown.TrickleDown);

            });
            closeButton.clicked += () =>
            {
                ShowScreen?.Invoke(UIPanel.OrientationChecklist);
                ShowScreen?.Invoke(UIPanel.Toolbar);
                ShowScreen?.Invoke(UIPanel.POIToolbar);
                Hide();
            };
        }

        private void OnMouseDownButton(PointerDownEvent evt)
        {
            // Cast the event target to a Button
            if (evt.target is Button button)
            {
                button?.AddToClassList("button-category-select");
                button?.RemoveFromClassList("button-category");
                button?.RemoveFromClassList("button-category-unselected");
            }
        }

        private void OnMouseLeaveButton(PointerLeaveEvent evt)
        {
            // Cast the event target to a Button
            if (evt.target is Button button)
            {
                button?.RemoveFromClassList("button-category-select");
                button?.AddToClassList("button-category");
                button?.AddToClassList("button-category-unselected");
            }
        }
        /// <summary>
        /// Open the inventory screen and perform any operations that need to be done as it opens
        /// </summary>
        public void Show()
        {
            HideScreen?.Invoke(UIPanel.OrientationChecklist);
            HideScreen?.Invoke(UIPanel.Toolbar);
            HideScreen?.Invoke(UIPanel.POIToolbar);
            inventoryRoot.Show();
            UpdateUI();
            HandleCategorySwitch(currentlyHighlightedButton);
        }

        /// <summary>
        /// Hides the inventory screen and perform any operations that need to be done as it closes
        /// </summary>
        public void Hide()
        {
            inventoryRoot.Hide();
        }

        /// <summary>
        /// Highlights/unhighlights the corresponding cateogry buttons as they are clicked
        /// </summary>
        /// <param name="buttonToHighlight">The category button to be highlighted (the one we just clicked)</param>
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

        /// <summary>
        /// Handles the events to occur when a new category is clicked on inside the inventory
        /// </summary>
        /// <param name="button">The button that was clicked on</param>
        private void HandleCategorySwitch(Button button)
        {
            string categoryName = GetCategoryName(button);
            int amountOfSupplies = allCategories[categoryName].Count;
            OnCategoryChanged?.Invoke(GetProgressIndicatorCategoryName(button));
            ChangeHighlightedCategory(button);
            ResetScrollView();
            GeneratePanels(allCategories[categoryName]);
            UpdateAmountOfSuppliesText(amountOfSupplies);
        }

        private string GetCategoryName(Button button)
        {
            InventoryCategorySO inventoryCategorySO = button.dataSource as InventoryCategorySO;
            return inventoryCategorySO.InventoryCategory.ToString();
        }

        private string GetProgressIndicatorCategoryName(Button button)
        {
            InventoryCategorySO inventoryCategorySO = button.dataSource as InventoryCategorySO;
            return inventoryCategorySO.InventoryCategory.ToDescription();
        }

        /// <summary>
        /// Resets the scroll view to the top of the scrollbar when switching tabs
        /// </summary>
        private void ResetScrollView()
        {
            if (!isEmptyScrollView)
            {
                scrollView.ScrollTo(scrollViewContent.ElementAt(0));
            }
            scrollViewContent.Clear();
        }

        /// <summary>
        /// Handles the generation of panels when switching category tabs in the inventory
        /// </summary>
        /// <param name="gameObjects">List of gameobjects to show</param>
        private void GeneratePanels(List<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                if (!gameObject.TryGetComponent(out Explorable explorable)) { continue; }

                VisualElement card = Card.CloneTree();
                VisualElement clickableElement = card.Q<VisualElement>("Card");
                Label cardText = card.Q<Label>("CardText");
                VisualElement checkmark = card.Q<VisualElement>("Checkmark");
                VisualElement explorableImage = card.Q<VisualElement>("ExplorableImage");

                cardText.text = explorable.explorableInformation.ExplorableName ?? defaultCardText;
                checkmark.visible = explorable.IsExplored;

                explorableImage.style.backgroundImage = explorable.explorableInformation.ExplorableSprite != null ?
                    explorable.explorableInformation.ExplorableSprite.texture : defaultPanelImage.texture;


                clickableElement.RegisterCallback<ClickEvent>(evt =>
                {
                    HandlePanelClick(card, gameObject);
                });
                scrollViewContent.Add(card);
            }
        }


        /// <summary>
        /// Used to update the amount of supplies text
        /// </summary>
        /// <param name="amount">The amount of supplies</param>
        private void UpdateAmountOfSuppliesText(int amount)
        {
            categorySupplyAmount.text = $"{amount} supplies";
        }

        /// <summary>
        /// Used to update the total found text and color
        /// </summary>
        /// <param name="amountFound">The amount of supplies found</param>
        /// <param name="maxAmount">The total amount of supplies to find</param>
        private void UpdateTotalFoundTextAndColor(int amountFound, int maxAmount)
        {
            totalSuppliesFound.text = $"({amountFound}/{maxAmount} found)";

            string colorHex = StyleUtilities.InventoryNoProgress;
            if (amountFound > 0 && amountFound < maxAmount)
            {
                colorHex = StyleUtilities.InventoryInProgress;
            }
            if (amountFound >= maxAmount)
            {
                colorHex = StyleUtilities.InventoryComplete;
            }
            StyleUtilities.ChangeStyleColor(totalSuppliesFound.style, colorHex);
        }

        /// <summary>
        /// Handles the flow of events when clicking on a panel in the inventory
        /// </summary>
        /// <param name="card">The card that was clicked on</param>
        /// <param name="clickedObject">The corresponding object to the card</param>
        private void HandlePanelClick(VisualElement card, GameObject clickedObject)
        {
            if (!clickedObject.TryGetComponent(out Explorable explorable)) { return; }
            card.Q<VisualElement>("Checkmark").visible = true;
            SetActiveObject(clickedObject);
            ShowObjectViewerPanel?.Invoke(clickedObject);
            UpdateUI();
        }

        private int GetTotalExploredCount()
        {
            int total = 0;
            foreach (KeyValuePair<string, List<GameObject>> list in allCategories)
            {
                total += list.Value.Count(obj =>
                {
                    Explorable explorable = obj.GetComponent<Explorable>();
                    return explorable != null && explorable.IsExplored;
                });
            }
            return total;
        }

        /// <summary>
        /// Gets the total count of all categories
        /// </summary>
        private int GetTotalCount()
        {
            return Medications.Count + GeneralSupplies.Count + BronchoscopySupplies.Count;
        }

        /// <summary>
        /// Updates all Inventory UI that will be changing during runtime
        /// </summary>
        private void UpdateUI()
        {
            amountFound = GetTotalExploredCount();
            UpdateTotalFoundTextAndColor(amountFound, GetTotalCount());
            progressBar.value = amountFound;
        }

        private void SetActiveObject(GameObject obj)
        {
            obj.SetActive(true);
            currentlyActiveObject = obj;
        }

        public void ReleaseActiveObject()
        {
            if (currentlyActiveObject != null)
            {
                currentlyActiveObject.SetActive(false);
                currentlyActiveObject = null;
            }
        }
    }
}