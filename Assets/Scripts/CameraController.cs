using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    //=============================================================
    public WebCamTexture webcamTexture;
    private Color32[] color32;

    private Texture2D texture;

    private Text text;
    private SoundManager soundManager;

    //インターバル関係
    private int interval;
    private bool intervalFlag;

    //画像描画オブジェクト
    private GameObject quad1;
    private GameObject quad2;

    //=============================================================
    private void Init () {
        CRef();
    }

    //=============================================================
    private void CRef () {
        text = GameObject.Find("Canvas/Debug/Text").GetComponent<Text>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        quad1 = GameObject.Find("Display/Quad1");
        quad2 = GameObject.Find("Display/Quad2");
    }

    //=============================================================
    private void Awake () {
        Init();
    }

    private void Start () {
        text.text = "started";
        WebCamDevice[] devices = WebCamTexture.devices;
        webcamTexture = new WebCamTexture(devices[0].name,DeV.IMAGE_WIDTH,DeV.IMAGE_HEIGHT,DeV.FPS);
        quad1.GetComponent<Renderer>().material.mainTexture = webcamTexture;
        webcamTexture.Play();

        quad1.SetActive(true);
        quad2.SetActive(false);
    }

    private void Update () {
        if(Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0) {
            quad1.SetActive(true);
            quad2.SetActive(false);

            soundManager.TriggerSE(0);
            text.text = "touched";

            color32 = webcamTexture.GetPixels32();
            texture = new Texture2D(webcamTexture.width,webcamTexture.height);
            GameObject.Find("Display/Quad1").GetComponent<Renderer>().material.mainTexture = texture;
            texture.SetPixels32(color32);
            texture.Apply();
            var bytes = texture.EncodeToPNG();

            //アンドロイド端末じゃないなら画像を保存
            if(Application.platform != RuntimePlatform.Android) {
                File.WriteAllBytes(Application.dataPath + "/SaveImages/SavedScreen.png",bytes);
            }

            if(intervalFlag == false) {
                StartCoroutine(SendImage());
            } else {
                interval++;
                if(interval >= DeV.INTERVAL) {
                    intervalFlag = false;
                    interval = 0;
                }
            }
        }

    }

    //===========================================================================================
    IEnumerator SendImage () {
        WWWForm form = new WWWForm();
        form.AddBinaryData("post_data",texture.EncodeToPNG(),"test.png","image/png");

        //Debug.Log(texture.EncodeToPNG().Length);
        WWW www = new WWW("http://10.24.194.13:5000/",form);

        yield return www;

        if(www.error != null) {
            Debug.LogError(www.error);
            text.text = www.error;
        } else {
            Debug.Log("SUCCESS");
            text.text = "SUCCESS";
        }

        Debug.Log(www.texture);
        Debug.Log(www.url);
        texture = www.texture;

        quad1.SetActive(false);
        quad2.SetActive(true);
        quad2.GetComponent<Renderer>().material.mainTexture = texture;

        yield return new WaitForSeconds(10);
    }
}
