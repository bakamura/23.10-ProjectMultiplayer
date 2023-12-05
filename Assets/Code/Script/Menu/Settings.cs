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
            _musicSlider.value = AudioMixerToSlider(musicVolume);
            _masterMixer.GetFloat("SFXVolume", out float sfxVolume);
            _sfxSlider.value = AudioMixerToSlider(sfxVolume);

            _colorSchemeDropdown.onValueChanged.AddListener(UpdateColorScheme);
        }

        private float AudioMixerToSlider(float value)
        {
            return Mathf.Log10(value) * 20;
        }

        public void ChangeVolumeMusic(float volume)
        {
            _masterMixer.SetFloat("MusicVolume", AudioMixerToSlider(volume));
            SaveSettings();
        }

        public void ChangeVolumeSFX(float volume)
        {
            _masterMixer.SetFloat("SFXVolume", AudioMixerToSlider(volume));
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
            _saveSettings.VolumeMusic = AudioMixerToSlider(musicVolume);
            _masterMixer.GetFloat("SFXVolume", out float sfxVolume);
            _saveSettings.VolumeSfx = AudioMixerToSlider(sfxVolume);
            _saveSettings.WindowSize = new float[] { Screen.width, Screen.height };
        }
    }
}