using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;
using Button = UnityEngine.UIElements.Button;

namespace Tests.PlayMode
{
    public class InventoryControllerTests
    {
        private GameObject inventoryObject;
        private GameObject testPrefabOne;
        private GameObject testPrefabTwo;
        private GameObject uiManagerObject;
        private InventoryController inventoryController;
        private VisualElement inventoryRoot;

        private Button firstCategory;
        private Button secondCategory;
        private Button thirdCategory;

        private ProgressBar progressBar;
        private VisualElement panel;
        private VisualElement checkmark;

        private Label cardText;
        private Label totalFoundText;
        private Sprite defaultImage;
        private Sprite customImage;

        private int sceneCounter = 0;

        [UnitySetUp]
        [Category("BuildServer")]
        public IEnumerator Setup()
        {
            sceneCounter = TestUtils.ClearScene(sceneCounter, "InventoryControllerScene");

            //Load things for UI Doc
            var uxmlTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/UI Toolkit/Templates/Inventory/Inventory.uxml");
            var cardTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
               "Assets/UI Toolkit/Templates/Inventory/InventoryCard.uxml");
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(
              "Assets/UI Toolkit/Common/RT Panel Settings.asset");

            defaultImage = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Images/Inventory Item Thumbnails/18-Gauge Dispensing Needle.png");
            customImage = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/Common/Back_Arrow_Sprite.png");

            //Setup UI + GameObjects
            uiManagerObject = new GameObject("UIManager");
            uiManagerObject.AddComponent<UIManager>();
            UIDocument toolbarUIDocument = uiManagerObject.AddComponent<UIDocument>();
            VisualTreeAsset toolbarUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Toolbars/Toolbar.uxml");
            toolbarUXML.CloneTree(toolbarUIDocument.rootVisualElement);
            UIManager.Instance.ToolbarController = uiManagerObject.AddComponent<ToolbarController>();

            inventoryObject = new();
            testPrefabOne = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Tests/Prefabs/Inventory/InventoryTestPrefabOne.prefab");
            testPrefabOne.GetComponent<Explorable>().IsExplored = false;

            testPrefabTwo = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Tests/Prefabs/Inventory/InventoryTestPrefabTwo.prefab");
            testPrefabTwo.GetComponent<Explorable>().IsExplored = false;

            var uiDoc = inventoryObject.AddComponent<UIDocument>();
            uiDoc.panelSettings = panelSettings;
            uiDoc.visualTreeAsset = uxmlTemplate;
            inventoryRoot = uiDoc.rootVisualElement;
            inventoryController = inventoryObject.AddComponent<InventoryController>();
            yield return null;

            inventoryController.Medications.Add(testPrefabOne);
            inventoryController.GeneralSupplies.Add(testPrefabOne);
            inventoryController.BronchoscopySupplies.Add(testPrefabOne);
            inventoryController.defaultPanelImage = defaultImage;

            inventoryController.Card = cardTemplate;

            totalFoundText = inventoryRoot.Q<Label>("TotalFoundText");
            firstCategory = inventoryRoot.Q<Button>("CategoryMedications");
            secondCategory = inventoryRoot.Q<Button>("CategoryGenSupplies");
            thirdCategory = inventoryRoot.Q<Button>("CategoryBronchSupplies");

            progressBar = inventoryRoot.Q<ProgressBar>("ProgressBar");
        }

        [UnityTearDown]
        [Category("BuildServer")]
        public void TearDown()
        {
            uiManagerObject = null;
            UIManager.Instance = null;
            inventoryObject = null;
            inventoryController = null;
            firstCategory = null;
            secondCategory = null;
            thirdCategory = null;
            testPrefabOne = null;
            testPrefabTwo = null;
            defaultImage = null;
            customImage = null;
        }

        /// <summary>
        /// Test ensures when you attempt to show the inventory, that it actually shows
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Show_should_Open_Inventory_Panel()
        {
            //Act
            inventoryController.Show();
            yield return null;

            Assert.AreEqual((StyleEnum<DisplayStyle>)DisplayStyle.Flex, inventoryRoot.style.display);
        }

