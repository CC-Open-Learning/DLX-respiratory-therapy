using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class BronchMonitorIntegrationTests
    {
        private readonly string scenePath = "Assets/Scenes/BronchMonitorTestScene.unity";
        private UIDocument uIDocument;
        private BronchMonitorController bronchMonitorController;
        private VisualElement root;
        private Button topButton;
        private Button bottomButton;
        
        private BronchMonitorTaskSO bronchMonitorTaskSO;

        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath,
                new LoadSceneParameters(LoadSceneMode.Single));

            bronchMonitorController = Object.FindAnyObjectByType<BronchMonitorController>();
            uIDocument = bronchMonitorController.GetComponent<UIDocument>();
            root = uIDocument.rootVisualElement;
            topButton = root.Q<VisualElement>("TopContainer").Q<Button>();
            bottomButton = root.Q<VisualElement>("BottomContainer").Q<Button>();

            bronchMonitorTaskSO = ScriptableObject.CreateInstance<BronchMonitorTaskSO>();
            bronchMonitorTaskSO.AutoCycleImages = true;
            
            var texture = new Texture2D(64, 64);
            
            var sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));

            bronchMonitorTaskSO.BronchMonitorDataList = new List<BronchMonitorTaskSO.BronchMonitorData>
            {
                new BronchMonitorTaskSO.BronchMonitorData { Sprite = sprite, PromptTask = null },
                new BronchMonitorTaskSO.BronchMonitorData { Sprite = sprite, PromptTask = null }
            };

            var material = new Material(Shader.Find("Unlit/Texture")) { mainTexture = texture };
            bronchMonitorController.EmptyAirwayBronchMonitorMaterial = material;
            bronchMonitorController.AdvanceNeedleButtonText = "Advance Needle";
            bronchMonitorController.RetractNeedleButtonText = "Retract Needle";
        }

        [UnityTest]
        public IEnumerator BronchMonitorTask_SetBronchMonitorStatus_ShouldSetBronchMonitorStatus()
        {
            bronchMonitorTaskSO.MonitorStatus = BronchMonitorStatus.Default;
            yield return null;
            Assert.AreEqual(BronchMonitorStatus.Default, bronchMonitorTaskSO.MonitorStatus);
        }

        [UnityTest]
        public IEnumerator AutoCycleImages_ShouldCycleImages()
        {
            //Assert
            //Created two sprite as we need to check the sprite change in the auto image change.
            var textureOne = new Texture2D(64, 64);
            var textureTwo = new Texture2D(128, 128);
            
            //Created two sprite as we need to check the sprite change in the auto image change.
            var spriteOne = Sprite.Create(textureOne, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
            var spriteTwo = Sprite.Create(textureTwo, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));

            bronchMonitorTaskSO.BronchMonitorDataList = new List<BronchMonitorTaskSO.BronchMonitorData>
            {
                new BronchMonitorTaskSO.BronchMonitorData { Sprite = spriteOne, PromptTask = null },
                new BronchMonitorTaskSO.BronchMonitorData { Sprite = spriteTwo, PromptTask = null }
            };

            var expectedResultOne = bronchMonitorTaskSO.BronchMonitorDataList[0].Sprite.texture;
            var expectedResultTwo = bronchMonitorTaskSO.BronchMonitorDataList[1].Sprite.texture;
            
            bronchMonitorTaskSO.MonitorStatus = BronchMonitorStatus.Default;
            bronchMonitorController.SetBronchMonitorData(bronchMonitorTaskSO);

            Assert.AreEqual(expectedResultOne, bronchMonitorController.BronchMonitorMaterial.mainTexture);

            yield return new WaitForSeconds(3f);

            Assert.AreEqual(expectedResultTwo, bronchMonitorController.BronchMonitorMaterial.mainTexture);
        }

        [UnityTest]
        public IEnumerator SetMonitorMaterial_ShouldResetColorAndTexture()
        {
            bronchMonitorController.BronchMonitorMaterial.color = Color.red;
            bronchMonitorController.BronchMonitorMaterial.mainTexture = Texture2D.redTexture;

            bronchMonitorController.SetMonitorMaterial();
            yield return null;

            Assert.IsNull(bronchMonitorController.BronchMonitorMaterial.mainTexture);
            Assert.AreEqual(
                bronchMonitorController.DefaultBronchMonitorMaterial.color,
                bronchMonitorController.BronchMonitorMaterial.color
            );
        }

        [UnityTest]
        public IEnumerator Execute_Should_Enable_AdvanceNeedleUI_When_BronchMonitorStatus_AdvanceNeedle()
        {
            bronchMonitorTaskSO.MonitorStatus = BronchMonitorStatus.AdvanceNeedle;
            bronchMonitorTaskSO.Execute();
            yield return null;

            Assert.IsTrue(root.style.display == DisplayStyle.Flex);
            Assert.AreEqual("Advance Needle", bronchMonitorController.AdvanceNeedleButtonText);
        }

        [UnityTest]
        public IEnumerator Execute_Should_Enable_RetractNeedleUI_When_BronchMonitorStatus_RetractNeedle()
        {
            PromptTaskSO promptTask = ScriptableObject.CreateInstance<PromptTaskSO>();
            PromptSO promptSO = ScriptableObject.CreateInstance<PromptSO>();
            promptTask.PromptSO = promptSO;
            promptTask.prompt = Object.FindAnyObjectByType<Prompt>();
            
            bronchMonitorTaskSO.MonitorStatus = BronchMonitorStatus.RetractNeedle;
            bronchMonitorTaskSO.BronchMonitorDataList[0].PromptTask = promptTask;
            bronchMonitorTaskSO.Execute();
            yield return null;

            Assert.IsTrue(root.style.display == DisplayStyle.Flex);
            Assert.AreEqual("Retract Needle", bronchMonitorController.RetractNeedleButtonText);
        }

        [UnityTest]
        public IEnumerator Execute_Should_Call_AutoCycleImages_When_BronchMonitorStatus_Default_And_AutoCycleImages_True()
        {
            bronchMonitorTaskSO.MonitorStatus = BronchMonitorStatus.Default;
            bronchMonitorTaskSO.AutoCycleImages = true;

            bronchMonitorTaskSO.Execute();
            yield return null;

            Assert.NotNull(bronchMonitorController.autoCycleImagesCoroutine);
            Assert.AreEqual(
                bronchMonitorTaskSO.BronchMonitorDataList[1].Sprite.texture,
                bronchMonitorController.BronchMonitorMaterial.mainTexture
            );
            Assert.IsNull(bronchMonitorTaskSO.BronchMonitorDataList[1].PromptTask);
        }

        [UnityTest]
        public IEnumerator ExecuteAction_Should_Set_EmptyAirwayTexture_When_ConditionsAreMet()
        {
            bronchMonitorTaskSO.GoToNextTask = false;
            bronchMonitorTaskSO.GoToNextScenario = false;
            bronchMonitorTaskSO.ShowEmptyAirway = true;

            bronchMonitorTaskSO.Execute();
            bronchMonitorTaskSO.ExecuteAction();
            yield return null;

            Assert.AreEqual(
                bronchMonitorController.EmptyAirwayBronchMonitorMaterial.mainTexture,
                bronchMonitorController.BronchMonitorMaterial.mainTexture
            );
        }
        
        [UnityTest]
        public IEnumerator ResetImageIndex_Should_Set_Image_Index_To_Zero()
        {
            bronchMonitorController.BronchMonitorDataIndex = 4;
            
            bronchMonitorController.ResetImageIndex();
            yield return null;
            
            Assert.AreEqual(0, bronchMonitorController.BronchMonitorDataIndex);
        }
        
        [UnityTest]
        public IEnumerator UpdateImageIndex_Should_Disable_TopButton_When_AdvanceNeedle_Reaches_LastIndex()
        {
            bronchMonitorTaskSO.MonitorStatus = BronchMonitorStatus.AdvanceNeedle;
            bronchMonitorController.SetBronchMonitorData(bronchMonitorTaskSO);
            bronchMonitorController.BronchMonitorDataIndex = 0;

            bronchMonitorController.UpdateImageIndex();
            yield return null;

            Assert.IsFalse(topButton.enabledSelf);
            Assert.IsTrue(bottomButton.enabledSelf);
        }
        
        [UnityTest]
        public IEnumerator UpdateImageIndex_Should_Disable_BottomButton_When_RetractNeedle_Reaches_LastIndex()
        {
            bronchMonitorTaskSO.MonitorStatus = BronchMonitorStatus.RetractNeedle;
            bronchMonitorController.SetBronchMonitorData(bronchMonitorTaskSO);
            bronchMonitorController.BronchMonitorDataIndex = 0;

            bronchMonitorController.UpdateImageIndex();
            yield return null;

            Assert.IsTrue(topButton.enabledSelf);
            Assert.IsFalse(bottomButton.enabledSelf);
        }
        
        [UnityTest]
        public IEnumerator UpdateImageIndex_Should_StopCoroutine_And_ExecuteAction_When_AtEnd_If_AutoCycleImages_True()
        {
            bronchMonitorTaskSO.MonitorStatus = BronchMonitorStatus.Default;
            bronchMonitorTaskSO.AutoCycleImages = true;

            bronchMonitorController.SetBronchMonitorData(bronchMonitorTaskSO);
            yield return new WaitForSeconds(5f); //give it time to reach last index via autoCycleImages

            Assert.IsNull(bronchMonitorController.autoCycleImagesCoroutine);
            Assert.AreEqual(
                bronchMonitorController.EmptyAirwayBronchMonitorMaterial.mainTexture,
                bronchMonitorController.BronchMonitorMaterial.mainTexture
            );
        }
    }
}