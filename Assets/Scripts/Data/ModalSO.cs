using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "ModalSO", menuName = "RT Scriptable Objects/Dialogs/ModalSO")]
    public class ModalSO : ScriptableObject
    {
        [Header("Content"), Space(5)]
        [TextArea(1, 3), Tooltip("The [Name] label of the modal")]
        public string Name;

        [TextArea(1, 3), Tooltip("The [TitleLabel] label of the modal in the content section")]
        public string Title;

        [TextArea(1, 3), Tooltip("The [DescriptionLabel] label of the modal in the content section")]
        public string Description;

        [TextArea(1, 3), Tooltip("The [SubDescriptionLabel] label of the modal in the content section")]
        public string SubDescription;

        [TextArea(1, 3), Tooltip("The [NoteLabel] label of the modal in the content section")]
        public string Note;

        [TextArea(1, 3), Tooltip("The text of the button at the bottom of the modal")]
        public string ButtonText;

        [Tooltip("The NPC image not including the background")]
        public Sprite Image;

        [Tooltip("The Note image")]
        public Sprite NoteImage;

        [Tooltip("Whether the background behind the UI is dimmed or not")]
        public bool IsCanvasDimmed = false;
    }
}