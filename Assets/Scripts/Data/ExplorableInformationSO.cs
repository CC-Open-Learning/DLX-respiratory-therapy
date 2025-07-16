using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    /// <summary>
    /// Scriptable Objects that holds information about activeSceneExplorables
    /// </summary>
    [CreateAssetMenu(fileName = "ExplorableInformationSO", menuName = "RT Scriptable Objects/ExplorableInformationSO")]
    public class ExplorableInformationSO : ScriptableObject
    {
        [Tooltip("Explorable Name")]
        public string ExplorableName;

        [Tooltip("Explorable Description")]
        [TextArea(10, 100)]
        public string ExplorableDescription;

        [Tooltip("Explorable Sprite")]
        public Sprite ExplorableSprite;

        [Tooltip("Explorable Category")]
        public ExplorableCategory ExplorableCategory;
        
        [Tooltip("Explorable POI Type")]
        public POIType PoiType;
        
        [Header("Audio Clips")]
        [Tooltip("Explorable Audio")]
        public AudioClip ExplorableAudioClip;
    }
}