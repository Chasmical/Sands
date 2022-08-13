using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace BetterFonts;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.betterfonts", "[S&S] Better Fonts", "1.0.0")]
public class BetterFontsPlugin : BepInEx.BaseUnityPlugin
{
    private static Font? MunroExtended;
    private static Font? FusionPixel;
    public void Awake()
    {
        Harmony harmony = new Harmony(Info.Metadata.GUID);
        harmony.Patch(AccessTools.Method(typeof(GameController), "Update"), null,
                      new HarmonyMethod(typeof(BetterFontsPlugin).GetMethod(nameof(GameController_Update))));
    }
    public IEnumerator Start()
    {
        AssetBundleCreateRequest req = AssetBundle.LoadFromMemoryAsync(Properties.Resources.BetterFontsBundle);
        yield return req;
        MunroExtended = req.assetBundle.LoadAsset<Font>("MunroExtended");
        FusionPixel = req.assetBundle.LoadAsset<Font>("FusionPixel");
    }
    public static void GameController_Update(GameController __instance)
    {
        Font? fontOfChoice = __instance.sessionDataBig.gameLanguage is @"schinese" ? FusionPixel : MunroExtended;
        if (__instance.munroFont != fontOfChoice && fontOfChoice != null)
        {
            __instance.munroFont = fontOfChoice;
            __instance.munroExpandedFont = fontOfChoice;
            __instance.russianFont = fontOfChoice;
            __instance.ChangeFont();
        }
    }
}
