using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{

    public static Loading Instance { get; private set; }

    public bool shift;
    private bool startShift;
    private float timeToStart; 
    public bool nextShift;
    public bool loadScene;

    public GameObject img1;
    public GameObject img2;

    void Start()
    {
        Instance = this;
        if (shift)
        {
            startShift = true;
            timeToStart = Time.realtimeSinceStartup + 0.25f;
        }
        /*if(loadScene)
        {
            load = SceneManager.LoadSceneAsync("Game");
        }*/
    }

    void Update()
    {
        if(startShift && Time.realtimeSinceStartup > timeToStart)
        {
            StartShift();
            startShift = false;
        }
        if(nextShift)
        {
            LeanTween.moveLocal(img1, new Vector3(50, 50), 0.25f).setOnComplete(StartShift).setEaseInOutCubic().setIgnoreTimeScale(true);
            LeanTween.moveLocal(img2, new Vector3(-50, -50), 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true);

            GameObject a = img1;
            img1 = img2;
            img2 = a;

            nextShift = false;
        }
    }

    public void StopShift()
    {
        shift = false;
    }

    public void PreStart()
    {
        LeanTween.scale(img1, Vector3.one, 0.5f).setEaseInBack().setIgnoreTimeScale(true);
        LeanTween.scale(img2, Vector3.one, 0.5f).setEaseInBack().setIgnoreTimeScale(true);
    }

    public void StartShift()
    {
        if(shift)
        {
            LeanTween.moveLocal(img1, new Vector3(-50, 50), 0.25f).setOnComplete(NextMove).setEaseInOutCubic().setIgnoreTimeScale(true);
            LeanTween.moveLocal(img2, new Vector3(50, -50), 0.25f).setEaseInOutCubic().setIgnoreTimeScale(true);
        }
        else
        {
            LeanTween.scale(img1, Vector3.zero, 0.5f).setEaseInBack().setIgnoreTimeScale(true);
            LeanTween.scale(img2, Vector3.zero, 0.5f).setEaseInBack().setIgnoreTimeScale(true);
        }
    }

    void NextMove()
    {
        nextShift = true;
    }
}