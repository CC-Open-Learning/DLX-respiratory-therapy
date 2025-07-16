using System.Collections.Generic;
using System.Linq;
using EPOOutline;
using UnityEngine;
using UnityEngine.Events;

namespace VARLab.RespiratoryTherapy
{
    public class POIManager : MonoBehaviour
    {
        /// Invokes <see cref="CameraManager.ActivateCamera(GameObject, POICameraTransition)"/>
        public UnityEvent<GameObject, POICameraTransition> OnPOIOpened;

        /// Invokes <see cref="CameraManager.ActivateDefaultCamera(bool)"/>
        public UnityEvent<bool> OnPOIReset;

        /// Invokes <see cref="OrientationController.HandleCategoryFocused(string)"/>
        public UnityEvent<string> OnOrientationMainPOIOpened;

        public static POIManager Instance;

        private UIManager uiManager;

        private List<GameObject> allExplorables = new List<GameObject>();

        private List<GameObject> allPOIs = new List<GameObject>();

        // Main POIs are the first tier POIs (eg. BronchTower)
        private List<GameObject> allMainPOIs = new List<GameObject>();

        // Required for the medication cart 
        [SerializeField] private List<GameObject> pseudoMainPOIs;

        public POI CurrentPOI { get; private set; }
        public POIType TaskPOI { get; private set; }
        
        public bool lockPOI { get; private set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                OnPOIOpened ??= new UnityEvent<GameObject, POICameraTransition>();
                OnPOIReset ??= new UnityEvent<bool>();
                OnOrientationMainPOIOpened ??= new UnityEvent<string>();

                uiManager = UIManager.Instance;

                RefreshPOIs();
            }
        }

        /// <summary>
        /// Called when a POI is opened to update the CurrentPOI value
        /// </summary>
        public void HandlePOIOpened(POI poi, string poiName = null)
        {
            CurrentPOI = poi;

            if (uiManager == null)
            {
                uiManager = UIManager.Instance;
            }

            if (poi.IsMainPOI && poiName != null && uiManager.simulationStatus == SimulationStatus.Orientation)
            {
                OnOrientationMainPOIOpened?.Invoke(poiName);
            }

            ShowOutlineableWhenOnBronchoscopePOI(true);
        }

        public void ShowOutlineableWhenOnBronchoscopePOI(bool shouldShow)
        {
            GameObject bronchPoiGameObject = GameObject.Find("BronchoscopePOI");
            Outlinable[] outlinables = bronchPoiGameObject.GetComponentsInChildren<Outlinable>(true);
            outlinables = outlinables.Skip(1).ToArray();

            if (!shouldShow) //dont show on game load
            {
                foreach (var outlineGameComponent in outlinables)
                {
                    outlineGameComponent.GetComponent<Outlinable>().enabled = false;
                }
            }
            else if (CurrentPOI.POIName == POIType.BronchoscopeProcedure) //enable when on inner poi
            {
                foreach (var outlineGameComponent in outlinables)
                {
                    bool componentAccessed = outlineGameComponent.GetComponent<BiopsyProcedureComponentBase>().ComponentAccessed;
                    if (!componentAccessed) outlineGameComponent.enabled = true;
                }
            }
        }


        public void ToggleAllExplorables(bool toggle)
        {
            InteractionHelper.ToggleInteractivity(allExplorables, toggle);
        }

        public void ToggleAllPOIs(bool toggle)
        {
            InteractionHelper.ToggleInteractivity(allPOIs, toggle);
        }

        /// <summary>
        /// Enables the default POIs (ie. MainPOIs) for the current scene
        /// </summary>
        public void EnableDefaultPOIs()
        {
            CurrentPOI = null;
            InteractionHelper.ToggleInteractivity(allMainPOIs, true);
        }

