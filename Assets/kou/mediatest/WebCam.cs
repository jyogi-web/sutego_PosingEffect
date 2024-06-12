using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCam : MonoBehaviour
{
    private static int INPUT_SIZE = 256;
    private static int FPS = 30;

    // UI
    RawImage rawImage;
    WebCamTexture webCamTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        // Webカメラの開始
        this.rawImage = GetComponent<RawImage>();
        // rawImageがnullでないことを確認する
        if (this.rawImage != null)
        {
            this.webCamTexture = new WebCamTexture(INPUT_SIZE, INPUT_SIZE, FPS);
            this.rawImage.texture = this.webCamTexture;
            this.webCamTexture.Play();
        }
        else
        {
            Debug.LogError("RawImageコンポーネントが見つかりませんでした。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
