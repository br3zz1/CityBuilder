using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Options : MonoBehaviour
{

    public Volume volume;
    public ScriptableRendererFeature ssao;

    public Toggle postFxToggle;
    public Toggle motionBlurToggle;
    public Toggle ssaoToggle;
    public Toggle fullScreenToggle;
    public Dropdown resolutionDropdown;

    // POST FX
    private MotionBlur motionBlur;
    private Bloom bloom;
    private Vignette vignette;
    private ShadowsMidtonesHighlights smh;

    // Resolutions
    private Resolution[] resolutions;
   

    private void Start()
    {
        volume.profile.TryGet(out motionBlur);
        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out smh);

        postFxToggle.isOn = PlayerPrefs.GetInt("optionPFX", 1) == 1;
        motionBlurToggle.isOn = PlayerPrefs.GetInt("optionMB", 1) == 1;
        ssaoToggle.isOn = PlayerPrefs.GetInt("optionSSAO", 1) == 1;
        fullScreenToggle.isOn = PlayerPrefs.GetInt("optionFS", 1) == 1;
        TogglePostFX();
        ToggleMotionBlur();
        ToggleSSAO();
        ToggleFullscreen();

        resolutionDropdown.options.Clear();
        resolutions = Screen.resolutions;
        foreach(Resolution res in resolutions)
        {
            resolutionDropdown.options.Add( new Dropdown.OptionData() { text = res.width + "x" + res.height });
        }
        resolutionDropdown.value = PlayerPrefs.GetInt("optionRES", resolutions.Length - 1);
        SelectResolution();
    }

    public void TogglePostFX()
    {
        bool value = postFxToggle.isOn;
        bloom.active = value;
        vignette.active = value;
        smh.active = value;
        PlayerPrefs.SetInt("optionPFX", value ? 1 : 0);
    }

    public void ToggleMotionBlur()
    {
        bool value = motionBlurToggle.isOn;
        motionBlur.active = value;
        PlayerPrefs.SetInt("optionMB", value ? 1 : 0);
    }

    public void ToggleSSAO()
    {
        bool value = ssaoToggle.isOn;
        ssao.SetActive(value);
        PlayerPrefs.SetInt("optionSSAO", value ? 1 : 0);
    }

    public void ToggleFullscreen()
    {
        bool value = fullScreenToggle.isOn;
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("optionFS", value ? 1 : 0);
    }

    public void SelectResolution()
    {
        int value = resolutionDropdown.value;
        if (value > resolutions.Length - 1)
        {
            Debug.LogError("Invalid resolution, applying default");
            resolutionDropdown.value = resolutions.Length - 1;
            return;
        }
        Screen.SetResolution(resolutions[value].width, resolutions[value].height, fullScreenToggle.isOn, resolutions[value].refreshRate);
        PlayerPrefs.SetInt("optionRES", value);
    }

}