        public void ResetPOIs()
        {
            ResetToolbars();
            EnableDefaultPOIs();
            if (CurrentPOI != null)
            {
                CurrentPOI.CloseIntoReset();
            }
            else
            {
                EnableDefaultPOIs();
            }
        }

        public void ResetToolbars()
        {
            if (uiManager == null)
            {
                uiManager = UIManager.Instance;
            }
            
            if (uiManager.simulationStatus == SimulationStatus.Orientation)
            {
                uiManager.SetUpToolbar(new ToolbarConfig() { ShowMenuButton = true, HomeButtonCallback = null });
                uiManager.ResetPOIToolbar();
            }
            
            if (uiManager.simulationStatus == SimulationStatus.Biopsy)
            {
                uiManager.SetUpToolbar(new ToolbarConfig() { ShowMenuButton = true, HomeButtonCallback = null, ShowHandbookButton = true });
                uiManager.ResetPOIToolbar();
            }
        }

        /// <summary>
        /// Refreshes the managers lists and CurrentPOI values
        /// </summary>
        public void RefreshPOIs()
        {
            CurrentPOI = null;

            allExplorables.Clear();
            allPOIs.Clear();
            allMainPOIs.Clear();

            Explorable[] explorables = FindObjectsByType<Explorable>(FindObjectsSortMode.None);
            foreach (Explorable explorable in explorables)
            {
                allExplorables.Add(explorable.gameObject);
            }

            BronchoscopeComponent[] bronchoscopeComponents =
                FindObjectsByType<BronchoscopeComponent>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (BronchoscopeComponent component in bronchoscopeComponents)
            {
                allExplorables.Add(component.gameObject);
            }

            BiopsyTableItem[] biopsyTableItem = FindObjectsByType<BiopsyTableItem>(FindObjectsSortMode.None);
            foreach (BiopsyTableItem item in biopsyTableItem)
            {
                allExplorables.Add(item.gameObject);
            }

            POI[] pois = FindObjectsByType<POI>(FindObjectsSortMode.None);
            foreach (POI poi in pois)
            {
                allPOIs.Add(poi.gameObject);

                if (poi.IsMainPOI)
                {
                    allMainPOIs.Add(poi.gameObject);
                }
            }

            foreach (GameObject pseudoMainPOI in pseudoMainPOIs)
            {
                allPOIs.Add(pseudoMainPOI);
                allMainPOIs.Add(pseudoMainPOI);
            }
        }

        public void EnableIntractableAfterTransition(POICameraTransition animationType)
        {
            CurrentPOI.EnablePOIInteractions(animationType);
            if (CurrentPOI != null && CurrentPOI.POIName == POIType.PatientView)
            {
                uiManager.ShowScreen(UIPanel.PatientStatus);
            }
        }

        public void HandlePOICameraChange(GameObject camera, POICameraTransition transitionType)
        {
            OnPOIOpened?.Invoke(camera, transitionType);
        }

        public void HandlePOICameraReset(bool simulatedReset = false)
        {
            OnPOIReset?.Invoke(simulatedReset);
        }

        public void HandlePOICameraSimulatedReset()
        {
            CurrentPOI.CloseIntoSimulatedReset();
            ShowOutlineableWhenOnBronchoscopePOI(false);
        }

        public void HandleExecutePOITask(POIType taskPOI)
        {
            TaskPOI = taskPOI;
            foreach (GameObject poi in allPOIs)
            {
                //Alternate work flow for the front view POI because it's not actually a POI (default camera position for the procedure)
                if (CurrentPOI != null && taskPOI == POIType.FrontView)
                {
                    HandlePOICameraSimulatedReset();
                    return;
                }

                POI poiComponent = poi.GetComponent<POI>();
                if (poiComponent != null)
                {
                    if (poiComponent.POIName == taskPOI)
                    {
                        if (CurrentPOI == poiComponent)
                        {
                            return;
                        }
                        CurrentPOI = poiComponent;
                        CurrentPOI.Open();
                    }
                }
            }
        }
    }
}