using System.Reflection;

namespace OopsAllKillerRobots;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.oopsallkillerrobots", "Oops! All Killer Robots", "1.0.0")]
public class OopsAllKillerRobotsPlugin : BepInEx.BaseUnityPlugin
{
    public void Awake()
    {
        MethodInfo method = System.Array.Find(typeof(SpawnerMain).GetMethods(), static m => m.Name == "SpawnAgent" && m.GetParameters().Length == 14);
		new HarmonyLib.Harmony(Info.Metadata.GUID).Patch(method, new HarmonyLib.HarmonyMethod(GetType().GetMethod("SpawnAgentPatch")));
    }
    public static void SpawnAgentPatch(ref string agentType) => agentType = "Robot";
}
