using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    public Color positiveValueTextColor;
    public Color negativeValueTextColor;

    public GameObject SmokeEffect;

    public Text scoreText;
    public Text addedScoreText;

    public int score { get; private set; }

    public bool paused { get; private set; }

    //[SerializeField]
    //private List<TileObject> prefabs;

    public Dictionary<string, TileObject> namedPrefabs;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        Time.timeScale = 0;
        paused = true;

        namedPrefabs = new Dictionary<string, TileObject>();
        foreach(TileObject to in Resources.LoadAll<TileObject>("Prefabs/TileObjects/"))
        {
            namedPrefabs.Add(to.ObjectName, to);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused) UnpauseGame();
            else PauseGame();
        }
    }

    public void PauseGame(bool pauseMenu=true)
    {
        if (paused) return;
        Time.timeScale = 0;
        paused = true;
        if(ToolController.Instance.Useable != null) ToolController.Instance.Useable.Return();
        if(pauseMenu)
        {
            UIManager.Instance.PauseMenu();
            CameraController.Instance.desiredZoom += 5.5f;
        }
    }

    public void UnpauseGame()
    {
        if (!paused) return;
        Time.timeScale = 1;
        paused = false;
        UIManager.Instance.Game();
        CameraController.Instance.desiredZoom -= 5.5f;
    }

    public void UpdateScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
    }
}
