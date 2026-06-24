using HarmonyLib;
using PluginConfig.API;
using UnityEngine;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Close Quarters", $"{UltraTweaker.GUID}.mutator_close_quarters", "Blesses enemies when far.", $"{UltraTweaker.GUID}.mutators", 0, "Cross", true, true)]
    public class CloseQuarters : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.mutator_close_quarters");

        private static SubSettingsCreator.IntSettingValues enemyDistance = new(5, 30, 15);

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            _harmony.PatchAll(typeof(DistancePatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();
        }

        public override void CreateSubSettingsUI(ConfigDivision division)
        {
            SubSettingsCreator.CreateInt(this, "Enemy Distance", division, enemyDistance);
        }

        public static class DistancePatches
        {
            [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.Awake)), HarmonyPostfix]
            public static void AddComp(EnemyIdentifier __instance)
            {
                __instance.gameObject.AddComponent<BlessWhenFar>();
            }
        }

        public class BlessWhenFar : MonoBehaviour
        {
            private EnemyIdentifier eid;

            public void Start()
            {
                eid = GetComponent<EnemyIdentifier>();
            }

            public void Update()
            {
                if (Vector3.Distance(transform.position, NewMovement.Instance.transform.position) > enemyDistance.Value)
                {
                    if (!eid.blessed)
                    {
                        eid.Bless();
                    }
                }
                else
                {
                    if (eid.blessed)
                    {
                        eid.Unbless();
                    }
                }
            }
        }
    }
}
