using NUnit.Framework;
using System;
using VARLab.RespiratoryTherapy;

namespace Tests.EditMode
{
    public class NameFormatterTests
    {
        enum TestEnum { Test = 0 };


        /// <summary>
        /// This test ensures when we get the correct enum description value       
        /// </summary>
        [Test]
        public void NameFormatter_GetEnumDescription_CorrectDescriptionReturned()
        {
            //Arrange
            string expectedResult = "Bronch Tower";
            ExplorableCategory category = ExplorableCategory.BronchTower;

            //Act
            string actualResult = category.ToDescription();

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        /// <summary>
        /// Test ensures that when an enum without a description attribute is passed in
        /// that the correct value is returned
        /// </summary>
        [Test]
        public void NameFormatter_NoEnumDescription_CorrectValueReturned()
        {
            //Arrange
            TestEnum test = TestEnum.Test;
            string expectedResult = "Test";

            //Act
            string actualResult = test.ToDescription();

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }


        /// <summary>
        /// Test ensures when we pass in something that is not an enum that the
        /// correct exception is thrown
        /// </summary>
        [Test]
        public void NameFormatter_EnumNotPassedIn_ExceptionThrown()
        {
            //Arrange
            string notAnEnum = string.Empty;

            //Act + Assert
            Assert.Throws<ArgumentException>(() =>
            {
                NameFormatter.ToDescription(notAnEnum);
            });
        }
    }
}
