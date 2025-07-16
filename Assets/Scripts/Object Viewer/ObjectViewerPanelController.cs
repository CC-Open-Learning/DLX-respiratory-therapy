using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using VARLab.ObjectViewer;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    public class ObjectViewerPanelController : MonoBehaviour
    {
        ///Invokes <see cref="ObjectViewerController.View(GameObject)"/>
        ///Invokes <see cref="OutlineInteraction.HideOutline(GameObject)"/>
        [FormerlySerializedAs("OpenUI")]
        public UnityEvent<GameObject> ViewObject = new();

        ///Invokes <see cref="ObjectViewerController.MoveObjectBack"/>
        ///Invokes <see cref="InteractionHandler.enabled"/>
        [FormerlySerializedAs("CloseUI")]
        public UnityEvent ResetObject = new();

        public UnityEvent ReenableInteractions = new();

        ///Invokes <see cref="OrientationController.UpdateProgress(ExplorableCategory, string, int)"/>
        public UnityEvent<ExplorableCategory, string, int> AddProgress = new();

        public UnityEvent<UIPanel> ShowScreen = new();
        public UnityEvent<UIPanel> HideScreen = new();
        public UnityEvent ShowBlur = new();
        public UnityEvent HideBlur = new();

        public UnityEvent<AudioClip, SoundPlayOptions?> PlayAudio = new();
        public UnityEvent<SoundPlayOptions?> StopAudio = new();

        public UnityEvent ReleaseActiveObject = new();

        public UnityEvent OnCloseClicked = new();

        //Invokes <see cref="CustomSaveHandler.Save"/>
        public UnityEvent OnSaveItem;

        private UIDocument objectViewerWindow;

        private Button closeButton;
        private Button playAudioButton;

        private Label explorableNameLabel;
        private Label explorableDescriptionLabel;

        private VisualElement objectViewerRoot;
        private AudioClip explorableAudio;
        private const int MinimumProgressAmount = 1;

        private GameObject currentlyActiveObject;
        private Explorable selectedExplorable;
        public bool HideToolBar { set; get; }

        private void Start()
        {
            SetupUIReferences();
            SetupListeners();
        }
        /// <summary>
        /// Checks if what was clicked on is considered a valid explorable
        /// </summary>
        /// <param name="explorableObject">The clicked on objects GameObject</param>
        /// <param name="explorable">The explorable component of the explorable that will be returned</param>
        /// <returns></returns>
        private bool IsValidExplorableObject(GameObject explorableObject, out Explorable explorable)
        {
            explorable = null;
            return explorableObject != null && explorableObject.TryGetComponent(out explorable);
        }

        /// <summary>
        /// Handles showing the panel and putting the object inside the panel
        /// </summary>
        /// <param name="explorableObject">The explorable objects GameObject</param>
        private void DisplayObjectViewerPanel(GameObject explorableObject)
        {
            objectViewerRoot.Show();
            HideScreen?.Invoke(UIPanel.Toolbar);
            HideScreen?.Invoke(UIPanel.POIToolbar);
            ShowBlur?.Invoke();
            ViewObject?.Invoke(explorableObject);
        }

        /// <summary>
        /// Obtains and sets the explorable name and description into the object viewer panel
        /// </summary>
        /// <param name="explorable"></param>
        private void SetExplorableComponents(Explorable explorable)
        {
            string name = explorable.explorableInformation.ExplorableName;
            string description = explorable.explorableInformation.ExplorableDescription;

            explorableNameLabel.text = name;
            explorableDescriptionLabel.text = description;

            explorableAudio = explorable.explorableInformation.ExplorableAudioClip;

            if (!explorable.IsExplored)
            {
                explorable.MarkItemAsExplored();
                OnSaveItem?.Invoke();
            }
            explorable.SetExplorableTransformValues();
        }

        /// <summary>
        /// Sets up all of the UI references from the UIDocument
        /// </summary>
        private void SetupUIReferences()
        {
            objectViewerWindow = GetComponent<UIDocument>();
            objectViewerRoot = objectViewerWindow.rootVisualElement;
            
            objectViewerRoot.Hide();

            closeButton = objectViewerRoot.Q<TemplateContainer>().Q<Button>("CloseBtn");
            playAudioButton = objectViewerRoot.Q<TemplateContainer>().Q<Button>("Button");

            explorableNameLabel = objectViewerRoot.Q<Label>("ExplorableNameLabel");
            explorableDescriptionLabel = objectViewerRoot.Q<Label>("ExplorableDescriptionLabel");
        }

        /// <summary>
        /// Sets up all the listeners for the buttons click events
        /// </summary>
        private void SetupListeners()
        {
            closeButton.clicked += () =>
            {
                StartCoroutine(ClosePanelWithDelay());
            };

            playAudioButton.clicked += () =>
            {
                PlayAudio?.Invoke(explorableAudio, null);
            };
        }

        /// <summary>
        /// Handles the flow of opening the Object Viewer panel when clicking on an interactable
        /// </summary>
        public void HandleObjectClick(GameObject explorableObject)
        {
            if (!IsValidExplorableObject(explorableObject, out Explorable explorable))
            {
                ReenableInteractions?.Invoke();
                return;
            }
            currentlyActiveObject = explorableObject;
            selectedExplorable = explorable;
            DisplayObjectViewerPanel(explorableObject);
            SetExplorableComponents(explorable);
            explorable.HighlightExplorable();
            AddProgress?.Invoke(explorable.explorableInformation.ExplorableCategory, explorable.explorableInformation.ExplorableName, MinimumProgressAmount);
        }

        /// <summary>
        /// The purpose of this method is to deactivate the object in the object viewer for one frame then reactivate it so there isnt a after image 
        /// when you open it for the second time.
        /// </summary>
        private IEnumerator ClosePanelWithDelay()
        {
            currentlyActiveObject.SetActive(false);
            // Wait for one frame
            yield return null;
            currentlyActiveObject.SetActive(true);
            ClosePanel();

        }

        /// <summary>
        /// Closes the object viewer panel by hiding the UI element and stopping any playing audio.
        /// Resets the object state and triggers the on-close action if defined.
        /// </summary>
        private void ClosePanel()
        {
            objectViewerRoot.Hide();
            ShowScreen?.Invoke(UIPanel.Toolbar);
            ShowScreen?.Invoke(UIPanel.POIToolbar);

            /* There's a check to choose if its hidden or not due to the medical cart and bronch tower having different requirements for hiding it*/
            if (HideToolBar)
            {
                HideScreen?.Invoke(UIPanel.Toolbar);
            }

            /*Due to using a UnityEvent we have to include the optional parameter, this means the event
            is expecting something, so we have to pass in null*/
            StopAudio?.Invoke(null);
            ResetObject?.Invoke();
            OnCloseClicked?.Invoke();
            selectedExplorable.UnHighlightExplorable();
            selectedExplorable = null;
            currentlyActiveObject = null;

            ReleaseActiveObject?.Invoke();
            HideBlur?.Invoke();
        }
    }
}