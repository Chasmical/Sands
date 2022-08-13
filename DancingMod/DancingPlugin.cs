using UnityEngine;

namespace DancingMod;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.dancingmod", "S&S: Dancing Mod", "1.0.0")]
public class DancingPlugin : BepInEx.BaseUnityPlugin
{
    public Agent? Player => GameController.gameController?.playerAgent;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && Player is not null)
            Player.dancing = true;
        if (Player?.dancing is true && Player.rb.velocity.magnitude > 0.1f)
            Player.dancing = false;
    }
}
