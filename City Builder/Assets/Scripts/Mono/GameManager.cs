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

    public Text lastMilestoneText;
    public Text nextMilestoneText;

    public Slider milestoneSlider;
    public Slider previewMilestoneSlider;

    public GameObject milestoneGameObject;
    public GameObject endGameObject;

    public int score { get; private set; }

    [HideInInspector]
    public int nextMilestone;
    [HideInInspector]
    public int lastMilestone;

    public int cardSwaps;

    public bool paused { get; private set; }
    private bool disableEsc;

    public bool endGame;
    [HideInInspector]

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
        UnpauseGame();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !disableEsc)
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
        }
        else
        {
            disableEsc = true;
        }
    }

    public void UnpauseGame()
    {
        if (!paused) return;
        Time.timeScale = 1;
        paused = false;
        UIManager.Instance.Game();
        disableEsc = false;
    }

    public void UpdateScore(int score)
    {
        Queue<int> milestones = new Queue<int>();

        while (score >= nextMilestone)
        {
            lastMilestone = nextMilestone;
            float mtn = nextMilestone / 25f;
            mtn += 2;
            mtn *= 1.1f;
            nextMilestone = Mathf.CeilToInt(mtn) * 25;

            milestones.Enqueue(lastMilestone);

            PauseGame(false);
            UpdatePreviewScore(0);
        }

        if(milestones.Count > 0)
        {
            milestoneGameObject.SetActive(true);
            MilestoneScreen.Instance.milestones = milestones;
            MilestoneScreen.Instance.ShowScreen();
        }

        LeanTween.value(this.score, score, 1f).setOnUpdate(UpdateVis).setEaseOutCubic().setIgnoreTimeScale(true);
        this.score = score;
        

        lastMilestoneText.text = lastMilestone.ToString();
        nextMilestoneText.text = nextMilestone.ToString();

        LeanTween.delayedCall(0.1f, () =>
        {
            if (CardManager.Instance.holdingCards.Count == 0 && milestones.Count == 0) EndGame();
        });
    }

    private void UpdateVis(float value)
    {
        scoreText.text = ((int)value).ToString() + " / " + nextMilestone;
        milestoneSlider.value = (float)(value - lastMilestone) / (nextMilestone - lastMilestone);
    }

    public void UpdatePreviewScore(int score)
    {
        string txt = "";
        if (score > 0)
        {
            txt = "+" + score;
        }
        else if(score < 0)
        {
            txt = score.ToString();
        }
        addedScoreText.text = txt;

        previewMilestoneSlider.value = (float)(this.score + score - lastMilestone) / (nextMilestone - lastMilestone);
    }

    public void EndGame()
    {
        endGame = true;
        SaveSystem.Save();
        PauseGame(false);
        endGameObject.SetActive(true);
        EndgameScreen.Instance.ShowScreen();
    }
}
