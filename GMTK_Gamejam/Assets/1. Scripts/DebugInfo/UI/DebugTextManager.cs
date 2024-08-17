using TMPro;
using Unity.Entities;
using UnityEngine;
using VDFramework.Utility;

namespace DebugInfo.UI
{
    public class DebugTextManager : MonoBehaviour
    {
        private TMP_Text fpsLabel;

        private StringVariableWriter fpsLabelWriter;

        private void Awake()
        {
            fpsLabel = GetComponent<TMP_Text>();

            fpsLabelWriter = new StringVariableWriter(fpsLabel.text);
        }

        public void LateUpdate()
        {
            int entityCount = World.DefaultGameObjectInjectionWorld.EntityManager.GetAllEntities().Length;
            fpsLabel.text = fpsLabelWriter.UpdateText(1 / Time.unscaledDeltaTime, entityCount);
        }
    }
}
