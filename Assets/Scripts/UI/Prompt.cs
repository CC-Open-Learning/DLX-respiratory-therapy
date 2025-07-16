using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    public class Prompt : MonoBehaviour, IUserInterface
    {
        public UnityEvent OnPromptShown;
        public UnityEvent OnPromptHidden;
        private Notification notification;

        private VisualElement root;
        private VisualElement imageContainer;
        private VisualElement image;
        private VisualElement bar;
        private Label speakerLabel;
        private Label messageLabel;
        private Button closeButton;

        private void Start()
        {
            root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            
            imageContainer = root.Q<VisualElement>("ImageContainer");
            image = root.Q<VisualElement>("Image");
            bar = root.Q<VisualElement>("Bar");
            speakerLabel = root.Q<Label>("SpeakerLabel");
            messageLabel = root.Q<Label>("MessageLabel");

            closeButton = root.Q<Button>("CloseBtn");
            closeButton.clicked += () =>
            {
                Hide();
            };

            OnPromptShown ??= new UnityEvent();
            OnPromptHidden ??= new UnityEvent();
            
            notification = FindObjectOfType<Notification>();

            root.Hide();
        }

        /// <summary>
        /// This method is intended to be a single access public method to populate the prompt with text, 
        /// buttons, and display it
        /// </summary>
        /// <param name="promptSO"></param>
        public void HandleDisplayUI(PromptSO promptSO) 
        {
            SetContent(promptSO);
            Show();
        }

        /// <summary>
        /// Populates the prompt with the provided content and colours from PromptSO. The Speaker label expects the use 
        // of rich text to change it's colour at once
        /// </summary>
        /// <param name="promptSO"></param>
        public void SetContent(PromptSO promptSO)
        {
            speakerLabel.SetElementText(promptSO.Speaker);
            messageLabel.SetElementText(promptSO.Message);
            
            RTUIHelper.SetBackgroundImage(image, promptSO.Image);
            RTUIHelper.SetBackgroundColour(bar, promptSO.BarColour);
            RTUIHelper.SetBorderColour(imageContainer, promptSO.ImageBorderColour);
            speakerLabel.style.color = promptSO.SpeakerColour;
            closeButton.style.display = promptSO.IsBtnEnabled ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>
        /// Shows the root of the prompt and triggers OnPromptShown
        /// </summary>
        public void Show()
        {
            root.Show();
            notification?.Hide();
            OnPromptShown?.Invoke();
        }

        /// <summary>
        /// Hides the root of the prompt and triggers OnPromptHidden
        /// </summary>
        public void Hide()
        {
            root.Hide();
            OnPromptHidden?.Invoke();
        }
    }
}