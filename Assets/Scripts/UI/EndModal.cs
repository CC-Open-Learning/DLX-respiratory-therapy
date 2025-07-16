using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    [RequireComponent(typeof(UIDocument))]
    public class EndModal : MonoBehaviour, IUserInterface
    {
        [Header("Event Hook-Ins")]
        public UnityEvent OnModalShown;
        public UnityEvent OnModalHidden;
        public UnityEvent OnBtnClicked;

        public VisualElement Root {  get; private set; }
        private VisualElement sprite;
        private VisualElement canvas;
        private Button buttonReturnToMenu;
        private Button buttonNextModule;
        private Label nameLabel;
        private Label titleLabel;
        private Label descriptionLabel;
        private VisualElement lineSeparator;
        
        private const string DimmedBackgroundClass = "information-dialog-canvas";

        private void Start()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            sprite = Root.Q<VisualElement>("Image");
            canvas = Root.Q<VisualElement>("Canvas");
            buttonReturnToMenu = Root.Q<Button>("MenuButton");
            buttonNextModule = Root.Q<Button>("NextButton");

            nameLabel = Root.Q<TemplateContainer>().Q<Label>("NameLabel");
            titleLabel = Root.Q<Label>("TitleLabel");
            descriptionLabel = Root.Q<Label>("DescriptionLabel");
            lineSeparator = Root.Q<VisualElement>("LineSeperator");

            OnModalShown ??= new UnityEvent();
            OnModalHidden ??= new UnityEvent();
            OnBtnClicked ??= new UnityEvent();

            buttonReturnToMenu.clicked += () =>
            {
                OnBtnClicked?.Invoke();
                Hide();
            };
            
            buttonNextModule.clicked += () =>
            {
                OnBtnClicked?.Invoke();
                Hide();
            };

            Root.Hide();
        }

        /// <summary>
        /// This method is intended to be a single access public method to populate the modal with text, 
        /// buttons, and display it
        /// </summary>
        /// <param name="endModalSO"></param>
        public void HandleDisplayUI(EndModalSO endModalSO)
        {
            SetContent(endModalSO);
            Show();
        }

        /// <summary>
        /// Populates content for the modal, but does not display it. Modals only have one customization available:
        ///     - Canvas Dim: The background of the modal can be dimmed to bring focus to the UI and block interaction with other UI
        ///     - Note/SubDescription label left empty will be hidden using DisplayStyle.None
        /// </summary>
        /// <param name="endModalSO"></param>
        public void SetContent(EndModalSO endModalSO)
        {
            sprite.style.backgroundImage = new StyleBackground(endModalSO.Image);

            canvas.EnableInClassList(DimmedBackgroundClass, endModalSO.IsCanvasDimmed);
            buttonReturnToMenu.SetElementText(endModalSO.ReturnToMenuButtonText);
            buttonNextModule.SetElementText(endModalSO.NextModuleButtonText);
            nameLabel.SetElementText(endModalSO.Name);
            titleLabel.SetElementText(endModalSO.Title);
            descriptionLabel.SetElementText(endModalSO.Description);
            
            if (endModalSO.LineSeparator != null)
            {
                lineSeparator.Add(endModalSO.LineSeparator);
            }
            else
            {
                lineSeparator.Hide();
            }
        }

        /// <summary>
        /// Shows the root of the modal and triggers OnModalShown
        /// </summary>
        public void Show()
        {
            Root.Show();
            OnModalShown?.Invoke();
        }

        /// <summary>
        /// Shows the root of the modal and triggers OnModalHidden
        /// </summary>
        public void Hide()
        {
            Root.Hide();
            OnModalHidden?.Invoke();
        }
    }
}