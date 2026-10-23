using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.Reflection;
using System.IO;
using UnityEngine;

namespace BBCExitOverElevatorGate
{
    [HarmonyPatch(typeof(Elevator))]
    public class Patches
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        public static void InitializePostfix(Elevator __instance)
        {
            EnvironmentController ec = __instance.ReflectionGetVariable("ec") as EnvironmentController;
            if (BasePlugin.assetMan.TryGet("Thunk", out SoundObject sound) && BasePlugin.Instance.ConfigOverrideSounds.Value)
            {
                __instance.ReflectionSetVariable("audGateClose", sound);
            }
            if (ec == null)
            {
                return;
            }
            if (ec.mainHall == null)
            {
                return;
            }
            if (ec.mainHall.wallTex == null)
            {
                return;
            }
            if (__instance.gameObject.transform.TryFind("Gate", out Transform gateTransform))
            {
                void SMT(int i)
                {
                    if (gateTransform.GetChild(i).TryGetComponent(out MeshRenderer red))
                    {
                        red.material.SetMainTexture(ec.mainHall.wallTex);
                    }
                }
                SMT(1);
                SMT(2);

                if (gateTransform.GetChild(0).TryGetComponent(out MeshRenderer renderer))
                {
                    Texture2D texture = ec.mainHall.wallTex;
                    string name = ec.mainHall.wallTex.name + "_GateOut";
                    if (BasePlugin.assetMan.TryGet(name, out Texture2D get))
                    {
                        texture = get;
                    }
                    else
                    {
                        BasePlugin.Log("Couldn'get \"" + name + ".png\".", 2);
                        if (BasePlugin.Instance.ConfigAutoGenerateMissingTexture2D.Value)
                        {
                            File.WriteAllBytes(Path.Combine(BasePlugin.Instance.ModPath, "Textures", name + ".png"), texture.EncodeToPNG());
                        }
                    }
                    renderer.material.SetMainTexture(texture);
                }

                GameObject ModdedGate_Obj = new GameObject("ModdedGate");
                ModdedGate_Obj.transform.SetParent(__instance.transform);
                ModdedGate_Obj.transform.localPosition = Vector3.zero;
                ModdedGate_Obj.transform.localScale = gateTransform.localScale;
                ModdedGate_Obj.transform.localRotation = gateTransform.localRotation;
                while (gateTransform.childCount > 0)
                {
                    gateTransform.GetChild(0).SetParent(ModdedGate_Obj.transform);
                }
                ModdedGate_Obj.SetActive(false);
            }
        }

        [HarmonyPatch("OpenGate")]
        [HarmonyPrefix]
        public static void OpenGatePrefix(Elevator __instance)
        {
            if (__instance.gameObject.transform.TryFind("ModdedGate", out Transform gateTransform))
            {
                gateTransform.gameObject.SetActive(false);
            }
        }

        [HarmonyPatch("CloseGate")]
        [HarmonyPrefix]
        public static void CloseGatePrefix(Elevator __instance)
        {
            if (__instance.gameObject.transform.TryFind("ModdedGate", out Transform gateTransform))
            {
                gateTransform.gameObject.SetActive(true);
                gateTransform.gameObject.layer = 0;
                if (gateTransform.childCount < 1)
                {
                    return;
                }
                for (int i = 0; i < gateTransform.childCount; i++)
                {
                    Vector3 vector = gateTransform.GetChild(i).localPosition;
                    gateTransform.GetChild(i).localPosition = new Vector3(vector.x, 5f, vector.z);
                }
            }
        }
    }

    [HarmonyPatch(typeof(ElevatorDoor))]
    public class ElevatorDoorPatches
    {
        [HarmonyPatch("Open")]
        [HarmonyPrefix]
        public static void OpenPrefix(ElevatorDoor __instance)
        {
            if (BasePlugin.assetMan.TryGet("Null", out SoundObject sound) && BasePlugin.Instance.ConfigOverrideSounds.Value)
            {
                __instance.ReflectionSetVariable("audDoorOpen", sound);
            }
        }


        [HarmonyPatch("Shut")]
        [HarmonyPrefix]
        public static void ShutPrefix(ElevatorDoor __instance)
        {
            if (BasePlugin.assetMan.TryGet("Null", out SoundObject sound) && BasePlugin.Instance.ConfigOverrideSounds.Value)
            {
                __instance.ReflectionSetVariable("audDoorShut", sound);
            }
        }
    }
}
