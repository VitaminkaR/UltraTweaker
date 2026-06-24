using HarmonyLib;
using PluginConfig.API;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Ice", $"{UltraTweaker.GUID}.mutator_ice", "Become slippery.", $"{UltraTweaker.GUID}.mutators", 6, "Ice", false, true)]
    public class Ice : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.mutator_ice");

        private static SubSettingsCreator.FloatSettingValues Slippyness = new(0, 0.5f, 0.1f);

        public override void CreateSubSettingsUI(ConfigDivision division)
        {
            SubSettingsCreator.CreateFloat(this, "Friction", division, Slippyness);
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            _harmony.PatchAll(typeof(IcePatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();
            NewMovement.Instance.modForcedFrictionMultip = 1;
        }

        public override void OnSubsettingUpdate()
        {
            NewMovement.Instance.modForcedFrictionMultip = Slippyness.Value;
        }

        public static class IcePatches
        {
            [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Start))]
            [HarmonyPostfix]
            public static void IcePlayer(NewMovement __instance)
            {
                __instance.modForcedFrictionMultip = Slippyness.Value;
            }
        }
    }
}
