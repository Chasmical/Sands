using RogueLibsCore;

namespace EnableRLDebugTools;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.enablerldebugtools", "[S&S] Enable RL Debug Tools", "1.0.0")]
[BepInEx.BepInDependency(RogueLibs.GUID, "3.0.0")]
public class EnableRLDebugToolsPlugin : BepInEx.BaseUnityPlugin
{
    public void Awake() => RogueFramework.DebugFlags |= DebugFlags.EnableTools;
}
