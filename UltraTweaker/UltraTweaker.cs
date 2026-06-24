using BepInEx;
using BepInEx.Logging;
using PluginConfig.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UltraTweaker.Handlers;
using UltraTweaker.Tweaks;
using UnityEngine;

namespace UltraTweaker
{
    [BepInPlugin(GUID, Name, Version)]
    [BepInDependency("com.eternalUnion.pluginConfigurator")]
    public class UltraTweaker : BaseUnityPlugin
    {
        public const string GUID = "waffle.ultrakill.ultratweaker";
        public const string Name = "UltraTweaker";
        public const string Version = "1.1.0";

        internal static new ManualLogSource Logger;

        internal static PluginConfigurator configurator;

        public static Dictionary<Type, Tweak> AllTweaks = new();
        internal static List<Assembly> AssembliesToCheck = new()
        {
            Assembly.GetExecutingAssembly()
        };

        public static void AddAssembly(Assembly asm)
        {
            AssembliesToCheck.Add(asm);
            Tweak.RefreshTweakHolder();
        }

        public void Start()
        {
            Logger = base.Logger;
            configurator = PluginConfigurator.Create(Name, GUID);
            Debug.Log($"{Name} v{Version} has started.");

            AssetHandler.LoadBundle();
            Tweak.CreateTweakHolder();
            SettingUIHandler.Patch();
            MutatorHandler.Patch();
        }
    }
}
