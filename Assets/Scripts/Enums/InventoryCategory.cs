using System.ComponentModel;

namespace VARLab.RespiratoryTherapy
{
    public enum InventoryCategory
    {
        [Description("Medications")]
        Medications,

        [Description("General Supplies")]
        GenSupplies,

        [Description("Bronchoscopy Supplies")]
        BronchSupplies
    }
}
