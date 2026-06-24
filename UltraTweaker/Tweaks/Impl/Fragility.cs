using HarmonyLib;
using PluginConfig.API;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Fragility", $"{UltraTweaker.GUID}.mutator_fragility", "Change your max health.", $"{UltraTweaker.GUID}.mutators", 3, "Fragility", true, true)]
    public class Fragility : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.mutator_fragility");

        private static SubSettingsCreator.IntSettingValues maxHealth = new(1, 100, 25);

        public override void CreateSubSettingsUI(ConfigDivision division)
        {
            SubSettingsCreator.CreateInt(this, "HP", division, maxHealth);
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            _harmony.PatchAll(typeof(FragilityPatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();
        }

        public static class FragilityPatches
        {
            [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Update)), HarmonyPostfix]
            private static void LimitHp(NewMovement __instance)
            {
                int thing = maxHealth.Value;

                if (__instance.antiHp < 100 - thing)
                {
                    __instance.antiHp = 100 - thing;
                }

                if (__instance.hp > thing)
                {
                    __instance.hp = thing;
                }
            }
        }
    }
}
