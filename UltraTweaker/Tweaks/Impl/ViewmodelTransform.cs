using HarmonyLib;
using PluginConfig.API;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Viewmodel Transform", $"{UltraTweaker.GUID}.viewmodel_transform", "Resize, change the FOV of, and otherwise tweak the viewmodel.", $"{UltraTweaker.GUID}.misc", 1)]
    public class ViewmodelTransform : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.viewmodel_transform");
        private static Dictionary<GameObject, Vector3> _originalScale = new();

        private static SubSettingsCreator.IntSettingValues viewmodelFov = new(50, 150, 90);
        private static SubSettingsCreator.IntSettingValues viewmodelSizeMultiplier = new(0, 125, 100);
        private static SubSettingsCreator.BoolSettingValues viewmodelBob = new(true);
        private static SubSettingsCreator.BoolSettingValues viewmodelTilt = new(true);

        public override void CreateSubSettingsUI(ConfigDivision division)
        {
            SubSettingsCreator.CreateInt(this, "FOV", division, viewmodelFov);
            SubSettingsCreator.CreateInt(this, "Size", division, viewmodelSizeMultiplier);
            SubSettingsCreator.CreateBool(this, "Bobbing", division, viewmodelBob);
            SubSettingsCreator.CreateBool(this, "Aim-assist Tilt", division, viewmodelTilt);
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            _harmony.PatchAll(typeof(ViewmodelPatches));

            if (GunControl.Instance != null && FistControl.Instance != null)
            {
                UpdateBobAndTilt();
            } 
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();

            if (GunControl.Instance != null && FistControl.Instance != null)
            {
                NewMovement.Instance.gameObject.ChildByName("Main Camera").ChildByName("HUD Camera").GetComponent<Camera>().fieldOfView = 90;

                FistControl.Instance.transform.localScale = Vector3.one;

                GunControl.Instance.GetComponent<WalkingBob>().enabled = true;
                GunControl.Instance.GetComponent<RotateToFaceFrustumTarget>().enabled = true;
            }

            foreach (GameObject go in _originalScale.Keys)
            {
                go.transform.localScale = _originalScale[go];
            }

            _originalScale.Clear();
        }

        public override void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            _originalScale.Clear();
        }

        public void LateUpdate()
        {
            if (NewMovement.Instance != null)
            {
                NewMovement.Instance.gameObject.ChildByName("Main Camera").ChildByName("HUD Camera").GetComponent<Camera>().fieldOfView
                    = viewmodelFov.Value;
            }
        }

        public override void OnSubsettingUpdate()
        {
            UpdateBobAndTilt();

            if (GunControl.Instance != null && FistControl.Instance != null)
            {
                if (GunControl.Instance.currentWeapon != null && GunControl.Instance.currentWeapon.GetComponent<WeaponPos>() != null)
                {
                    GunControl.Instance.currentWeapon.GetComponent<WeaponPos>().CheckPosition();
                }
                UpdateBobAndTilt();

                FistControl.Instance.transform.localScale = Vector3.one;
            }
        }

        public static void UpdateBobAndTilt()
        {
            if (GunControl.Instance != null)
            {
                GunControl.Instance.GetComponent<WalkingBob>().enabled = viewmodelBob.Value;
                GunControl.Instance.GetComponent<RotateToFaceFrustumTarget>().enabled = viewmodelTilt.Value;
            }
        }

        public static class ViewmodelPatches
        {
            [HarmonyPatch(typeof(FistControl), nameof(FistControl.Update)), HarmonyPostfix]
            private static void FistSize(FistControl __instance)
            {
                if (__instance.transform.localScale == Vector3.one)
                {
                    float size = viewmodelSizeMultiplier.Value;

                    // this breaks parries help idk why 

                    //__instance.transform.localScale *=  size / 100f;
                    //if (size != 0)
                    //{
                    //    __instance.gameObject.ChildByName("Projectile Parry Zone").transform.localScale /= size / 100f;
                    //}
                }
            }

            [HarmonyPatch(typeof(WeaponPos), nameof(WeaponPos.CheckPosition))]
            [HarmonyPostfix]
            static void PatchWeaponScale_Check(WeaponPos __instance)
            {
                if (__instance.gameObject.name.Contains("Revolver"))
                {
                    if (!_originalScale.ContainsKey(__instance.gameObject))
                    {
                        _originalScale.Add(__instance.gameObject, __instance.gameObject.transform.localScale);
                    }
                    __instance.gameObject.transform.localScale = _originalScale[__instance.gameObject] * viewmodelSizeMultiplier.Value / 100;
                } else
                {
                    foreach (GameObject child in __instance.gameObject.ChildrenList())
                    {
                        if (!_originalScale.ContainsKey(child))
                        {
                            _originalScale.Add(child, child.transform.localScale);
                        }

                        child.transform.localScale = _originalScale[child] * viewmodelSizeMultiplier.Value / 100;
                    }
                }
            }

            [HarmonyPatch(typeof(WeaponPos), nameof(WeaponPos.Start))]
            [HarmonyPostfix]
            static void PatchWeaponScale_Start(WeaponPos __instance)
            {
                if (!__instance.gameObject.name.Contains("Revolver"))
                {
                    
                }
            }


            [HarmonyPatch(typeof(GunControl), nameof(GunControl.Start)), HarmonyPostfix]
            private static void BobAndTilt()
            {
                UpdateBobAndTilt();
            }
        }
    }
}
