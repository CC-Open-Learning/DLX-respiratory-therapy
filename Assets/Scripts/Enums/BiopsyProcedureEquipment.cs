using System.ComponentModel;

namespace VARLab.RespiratoryTherapy
{
    public enum BiopsyProcedureEquipment
    {
        [Description("Bronchoscope")]
        Bronchoscope = 0,
        
        [Description("Saline 20cc")]
        Saline20cc = 1,
        
        [Description("Biopsy Forceps")]
        BiopsyForceps = 2,
        
        [Description("Specimen Jar")]
        SpecimenJar = 3,
        
        [Description("Cytology Brush")]
        CytologyBrush = 4,
        
        [Description("Magill Forceps")]
        MagillForceps = 5,
        
        [Description("Lidocaine Spray")]
        LidocaineSpray = 6,
        
        [Description("Lidocaine Jelly")]
        LidocaineJelly = 7,
        
        [Description("Lidocaine 2%")]
        LidocaineTwoPercent = 8,
        
        [Description("Sterile Sputum Specimen Container")]
        SterileSputumSpecimenContainer = 9,

        [Description("Bronchoscope With Saline 20 CC")]
        BronchoscopeWithSaline20CC =10,

        [Description("Bronchoscope With Saline 10 CC")]
        BronchoscopeWithSaline10CC =11,

        [Description("Bronchoscope With Biopsy Forceps")]
        BronchoscopeWithBiopsyForceps =12,
        
        [Description("Bronchoscope Connector")]
        BronchoscopeConnector =13,
        
        [Description("Stethoscope")]
        Stethoscope = 14,
        
        [Description("Epinephrine")]
        Epinephrine  = 15,
        
        [Description("Bronchoscope With Epinephrine")]
        BronchoscopeWithEpinephrine  = 16,
        
        [Description("Suction Tube Connector")]
        SuctionTubeConnector =17,
        
        [Description("Saline 10cc")]
        Saline10cc =18,

        None = 19
    }
}
