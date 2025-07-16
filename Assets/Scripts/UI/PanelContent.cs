using System;
using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [Serializable]
    public class PanelContent
    {
        public ContentInfo Type;
        public string Header;
        [TextArea]
        public string Title;
        [TextArea]
        public string Message;
        public string ButtonText;
    }
}
