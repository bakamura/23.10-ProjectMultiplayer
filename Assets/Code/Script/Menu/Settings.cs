using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour {

    [SerializeField] private AudioMixer _masterMixer;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private TMP_Dropdown _colorSchemeDropdown;

    private static ColorScheme _currentColorScheme;
    private SaveSettings _saveSettings =  new SaveSettings();
    
    public enum ColorScheme {
        Normal,
        Deuteranopia,
        Protanopia,
        Tritanopia
    }

    private void Awake()
    {
        _masterMixer.GetFloat("MusicVolume", out float musicVolume);
        _musicSlider.value = musicVolume;
        _masterMixer.GetFloat("SFXVolume", out float sfxVolume);
        _sfxSlider.value = sfxVolume;

        _colorSchemeDropdown.onValueChanged.AddListener(UpdateColorScheme);
    }

    public void ChangeVolumeMusic(float volume)
    {
        _masterMixer.SetFloat("MusicVolume", volume);
        SaveConfigurations();
    }

    public void ChangeVolumeSFX(float volume)
    {
        _masterMixer.SetFloat("SFXVolume", volume);
        SaveConfigurations();
    }

    public void ToggleFullScreen()
    {
        Screen.fullScreenMode = Screen.fullScreenMode == FullScreenMode.Windowed ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        SaveConfigurations();
    }

    public void ChangeColorScheme(ColorScheme scheme)
    {
        _currentColorScheme = scheme;
        SaveConfigurations();
    }

    public void UpdateColorScheme(int index)
    {
        ChangeColorScheme((ColorScheme)index);       
    }

    private void SaveConfigurations()
    {
        _saveSettings.ColorScheme = _currentColorScheme;
        _masterMixer.GetFloat("MusicVolume", out float musicVolume);
        _saveSettings.VolumeMusic = musicVolume;
        _masterMixer.GetFloat("SFXVolume", out float sfxVolume);
        _saveSettings.VolumeSfx = sfxVolume;
        _saveSettings.WindowSize = new float[] { Screen.width, Screen.height };
    }
}
