#define UsesDLC
using UnityEngine;
using System;
using System.Collections.Generic;
using HarmonyLib;
using static STRINGS.UI.NEWBUILDCATEGORIES;
using PeterHan.PLib.Options;

namespace FixPack.DestructibleFeatures {
    [HarmonyPatch]
    public class FeaturePatches {

        [HarmonyPatch(typeof(Geyser), "OnSpawn")]
        public static class Geyser_OnSpawn_Patch {
            public static void Postfix(Geyser __instance) {
                if (!SingletonOptions<Option>.Instance.ActiveDestructibleFeatures) return;
                if (__instance.GetType() != typeof(Geyser)) {
                    DestructibleWorkable destWorkable = __instance.GetComponent<DestructibleWorkable>();

                    if ((UnityEngine.Object)destWorkable != (UnityEngine.Object)null)
                        UnityEngine.Object.Destroy(__instance.GetComponent<DestructibleWorkable>());

                    return;
                } else {
                    DestructibleWorkable destWorkable = __instance.FindOrAddComponent<DestructibleWorkable>();

                    int dTime = (int)Math.Floor(SingletonOptions<Option>.Instance.DeconstructTime);
                    if (dTime <= 0 || dTime > 10000)
                        LogManager.LogException("Deconstruct time is invalid (less than 0 or greater then 10000) in the config: " + dTime,
                            new ArgumentException("DeconstructTime:" + dTime));
                    else if (dTime != 1800)
                        destWorkable.SetWorkTime(dTime);
                }
            }
        }

        [HarmonyPatch(typeof(Studyable), "OnPrefabInit")]
        public static class Studyable_OnPrefabInit_Patch {
            public static void Postfix(Studyable __instance) {
                if (!SingletonOptions<Option>.Instance.ActiveDestructibleFeatures) return;
                int aTime = (int)Math.Floor(SingletonOptions<Option>.Instance.AnaylsisTime);
                if (aTime <= 0 || aTime > 10000)
                    LogManager.LogException("Anaylsis time is invalid (less than 0 or greater then 10000) in the config: " + aTime,
                        new ArgumentException("AnaylsisTime:" + aTime));
                else if (aTime != 3600)
                    __instance.SetWorkTime(aTime);
            }
        }

#if UsesDLC
        [HarmonyPatch(typeof(ButtonMenuSideScreen), "SetTarget")]
        public static class SingleButtonSideScreen_SetTarget_Patch {
            public static void Postfix(ButtonMenuSideScreen __instance, GameObject new_target, List<ISidescreenButtonControl> ___targets) {
                if (!SingletonOptions<Option>.Instance.ActiveDestructibleFeatures) return;
                if ((UnityEngine.Object)new_target == (UnityEngine.Object)null)
                    Debug.LogError((object)"Invalid gameObject received");
                else {
                    var buttonControl = new_target.GetComponent<ISidescreenButtonControl>();

                    if (buttonControl == null || !(buttonControl is Studyable))
                        return;
                    else if (((Studyable)buttonControl).Studied) {
                        DestructibleWorkable destWorkable = ((Studyable)buttonControl).gameObject.GetComponent<DestructibleWorkable>();

                        if ((UnityEngine.Object)destWorkable == (UnityEngine.Object)null)
                            return;
                        else {
                            ___targets = new List<ISidescreenButtonControl>();
                            ___targets.Add(destWorkable);
                            ___targets.AddRange(((Studyable)buttonControl).gameObject.GetComponents<DestructibleWorkable>());

                            Traverse.Create(__instance).Method("Refresh").GetValue();
                        }
                    }
                }
            }
        }
#else
        [HarmonyPatch(typeof(SingleButtonSideScreen), "SetTarget")]
        public static class SingleButtonSideScreen_SetTarget_Patch
        {
            public static void Postfix(SingleButtonSideScreen __instance, GameObject new_target, ref ISidescreenButtonControl ___target)
            {
                if (!SingletonOptions<Option>.Instance.ActiveDestructibleFeatures) return;
                if ((UnityEngine.Object)new_target == (UnityEngine.Object)null)
                    Debug.LogError((object)"Invalid gameObject received");
                else
                {
                    var buttonControl = new_target.GetComponent<ISidescreenButtonControl>();

                    if (buttonControl == null || !(buttonControl is Studyable))
                        return;
                    else if (((Studyable)buttonControl).Studied)
                    {
                        DestructibleWorkable destWorkable = ((Studyable)buttonControl).gameObject.GetComponent<DestructibleWorkable>();

                        if ((UnityEngine.Object)destWorkable == (UnityEngine.Object)null)
                            return;
                        else
                        {
                            ___target = destWorkable;
                            Traverse.Create(__instance).Method("Refresh").GetValue();
                        }
                    }
                }
            }
        }
#endif

    }
}
