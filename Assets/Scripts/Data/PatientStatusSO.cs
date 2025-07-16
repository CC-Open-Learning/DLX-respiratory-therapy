using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "PatientStatusSO", menuName = "RT Scriptable Objects/PatientStatusSO")]
    public class PatientStatusSO : ScriptableObject
    {
        [Header("Content"), Space(5)]
        [TextArea(1, 6), Tooltip("Patient Status Description")]
        public string StatusDescription;
    }
}
