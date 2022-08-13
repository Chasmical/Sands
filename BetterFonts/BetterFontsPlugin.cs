using System.Collections;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BetterFonts;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.betterfonts", "S&S: Better Fonts", "1.0.0")]
public class BetterFontsPlugin : BepInEx.BaseUnityPlugin
{
    private static Font? MunroExtended;
    private static Font? FusionPixel;
    public void Awake()
    {
        Harmony harmony = new Harmony(Info.Metadata.GUID);
        harmony.Patch(AccessTools.Method(typeof(GameController), "Update"), null,
                      new HarmonyMethod(typeof(BetterFontsPlugin).GetMethod(nameof(GameController_Update))));
        harmony.Patch(AccessTools.Method(typeof(GameController), nameof(GameController.SetFont)),
                      new HarmonyMethod(typeof(BetterFontsPlugin).GetMethod(nameof(GameController_SetFont))));
        harmony.Patch(AccessTools.Method(typeof(MenuButtonHelper), nameof(MenuButtonHelper.SetupText3)),
                      null, new HarmonyMethod(typeof(BetterFontsPlugin).GetMethod(nameof(MenuButtonHelper_SetupText3))));
        harmony.Patch(AccessTools.Method(typeof(MenuGUI), nameof(MenuGUI.RealAwake)),
                      null, new HarmonyMethod(typeof(BetterFontsPlugin).GetMethod(nameof(MenuGUI_RealAwake))));
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
            __instance.ChangeFont();
        }
        __instance.munroExpandedFont = MunroExtended;
        __instance.russianFont = MunroExtended;
        __instance.polishFont = MunroExtended;
    }
    public static bool GameController_SetFont(GameController __instance, Text myText)
    {
        string language = __instance.sessionDataBig.gameLanguage;
        Font? font = language is @"schinese" or @"koreana" ? FusionPixel : MunroExtended;
        if (font is not null) myText.font = font;
        return false;
    }
    public static void MenuButtonHelper_SetupText3(MenuButtonHelper __instance)
    {
        if (__instance.name is "RussianButton")
            __instance.myText.text = @"Русский";
    }
    public static void MenuGUI_RealAwake(MenuGUI __instance)
    {
        __instance.settingsLanguagesContent.transform.Find("ChineseButton").GetChild(0).GetComponent<Text>().font = FusionPixel;
        __instance.settingsLanguagesContent.transform.Find("KoreanButton").GetChild(0).GetComponent<Text>().font = FusionPixel;
    }
}
