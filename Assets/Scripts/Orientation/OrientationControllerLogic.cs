using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    public class OrientationControllerLogic
    {
        public List<Explorable> Explorables { get; private set; } = new();
        
        public bool IsOrientationCompleted() => Explorables.All(explorable => explorable.IsExplored);

        public void AddExplorable(List<Explorable> explorableList)
        {
            if (explorableList.Any())
            {
                Explorables.AddRange(explorableList);
            }
            else
            {
                //TODO: remove this when we go live
                Debug.LogWarning("There are no explorable active on the scene");
            }
        }
    }
}
