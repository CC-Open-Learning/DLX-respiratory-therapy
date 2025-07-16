using UnityEngine;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    public static class RTUIHelper
    {
        /// <summary>
        /// Sets the border colour of the incoming VisualElement to the incoming borderColour
        /// </summary>
        /// <param name="element"></param>
        /// <param name="borderColour"></param>
        public static void SetBorderColour(VisualElement element, StyleColor borderColour)
        {
            if (element == null)
            {
                Debug.LogError("RTUIHelper.SetBorderColour() - Incoming VisualElement is Null!");
                return;
            }

            element.style.borderTopColor = borderColour;
            element.style.borderBottomColor = borderColour;
            element.style.borderRightColor = borderColour;
            element.style.borderLeftColor = borderColour;
        }

        /// <summary>
        /// Sets the background image property of the incoming VisualElement to the incoming sprite
        /// </summary>
        /// <param name="element"></param>
        /// <param name="sprite"></param>
        public static void SetBackgroundImage(VisualElement element, Sprite sprite)
        {
            if (element == null)
            {
                Debug.LogError("UIHelper.SetElementSprite() - Incoming VisualElement is Null!");
                return;
            }

            if (sprite == null)
            {
                element.Hide();
            }
            else
            {
                element.Show();
                element.style.backgroundImage = new StyleBackground(sprite);
            }
        }

        /// <summary>
        /// Sets the background colour property of the incoming VisualElement to the incoming backgroundColour
        /// </summary>
        /// <param name="element"></param>
        /// <param name="backgroundColour"></param>
        public static void SetBackgroundColour(VisualElement element, StyleColor backgroundColour)
        {
            if (element == null)
            {
                Debug.LogError("RTUIHelper.SetBackgroundColour() - Incoming VisualElement is Null!");
                return;
            }

            element.style.backgroundColor = backgroundColour;
        }
    }
}