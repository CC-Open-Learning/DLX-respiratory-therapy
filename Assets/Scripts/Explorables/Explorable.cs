using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using VARLab.CloudSave;
using VARLab.Interactions;

namespace VARLab.RespiratoryTherapy
{
    [CloudSaved]
    [JsonObject(MemberSerialization.OptIn)]
    [RequireComponent(typeof(Interactable))]
    public class Explorable : MonoBehaviour
    {
        [Tooltip("Associated Explorable Information Scriptable Object for explorable")]
        public ExplorableInformationSO explorableInformation;

        [Tooltip("Assoicated values to set as the activeSceneExplorables transform when first entering the object viewer panel")]
        public ExplorableTransformDetails transformDetails;

        [JsonProperty]
        public bool IsExplored = false;
        
        [Header("Events")]
        ///Invokes <see cref="BronchTowerItemHighliter.HighlightExplorable()"/>
        public UnityEvent OnHighlightExplorable  = new UnityEvent();
        ///Invokes <see cref="BronchTowerItemHighliter.RestoreOriginalMaterials()"/>
        public UnityEvent OnUnHighlightExplorable  = new UnityEvent();

        void Start()
        {
            if (explorableInformation == null)
            {
                Debug.LogWarning("No Explorable Information associated with Explorable");
                return;
            }
        }

        /// <summary>
        /// Marks the item as explored by setting the IsExplored property to true.
        /// </summary>
        public void MarkItemAsExplored()
        {
            IsExplored = true;
        }

        /// <summary>
        /// Sets the activeSceneExplorables trasnform values upon entering the object viewer panel
        /// </summary>
        public void SetExplorableTransformValues()
        {
            if (transformDetails != null)
            {
                transform.rotation = transformDetails.rotationValues;
                transform.localScale = transformDetails.scaleValues;
            }
        }

        /// <summary>
        /// Invokes the <c>OnHighlightExplorable</c> event to trigger any actions 
        /// associated with highlighting explorable items.
        /// </summary>
        public void HighlightExplorable()
        {
            OnHighlightExplorable?.Invoke();
        }

        /// <summary>
        /// Invokes the OnUnHighlightExplorable event to trigger any actions 
        /// associated with removing the highlight from explorable items.
        /// </summary>
        public void UnHighlightExplorable()
        {
            OnUnHighlightExplorable?.Invoke();
        }
    }
}