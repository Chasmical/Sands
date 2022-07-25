using HarmonyLib;
using UnityEngine;

namespace DemolishThatFreakingWall;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.demolishthatfreakingwall", "[S&S] Demolish That Freaking Wall", "1.0.0")]
public class DemolishThatFreakingWallPlugin : BepInEx.BaseUnityPlugin
{
    public void Awake()
    {
        Harmony harmony = new Harmony(Info.Metadata.GUID);
        harmony.Patch(AccessTools.Method(typeof(LoadLevel), nameof(LoadLevel.HomeBaseAgentSpawns)),
                      new HarmonyMethod(typeof(DemolishThatFreakingWallPlugin).GetMethod(nameof(PatchHomeBase))));
    }

    public static void PatchHomeBase()
    {
        Demolish(76f, 88f);
        Demolish(76f, 89f);
        Demolish(76f, 90f);
    }
    private static void Demolish(float x, float y)
    {
        GameController gc = GameController.gameController;
        Vector2 pos = new Vector2(x, y) * 0.64f;
        gc.tileInfo.DestroyWallTileAtPosition(pos.x, pos.y, true, gc.playerAgent);
        InvItem item = new InvItem { invItemName = "Wreckage" };
        item.SetupDetails(false);
        item.LoadItemSprite("WallBorderWreckage1");
        gc.spawnerMain.SpawnWreckage(pos, item, null, null, false).wallType = "WallBorder";
        item.LoadItemSprite("WallBorderWreckage2");
        gc.spawnerMain.SpawnWreckage(pos, item, null, null, false).wallType = "WallBorder";
        item.LoadItemSprite("WallBorderWreckage3");
        gc.spawnerMain.SpawnWreckage(pos, item, null, null, false).wallType = "WallBorder";
    }
}
