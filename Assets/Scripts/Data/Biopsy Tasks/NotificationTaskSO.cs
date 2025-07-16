using UnityEngine;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "Notification Task", menuName = "RT Scriptable Objects/Biopsy/Scenario Tasks/Notification Task")]
    public class NotificationTaskSO : BiopsyBaseTaskSO
    {
        public NotificationSO NotificationSO;
        private Velcro.Notification notification;

        public override void Execute()
        {
            if (notification == null)
            {
                notification = FindFirstObjectByType<Velcro.Notification>();
            }
            notification.HandleDisplayUI(NotificationSO);
        }
    }
}
