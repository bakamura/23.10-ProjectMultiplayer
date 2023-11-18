using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

namespace ProjectMultiplayer.UI
{
    public class Settings : MonoBehaviour
    {

        [SerializeField] private AudioMixer _masterMixer;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private TMP_Dropdown _colorSchemeDropdown;

        private static ColorScheme _currentColorScheme;
        private SaveSettings _saveSettings = new SaveSettings();

        public enum ColorScheme
        {
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
            SaveSettings();
        }

        public void ChangeVolumeSFX(float volume)
        {
            _masterMixer.SetFloat("SFXVolume", volume);
            SaveSettings();
        }

        public void ToggleFullScreen()
        {
            Screen.fullScreenMode = Screen.fullScreenMode == FullScreenMode.Windowed ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            SaveSettings();
        }

        public void ChangeColorScheme(ColorScheme scheme)
        {
            _currentColorScheme = scheme;
            SaveSettings();
        }

        public void UpdateColorScheme(int index)
        {
            ChangeColorScheme((ColorScheme)index);
        }

        private void SaveSettings()
        {
            _saveSettings.ColorScheme = _currentColorScheme;
            _masterMixer.GetFloat("MusicVolume", out float musicVolume);
            _saveSettings.VolumeMusic = musicVolume;
            _masterMixer.GetFloat("SFXVolume", out float sfxVolume);
            _saveSettings.VolumeSfx = sfxVolume;
            _saveSettings.WindowSize = new float[] { Screen.width, Screen.height };
        }
    }
}