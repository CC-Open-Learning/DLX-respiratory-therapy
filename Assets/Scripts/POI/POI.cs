using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    public class POI : MonoBehaviour
    {
        private const int ImmediateParentIndex = 1;

        public bool IsMainPOI;
        public POIType POIName;
        
        [SerializeField] private CinemachineVirtualCamera poiCamera;
        [SerializeField] private List<GameObject> childPOI;
        [SerializeField] private List<GameObject> explorables;
        [SerializeField] private Sprite POIIcon;

        private UIManager uiManager;
        private POIManager poiManager;

        private POI parent = null;

        private void Start()
        {
            uiManager = UIManager.Instance;
            poiManager = POIManager.Instance;

            POI[] parents = GetComponentsInParent<POI>();
            if (parents.Length > ImmediateParentIndex) { parent = parents[ImmediateParentIndex]; }
        }

        public void Open()
        {
            poiManager.HandlePOIOpened(this, POIName.ToDescription());

            if (uiManager.simulationStatus == SimulationStatus.Orientation)
            {
                uiManager.SetUpPOIToolbar(POIName.ToDescription(), CloseIntoParent, POIIcon);
                uiManager.SetUpToolbar(new ToolbarConfig() { ShowMenuButton = true, HomeButtonCallback = CloseIntoReset });     
            }

            if (uiManager.simulationStatus == SimulationStatus.Biopsy)
            {
                uiManager.SetUpToolbar(new ToolbarConfig() { ShowMenuButton = true, HomeButtonCallback = CloseIntoReset, ShowHandbookButton = true});     
            }
           
            ActivatePOICamera(POICameraTransition.In);
        }

        /// <summary>
        /// Close this POI and return to the default camera
        /// </summary>
        public void CloseIntoReset()
        {
            poiManager.ResetToolbars();
            ActivatePOICamera(POICameraTransition.Reset);
        }

        /// <summary>
        /// Close this POI and return to the default camera, but don't perform any other reset related tasks
        /// </summary>
        public void CloseIntoSimulatedReset()
        {
            poiManager.ResetToolbars();
            ActivatePOICamera(POICameraTransition.SimulatedReset);
        }

        /// <summary>
        /// Close this POI and return to its immediate parent
        /// </summary>
        public void CloseIntoParent()
        {
            // If this POI doesn't have a parent act like its parent is the default camera
            if (parent == null)
            {
                CloseIntoSimulatedReset();
                return;
            }

            if (uiManager.simulationStatus == SimulationStatus.Orientation)
            {
                uiManager.SetUpPOIToolbar(parent.POIName.ToDescription(), parent.CloseIntoParent, parent.POIIcon);
                uiManager.SetUpToolbar(new ToolbarConfig() { ShowMenuButton = true, HomeButtonCallback = parent.CloseIntoReset });
            }
            parent.ActivatePOICamera(POICameraTransition.Out);
        }

        /// <summary>
        /// Perform pre-transition tasks, then start the required camera transition
        /// </summary>
        private void ActivatePOICamera(POICameraTransition transitionType)
        {
            uiManager.HideScreen(UIPanel.POIToolbar);
            uiManager.HideScreen(UIPanel.Toolbar);
            uiManager.HideScreen(UIPanel.ProcedureToolbar);
            uiManager.HideScreen(UIPanel.PatientStatus);

            poiManager.ToggleAllExplorables(false);
            poiManager.ToggleAllPOIs(false);

            if (transitionType == POICameraTransition.Reset || transitionType == POICameraTransition.SimulatedReset)
            {
                poiManager.HandlePOICameraReset(transitionType == POICameraTransition.SimulatedReset);
            }
            else
            {
                poiManager.HandlePOICameraChange(poiCamera.VirtualCameraGameObject, transitionType);
            }
        }

        /// <summary>
        /// Enables POI/explorable interactions after a camera transition is completed based on the transition type.
        /// </summary>
        public void EnablePOIInteractions(POICameraTransition transitionType)
        {
            switch (transitionType)
            {
                case POICameraTransition.Reset:
                    POIManager.Instance.EnableDefaultPOIs();
                    break;
                case POICameraTransition.SimulatedReset:
                    POIManager.Instance.EnableDefaultPOIs();
                    break;
                case POICameraTransition.Out:
                    parent.EnablePOIInteractions(POICameraTransition.In);
                    // Update CurrentPOI value to the parent
                    poiManager.HandlePOIOpened(parent);
                    break;
                case POICameraTransition.In:
                    InteractionHelper.ToggleInteractivity(childPOI, true);
                    InteractionHelper.ToggleInteractivity(explorables, true);
                    break;
            }

            UIPanel toolbar;
            switch (uiManager.simulationStatus)
            {
                case SimulationStatus.Orientation:
                    toolbar = UIPanel.POIToolbar;
                    break;
                default:
                    toolbar = UIPanel.ProcedureToolbar;
                    break;
            }

            if (transitionType == POICameraTransition.Reset)
            {
                uiManager.HideScreen(toolbar);
            }
            else if (transitionType == POICameraTransition.SimulatedReset && uiManager.simulationStatus == SimulationStatus.Orientation)
            {
                uiManager.HideScreen(toolbar);
                uiManager.ShowScreen(UIPanel.Toolbar);
            }
            else
            {
                uiManager.ShowScreen(toolbar);
                uiManager.ShowScreen(UIPanel.Toolbar);
            }
        }
    }
}
