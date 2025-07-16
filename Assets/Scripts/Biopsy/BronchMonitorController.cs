using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    public class BronchMonitorController : MonoBehaviour
    {
        private Button topButton;
        private Button bottomButton;
        private Label topButtonLabel;
        private Label bottomButtonLabel;
        private VisualElement topButtonSprite;
        private VisualElement bottomButtonSprite;
        private BronchMonitorStatus monitorStatus;
        private BronchMonitorTaskSO bronchMonitorTask;
        public Coroutine autoCycleImagesCoroutine;
        
        public VisualElement Root {  get; private set; }
        public Material BronchMonitorMaterial;
        public Material DefaultBronchMonitorMaterial;
        public Material EmptyAirwayBronchMonitorMaterial;
        
        [Header("Bronch Monitor Control Button Images")]
        public Sprite OpenForcepsImage;
        public Sprite CloseForcepsImage;
        public Sprite RetractNeedleImage;
        public Sprite AdvanceNeedleImage;
        
        [Header("Bronch Monitor Control Button Text")]
        public string OpenForcepsButtonText;
        public string CloseForcepsButtonText;
        public string RetractNeedleButtonText;
        public string AdvanceNeedleButtonText;
        
        [Header("Bronch Monitor Events")][Tooltip("These events will be used to get the prompt tasks while we show the images by updating the task / scenario index")]
        public UnityEvent OnTopButtonCliked;
        public UnityEvent OnBottomButtonCliked;
        
        public int BronchMonitorDataIndex;
        public ProcedurePOIToolbar POIToolbar;
        public ToolbarController ToolbarController;
        
        private void Start()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            topButton = Root.Q<VisualElement>("TopContainer").Q<Button>();
            bottomButton = Root.Q<VisualElement>("BottomContainer").Q<Button>();
            topButtonLabel = Root.Q<VisualElement>("TopContainer").Q<Label>();
            bottomButtonLabel = Root.Q<VisualElement>("BottomContainer").Q<Label>();
            topButtonSprite = Root.Q<VisualElement>("TopContainer").Q<VisualElement>("Sprite");
            bottomButtonSprite = Root.Q<VisualElement>("BottomContainer").Q<VisualElement>("Sprite");
            
            SetMonitorMaterial();
            
            topButton.clicked += () =>
            {
                UpdateImageIndex();

                if (monitorStatus == BronchMonitorStatus.RetractNeedle)
                {
                    bronchMonitorTask.ExecuteAction();
                    Root.Hide();
                }
            };
           
            bottomButton.clicked += () =>
            {
                switch (monitorStatus)
                {
                    case BronchMonitorStatus.AdvanceNeedle:
                        ResetImageIndex();
                        bronchMonitorTask.ExecuteAction();
                        break;
                    case BronchMonitorStatus.RetractNeedle:
                        UpdateImageIndex();
                        break;
                }
            };
            
            BronchMonitorDataIndex = 0;
            Root.style.display = DisplayStyle.None;
        }
        
        /// <summary>
        /// Configures the UI for the advanced needle controller UI by updating button labels, 
        /// setting background images, and disabling the bottom button.
        /// </summary>
        private void EnableAdvanceNeedleController()
        {
           topButtonLabel.SetElementText(AdvanceNeedleButtonText);
           bottomButtonLabel.SetElementText(OpenForcepsButtonText);
           RTUIHelper.SetBackgroundImage(topButtonSprite, AdvanceNeedleImage);
           RTUIHelper.SetBackgroundImage(bottomButtonSprite, OpenForcepsImage);
           
           bottomButton.enabledSelf = false;
        }
        
        /// <summary>
        /// Configures the UI for the retract needle controller UI by updating button labels, 
        /// setting background images, and disabling the top button.
        /// </summary>
        private void EnableRetractNeedleController()
        {
            topButtonLabel.SetElementText(RetractNeedleButtonText);
            bottomButtonLabel.SetElementText(CloseForcepsButtonText);
            RTUIHelper.SetBackgroundImage(topButtonSprite, RetractNeedleImage);
            RTUIHelper.SetBackgroundImage(bottomButtonSprite, CloseForcepsImage);
            
            topButton.enabledSelf = false;
        }

        /// <summary>
        /// Resets the image index to its default state.
        /// </summary>
        public void ResetImageIndex()
        {
            BronchMonitorDataIndex = 0;
        }

        /// <summary>
        /// Updates the image index for the bronchoscope monitor. If there are more images, 
        /// it increments the index and updates the monitor display. Otherwise, it adjusts 
        /// UI elements and executes actions based on the monitor status.
        /// </summary>
        public void UpdateImageIndex()
        {
            if (BronchMonitorDataIndex < bronchMonitorTask.BronchMonitorDataList.Count -1)
            {
                BronchMonitorDataIndex++;
                UpdateBronchMonitor();
                
                if (BronchMonitorDataIndex == bronchMonitorTask.BronchMonitorDataList.Count -1)
                {
                    switch (monitorStatus)
                    {
                        case BronchMonitorStatus.AdvanceNeedle:
                            topButton.enabledSelf = false;
                            bottomButton.enabledSelf = true;
                            break;
                        case BronchMonitorStatus.RetractNeedle:
                            topButton.enabledSelf = true;
                            bottomButton.enabledSelf = false;
                            break;
                    }
                }
            }
            else
            {
                if (autoCycleImagesCoroutine != null)
                {
                    StopCoroutine(autoCycleImagesCoroutine);
                    autoCycleImagesCoroutine = null;
                    ResetImageIndex();
                    bronchMonitorTask.ExecuteAction();
                }
            }
        }

        /// <summary>
        /// Updates the bronchoscope monitor display by setting the material's main texture 
        /// to the current image based on the image index.
        /// </summary>
        private void UpdateBronchMonitor()
        {
            if (BronchMonitorMaterial.color == DefaultBronchMonitorMaterial.color) BronchMonitorMaterial.color = Color.white;
            BronchMonitorMaterial.mainTexture = bronchMonitorTask.BronchMonitorDataList[BronchMonitorDataIndex].Sprite.texture;
            
            if (bronchMonitorTask.BronchMonitorDataList[BronchMonitorDataIndex].PromptTask != null)
            {
                bronchMonitorTask.BronchMonitorDataList[BronchMonitorDataIndex].PromptTask.Execute();
            }
        }

        public void SetMonitorMaterial()
        {
            BronchMonitorMaterial.color = DefaultBronchMonitorMaterial.color;
            BronchMonitorMaterial.mainTexture = null;
        }
        
        /// <summary>
        /// Sets the bronchoscope monitor data using the provided task scriptable object.
        /// Configures the monitor status, image data, and updates the UI accordingly.
        /// </summary>
        /// <param name="bronchMonitorTaskSO">The task data containing monitor status 
        /// and image information.</param>
        public void SetBronchMonitorData(BronchMonitorTaskSO bronchMonitorTaskSO)
        {
            bronchMonitorTask = bronchMonitorTaskSO;
            monitorStatus = bronchMonitorTaskSO.MonitorStatus;
            BronchMonitorDataIndex = 0;

            switch (monitorStatus)
            {
                case  BronchMonitorStatus.AdvanceNeedle:
                    EnableAdvanceNeedleController();
                    UpdateBronchMonitor();
                    Root.style.display = DisplayStyle.Flex;
                    break;
                case  BronchMonitorStatus.RetractNeedle:
                    EnableRetractNeedleController();
                    UpdateBronchMonitor();
                    Root.style.display = DisplayStyle.Flex;
                    break;
                case  BronchMonitorStatus.Default:
                    UpdateBronchMonitor();
                    if (bronchMonitorTask.AutoCycleImages) autoCycleImagesCoroutine = StartCoroutine(AutoCycleImagesCoroutine());
                    break;
            }
        }

        private IEnumerator AutoCycleImagesCoroutine()
        {
            const float cycleDelay = 0.5f;
            while (true)
            {
                yield return new WaitForSeconds(cycleDelay);
                UpdateImageIndex();
            }
        }
    }
}
