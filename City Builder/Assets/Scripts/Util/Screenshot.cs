using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Screenshot : MonoBehaviour
{

    public string path;

    // Start is called before the first frame update
    void Start()
    {
        Camera camera = GetComponent<Camera>();
        Texture2D scrTexture = new Texture2D(2000, 2000, TextureFormat.ARGB32, false);
        RenderTexture scrRenderTexture = new RenderTexture(scrTexture.width, scrTexture.height, 24);
        RenderTexture camRenderTexture = camera.targetTexture;

        camera.targetTexture = scrRenderTexture;
        camera.Render();
        camera.targetTexture = camRenderTexture;

        RenderTexture.active = scrRenderTexture;
        scrTexture.ReadPixels(new Rect(0, 0, scrTexture.width, scrTexture.height), 0, 0);
        scrTexture.Apply();

        byte[] png = scrTexture.EncodeToPNG();
        Debug.Log(path);
        File.WriteAllBytes(path, png);

        //ScreenCapture.CaptureScreenshot(path);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            
        }
    }
}
