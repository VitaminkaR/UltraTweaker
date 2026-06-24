using HarmonyLib;
using PluginConfig.API;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Unobtrusive Blood", $"{UltraTweaker.GUID}.unobtrusive_blood", "Make the screen blood more transparent, or gone.", $"{UltraTweaker.GUID}.hud", 3)]
    public class UnobtrusiveBlood : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.unobtrusive_blood");

        private static SubSettingsCreator.IntSettingValues transparency = new(0, 100, 44);

        public override void CreateSubSettingsUI(ConfigDivision division)
        {
            SubSettingsCreator.CreateInt(this, "Blood Opacity", division, transparency);
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            _harmony.PatchAll(typeof(UnobtrusiveBloodPatches));

            OnSubsettingUpdate();
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();
        }

        public static class UnobtrusiveBloodPatches
        {
            [HarmonyPatch(typeof(ScreenBlood), nameof(ScreenBlood.Start)), HarmonyPostfix]
            private static void ChangeOpacity(ScreenBlood __instance)
            {
                __instance.clr.a = transparency.Value / 100f;
            }
        }
    }
}
