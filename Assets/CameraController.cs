using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    //=============================================================
    private WebCamTexture webCamTexture;
    public GameObject plane;

    //=============================================================
    private void Init () {
        CRef();
    }

    //=============================================================
    private void CRef () {
    }

    //=============================================================
    private void Awake () {
        Init();
    }

    private void Start () {
        webCamTexture = new WebCamTexture();
        plane.GetComponent<Renderer>().material.mainTexture = webCamTexture;
        webCamTexture.Play();
    }

    private void Update () {

    }
}