using System.ComponentModel;

namespace VARLab.RespiratoryTherapy
{
    public enum POIType
    {
        [Description("Bronch Tower")] 
        BronchTower = 0,

        [Description("Bronch Tower Upper Area")]
        BronchTowerUpperArea = 1,

        [Description("Bronch Tower Mid Area")] 
        BronchTowerMidArea = 2,

        [Description("Bronch Tower Lower Area")]
        BronchTowerLowerArea = 3,

        [Description("Bronchoscope")] 
        Bronchoscope = 4,

        [Description("Bronch Monitor")] 
        BronchMonitor = 5,

        [Description("Patient Monitor")] 
        PatientMonitor = 6,

        [Description("Procedure Table")] 
        ProcedureTable = 7,

        [Description("Front View")] 
        FrontView = 8,

        [Description("Patient View")] 
        PatientView = 9,

        [Description("Room Utilities")] 
        RoomUtilities = 10,

        [Description("Supply Cart")] 
        SupplyCart = 11,

        [Description("Procedure Bronchoscope")]
        BronchoscopeProcedure = 12
    }
}