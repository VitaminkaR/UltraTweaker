using HarmonyLib;
using PluginConfig.API;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Mitosis", $"{UltraTweaker.GUID}.mutator_mitosis", "Duplicates enemies.", $"{UltraTweaker.GUID}.mutators", 7, "Mitosis", false, true)]
    public class Mitosis : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.mutator_mitosis");
        public static List<ActivateNextWave> AlreadyMultiplied = new();

        private static SubSettingsCreator.IntSettingValues enemyAmount = new(2, 10, 2);

        public override void CreateSubSettingsUI(ConfigDivision division)
        {
            SubSettingsCreator.CreateInt(this, "Amount", division, enemyAmount);
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            AlreadyMultiplied.Clear();
            _harmony.PatchAll(typeof(MitosisPatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();
        }

        public override void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            AlreadyMultiplied.Clear();
        }

        public static class MitosisPatches
        {
            [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.Start)), HarmonyPrefix]
            public static void HmmTodayIWillUndergoMitosis(EnemyIdentifier __instance)
            {
                if (!__instance.gameObject.name.Contains("(MITOSISED)"))
                {
                    for (int i = 0; i < enemyAmount.Value - 1; i++)
                    {
                        GameObject obj = Instantiate(__instance.gameObject, __instance.transform.parent);

                        obj.name = __instance.gameObject.name + "(MITOSISED)";
                        obj.transform.position = __instance.transform.position;
                        obj.GetComponent<EnemyIdentifier>().blessed = false;
                    }
                }
            }

            [HarmonyPatch(typeof(ActivateNextWave), nameof(ActivateNextWave.Awake)), HarmonyPostfix]
            public static void IncreaseAnw(ActivateNextWave __instance)
            {               
                GoreZone goreZone = __instance.gameObject.GetComponentInParent<GoreZone>();
                if (goreZone == null) //No gorezone? ,':^(
                {
                    Debug.Log($"No GoreZone found in hierarchy of {__instance.gameObject.name}");
                    return;    
                }
                    
                Debug.Log($"{__instance.enemyCount} | {((goreZone.transform.parent == null) ? "orphan" : goreZone.transform.parent.name)} / {goreZone.transform.name} / {__instance.gameObject.name}");

                if (!goreZone.gameObject.name.Contains("(Clone)"))
                    return;

                __instance.enemyCount *= enemyAmount.Value;

                foreach (DeathMarker deathMarker in __instance.gameObject.GetComponentsInChildren<DeathMarker>(true))
                {
                    __instance.enemyCount -= 1;
                }

                Debug.Log($"{__instance.enemyCount} | Whar?");        
            }
        }
    }
}
