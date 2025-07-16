using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "ExplorableTransformDetails", menuName = "RT Scriptable Objects/ExplorableTransformDetails")]
    public class ExplorableTransformDetails : ScriptableObject
    {
        public Quaternion rotationValues;
        public Vector3 scaleValues = Vector3.one;
    }
}
