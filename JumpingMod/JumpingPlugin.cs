using UnityEngine;

namespace JumpingMod;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.jumpingmod", "[S&S] Jumping Mod", "1.0.0")]
public class JumpingPlugin : BepInEx.BaseUnityPlugin
{
    public Agent? Player => GameController.gameController?.playerAgent;
    public void Update()
    {
        if (Player is not null && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Player.jumpDirection = Player.tr.right;
            Player.Jump();
        }
    }
}
