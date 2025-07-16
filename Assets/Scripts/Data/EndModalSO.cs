using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "EndModalSO", menuName = "RT Scriptable Objects/Dialogs/EndModalSO")]
    public class EndModalSO : ScriptableObject
    {
        [Header("Content"), Space(5)]
        [TextArea(1, 3), Tooltip("The [Name] label of the modal")]
        public string Name;
        
        [TextArea(1, 3), Tooltip("The [TitleLabel] label of the modal in the content section")]
        public string Title;

        [Tooltip("The visual element representing the horizontal line")]
        public VisualElement LineSeparator;
        
        [TextArea(1, 3), Tooltip("The [DescriptionLabel] label of the modal in the content section")]
        public string Description;

        [TextArea(1, 3), Tooltip("The text of the button at the bottom left of the modal")]
        public string ReturnToMenuButtonText;
        
        [TextArea(1, 3), Tooltip("The text of the button at the bottom right of the modal")]
        public string NextModuleButtonText;

        [Tooltip("The NPC image not including the background")]
        public Sprite Image;

        [Tooltip("Whether the background behind the UI is dimmed or not")]
        public bool IsCanvasDimmed = false;
    }
}