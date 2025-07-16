using System.ComponentModel;

namespace VARLab.RespiratoryTherapy
{
    /// <summary>
    /// This enum holds the various categories that a Explorable can be apart of
    /// </summary>
    public enum ExplorableCategory
    {
        [Description("Medications")]
        Medications = 0,

        [Description("General Supplies")]
        GeneralSupplies = 1,

        [Description("Bronchoscopy Supplies")]
        BronchoscopySupplies = 2,

        [Description("Room Utilities")]
        RoomUtilities = 3,

        [Description("Bronch Tower")]
        BronchTower = 4
    }
}