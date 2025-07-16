using System.Collections;
using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    public class TableController : MonoBehaviour
    {
        private Prompt prompt;
        private ItemRequestTaskSO task;
        private BiopsyController biopsyController;
        private BiopsyProcedureEquipment requestedItem;

        public PromptSO NoItemRequestedPrompt;
        public PromptSO WrongItemSelectedPrompt;
        public PromptSO ItemRequestPrompt;

        private void Start()
        {
            prompt = FindFirstObjectByType<Prompt>();
            biopsyController = FindFirstObjectByType<BiopsyController>();
            requestedItem = BiopsyProcedureEquipment.None;
        }

        private IEnumerator SchedulePromptHide()
        {
            const float promptDelay = 1f;
            yield return new WaitForSeconds(promptDelay);
            if (requestedItem != BiopsyProcedureEquipment.None)
            {
                prompt.HandleDisplayUI(ItemRequestPrompt);
            }
            else
            {
                prompt.HandleDisplayUI(biopsyController.CurrentTaskPrompt);
            }
        }

        /// <summary>
        /// The current scenario will pass the required item game object to this method to set it up to be checked if clicked when prompted
        /// </summary>
        /// <param name="requestedGameObject"></param>
        /// <param name="requestTask"></param>
        public void SetRequestedItem(BiopsyProcedureEquipment requestedGameObject, ItemRequestTaskSO requestTask)
        {
            requestedItem = requestedGameObject;
            task = requestTask;
            ItemRequestPrompt.Message = requestTask.RequestPrompt.Message;
            WrongItemSelectedPrompt.Message = $"That's not the {requestedItem.ToDescription()}. Please try again.";
        }

        /// <summary>
        /// Checks to see if the item being clicked matches the item required by the physician
        /// </summary>
        /// <param name="gameobject"></param>
        public void ValidateSelectedItem(GameObject clickedGameObject)
        {
            BiopsyProcedureEquipment selectedItem =
                clickedGameObject.GetComponent<BiopsyTableItem>().procedureEquipment;

            if (requestedItem == selectedItem)
            {
                task.ExecuteAction();
                requestedItem = BiopsyProcedureEquipment.None;
            }
            else
            {
                prompt.HandleDisplayUI(requestedItem == BiopsyProcedureEquipment.None
                    ? NoItemRequestedPrompt
                    : WrongItemSelectedPrompt);

                StartCoroutine(SchedulePromptHide());
            }
        }
    }
}