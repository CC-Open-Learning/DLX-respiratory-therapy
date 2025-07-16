using System;
using JetBrains.Annotations;

namespace VARLab.RespiratoryTherapy
{
    public struct ToolbarConfig
    {
        public bool ShowMenuButton;
        public bool ShowHandbookButton;
        [CanBeNull] public Action HandbookButtonCallback;
        [CanBeNull] public Action HomeButtonCallback;
    }
}
