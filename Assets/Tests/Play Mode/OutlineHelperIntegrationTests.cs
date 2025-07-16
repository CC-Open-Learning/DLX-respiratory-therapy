using EPOOutline;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class OutlineHelperIntegrationTests : MonoBehaviour
    {
        private readonly Color BaseHighlightColour = new Color(0.1f, 0.1f, 0.1f);
        private readonly Color ActiveHighlightColour = new Color(0.2f, 0.2f, 0.2f);

        private int sceneCounter = 0;

        private Outlinable outline;
        private OutlineHelper outlineHelper;

        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            sceneCounter = TestUtils.ClearScene(sceneCounter, "OutlineHelperScene");

            //Set up <GameObject> with Outlinable
            GameObject outlineObject = new GameObject("Outline Object");
            outline = outlineObject.AddComponent<Outlinable>();
            outlineHelper = outlineObject.AddComponent<OutlineHelper>();

            //Reference outline colours as SerializedFields
            SerializedObject so = new SerializedObject(outlineHelper);
            so.FindProperty("baseHighlight").colorValue = BaseHighlightColour;
            so.FindProperty("activeHighlight").colorValue = ActiveHighlightColour;
            so.ApplyModifiedProperties();

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            outline = null;
            outlineHelper = null;

            yield return null;
        }

        [Test]
        public void Start_ShouldSetOutlineColourToBase()
        {
            //Assert
            Assert.AreEqual(outline.OutlineParameters.Color, BaseHighlightColour);
        }

        [Test]
        public void SetHighlightActive_True_ShouldSetOutlineColourToActive()
        {
            //Arrange
            outline.OutlineParameters.Color = BaseHighlightColour;

            //Act
            outlineHelper.SetHighlightActive(true);

            //Assert
            Assert.AreEqual(outline.OutlineParameters.Color, ActiveHighlightColour);
        }

        [Test]
        public void SetHighlightActive_False_ShouldSetOutlineColourToBase()
        {
            //Arrange
            outline.OutlineParameters.Color = ActiveHighlightColour;

            //Act
            outlineHelper.SetHighlightActive(false);

            //Assert
            Assert.AreEqual(outline.OutlineParameters.Color, BaseHighlightColour);
        }

        [Test]
        public void SetOutlineActive_True_ShouldEnableOutlinableComponent()
        {
            //Arrange
            outline.enabled = false;

            //Act
            outlineHelper.SetOutlineActive(true);

            //Assert
            Assert.IsTrue(outline.enabled);
        }

        [Test]
        public void SetOutlineActive_False_ShouldDisableOutlinableComponent()
        {
            //Arrange
            outline.enabled = true;

            //Act
            outlineHelper.SetOutlineActive(false);

            //Assert
            Assert.IsTrue(!outline.enabled);
        }
    }
}
