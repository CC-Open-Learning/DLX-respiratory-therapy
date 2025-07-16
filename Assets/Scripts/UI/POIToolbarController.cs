using System;
using UnityEngine;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    public class POIToolbarController : MonoBehaviour,IUserInterface
    {
        private VisualElement poiToolbarRoot;
        private Button backButton;
        private Label poiLabel;
        private VisualElement poiIconContainer;

        private Action backButtonCallback;

        private bool isInitalized;
        
        private void Start()
        {
            poiToolbarRoot = GetComponent<UIDocument>().rootVisualElement;
            backButton = poiToolbarRoot.Q<Button>("BackButton");
            poiLabel = poiToolbarRoot.Q<Label>("POILabel");
            poiIconContainer = poiToolbarRoot.Q<VisualElement>("POIImage");

            isInitalized = false;
            poiToolbarRoot.style.display = DisplayStyle.None;
        }
        
        private void OnBackButtonClicked()
        {
            backButtonCallback?.Invoke();
        }

        public void ResetPOIToolbar()
        {
            backButton.clicked -= OnBackButtonClicked;
            isInitalized = false;

            poiToolbarRoot.style.display = DisplayStyle.None;
        }

        public void SetPOIToolbarUI(string poiName, Action poiBackCallback, Sprite poiIcon = null)
        {
            ResetPOIToolbar();
            isInitalized = true;

            poiLabel.text = poiName;
            poiIconContainer.style.backgroundImage = new StyleBackground(poiIcon);
            backButtonCallback = poiBackCallback;
            backButton.clicked += OnBackButtonClicked;
        }

        /// <summary>
        /// Displays the POI toolbar by calling the UI helper to show the POI toolbar UI element.
        /// </summary>
        public void Show()
        {
            if (isInitalized)
            {
                poiToolbarRoot.Show();
            }
        }

        /// <summary>
        /// Hides the POI toolbar by calling the UI helper to hide the POI toolbar UI element.
        /// </summary>
        public void Hide()
        {
            if (isInitalized)
            {
                poiToolbarRoot.Hide();
            }
        }
    }
}
