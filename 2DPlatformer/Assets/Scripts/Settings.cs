using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour {
    [SerializeField]
    private bool isFullScreen;
    public AudioMixer am;
    Resolution[] rsl;
    List<string> resolutions;
    public Dropdown dropdown;
    [SerializeField]
    public Menu_table menu;
    private bool isAndroid;
    [SerializeField]
    Butoon quality;

    public void Awake()
    {
        isAndroid = menu.isAndroid;
        if (!isAndroid)
        {
            resolutions = new List<string>();
            rsl = Screen.resolutions;
            for (int i = 0; i < rsl.Length; i++)
            {
                resolutions.Add(rsl[i].width.ToString() + "x" + rsl[i].height.ToString());
            }
            dropdown.ClearOptions();
            dropdown.AddOptions(resolutions);
        }
    }

    public void Resolutions(int p)
    {
        Screen.SetResolution(rsl[p].width, rsl[p].height, isFullScreen);
    }

    public void FullScreenToggle()
    {
        isFullScreen = !isFullScreen;
        Screen.fullScreen = isFullScreen;
    }

    public void SetAct(bool q)
    {
        gameObject.SetActive(q);
        if (isAndroid)
            quality.SetAct(false);
    }

    public void AudioVolume(float Slider)
    {
        am.SetFloat("mastervolume", Slider);
    }

    public void QualityChanged(int q)
    {
        QualitySettings.SetQualityLevel(q);
    }
}
