using HarmonyLib;
using Light2D;
using UnityEngine;

namespace MoreMouseControls;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.moremousecontrols", "[S&S] More Mouse Controls", "1.0.0")]
public class MoreMouseControlsPlugin : BepInEx.BaseUnityPlugin
{
    public void Awake()
    {
        Harmony harmony = new Harmony(Info.Metadata.GUID);
        harmony.Patch(AccessTools.Method(typeof(PlayerControl), "Update"),
                      new HarmonyMethod(typeof(MoreMouseControlsPlugin).GetMethod(nameof(PlayerControl_Update))));
    }
    public static void PlayerControl_Update()
    {
        GameController gc = GameController.gameController;
        Vector2 dir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - gc.playerAgent.curPosition).normalized;
        bool any = Input.GetKey(KeyCode.Mouse3) | Input.GetKey(KeyCode.Mouse4);
        if (any)
        {
            float angle = Mathf.Repeat(dir.AngleZ() + 90f, 360f);
            gc.playerAgent.playerDir = gc.playerAgent.movement.FindAngle8Dir(angle);
            gc.playerAgent.walking = true;
            if (Input.GetKey(KeyCode.Mouse3)) dir *= -1f;
            gc.playerAgent.rb.AddForce(dir * gc.playerAgent.GetCurSpeed() * Time.timeScale);
        }
    }
}
