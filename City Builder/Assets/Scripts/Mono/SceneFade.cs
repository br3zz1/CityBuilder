using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    public static SceneFade Instance;

    public Image fade;

    void Awake()
    {
        Instance = this;
        LeanTween.value(1, 0, 0.5f).setOnUpdate(UpdateFade).setDelay(2f).setIgnoreTimeScale(true).setOnComplete(() => { fade.gameObject.SetActive(false); });
    }

    public void UpdateFade(float alpha)
    {
        Color c = fade.color;
        c.a = alpha;
        fade.color = c;
    }

    public void LoadScene()
    {
        LeanTween.value(0, 1, 0.5f).setOnUpdate(UpdateFade).setIgnoreTimeScale(true);
        fade.gameObject.SetActive(true);
        LeanTween.delayedCall(0.5f, () => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }).setIgnoreTimeScale(true);
    }
}
