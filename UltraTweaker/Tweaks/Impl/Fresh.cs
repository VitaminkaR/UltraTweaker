using PluginConfig.API;
using System.Collections.Generic;
using UnityEngine;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Fresh", $"{UltraTweaker.GUID}.mutator_fresh", "Hurts you if you're not stylish.", $"{UltraTweaker.GUID}.mutators", 4, "Fresh", true, true)]
    public class Fresh : Tweak
    {
        private float _toRemove = 0;

        private static SubSettingsCreator.IntSettingValues fresh = new(0, 100, 0);
        private static SubSettingsCreator.IntSettingValues used = new(0, 100, 4);
        private static SubSettingsCreator.IntSettingValues stale = new(0, 100, 8);
        private static SubSettingsCreator.IntSettingValues dull = new(0, 31000, 12);

        public override void CreateSubSettingsUI(ConfigDivision division)
        {
            SubSettingsCreator.CreateInt(this, "Fresh", division, fresh);
            SubSettingsCreator.CreateInt(this, "Used", division, used);
            SubSettingsCreator.CreateInt(this, "Stale", division, stale);
            SubSettingsCreator.CreateInt(this, "Dull", division, dull);
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
        }

        public void Update()
        {
            Dictionary<StyleFreshnessState, float> dict = new Dictionary<StyleFreshnessState, float>()
            {
                { StyleFreshnessState.Fresh, fresh.Value },
                { StyleFreshnessState.Used, used.Value },
                { StyleFreshnessState.Stale, stale.Value },
                { StyleFreshnessState.Dull, dull.Value }
            };

            if (NewMovement.Instance != null && StatsManager.Instance.timer && GunControl.Instance.activated)
            {
                _toRemove += dict[StyleHUD.Instance.GetFreshnessState(GunControl.Instance.currentWeapon)] * Time.deltaTime;

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
