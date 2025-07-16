using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VARLab.RespiratoryTherapy;

namespace Tests.PlayMode
{
    public class ItemRequestTaskSOTests
    {
        private ItemRequestTaskSO itemRequestTask;
        private GameObject tableControllerObject;
        private TableController tableController;
        private GameObject biopsyControllerObject;
        private BiopsyController biopsyController;
        private PromptSO promptSO;

        [SetUp]
        [Category("BuildServer")]
        public void Setup()
        {
            itemRequestTask = ScriptableObject.CreateInstance<ItemRequestTaskSO>();
            promptSO = ScriptableObject.CreateInstance<PromptSO>();

            tableControllerObject = new GameObject();
            tableController = tableControllerObject.AddComponent<TableController>();

            biopsyControllerObject = new GameObject();
            biopsyController = biopsyControllerObject.AddComponent<BiopsyController>();
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(itemRequestTask);
            Object.Destroy(tableControllerObject);
            Object.Destroy(biopsyControllerObject);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator ItemRequestTask_SetRequestedItem_ShouldSetTheRequestedItem()
        {
            //Arrange
            BiopsyProcedureEquipment expectedItem = BiopsyProcedureEquipment.BiopsyForceps;

            //Act
            itemRequestTask.requestedItem = expectedItem;
            yield return null;

            //Assert
            Assert.AreEqual(expectedItem, itemRequestTask.requestedItem);
        }

        [UnityTest]
        [Category("BuildServer")]
        public IEnumerator ItemRequestTask_Execute_ShouldSetPromptMessagesOnTableController()
        {
            //Arrange
            itemRequestTask.requestedItem = BiopsyProcedureEquipment.BiopsyForceps;
            itemRequestTask.RequestPrompt = promptSO;
            promptSO.Message = "Please select the Biopsy Forceps.";

            tableController.ItemRequestPrompt = ScriptableObject.CreateInstance<PromptSO>();
            tableController.WrongItemSelectedPrompt = ScriptableObject.CreateInstance<PromptSO>();

            //Act
            itemRequestTask.Execute();
            yield return null;

            //Assert
            Assert.AreEqual(promptSO.Message, tableController.ItemRequestPrompt.Message);
            Assert.AreEqual($"That's not the Biopsy Forceps. Please try again.",
                tableController.WrongItemSelectedPrompt.Message);
        }
    }
}
