using System.Collections.Generic;
using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "UIPanelDataSO", menuName = "RT Scriptable Objects/UIPanelDataSO")]
    public class UIPanelDataSO : ScriptableObject
    {
        public List<PanelContent> PanelContents = new ();
    }
}
