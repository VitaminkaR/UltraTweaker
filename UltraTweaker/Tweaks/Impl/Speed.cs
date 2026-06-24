using HarmonyLib;
using PluginConfig.API;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Speed", $"{UltraTweaker.GUID}.mutator_speed", "Speed up yourself, and enemies", $"{UltraTweaker.GUID}.mutators", 9, "Speed", false, true)]
    public class Speed : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.mutator_speed");
        private static float _startSpeed = 0;

        private static SubSettingsCreator.FloatSettingValues playerSpeedMult = new(0, 10, 2);
        private static SubSettingsCreator.FloatSettingValues enemySpeedMult = new(0, 10, 2);

        public override void CreateSubSettingsUI(ConfigDivision division)
        {
            SubSettingsCreator.CreateFloat(this, "Player Speed", division, playerSpeedMult);
            SubSettingsCreator.CreateFloat(this, "Enemy Speed", division, enemySpeedMult);
        }

        public override void OnSubsettingUpdate()
        {
            NewMovement.Instance.walkSpeed = _startSpeed * playerSpeedMult.Value;
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();

            if (NewMovement.Instance != null)
            {
                _startSpeed = NewMovement.Instance.walkSpeed;
                NewMovement.Instance.walkSpeed = _startSpeed * playerSpeedMult.Value;
            }

            _harmony.PatchAll(typeof(SpeedPatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();

            if (NewMovement.Instance != null)
            {
                NewMovement.Instance.walkSpeed = _startSpeed;
            }

            _harmony.UnpatchSelf();
        }

        public static class SpeedPatches
        {
            [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Start)), HarmonyPostfix]
            public static void SpeedPlayer(NewMovement __instance)
            {
                _startSpeed = __instance.walkSpeed;
                __instance.walkSpeed = _startSpeed * playerSpeedMult.Value;
            }

            [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.Start)), HarmonyPostfix]
            public static void SpeedEnemy(EnemyIdentifier __instance)
            {
                float mult = enemySpeedMult.Value;

                if (!__instance.speedBuff)
                {
                    __instance.gameObject.AddComponent<DisableDoubleRender>();
                    __instance.SpeedBuff(mult);
                }
                else
                {
                    __instance.speedBuffModifier *= mult;
                }
            }
        }
    }
}
