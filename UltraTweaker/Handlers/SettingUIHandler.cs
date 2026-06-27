using HarmonyLib;
using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using System;
using System.Collections.Generic;
using UltraTweaker.Tweaks;
using UnityEngine;
namespace UltraTweaker.Handlers
{
    public static class SettingUIHandler
    {
        public static Harmony Harmony = new($"{UltraTweaker.GUID}.setting_ui_handler");

        public static GameObject OriginalSettingMenu;
        public static GameObject OriginalSettingPage;
        public static GameObject OriginalPageButton;
        public static GameObject CurrentSettingMenu;
        public static GameObject OriginalResetButton;
        public static GameObject CurrentResetButton;
        public static GameObject NewButton;

        public static Dictionary<string, Page> Pages = new()
        {
            { $"{UltraTweaker.GUID}.misc", new Page("TWEAKS: MISC") },
            { $"{UltraTweaker.GUID}.hud", new Page("TWEAKS: HUD") },
            { $"{UltraTweaker.GUID}.cybergrind", new Page("TWEAKS: CYBERGRIND") },
            { $"{UltraTweaker.GUID}.fun", new Page("TWEAKS: FUN") },
            { $"{UltraTweaker.GUID}.mutators", new Page("TWEAKS: MUTATORS") },
        };

        public static void Patch()
        {
            CreateUI(UltraTweaker.configurator);
        }

        public static void CreateUI(PluginConfigurator configurator)
        {
            Dictionary<string, ConfigPanel> subPanels = new Dictionary<string, ConfigPanel>();
            foreach (var pair in Pages)
            {
                subPanels.Add(pair.Key, new ConfigPanel(configurator.rootPanel, pair.Value.PageName, pair.Key));
            }

            foreach (Tweak tweak in UltraTweaker.AllTweaks.Values)
            {
                TweakMetadata meta = Attribute.GetCustomAttribute(tweak.GetType(), typeof(TweakMetadata)) as TweakMetadata;
                ConfigPanel panel = subPanels[meta.PageId];
                ConfigHeader header = new ConfigHeader(panel, meta.Name);

                BoolField toggle = new BoolField(panel, "TOGGLE", $"{meta.Name}_toggle", false);
                tweak.IsEnabled = toggle.value;
                toggle.onValueChange += (BoolField.BoolValueChangeEvent data) => tweak.IsEnabled = data.value;

                ConfigDivision division = new ConfigDivision(panel, $"{meta.Name}_division");
                tweak.CreateSubSettingsUI(division);
            }
        }

        public class Page
        {
            public string PageName;
            public GameObject PageObject;

            public Page(string PageName)
            {
                this.PageName = PageName;
            }
        }
    }
}
