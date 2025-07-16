using UnityEngine;

namespace VARLab.RespiratoryTherapy
{
    [CreateAssetMenu(fileName = "Prompt Task", menuName = "RT Scriptable Objects/Biopsy/Scenario Tasks/Prompt Task")]
    public class PromptTaskSO : BiopsyBaseTaskSO
    {
        public string Name;
        public PromptSO PromptSO;
        public Prompt prompt;

        public override void Execute()
        {
            if (prompt == null)
            {
                prompt = FindFirstObjectByType<Prompt>();
            }
            prompt.HandleDisplayUI(PromptSO);
        }
    }
}
