using HarmonyLib;
using PluginConfig.API;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Tankify", $"{UltraTweaker.GUID}.mutator_tankify", "Change enemy health.", $"{UltraTweaker.GUID}.mutators", 11, "Tankify", false, true)]
    public class Tankify : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.mutator_tankify");

        private static SubSettingsCreator.FloatSettingValues multiplier = new(0, 10, 2);

        public override void CreateSubSettingsUI(ConfigDivision division)
        {
            SubSettingsCreator.CreateFloat(this, "Health Multplier", division, multiplier);
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            _harmony.PatchAll(typeof(TankifyPatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();
        }

        public static class TankifyPatches
        {
            [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.Start)), HarmonyPostfix]
            public static void IncreaseHealth(EnemyIdentifier __instance)
            {
                float mult = multiplier.Value;

                if (!__instance.healthBuff)
                {
                    __instance.gameObject.AddComponent<DisableDoubleRender>();
                    __instance.HealthBuff(mult);
                }
                else
                {
                    __instance.healthBuffModifier *= mult;
                }
            }
        }
    }
}
