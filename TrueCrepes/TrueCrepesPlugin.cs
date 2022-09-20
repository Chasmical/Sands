using HarmonyLib;

namespace TrueCrepes;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.truecrepes", "True Crepes", "1.0.0")]
public class TrueCrepesPlugin : BepInEx.BaseUnityPlugin
{
    public void Awake()
    {
        Harmony harmony = new Harmony(Info.Metadata.GUID);
        harmony.Patch(typeof(StatusEffects).GetMethod(nameof(StatusEffects.NormalGib)),
                      new HarmonyMethod(GetType(), nameof(NormalGib_Prefix)));
    }
    public static bool NormalGib_Prefix(StatusEffects __instance)
    {
        if (__instance.agent.agentName is @"Gangbanger")
        {
            __instance.GhostGib();
            return false;
        }
        return true;
    }
}
