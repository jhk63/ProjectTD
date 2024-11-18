using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Slider mvSlider;
    public Slider bgSlider;
    public Slider sfxSlider;

    public enum ScreenMode
    {
        Windowed,
        FullScreenWindow
    }

    private void Start()
    {
        List<string> options = new List<string> {"Windowed", "FullScreen Window"};

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(index => ChangeFullScreenMode((ScreenMode)index));

        switch (dropdown.value)
        {
            case 0:
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
                break;
            case 1:
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;
        }
    }

    private void ChangeFullScreenMode(ScreenMode mode)
    {
        switch (mode)
        {
            case ScreenMode.Windowed:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case ScreenMode.FullScreenWindow:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
    }

    public void OnClick_Entry()
    {
        gameObject.SetActive(true);
    }

    public void OnClick_Exit()
    {
        gameObject.SetActive(false);
    }

    public void ChangeMV()
    {
        AudioManager.Instance.SetAudioVolume(EAudioMixerType.Master, mvSlider.value);
    }

    public void ChangeBV()
    {
        AudioManager.Instance.SetAudioVolume(EAudioMixerType.BGM, bgSlider.value);
    }

    public void ChangeSFX()
    {
        AudioManager.Instance.SetAudioVolume(EAudioMixerType.SFX, sfxSlider.value);
    }
}
