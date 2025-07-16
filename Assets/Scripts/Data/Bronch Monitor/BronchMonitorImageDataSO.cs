using System.Collections.Generic;
using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "BronchMonitorImageDataSO", 
        menuName = "RT Scriptable Objects/Biopsy/Bronch Monitor/Bronch Monitor Image Data")]
    public class BronchMonitorImageDataSO : ScriptableObject
    {
        public List<Sprite> Sprites;
    }
}
