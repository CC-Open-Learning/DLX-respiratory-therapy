using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class POIToolbarIntegrationTests
    {
        private GameObject poiToolbarGameObject;
        private POIToolbarController poiToolbarController;
        private UIDocument poiToolbarUIDocument;
        
        private VisualElement poiToolbarRoot;
        private Label poiLabel;
        private VisualElement poiIconContainer;
        
        [SetUp]
        [Category("BuildServer")]
        public void SetUp()
        {
            //Set up POI tool bar
            poiToolbarGameObject = new GameObject();
            poiToolbarUIDocument = poiToolbarGameObject.AddComponent<UIDocument>();
            VisualTreeAsset poiToolbarUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/Toolbars/POIToolbar.uxml");
            poiToolbarUXML.CloneTree(poiToolbarUIDocument.rootVisualElement);
            poiToolbarController = poiToolbarGameObject.AddComponent<POIToolbarController>();

            poiToolbarRoot = poiToolbarUIDocument.rootVisualElement;
            poiLabel = poiToolbarRoot.Q<Label>("POILabel");
            poiIconContainer = poiToolbarRoot.Q<VisualElement>("POIImage");
        }

        [TearDown]
        public void TearDown()
        {
            poiToolbarGameObject = null;
        }
        
        [UnityTest]
        public IEnumerator ResetPOIToolbar_ShouldResetPOIToolbar()
        {
            //Act
            poiToolbarController.SetPOIToolbarUI("Bronch Tower", BackButtonCallback);
            poiToolbarController.ResetPOIToolbar();

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.IsTrue(poiToolbarRoot.style.display == DisplayStyle.None);
        }
        
        [UnityTest]
        public IEnumerator SetPOIToolbarUI_ShouldSetupPOIToolbarUI()
        {
            //Act
            Sprite testIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/POI Toolbar Icons/BronchTower_Poi_Icon.png");
            poiToolbarController.SetPOIToolbarUI("Bronch Tower", BackButtonCallback, testIcon);

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.AreEqual(poiLabel.text, "Bronch Tower");
            Assert.AreEqual(poiIconContainer.style.backgroundImage.value.ToString(), testIcon.ToString());
        }
        
        [UnityTest]
        public IEnumerator SetPOIToolbarUI_WithoutProvidingIcon_IconShouldBeBlank()
        {
            //Act
            poiToolbarController.SetPOIToolbarUI("Bronch Tower", BackButtonCallback);

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.AreEqual(poiLabel.text, "Bronch Tower");
            Assert.AreEqual(poiIconContainer.style.backgroundImage.value.ToString(), "");
        }
        
        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Show_WhenInitialized_ShouldDisplayPOIToolbar()
        {
            //Act
            poiToolbarController.SetPOIToolbarUI("Bronch Tower", BackButtonCallback);
            poiToolbarController.Show();

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.IsTrue(poiToolbarRoot.style.display == DisplayStyle.Flex);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Hide_WhenInitialized_ShouldHidePOIToolbar()
        {
            //Act
            poiToolbarController.SetPOIToolbarUI("Bronch Tower", BackButtonCallback);
            poiToolbarController.Hide();

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.IsTrue(poiToolbarRoot.style.display == DisplayStyle.None);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Show_WhenNotInitialized_ShouldNotDisplayPOIToolbar()
        {
            //Act
            poiToolbarController.SetPOIToolbarUI("Bronch Tower", BackButtonCallback);
            poiToolbarController.ResetPOIToolbar();
            poiToolbarController.Show();

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.IsTrue(poiToolbarRoot.style.display == DisplayStyle.None);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator Hide_WhenNotInitialized_ShouldNotHidePOIToolbar()
        {
            //Act
            poiToolbarController.SetPOIToolbarUI("Bronch Tower", BackButtonCallback);
            poiToolbarController.ResetPOIToolbar();
            poiToolbarRoot.style.display = DisplayStyle.Flex;
            poiToolbarController.Hide();

            //Assert
            yield return null; //Wait for one frame so UI can update

            Assert.IsTrue(poiToolbarRoot.style.display == DisplayStyle.Flex);
        }

        private void BackButtonCallback()
        {
            Debug.Log("BackButtonCallback");
        }      
    }
}
