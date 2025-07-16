using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "InventoryCategorySO", menuName = "RT Scriptable Objects/InventoryCategorySO")]
    public class InventoryCategorySO : ScriptableObject
    {
        public InventoryCategory InventoryCategory;
    }
}
