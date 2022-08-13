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
            MenuGUI_RealAwake(__instance.menuGUI);
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
        switch (__instance.name)
        {
            case "RussianButton":
                __instance.myText.text = @"Русский";
                break;
            case "FrenchButton":
                __instance.myText.text = @"Français";
                break;
        }
    }
    public static void MenuGUI_RealAwake(MenuGUI __instance)
    {
        Text Get(string name) => __instance.settingsLanguagesContent.transform.Find(name).GetChild(0).GetComponent<Text>();

        Get("EnglishButton").font = MunroExtended;
        Get("GermanButton").font = MunroExtended;
        Get("SpanishButton").font = MunroExtended;
        Get("BrazilianButton").font = MunroExtended;
        Get("RussianButton").font = MunroExtended;
        Get("FrenchButton").font = MunroExtended;
        Get("ChineseButton").font = FusionPixel;
        Get("KoreanButton").font = FusionPixel;
    }
}
