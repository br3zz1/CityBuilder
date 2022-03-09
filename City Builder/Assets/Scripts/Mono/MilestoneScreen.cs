using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MilestoneScreen : MonoBehaviour
{

    public static MilestoneScreen Instance { get; private set; }

    public float delayBetween;
    public float animationLength;

    public Image fade;
    public Text score;

    public Card bulldozeCard;

    public GameObject[] animationObjects;

    public Queue<int> milestones;

    public Queue<Action> delayCalls;

    private void Awake()
    {
        Instance = this;
        milestones = new Queue<int>();
        delayCalls = new Queue<Action>();
    }

    private void Start()
    {
        foreach (GameObject ab in animationObjects)
        {
            ab.transform.localScale = Vector3.zero;
        }
        gameObject.SetActive(false);
    }

    public void ShowScreen(bool again = false)
    {
        if(!again)
        {
            LeanTween.value(0, 0.75f, animationLength).setOnUpdate(UpdateFade).setIgnoreTimeScale(true);
            TutorialSystem.Instance.Milestone();
        }

        score.text = milestones.Dequeue().ToString();

        float delay = delayBetween;
        foreach(GameObject ab in animationObjects)
        {
            LeanTween.scale(ab, Vector3.one, animationLength).setDelay(delay).setEaseOutBack().setIgnoreTimeScale(true);
            delay += delayBetween;
        }

    }

    public void UpdateFade(float value)
    {
        Color c = fade.color;
        c.a = value;
        fade.color = c;
    }

    public void HideScreen()
    {

        delayCalls.Enqueue(() => { CardManager.Instance.AddRandomCard(); });
        delayCalls.Enqueue(() => { CardManager.Instance.AddRandomCard(); });
        delayCalls.Enqueue(() => { CardManager.Instance.AddRandomCard(); });

        float delay = delayBetween;
        foreach (GameObject ab in animationObjects)
        {
            if(ab == animationObjects[animationObjects.Length - 1])
            {
                LeanTween.scale(ab, Vector3.zero, animationLength).setDelay(delay).setEaseInBack().setOnComplete(Again).setIgnoreTimeScale(true);
                return;
            }
            LeanTween.scale(ab, Vector3.zero, animationLength).setDelay(delay).setEaseInBack().setIgnoreTimeScale(true);
            delay += delayBetween;
        }
    }

    public void Again()
    {
        if (milestones.Count > 0)
        {
            ShowScreen(true);
            return;
        }

        LeanTween.value(0.75f, 0, animationLength).setOnUpdate(UpdateFade).setIgnoreTimeScale(true).setOnComplete(() => { GameManager.Instance.UnpauseGame(); });
        float delay = delayBetween;
        while (delayCalls.Count > 0)
        {
            LeanTween.delayedCall(delay, delayCalls.Dequeue());
            delay += delayBetween;
        }
        LeanTween.delayedCall(delay + delayBetween, () => {
            gameObject.SetActive(false);
            SaveSystem.Save();
        });
    }

    public void ChoiceOneClick()
    {
        delayCalls.Enqueue(() => { CardManager.Instance.AddRandomCard(); });

        HideScreen();
    }

    public void ChoiceTwoClick()
    {
        GameManager.Instance.cardSwaps += 2;
        HideScreen();
    }

    public void ChoiceThrClick()
    {
        delayCalls.Enqueue(() => { CardManager.Instance.AddCard(bulldozeCard); });

        HideScreen();
    }
}
