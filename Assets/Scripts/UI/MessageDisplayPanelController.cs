using System;
using UnityEngine;
using VARLab.Velcro;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace VARLab.RespiratoryTherapy
{
    public class MessageDisplayPanelController : MonoBehaviour, IUserInterface
    {
        private Label title;
        private Label header;
        private Label message;
        private Button beginButton;
        private Action callBackAction;
        private VisualElement messageDisplayPanelRoot;
        private VisualElement contentImage;

        private PanelContent defaultPanelContent = new ()
        {
            Message = "",
            ButtonText = "OK",
            Header = "Warning",
            Title = "No content found" 
        };
        
        private void Awake()
        {
            messageDisplayPanelRoot = GetComponent<UIDocument>().rootVisualElement;
            beginButton = messageDisplayPanelRoot.Q<Button>("BeginButton");
            title = messageDisplayPanelRoot.Q<Label>("Title");
            message = messageDisplayPanelRoot.Q<Label>("Message");
            header = messageDisplayPanelRoot.Q<Label>("Name");
            contentImage = messageDisplayPanelRoot.Q<VisualElement>("ContentImage");

            HideContentImage();
            Hide();
        }

        /// <summary>
        /// Handles the event when the Begin button is clicked. 
        /// Executes the specified callback action, hides the current UI element, 
        /// and removes any listeners associated with the button.
        /// </summary>
        private void OnBeginButtonClick()
        {
            Hide();
            callBackAction?.Invoke();
            RemoveListener();
        }

        /// <summary>
        /// Removes the click event listener from the Begin button by detaching the OnBeginButtonClick handler.
        /// </summary>
        private void RemoveListener()
        {
            beginButton.clicked -= OnBeginButtonClick;
        }

        private void ShowContentImage()
        {
            contentImage.style.display = DisplayStyle.Flex;
        }
        
        private void HideContentImage()
        {
            contentImage.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Displays the welcome screen by setting its root element's display style to Flex,
        /// making it visible in the UI.
        /// </summary>
        public void Show()
        {
            messageDisplayPanelRoot.Show();
        }

        /// <summary>
        /// Hides the welcome screen by setting its root element's display style to None, making it invisible in the UI.
        /// </summary>
        public void Hide()
        {
            messageDisplayPanelRoot.Hide();
        }

        /// <summary>
        /// Sets the title and message content for the welcome screen by updating the corresponding UI elements.
        /// </summary>
        /// <param name="panelTitle">The title text to display on the welcome screen.</param>
        /// <param name="panelMessage">The message content to display on the welcome screen.</param>
        public void SetContent(MessageType type,[CanBeNull] PanelContent content= null, Action onComplete = null,
            [CanBeNull] bool showContentImage = false)
        {
            PanelContent panelContent = content ?? defaultPanelContent;
            HideContentImage();
            
            title.text = panelContent.Title;
            header.text = panelContent.Header;
            message.text = panelContent.Message;
            beginButton.text = panelContent.ButtonText;
            callBackAction = onComplete;

            if (type == MessageType.Completion)
            {
                ShowContentImage();
            }
            
            beginButton.clicked += OnBeginButtonClick;
        } 
    }
}