        /// <summary>
        /// Test ensures when you attempt to hide the inventory, that it actually hides
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_HideInventory_PanelIsHidden()
        {

            //Act
            //Show then hide panel
            inventoryController.Show();
            yield return null;

            inventoryController.Hide();
            yield return null;

            Assert.AreEqual((StyleEnum<DisplayStyle>)DisplayStyle.None, inventoryRoot.style.display);
        }

        /// <summary>
        /// Test ensures that upon first opening the inventory, the first category is set as selected
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_OnStartup_FirstCategoryIsSet()
        {
            //Arrange
            string expectedClass = "button-category-selected";

            //Act
            inventoryController.Show();
            yield return null;

            //Assert
            Assert.IsTrue(firstCategory.ClassListContains(expectedClass));
        }

        /// <summary>
        /// Test ensures that when we click on a new tab, the previously selected one loses highlighting
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_OnButtonClick_PreviousCategoryLosesHighlighting()
        {
            //Arrange       
            string expectedClass = "button-category-unselected";

            //Act
            yield return TestUtils.ClickOnButton(secondCategory);

            //Assert
            Assert.IsTrue(firstCategory.ClassListContains(expectedClass));
        }

        /// <summary>
        /// Test ensures that when we select on a new category, it is correctly highlighted
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_OnButtonClick_NewCategoryGainsHighlighting()
        {
            //Arrange
            string[] buttonNames = { "CategoryMedications", "CategoryGenSupplies", "CategoryBronchSupplies" };
            string expectedClass = "button-category-selected";

            //Act & Assert
            foreach (var button in buttonNames)
            {
                Button buttonToTest = inventoryRoot.Q<Button>(button);
                yield return null;

                yield return TestUtils.ClickOnButton(buttonToTest);

                Assert.IsTrue(buttonToTest.ClassListContains(expectedClass));
            }
        }

        /// <summary>
        /// Test ensures that when we open, close and then reopen the inventory tab, the selected category
        /// is saved
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_OnReOpen_HighlightedCategoryIsSaved()
        {
            //Arrange
            string expectedClass = "button-category-selected";

            //Act          
            inventoryController.Show();
            yield return TestUtils.ClickOnButton(thirdCategory);

            inventoryController.Hide();
            yield return null;

            inventoryController.Show();
            yield return null;

            //Assert
            Assert.IsTrue(thirdCategory.ClassListContains(expectedClass));
        }

        /// <summary>
        /// Test ensures that when we attempt to close the inventory panel, it correctly closes
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_OnCloseClick_PanelCloses()
        {
            //Arrange
            Button closeButton = inventoryRoot.Q<TemplateContainer>().Q<Button>("CloseBtn");

            //Act
            inventoryController.Show();
            yield return null;

            yield return TestUtils.ClickOnButton(closeButton);

            //Assert           
            Assert.AreEqual((StyleEnum<DisplayStyle>)DisplayStyle.None, inventoryRoot.style.display);
        }

        /// <summary>
        /// Test ensures the supply amount for a given category is set correctly
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_SupplyAmount_IsCorrectAmount()
        {
            //Arrange
            string expectedResult = "1 supplies";
            Label supplyAmount = inventoryRoot.Q<Label>("InventoryAmountOf");

            //Act
            inventoryController.Show();
            yield return null;

            //Assert
            Assert.AreEqual(expectedResult, supplyAmount.text);
        }

        /// <summary>
        /// Test ensures that the total supply amount text is correclty updated
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_TotalSupplyAmount_IsCorrectAmount()
        {
            //Arrange
            string expectedResult = "(0/3 found)";
            Label supplyAmount = inventoryRoot.Q<Label>("TotalFoundText");

            //Act
            inventoryController.Show();
            yield return null;

            //Assert
            Assert.AreEqual(expectedResult, supplyAmount.text);
        }

