using NUnit.Framework;
using UnityEngine;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class BiopsyProcedureComponentBaseIntegrationTests
    {
        private BiopsyProcedureComponentBase biopsyProcedureComponentBase;
        
        [SetUp]
        public void SetUp()
        {
            GameObject biopsyProcedureComponentBaseObject = new GameObject("BiopsyProcedureComponentBaseObject");
            biopsyProcedureComponentBase = biopsyProcedureComponentBaseObject.AddComponent<BiopsyProcedureComponentBase>();
        }

        [TearDown]
        public void TearDown()
        {
            biopsyProcedureComponentBase = null;
        }

        [Test]
        public void ClickOnComponent_ShouldSetComponentAccessedTrue()
        {
            //Assign
            biopsyProcedureComponentBase.ComponentAccessed = false;

            //Act
            biopsyProcedureComponentBase.ClickOnComponent();

            //Assert
            Assert.IsTrue(biopsyProcedureComponentBase.ComponentAccessed);
        }

        [Test]
        public void ResetComponent_ShouldSetComponentAccessedFalse()
        {
            //Assign
            biopsyProcedureComponentBase.ComponentAccessed = true;

            //Act
            biopsyProcedureComponentBase.ResetComponent();

            //Assert
            Assert.IsTrue(!biopsyProcedureComponentBase.ComponentAccessed);
        }
    }
}
