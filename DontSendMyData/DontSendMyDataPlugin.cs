using HarmonyLib;

namespace DontSendMyData;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.dontsendmydata", "S&S: Don't Send My Data", "1.0.0")]
public class DontSendMyDataPlugin : BepInEx.BaseUnityPlugin
{
    public void Awake()
    {
        Harmony harmony = new Harmony(Info.Metadata.GUID);
        harmony.Patch(AccessTools.Method(typeof(AnalyticsFunctions), nameof(AnalyticsFunctions.SendData)),
            new HarmonyMethod(typeof(DontSendMyDataPlugin).GetMethod(nameof(Patch))));
    }
    public static bool Patch() => false;
}
