using UnityEngine;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    public class PatientStatusController : MonoBehaviour, IUserInterface
    {
        private VisualElement root;
        private Label title, content;
        private const string TitleText = "Patient Status";

        [SerializeField] private PatientStatusSO initialStatus;

        private void Start()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            title = root.Q<Label>("Name");
            content = root.Q<Label>("ContentLabel");

            title.text = TitleText;
            HandleSetStatus(initialStatus);
            Hide();
        }
        
        public void Hide()
        {
            root.Hide();
        }

        public void Show()
        {
            root.Show();
        }

        public void HandleSetStatus(PatientStatusSO patientStatusSO)
        {
            content.text = patientStatusSO.StatusDescription;
        }
    }
}
