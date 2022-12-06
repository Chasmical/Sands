using System;
using System.Collections;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BetterFonts;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.betterfonts", "S&S: Better Fonts", "1.1.0")]
public class BetterFontsPlugin : BepInEx.BaseUnityPlugin
{
    private static Font? MunroExtended;
    private static Font? FusionPixel;
    private static Font? EditUndoBRK;
    private static Font? PressStart2P;
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
        harmony.Patch(AccessTools.Method(typeof(ButtonHelper), nameof(ButtonHelper.RealStart)),
                      null, new HarmonyMethod(typeof(BetterFontsPlugin).GetMethod(nameof(ButtonHelper_RealStart))));
        harmony.Patch(AccessTools.Method(typeof(ButtonHelper), nameof(ButtonHelper.RealStartB)),
                      null, new HarmonyMethod(typeof(BetterFontsPlugin).GetMethod(nameof(ButtonHelper_RealStartB))));
        harmony.Patch(AccessTools.Method(typeof(MenuButtonHelper), "Start"),
                      null, new HarmonyMethod(typeof(BetterFontsPlugin).GetMethod(nameof(MenuButtonHelper_Start))));
        harmony.Patch(AccessTools.Method(typeof(WorldSpaceGUI), "RealStart"),
                      null, new HarmonyMethod(typeof(BetterFontsPlugin).GetMethod(nameof(WorldSpaceGUI_RealStart))));
        harmony.Patch(AccessTools.Method(typeof(TalkText), "Start"),
                      null, new HarmonyMethod(typeof(BetterFontsPlugin).GetMethod(nameof(TalkText_Start))));
    }
    public IEnumerator Start()
    {
        AssetBundleCreateRequest req = AssetBundle.LoadFromMemoryAsync(Properties.Resources.BetterFontsBundle);
        yield return req;
        MunroExtended = req.assetBundle.LoadAsset<Font>("MunroExtended");
        FusionPixel = req.assetBundle.LoadAsset<Font>("FusionPixel");
        EditUndoBRK = req.assetBundle.LoadAsset<Font>("EditUndoBRK");
        PressStart2P = req.assetBundle.LoadAsset<Font>("PressStart2P-Regular");
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
        string? language = __instance.sessionDataBig?.gameLanguage;
        if (language is @"schinese" or @"koreana")
        {
            if (FusionPixel is not null)
                myText.font = FusionPixel;
        }
        else
        {
            Font? newFont = myText.font?.name switch
            {
                "Munro" or "munro2" or "munro-expanded" or "MunroNarrow" or "MunroExtended" or "FusionPixel" => MunroExtended,
                @"editundo" or "EditUndoBRK" => EditUndoBRK,
                @"joystix monospace" or "PressStart2P-Regular" => PressStart2P,
                _ => MunroExtended,
            };
            if (newFont is not null) myText.font = newFont;
        }
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
    public static void ButtonHelper_RealStart(ButtonHelper __instance)
    {
        if (__instance.name.StartsWith("ScrollingMenuButton", StringComparison.Ordinal))
        {
            Vector2 offsetMin = __instance.myTextRect.offsetMin;
            offsetMin.y = __instance.gc.munroFont == FusionPixel ? -34f : -40f;
            __instance.myTextRect.offsetMin = offsetMin;
            Vector2 offsetMax = __instance.myTextRect.offsetMax;
            offsetMax.y = __instance.gc.munroFont == FusionPixel ? 36f : 32f;
            __instance.myTextRect.offsetMax = offsetMax;
        }
        else if (__instance.gc.munroFont == FusionPixel)
        {
            Vector2 offsetMin = __instance.myTextRect.offsetMin;
            offsetMin.y += 10f;
            __instance.myTextRect.offsetMin = offsetMin;
        }
    }
    public static void ButtonHelper_RealStartB(ButtonHelper __instance, out bool ___didRealStart)
    {
        ___didRealStart = false;
    }
    public static void MenuButtonHelper_Start(MenuButtonHelper __instance)
    {
        if (__instance.gc.munroFont == FusionPixel)
        {
            Vector2 offsetMin = __instance.myTextRect.offsetMin;
            offsetMin.y += 10f;
            __instance.myTextRect.offsetMin = offsetMin;
        }
    }
    public static void WorldSpaceGUI_RealStart(WorldSpaceGUI __instance)
    {
        if (GameController.gameController.munroFont == FusionPixel)
        {
            Vector2 offsetMin = __instance.objectNameDisplayTextRect.offsetMin;
            offsetMin.y += 10f;
            __instance.objectNameDisplayTextRect.offsetMin = offsetMin;
        }
    }
    public static void TalkText_Start(TalkText __instance)
    {
        if (GameController.gameController.munroFont == FusionPixel)
        {
            Vector2 offsetMin = __instance.myTextRect.offsetMin;
            offsetMin.y += 10f;
            __instance.myTextRect.offsetMin = offsetMin;
        }
    }

}
