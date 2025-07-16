using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.RespiratoryTherapy
{
    public class SettingsData : SettingsMenuSimple
    {
        private SoundManager soundManager;
        private CameraManager cameraManager;

        private void Awake()
        {
            soundManager = FindFirstObjectByType<SoundManager>();
            cameraManager = FindFirstObjectByType<CameraManager>();
            
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            soundToggle = Root.Q<TemplateContainer>("SoundToggle").Q<SlideToggle>();
            volumeSlider = Root.Q<TemplateContainer>("VolumeSlider").Q<FillSlider>();
            cameraSensitivitySlider = Root.Q<TemplateContainer>("SensitivitySlider").Q<FillSlider>();
        }

        public void LoadSettingsData()
        {
            soundToggle.value = soundManager.soundOn;
            volumeSlider.value = SettingsMenuHelper.ConvertLogVolumeToLinear(soundManager.volumeLevel);
            cameraSensitivitySlider.value = cameraManager.mouseSensitivity;

            soundManager.HandleMuteSound(soundToggle.value);
        }
    }
}