        /// <summary>
        /// Test ensures that when we try to spawn cards for a given category, that the correct amount is spawned
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_SpawnPanels_CorrectAmountSpawned()
        {
            //Arrange
            int expectedResult = 1;
            ScrollView scrollView = inventoryRoot.Q<ScrollView>("ScrollView");

            //Act
            inventoryController.Show();
            yield return null;

            //Assert
            Assert.AreEqual(expectedResult, scrollView.childCount);
        }

        /// <summary>
        /// Test ensures that when switching tabs, the code can correctly clear and spawn the new tabs for the new category
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_ChangeCategoryTab_CorrectAmountOfPanelsSpawned()
        {
            //Arrange
            int expectedResult = 1;
            ScrollView scrollView = inventoryRoot.Q<ScrollView>("ScrollView");
            Button buttonToClick = inventoryRoot.Q<Button>("CategoryGenSupplies");

            //Act
            inventoryController.Show();
            yield return TestUtils.ClickOnButton(buttonToClick);

            //Assert
            Assert.AreEqual(expectedResult, scrollView.childCount);
        }

        /// <summary>
        /// Test ensures that the name on the panel is correct
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_GeneratedPanel_InformationIsCorrect()
        {
            //Arrange
            string expectedResult = "Inventory";

            //Act
            inventoryController.Show();

            cardText = inventoryRoot.Q<Label>("CardText");
            yield return null;
            //Assert
            Assert.AreEqual(expectedResult, cardText.text);
        }

        /// <summary>
        /// Test ensures that the checkmark on the panel starts off as invisible
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_GeneratedPanel_CheckmarkIsHidden()
        {
            //Act
            inventoryController.Show();
            checkmark = inventoryRoot.Q<VisualElement>("Checkmark");
            yield return null;

            //Assert
            Assert.AreEqual((StyleEnum<Visibility>)Visibility.Hidden, checkmark.style.visibility);
        }

        /// <summary>
        /// Test ensures that the checkmark on the panel is shown after clicking a panel
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_ClickOnPanel_CheckmarkIsShown()
        {
            //Act
            inventoryController.Show();
            panel = inventoryRoot.Q<VisualElement>("Card");
            checkmark = inventoryRoot.Q<VisualElement>("Checkmark");
            var clickEvent = ClickEvent.GetPooled();
            clickEvent.target = panel;
            panel.SendEvent(clickEvent);

            yield return null;
            //Assert
            Assert.AreEqual((StyleEnum<Visibility>)Visibility.Visible, checkmark.style.visibility);
        }

        /// <summary>
        /// Test ensures that the progress is updated on the progress bar when a panel is clicked on
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_ClickOnPanel_ProgressIsUpdated()
        {
            //Arrange
            int expectedResult = 3;

            //Act

            //Simulate IsExplored being set to true, since we have no event here
            testPrefabOne.GetComponent<Explorable>().IsExplored = true;
            inventoryController.Show();
            panel = inventoryRoot.Q<VisualElement>("CardContainer");
            var clickEvent = ClickEvent.GetPooled();
            clickEvent.target = panel;
            panel.SendEvent(clickEvent);
            yield return null;

            //Assert
            Assert.AreEqual(expectedResult, progressBar.value);
        }

        /// <summary>
        /// TODO: this test has been commented out, because Unity is refusing to use the updated version of the script
        /// that allows for this to happen
        /// Test ensures that explorables are marked as explored when their panel is clicked on
        /// </summary>
        /*[UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_ClickOnPanel_MarksExplorableAsExplored()
        {
            //Arrange
            VisualElement panel;

            //Act
            inventoryController.Show();
            panel = inventoryRoot.Q<VisualElement>("CardContainer");

            var clickEvent = ClickEvent.GetPooled();
            clickEvent.target = panel;
            panel.SendEvent(clickEvent);

            yield return null;
            //Assert         
            Assert.IsTrue(testPrefabOne.GetComponent<Explorable>().IsExplored);
        }*/

