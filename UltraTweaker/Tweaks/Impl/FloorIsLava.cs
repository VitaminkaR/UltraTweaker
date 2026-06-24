using PluginConfig.API;
using UnityEngine;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Floor Is Lava", $"{UltraTweaker.GUID}.mutator_floor_is_lava", "Take damage when on the floor.", $"{UltraTweaker.GUID}.mutators", 2, "Lava", true, true)]
    public class FloorIsLava : Tweak
    {
        private float _toRemove = 0;
        private float _onFloorFor;

        private static SubSettingsCreator.FloatSettingValues damageAfter = new(0, 5, 0.1f);
        private static SubSettingsCreator.IntSettingValues damagePerSecond = new(0, 100, 25);

        public override void CreateSubSettingsUI(ConfigDivision division)
        {
            SubSettingsCreator.CreateFloat(this, "Damage After", division, damageAfter);
            SubSettingsCreator.CreateInt(this, "Damage Per Second", division, damagePerSecond);
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
        }

        private void Update()
        {
            if (NewMovement.Instance != null)
            {
                if (NewMovement.Instance.gc.touchingGround)
                {
                    _onFloorFor += Time.deltaTime;
                }
                else
                {
                    _onFloorFor = 0;
                }

                if (StatsManager.Instance.timer && _onFloorFor > damageAfter.Value)
                {
                    _toRemove += Time.deltaTime * damagePerSecond.Value;

                    if ((int)_toRemove >= 1)
                    {
                        NewMovement.Instance.hp -= (int)_toRemove;
                        _toRemove -= (int)_toRemove;
                    }

                    if (NewMovement.Instance.hp <= 0 && !NewMovement.Instance.dead)
                    {
                        NewMovement.Instance.GetHurt(int.MaxValue, false, 1, true, true);
                    }
                }
            }
        }
    }
}
