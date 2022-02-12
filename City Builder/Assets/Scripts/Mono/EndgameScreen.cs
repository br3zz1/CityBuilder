using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EndgameScreen : MonoBehaviour
{
    public static EndgameScreen Instance { get; private set; }

    public float delayBetween;
    public float animationLength;

    public Image fade;
    public Text score;

    public GameObject newMapBigButton;

    public GameObject[] animationObjects;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (GameObject ab in animationObjects)
        {
            ab.transform.localScale = Vector3.zero;
        }
        gameObject.SetActive(false);
    }

    public void ShowScreen()
    {

        LeanTween.value(0, 0.75f, animationLength).setOnUpdate(UpdateFade).setIgnoreTimeScale(true);

        float delay = delayBetween;
        foreach (GameObject ab in animationObjects)
        {
            LeanTween.scale(ab, Vector3.one, animationLength).setDelay(delay).setEaseOutBack().setIgnoreTimeScale(true);
            delay += delayBetween;
        }

        Debug.Log(GameManager.Instance.score);
        LeanTween.value(0, GameManager.Instance.score, 2f).setOnUpdate(UpdateVis).setEaseOutCubic().setIgnoreTimeScale(true);
    }

    public void UpdateFade(float value)
    {
        Color c = fade.color;
        c.a = value;
        fade.color = c;
    }

    public void HideScreen()
    {
        newMapBigButton.SetActive(true);
        LeanTween.scale(newMapBigButton, Vector3.one, 0.3f).setDelay(0.1f).setIgnoreTimeScale(true);

        float delay = delayBetween;
        foreach (GameObject ab in animationObjects)
        {
            if (ab == animationObjects[animationObjects.Length - 1])
            {
                LeanTween.scale(ab, Vector3.zero, animationLength).setDelay(delay).setEaseInBack().setOnComplete(Again).setIgnoreTimeScale(true);
                return;
            }
            LeanTween.scale(ab, Vector3.zero, animationLength).setDelay(delay).setEaseInBack().setIgnoreTimeScale(true);
            delay += delayBetween;
        }
    }

    private void UpdateVis(float value)
    {
        score.text = ((int)value).ToString();
    }

    public void Again()
    {

        LeanTween.value(0.75f, 0, animationLength).setOnUpdate(UpdateFade).setIgnoreTimeScale(true).setOnComplete(() => { GameManager.Instance.UnpauseGame(); });
        float delay = delayBetween;
        LeanTween.delayedCall(delay + delayBetween, () => {
            gameObject.SetActive(false);
            SaveSystem.Save();
        });
    }

    public void NotYetClick()
    {
        HideScreen();
    }

    public void NewMapClick()
    {
        LeanTween.cancelAll();
        File.Delete(Application.persistentDataPath + "/save.json");
        SceneFade.Instance.LoadScene();
    }
}
