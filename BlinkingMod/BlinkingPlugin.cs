using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace BlinkingMod;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.blinkingmod", "[S&S] Blinking Mod", "1.0.0")]
public class BlinkingPlugin : BepInEx.BaseUnityPlugin
{
    public void Awake()
    {
        Harmony harmony = new Harmony(Info.Metadata.GUID);
        harmony.Patch(AccessTools.Method(typeof(Agent), "Start"),
                      new HarmonyMethod(typeof(BlinkingPlugin).GetMethod(nameof(Agent_Start))));
    }
    private static readonly Dictionary<Agent, int> coroutineIds = new Dictionary<Agent, int>();
    private static readonly Dictionary<Agent, Coroutine> coroutines = new Dictionary<Agent, Coroutine>();
    private static readonly Dictionary<Agent, float> blinkTimes = new Dictionary<Agent, float>();

    private static void Agent_Start(Agent __instance)
    {
        if (coroutines.TryGetValue(__instance, out Coroutine? coroutine))
        {
            coroutineIds[__instance]++;
            __instance.StopCoroutine(coroutine);
        }
        else coroutineIds[__instance] = 0;
        blinkTimes[__instance] = 0f;
        coroutines[__instance] = __instance.StartCoroutine(BlinkingCoroutine(coroutineIds[__instance], __instance));
    }
    private static IEnumerator BlinkingCoroutine(int id, Agent agent)
    {
        const float blinkDuration = 0.1f;
        while (coroutineIds[agent] == id)
        {
            float nextBlink = blinkTimes[agent];
            if (Time.time >= nextBlink)
            {
                float unBlinkAt = Time.time + blinkDuration;
                while (Time.time < unBlinkAt)
                {
                    agent.agentHitboxScript?.eyes?.SetSprite("Clear");
                    agent.agentHitboxScript?.eyesH?.SetSprite("Clear");
                    yield return null;
                }
                agent.agentHitboxScript?.MustRefresh();
                blinkTimes[agent] = Time.time + Random.Range(2f, 5f);
            }
            yield return null;
        }
    }
}
