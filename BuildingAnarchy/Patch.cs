using ColossalFramework.UI;
using Harmony;
using UnityEngine;

namespace BuildingAnarchy
{
    [HarmonyPatch(typeof(ToolBase), "ShowToolInfo")]
    public class Patch
    {
        static void Prefix(ToolBase __instance, bool show, ref string text, Vector3 worldPos)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text += "\n\n";
            }
            if (BuildingAnarchy.instance.IsBuildingTool)
            {
                text += BuildingAnarchy.instance.GenerateTooltipText();
            }
        }
    }
}
