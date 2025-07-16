using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "PromptSO", menuName = "RT Scriptable Objects/PromptSO")]
    public class PromptSO : ScriptableObject
    {
        [Header("Content"), Space(5)]
        [TextArea(1, 3), Tooltip("The [Speaker] label of the prompt")]
        public string Speaker;

        [TextArea(1, 3), Tooltip("The [Message] label of the prompt")]
        public string Message;

        [Tooltip("The NPC image not including the background")]
        public Sprite Image;

        [Tooltip("Colour of the [Bar] element on the left of the prompt")]
        public Color BarColour;

        [Tooltip("Border colour of the [ImageContainer] element on the left of the prompt")]
        public Color ImageBorderColour;
        
        [Tooltip("Colour of the [Speaker] label on the top of the prompt")]
        public Color SpeakerColour;

        [Tooltip("Whether the dismiss (>) button is enabled or not")]
        public bool IsBtnEnabled = false;
    }
}