using HarmonyLib;
using PluginConfig.API;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("UI Scale", $"{UltraTweaker.GUID}.ui_scale", "Change the size of your HUD and UI.", $"{UltraTweaker.GUID}.hud", 0)]
    public class UIScale : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.ui_scale");

        public static bool DoesntKnowOriginal = true;
        public static Vector3 OriginalInfoScale;
        public static Vector3 OriginalStyleScale;
        public static Vector3 OriginalResultsScale;

        private static GameObject _info;
        private static GameObject _style;
        private static GameObject _results;

        private static SubSettingsCreator.IntSettingValues infoHudScale = new(0, 110, 100);
        private static SubSettingsCreator.IntSettingValues styleHudScale = new(0, 110, 100);
        private static SubSettingsCreator.IntSettingValues finalrankHudScale = new(0, 110, 100);
        private static SubSettingsCreator.IntSettingValues bossbarHudScale = new(0, 100, 100);
        private static SubSettingsCreator.IntSettingValues canvasScale = new(25, 100, 100);

        public override void CreateSubSettingsUI(ConfigDivision division)
        {
            SubSettingsCreator.CreateInt(this, "Info HUD Scale", division, infoHudScale);
            SubSettingsCreator.CreateInt(this, "Style HUD Scale", division, styleHudScale);
            SubSettingsCreator.CreateInt(this, "End HUD Scale", division, finalrankHudScale);
            SubSettingsCreator.CreateInt(this, "Boss Bar Scale", division, bossbarHudScale);
            SubSettingsCreator.CreateInt(this, "Canvas Scale", division, canvasScale);
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            DoesntKnowOriginal = true;
            _harmony.PatchAll(typeof(UIScalePatches));
            UpdateHUD();
            UpdateCanvas();
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            UpdateHUD(true);
            UpdateCanvas(true);
            _harmony.UnpatchSelf();
        }
        public override void OnSubsettingUpdate()
        {
            UpdateHUD();
            UpdateCanvas();
        }

        public static void UpdateHUD(bool toDefault = false)
        {
            if ((_info == null || _style == null || _results == null) && FindObjectsOfType<HudController>() != null)
            {
                foreach (HudController hc in FindObjectsOfType<HudController>())
                {
                    if (hc.gameObject.name == "HUD")
                    {
                        _info = hc.gameObject.ChildByName("GunCanvas");
                        _style = hc.gameObject.ChildByName("StyleCanvas");
                        _results = hc.gameObject.ChildByName("FinishCanvas");

                        if (DoesntKnowOriginal)
                        {
                            OriginalInfoScale = _info.transform.localScale;
                            OriginalStyleScale = _style.transform.localScale;
                            OriginalResultsScale = _results.transform.localScale;
                            DoesntKnowOriginal = false;
                        }
                    }
                }
            }

            if (!(_info == null || _style == null || _results == null))
            {
                float InfoScale = infoHudScale.Value;
                float StyleScale = styleHudScale.Value;
                float ResultsScale =finalrankHudScale.Value;

                if (!toDefault)
                {
                    _info.transform.localScale = OriginalInfoScale * (InfoScale / 100);
                    _style.transform.localScale = OriginalStyleScale * (StyleScale / 100);
                    _results.transform.localScale = OriginalResultsScale * (ResultsScale / 100);
                }
                else
                {
                    _info.transform.localScale = OriginalInfoScale;
                    _style.transform.localScale = OriginalStyleScale;
                    _results.transform.localScale = OriginalResultsScale;
                }
            }
        }

        public static void UpdateCanvas(bool toDefault = false)
        {
            float CanvasScale = canvasScale.Value;

            if (CanvasScale != 100)
            {
                GameObject canvas = CanvasController.Instance.gameObject;
                canvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                canvas.GetComponent<CanvasScaler>().scaleFactor = 1920 / Screen.width * 1.5f;

                if (!toDefault)
                {
                    canvas.GetComponent<CanvasScaler>().scaleFactor *= CanvasScale / 100;
                }
            }
        }

        public override void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            DoesntKnowOriginal = true;
        }

        public static class UIScalePatches
        {
            [HarmonyPatch(typeof(CanvasController), nameof(CanvasController.Awake)), HarmonyPostfix]
            private static void PatchCanvasScale(CanvasController __instance)
            {
                UpdateCanvas();
            }

            [HarmonyPatch(typeof(HudController), nameof(HudController.Start)), HarmonyPostfix]
            private static void PatchHUDScale(HudController __instance)
            {
                if (__instance.gameObject.name == "HUD")
                {
                    _info = __instance.gameObject.ChildByName("GunCanvas");
                    _style = __instance.gameObject.ChildByName("StyleCanvas");
                    _results = __instance.gameObject.ChildByName("FinishCanvas");

                    if (DoesntKnowOriginal)
                    {
                        OriginalInfoScale = _info.transform.localScale;
                        OriginalStyleScale = _style.transform.localScale;
                        OriginalResultsScale = _results.transform.localScale;
                        DoesntKnowOriginal = false;
                    }

                    UpdateHUD();
                }
            }

            [HarmonyPatch(typeof(BossHealthBar), nameof(BossHealthBar.Awake))]
            [HarmonyPostfix]
            static void PatchBossbarScale(BossHealthBar __instance)
            {
                float barScale = bossbarHudScale.Value;
                __instance.transform.localScale *= barScale / 100;
            }
        }
    }
}
