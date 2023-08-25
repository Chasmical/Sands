using System.IO;
using System;
using UnityEngine;

namespace Snap;
[BepInEx.BepInPlugin(@"abbysssal.streetsofrogue.snap", "S&S: Snap", "1.0.1")]
public class SnapPlugin : BepInEx.BaseUnityPlugin
{
    private float setZoom = -1f;
    public void Update()
    {
        GameController gc = GameController.gameController;

        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
            setZoom = setZoom < 0f ? gc.cameraScript.zoomLevel : Math.Min(setZoom + 0.1f, 10f);
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.Underscore) || Input.GetKeyDown(KeyCode.KeypadMinus))
            setZoom = setZoom < 0f ? gc.cameraScript.zoomLevel : Math.Max(setZoom - 0.1f, 0f);
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            setZoom = -1f;
            gc.cameraScript.zoomLevel = 1f;
        }

        if (setZoom > 0f) gc.cameraScript.zoomLevel = setZoom;

        if (Input.GetKeyDown(KeyCode.Print) || Input.GetKeyDown(KeyCode.KeypadMultiply) || Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            try
            {
                Camera camera = gc.cameraScript.actualCamera.ScreenCamera;
                Directory.CreateDirectory(Path.Combine(BepInEx.Paths.GameRootPath, "Snapshots"));
                string path = Path.Combine(BepInEx.Paths.GameRootPath, "Snapshots", $"{DateTime.Now:yy-MM-dd HHmmss}.png");

                static void SetInterface(bool value)
                {
                    GameController gc = GameController.gameController;
                    gc.nonClickableGUI.go.SetActive(value);
                    gc.mainGUI.gameObject.SetActive(value);
                    gc.questMarkerList.ForEach(q => q.go.SetActive(value));
                    gc.questMarkerSmallList.ForEach(q => q.gameObject.SetActive(value));
                }

                int scale = Input.GetKeyDown(KeyCode.KeypadDivide) ? 4 : 2;
                int width = Screen.width * scale;
                int height = Screen.height * scale;

                SetInterface(false);

                RenderTexture prevRender = camera.targetTexture;

                // create a render texture and render on it
                RenderTexture render = new RenderTexture(width, height, 24);
                camera.targetTexture = render;
                camera.Render();

                // set the render texture and read pixels from it
                RenderTexture.active = render;
                Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
                screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

                // return previous values
                camera.targetTexture = prevRender;
                RenderTexture.active = null;

                SetInterface(true);

                byte[] bytes = screenShot.EncodeToPNG();
                File.WriteAllBytes(path, bytes);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                throw;
            }
        }
    }
    public void FixedUpdate()
    {
        GameController gc = GameController.gameController;
        if (setZoom > 0f) gc.cameraScript.zoomLevel = setZoom;
    }
    public void LateUpdate()
    {
        GameController gc = GameController.gameController;
        if (setZoom > 0f) gc.cameraScript.zoomLevel = setZoom;
    }
}
