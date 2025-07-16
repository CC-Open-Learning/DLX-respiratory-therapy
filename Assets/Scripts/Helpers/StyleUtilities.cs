using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.RespiratoryTherapy
{
    /// <summary>
    /// A static class that holds various strings of hexcodes for colors, as well as generic UI 
    /// functions not currently provided through Velcro that we need
    /// </summary>
    public static class StyleUtilities
    {
        //For no progress in inventory
        public static string InventoryNoProgress = "#FFFFFF";

        //For when the inventory is in progress
        public static string InventoryInProgress = "#FFCD29";

        //For a compeletley explored Invetory
        public static string InventoryComplete = "#4BC17E";

        /// <summary>
        /// Update the color of anything with a style
        /// </summary>
        /// <param name="style">The style property to change</param>
        /// <param name="hexCode">Hex code of color to use</param>
        public static void ChangeStyleColor(IStyle style, string hexCode)
        {
            if (style == null || HexCodeToColor(hexCode, out Color color) == null) { return; }
            style.color = color;
        }

        /// <summary>
        /// Takes in a hex code and spits back the corresponding color, if it exists
        /// otherwise returns the default color of White
        /// </summary>
        /// <param name="colorHex">The code of color to obtain</param>
        /// <param name="color">The color being returned</param>
        /// <returns></returns>
        public static Color HexCodeToColor(string colorHex, out Color color)
        {
            if (!ColorUtility.TryParseHtmlString(colorHex, out color))
            {
                color = Color.white;
            }
            return color;
        }
    }
}