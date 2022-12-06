using System;
using RogueLibsCore;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Outhouse;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.outhouse", "Outhouse", "1.0.0")]
[BepInEx.BepInDependency(RogueLibs.GUID, RogueLibs.CompiledVersion)]
public class OuthousePlugin : BepInEx.BaseUnityPlugin
{
    public void Awake()
    {
        RogueLibs.CreateCustomSprite("Outhouse", SpriteScope.Objects, Properties.Resources.Outhouse, 48f);
        RoguePatcher patcher = new RoguePatcher(this);
        patcher.Postfix(typeof(SpawnerMain), nameof(SpawnerMain.spawnObjectReal),
                        new Type[6] { typeof(Vector3), typeof(PlayfieldObject), typeof(string), typeof(string), typeof(WorldDataObject), typeof(int) });
        patcher.Postfix(typeof(PowerBox), "Start");
        patcher.Postfix(typeof(ObjectReal), "DestroyMe", new Type[1] { typeof(PlayfieldObject) });

        RogueInteractions.CreateProvider<PowerBox>(static h =>
        {
            if (h.Object.HasHook<Outhouse>())
            {
                h.SetStopCallback(static m => m.Object.DestroyMe(m.Agent));
                h.Model.StopInteraction();
            }
        });
    }
    public static void SpawnerMain_spawnObjectReal(ObjectReal __result)
    {
        if (__result.objectName is VanillaObjects.PowerBox && __result.gc.percentChance(100))
            __result.AddHook<Outhouse>();
    }
    public static void PowerBox_Start(PowerBox __instance)
    {
        __instance.animateSpriteID = __instance.animateSpriteID2 = RogueFramework.ObjectSprites!.GetSpriteIdByName("Outhouse");
    }
    public static void ObjectReal_DestroyMe(ObjectReal __instance)
    {
        if (__instance.HasHook<Outhouse>())
            for (int i = 0; i < 12; i++)
            {
                Vector2 pos = __instance.tr.position;
                Agent soldier = __instance.gc.spawnerMain.SpawnAgent(pos + Random.insideUnitCircle.normalized * 0.32f, __instance, VanillaAgents.Soldier);
                soldier.relationships.SetSecretHate(__instance.gc.playerAgent, true);
                soldier.relationships.SetRel(__instance.gc.playerAgent, "Hateful");
                soldier.relationships.SetRelHate(__instance.gc.playerAgent, 5);
                soldier.warZoneAgent = true;
                soldier.inventory.AddRandWeapon();
            }
    }
}
public class Outhouse : HookBase<PlayfieldObject>
{
    protected override void Initialize() { }
}
