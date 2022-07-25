using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace CrispyMod
{
    [BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.crispymod", "[S&S] Crispy Mod", "1.0.0")]
    public class CrispyPlugin : BepInEx.BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony harmony = new Harmony(Info.Metadata.GUID);
            harmony.Patch(AccessTools.Method(typeof(CameraScript), nameof(CameraScript.RealStart)), null,
                          new HarmonyMethod(AccessTools.Method(typeof(CrispyPlugin), nameof(AttachCrispyScript))));
        }
        public static void AttachCrispyScript(CameraScript __instance)
        {
            GameObject go = __instance.GetComponentInChildren<Camera>().gameObject;
            _ = go.GetComponent<CrispyScript>() ?? go.AddComponent<CrispyScript>();
        }
    }
    public class CrispyScript : MonoBehaviour
    {
        private float brightness = 1.5f;
        private float saturation = 2f;
        private float contrast = 2f;
        private Material? material;

        public IEnumerator Start()
        {
            AssetBundleCreateRequest req = AssetBundle.LoadFromMemoryAsync(Properties.Resources.CrispyBundle);
            yield return req;
            material = req.assetBundle.LoadAsset<Material>("CrispyMaterial");
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert)) brightness += 0.1f;
            if (Input.GetKeyDown(KeyCode.Delete)) brightness -= 0.1f;
            if (Input.GetKeyDown(KeyCode.Home)) saturation += 0.1f;
            if (Input.GetKeyDown(KeyCode.End)) saturation -= 0.1f;
            if (Input.GetKeyDown(KeyCode.PageUp)) contrast += 0.1f;
            if (Input.GetKeyDown(KeyCode.PageDown)) contrast -= 0.1f;
        }
        public void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (material is not null && material)
            {
                material.SetFloat("_Brightness", brightness);
                material.SetFloat("_Saturation", saturation);
                material.SetFloat("_Contrast", contrast);
                Graphics.Blit(src, dest, material);
            }
            else Graphics.Blit(src, dest);
        }
    }
}
