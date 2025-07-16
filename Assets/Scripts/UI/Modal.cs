using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    [RequireComponent(typeof(UIDocument))]
    public class Modal : MonoBehaviour, IUserInterface
    {
        [Header("Event Hook-Ins")]
        public UnityEvent OnModalShown;
        public UnityEvent OnModalHidden;
        /// Invokes <see cref="BiopsyController.UpdateScenarioIndex"/>
        public UnityEvent OnScenarioAdvanceButtonClick;
        /// Invokes <see cref="BiopsyController.UpdateScenarioTaskIndex"/>
        public UnityEvent OnTaskAdvanceButtonClick;

        public VisualElement Root {  get; private set; }
        private VisualElement sprite;
        private VisualElement noteSprite;
        private VisualElement canvas;
        private Button button;
        private Label nameLabel;
        private Label titleLabel;
        private Label descriptionLabel;
        private Label subDescriptionLabel;
        private Label noteLabel;

        private bool shouldAdvanceScenarioIndex;

        private const string DimmedBackgroundClass = "information-dialog-canvas";

        private void Start()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            noteSprite = Root.Q<VisualElement>("NoteImage");
            sprite = Root.Q<VisualElement>("Image");
            canvas = Root.Q<VisualElement>("Canvas");
            button = Root.Q<TemplateContainer>().Q<Button>();

            nameLabel = Root.Q<TemplateContainer>().Q<Label>();
            titleLabel = Root.Q<Label>("TitleLabel");
            descriptionLabel = Root.Q<Label>("DescriptionLabel");
            subDescriptionLabel = Root.Q<Label>("SubDescriptionLabel");
            noteLabel = Root.Q<Label>("NoteLabel");

            OnModalShown ??= new UnityEvent();
            OnModalHidden ??= new UnityEvent();
            OnScenarioAdvanceButtonClick ??= new UnityEvent();
            OnTaskAdvanceButtonClick ??= new UnityEvent();

            shouldAdvanceScenarioIndex = true;

            button.clicked += () =>
            {
                Hide();
                if (shouldAdvanceScenarioIndex)
                {
                    OnScenarioAdvanceButtonClick?.Invoke();
                }
                else
                {
                    OnTaskAdvanceButtonClick?.Invoke();
                }
            };

            Root.Hide();
        }

        /// <summary>
        /// This method is intended to be a single access public method to populate the modal with text, 
        /// buttons, and display it
        /// </summary>
        /// <param name="modalSO"></param>
        public void HandleDisplayUI(ModalSO modalSO)
        {
            SetContent(modalSO);
            Show();
        }

        /// <summary>
        /// Toggles whether the modal will advance to the next scenario when the button is clicked
        /// </summary>
        public void ToggleScenarioIndexUpdate(bool toggle)
        {
            shouldAdvanceScenarioIndex = toggle;
        }

        /// <summary>
        /// Populates content for the modal, but does not display it. Modals only have one customization available:
        ///     - Canvas Dim: The background of the modal can be dimmed to bring focus to the UI and block interaction with other UI
        ///     - Note/SubDescription label left empty will be hidden using DisplayStyle.None
        /// </summary>
        /// <param name="modalSO"></param>
        public void SetContent(ModalSO modalSO)
        {
            RTUIHelper.SetBackgroundImage(sprite, modalSO.Image);
            RTUIHelper.SetBackgroundImage(noteSprite, modalSO.NoteImage);

            canvas.EnableInClassList(DimmedBackgroundClass, modalSO.IsCanvasDimmed);
            button.SetElementText(modalSO.ButtonText);
            nameLabel.SetElementText(modalSO.Name);
            titleLabel.SetElementText(modalSO.Title);
            descriptionLabel.SetElementText(modalSO.Description);

            PopulateLabelIfNotEmpty(subDescriptionLabel, modalSO.SubDescription);
            PopulateLabelIfNotEmpty(noteLabel, modalSO.Note);
        }

        /// <summary>
        /// Checks if the incoming string is empty or null. If empty or null, then the label will be hidden to not take up space in the modal
        /// </summary>
        /// <param name="label"></param>
        /// <param name="text"></param>
        private void PopulateLabelIfNotEmpty(Label label, string text)
        {
            if (string.IsNullOrWhiteSpace(text.Trim()))
            {
                label.Hide();
            }
            else
            {
                label.Show();
                label.SetElementText(text);
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