using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;

namespace Tests.EditMode
{
    public class StyleUtilitiesUnitTests
    {
        private Label label;

        [SetUp]
        public void Setup()
        {
            label = new();
        }

        [TearDown]
        public void TearDown()
        {
            label = null;
        }

        /// <summary>
        /// Test ensures StyleUtilities can correctly set 
        /// </summary>
        [Test]
        public void StyleUtilities_ChangeLabelColor_NewColorIsSet()
        {
            //Arrange
            string hexCode = "#000000";
            //Act
            StyleUtilities.ChangeStyleColor(label.style, hexCode);
            //Assert
            Assert.AreEqual(label.resolvedStyle.color, Color.black);
        }

        /// <summary>
        /// Test ensures StyleUtilities can correcty set the default color text of white
        /// when given a hexcode that doesnt exist
        /// </summary>
        [Test]
        public void StyleUtilities_InvalidHexCode_TextIsDefaultWhite()
        {
            //Arrange
            string hexCode = "#thisdoesntexist";
            //Act
            StyleUtilities.ChangeStyleColor(label.style, hexCode);
            //Assert
            Assert.AreEqual(label.resolvedStyle.color, Color.white);
        }
    }
}