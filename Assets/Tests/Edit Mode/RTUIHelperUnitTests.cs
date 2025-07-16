using UnityEngine;
using NUnit.Framework;
using UnityEngine.UIElements;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;
using UnityEditor;

namespace Tests.EditMode
{
    public class RTUIHelperUnitTests
    {
        VisualElement validElement;
        VisualElement invalidElement;
        Sprite sprite;

        [SetUp]
        public void SetUp()
        {
            sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/Prompt/PromptNPC_1_Sprite.png");
            validElement = new VisualElement();
            invalidElement = null;
        }

        [Test, Order(1)]
        public void SetBorderColour_WithValidVisualElement_SetsBorderColour()
        {
            //Arrange
            StyleColor expectedBorderColour = Color.blue;

            //Act
            RTUIHelper.SetBorderColour(validElement, Color.blue);

            //Assert
            Assert.AreEqual(expectedBorderColour, validElement.style.borderBottomColor);
            Assert.AreEqual(expectedBorderColour, validElement.style.borderTopColor);
            Assert.AreEqual(expectedBorderColour, validElement.style.borderLeftColor);
            Assert.AreEqual(expectedBorderColour, validElement.style.borderRightColor);
        }

        [Test, Order(2)]
        public void SetBorderColour_WithInvalidVisualElement_LogsError()
        {
            //Arrange
            string expectedError = "RTUIHelper.SetBorderColour() - Incoming VisualElement is Null!";

            //Act
            RTUIHelper.SetBorderColour(invalidElement, Color.blue);

            //Assert
            LogAssert.Expect(LogType.Error, expectedError);
        }

        [Test, Order(3)]
        public void SetBackgroundImage_WithValidVisualElement_SetsBackgroundImage()
        {
            //Arrange
            StyleBackground expectedSprite = new StyleBackground(sprite);

            //Act
            RTUIHelper.SetBackgroundImage(validElement, sprite);

            //Assert
            Assert.AreEqual(expectedSprite, validElement.style.backgroundImage);
        }

        [Test, Order(4)]
        public void SetBackgroundImage_WithInvalidVisualElement_LogsError()
        {
            //Arrange
            string expectedError = "UIHelper.SetElementSprite() - Incoming VisualElement is Null!";

            //Act
            RTUIHelper.SetBackgroundImage(invalidElement, sprite);

            //Assert
            LogAssert.Expect(LogType.Error, expectedError);
        }

        [Test, Order(5)]
        public void SetBackgroundImage_WithInvalidSprite_LogsError()
        {
            //Arrange
            string expectedError = "UIHelper.SetElementSprite() - Incoming VisualElement is Null!";

            //Act
            RTUIHelper.SetBackgroundImage(invalidElement, null);

            //Assert
            LogAssert.Expect(LogType.Error, expectedError);
        }

        [Test, Order(6)]
        public void SetBackgroundColour_WithValidVisualElement_SetsBackgroundColour()
        {
            //Arrange
            StyleColor expectedBackgroundColour = Color.red;

            //Act
            RTUIHelper.SetBackgroundColour(validElement, Color.red);

            //Assert
            Assert.AreEqual(expectedBackgroundColour, validElement.style.backgroundColor);
        }

        [Test, Order(7)]
        public void SetBackgroundColour_WithInvalidVisualElement_LogsError()
        {
            //Arrange
            string expectedError = "RTUIHelper.SetBackgroundColour() - Incoming VisualElement is Null!";

            //Act
            RTUIHelper.SetBackgroundColour(invalidElement, Color.red);

            //Assert
            LogAssert.Expect(LogType.Error, expectedError);
        }
    }
}