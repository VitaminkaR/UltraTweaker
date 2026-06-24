using HarmonyLib;
using PluginConfig.API;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Hitstop", $"{UltraTweaker.GUID}.hitstop", "Change hitstop duration, change the parry flash.", $"{UltraTweaker.GUID}.misc", 0)]
    public class Hitstop : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.hitstop");

        private static SubSettingsCreator.IntSettingValues hitstopLength = new(0, 200, 100);
        private static SubSettingsCreator.IntSettingValues truestopLength = new(0, 200, 100);
        private static SubSettingsCreator.IntSettingValues slowdownLength = new(0, 200, 100);
        private static SubSettingsCreator.BoolSettingValues parryFlash = new(true);

        public override void CreateSubSettingsUI(ConfigDivision division)
        {
            SubSettingsCreator.CreateInt(this, "Hitstop Length", division, hitstopLength);
            SubSettingsCreator.CreateInt(this, "Truestop Length", division, truestopLength);
            SubSettingsCreator.CreateInt(this, "Slowdown Length", division, slowdownLength);
            SubSettingsCreator.CreateBool(this, "Parry Flash", division, parryFlash);
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            _harmony.PatchAll(typeof(HitstopPatches));

            OnSubsettingUpdate();
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();

            if (TimeController.Instance != null)
            {
                if (TimeController.Instance.parryFlash != null)
                {
                    TimeController.Instance.parryFlash.GetComponent<Image>().enabled = true;
                }

                if (TimeController.Instance.parryLight != null)
                {
                    TimeController.Instance.parryLight.GetComponent<Light>().enabled = true;
                }
            }
        }

        public override void OnSubsettingUpdate()
        {
            if (TimeController.Instance != null)
            {
                if (TimeController.Instance.parryFlash != null)
                {
                    TimeController.Instance.parryFlash.GetComponent<Image>().enabled = parryFlash.Value;
                }

                if (TimeController.Instance.parryLight != null)
                {
                    TimeController.Instance.parryLight.GetComponent<Light>().enabled = parryFlash.Value;
                }
            }
        }

        public override void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (TimeController.Instance != null)
            {
                if (TimeController.Instance.parryFlash != null)
                {
                    TimeController.Instance.parryFlash.GetComponent<Image>().enabled = parryFlash.Value;
                }

                if (TimeController.Instance.parryLight != null)
                {
                    TimeController.Instance.parryLight.GetComponent<Light>().enabled = parryFlash.Value;
                }
            }
        }

        public static class HitstopPatches
        {
            [HarmonyPatch(typeof(TimeController), nameof(TimeController.HitStop)), HarmonyPrefix]
            private static void PatchHitstop(ref float length)
            {
                length *= (hitstopLength.Value / 100f);
            }

            [HarmonyPatch(typeof(TimeController), nameof(TimeController.TrueStop)), HarmonyPrefix]
            private static void PatchTruestop(ref float length)
            {
                length *= (truestopLength.Value / 100f);
            }

            [HarmonyPatch(typeof(TimeController), nameof(TimeController.SlowDown)), HarmonyPrefix]
            private static void PatchSlowdown(ref float amount)
            {
                amount *= (slowdownLength.Value / 100f);
            }
        }
    }
}