        /// <summary>
        /// Test ensures that the object clicked on is set to active
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_ClickOnPanel_ActiveObjectIsShown()
        {
            //Arrange
            VisualElement panel;
            testPrefabOne.SetActive(false);

            //Act
            inventoryController.Show();
            panel = inventoryRoot.Q<VisualElement>("Card");

            var clickEvent = ClickEvent.GetPooled();
            clickEvent.target = panel;
            panel.SendEvent(clickEvent);

            yield return null;
            //Assert         
            Assert.IsTrue(testPrefabOne.activeSelf);
        }

        /// <summary>
        /// Test ensures that the object clicked on is set to active
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_AfterClosingPanel_ActiveObjectIsHidden()
        {
            //Arrange
            VisualElement panel;
            testPrefabOne.SetActive(true);

            //Act
            inventoryController.Show();
            panel = inventoryRoot.Q<VisualElement>("Card");

            var clickEvent = ClickEvent.GetPooled();
            clickEvent.target = panel;
            panel.SendEvent(clickEvent);
            inventoryController.ReleaseActiveObject();

            yield return null;
            //Assert         
            Assert.IsFalse(testPrefabOne.activeSelf);
        }

        /// <summary>
        /// Test ensures that the object clicked on is set to active
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_AfterViewingObject_TextColorIsChangedToInProgress()
        {
            //Arrange
            string expectedResult = "FFCD29";

            //Add new object to list so we will be in progress for this test
            inventoryController.Medications.Add(testPrefabTwo);

            //Act
            //Simulate IsExplored being set to true, since we have no event here
            testPrefabOne.GetComponent<Explorable>().IsExplored = true;
            inventoryController.Show();
            yield return null;

            //Assert         
            Assert.AreEqual(expectedResult, ColorUtility.ToHtmlStringRGB(totalFoundText.resolvedStyle.color));
        }

        /// <summary>
        /// Test ensures that the object clicked on is set to active
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_AfterViewingAllObjects_TextColorIsChangedToComplete()
        {
            //Arrange
            string expectedResult = "4BC17E";

            //Act
            //Simulate IsExplored being set to true, since we have no event here
            testPrefabOne.GetComponent<Explorable>().IsExplored = true;
            inventoryController.Show();
            yield return null;

            //Assert         
            Assert.AreEqual(expectedResult, ColorUtility.ToHtmlStringRGB(totalFoundText.resolvedStyle.color));
        }

        /// <summary>
        /// Test ensures that a panel will use the default image when not given a custom one
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_NotGivenAnImage_PanelUsesDefaultImage()
        {
            //Arrange
            VisualElement panel;
            VisualElement expectedResult = new();
            testPrefabOne.GetComponent<Explorable>().explorableInformation.ExplorableSprite = null;

            //Act
            expectedResult.style.backgroundImage = defaultImage.texture;
            inventoryController.Show();
            panel = inventoryRoot.Q<VisualElement>("ExplorableImage");
            yield return null;

            Texture2D expectedTexture = expectedResult.resolvedStyle.backgroundImage.texture;
            Texture2D actualTexture = panel.resolvedStyle.backgroundImage.texture;


            //Assert
            Assert.AreEqual(expectedTexture, actualTexture);

        }

        /// <summary>
        /// Test ensures that when a panel will use a custom one when given a custom one
        /// </summary>
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Inventory_GivenAnImage_PanelUsesCustomImage()
        {
            //Arrange
            VisualElement panel;
            VisualElement expectedResult = new();
            testPrefabOne.GetComponent<Explorable>().explorableInformation.ExplorableSprite = customImage;

            //Act
            expectedResult.style.backgroundImage = customImage.texture;
            inventoryController.Show();
            panel = inventoryRoot.Q<VisualElement>("ExplorableImage");

            yield return null;

            //Assert 
            Assert.AreEqual(expectedResult.resolvedStyle.backgroundImage, panel.resolvedStyle.backgroundImage);

        }
    }
}