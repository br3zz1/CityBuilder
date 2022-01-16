using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{

    public static Loading Instance { get; private set; }

    public bool shift;
    public bool loadScene;

    public GameObject img1;
    public GameObject img2;

    private AsyncOperation load;

    void Start()
    {
        Instance = this;
        if (shift) StartShift();
        if(loadScene)
        {
            load = SceneManager.LoadSceneAsync("Game");
        }
    }

    void Update()
    {

    }

    public void StopShift()
    {
        shift = false;
    }

    public void StartShift()
    {
        if(shift)
        {
            LeanTween.moveLocal(img1, new Vector3(-50, 50), 0.25f).setOnComplete(NextMove).setEaseInOutCubic().setIgnoreTimeScale(true);
            LeanTween.moveLocal(img2, new Vector3(50, -50), 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true);
        }
    }

    void NextMove()
    {
        LeanTween.moveLocal(img1, new Vector3(50, 50), 0.25f).setOnComplete(StartShift).setEaseInOutCubic();
        LeanTween.moveLocal(img2, new Vector3(-50, -50), 0.25f).setEaseInOutCubic();
        GameObject a = img1;
        img1 = img2;
        img2 = a;
    }
}