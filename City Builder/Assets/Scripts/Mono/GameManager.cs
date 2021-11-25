using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    public GameObject SmokeEffect;

    public Text scoreText;
    public Text scoreTextShadow;

    public int score { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void UpdateScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
        scoreTextShadow.text = score.ToString();
    }
}
