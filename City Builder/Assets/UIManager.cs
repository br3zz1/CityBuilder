using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject menu;
    public GameObject cardHolder;

    public Button abandonCityButton;

    public GameObject yesNo;
    public Image yesNoPanel;
    public Text yesNoHeader;
    public Text yesNoContent;

    public float animationTime = 1f;

    private Action<bool> boolAction;
    private bool pausedBefore;

    private void Awake()
    {
        Instance = this;
        if (File.Exists(Application.persistentDataPath + "/save.json"))
        {
            abandonCityButton.interactable = true;
            abandonCityButton.GetComponentInChildren<Text>().color = Color.white;
        }
    }

    public void PauseMenu()
    {
        LeanTween.moveLocalX(menu, -640f, animationTime).setEaseInOutCubic().setIgnoreTimeScale(true);
        LeanTween.moveY(cardHolder, -200f, animationTime).setEaseInOutCubic().setIgnoreTimeScale(true);
        if (File.Exists(Application.persistentDataPath + "/save.json"))
        {
            abandonCityButton.interactable = true;
            abandonCityButton.GetComponentInChildren<Text>().color = Color.white;
        }
    }

    public void Game()
    {
        LeanTween.moveLocalX(menu, -1065f, animationTime).setEaseInOutCubic().setIgnoreTimeScale(true);
        LeanTween.moveY(cardHolder, 0, animationTime).setEaseInOutCubic().setIgnoreTimeScale(true);
    }

    public void YesNoBox(string header, string content, Action<bool> action)
    {
        pausedBefore = GameManager.Instance.paused;
        GameManager.Instance.PauseGame(false);
        yesNoHeader.text = header;
        yesNoContent.text = content;
        boolAction = action;
        LeanTween.scale(yesNo, Vector3.one, animationTime / 2f).setEaseOutBack().setIgnoreTimeScale(true);
        yesNoPanel.gameObject.SetActive(true);
        LeanTween.value(0, 0.05f, animationTime / 2f).setOnUpdate(UpdateYesNoPanel).setIgnoreTimeScale(true);
    }

    public void UpdateYesNoPanel(float alpha)
    {
        Color c = yesNoPanel.color;
        c.a = alpha;
        yesNoPanel.color = c;
    }

    public void YesButtonClick()
    {
        boolAction?.Invoke(true);
        LeanTween.scale(yesNo, Vector3.zero, animationTime / 2f).setEaseInBack().setIgnoreTimeScale(true);
        LeanTween.value(0.05f, 0, animationTime / 2f).setOnUpdate(UpdateYesNoPanel).setIgnoreTimeScale(true).setOnComplete(() => { yesNoPanel.gameObject.SetActive(false); });
        if (!pausedBefore) GameManager.Instance.UnpauseGame();
    }

    public void NoButtonClick()
    {
        boolAction?.Invoke(false);
        LeanTween.scale(yesNo, Vector3.zero, animationTime / 2f).setEaseInBack().setIgnoreTimeScale(true);
        LeanTween.value(0.05f, 0, animationTime / 2f).setOnUpdate(UpdateYesNoPanel).setIgnoreTimeScale(true).setOnComplete(() => { yesNoPanel.gameObject.SetActive(false); });
        if (!pausedBefore) GameManager.Instance.UnpauseGame();
    }

    public void PlayButtonClick()
    {
        GameManager.Instance.UnpauseGame();
    }

    public void AbandonCityButtonClick()
    {
        YesNoBox("New City", "Are you sure you want to abandon this city?\nAll of your progress will be lost.", AbandonAction);
    }

    public void AbandonAction(bool abandon)
    {
        if (abandon)
        {
            LeanTween.cancelAll();
            File.Delete(Application.persistentDataPath + "/save.json");
            SceneFade.Instance.LoadScene();
        }
    }

    public void QuitButtonClick()
    {
        YesNoBox("Quit", "Are you sure you want to quit the game? All of your progress will be saved.", QuitAction);
    }

    public void QuitAction(bool quit)
    {
        if(quit) Application.Quit();
    }
}
