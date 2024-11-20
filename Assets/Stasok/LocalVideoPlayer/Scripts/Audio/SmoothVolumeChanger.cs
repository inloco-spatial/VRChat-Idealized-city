
using UdonSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Stasok.Volume
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SmoothVolumeChanger : UdonSharpBehaviour 
    {
        public UdonSharpBehaviour OnAudioMuted;
        public AudioSource AudioSource;
        public Transform UIVolumeSetting;

        [SerializeField] private Toggle stateVolume;
        [SerializeField] private Slider maxVolumeSlider;
        [SerializeField] private float oldMaxVolume = 1f;
        [SerializeField] private bool startStateUIVolumeSetting = false;

        [Space(8)]

        [SerializeField] private float changeSpeed = 0.5f;
        [Space(4)]
        [SerializeField] private bool autoDisable = false;

        private float _targetVolume;
        private bool _changeVolume;

        private void Start()
        {
            UIVolumeSetting.gameObject.SetActive(startStateUIVolumeSetting);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_changeVolume || !AudioSource) return;

            if (AudioSource.volume == _targetVolume)
            {
                if (_targetVolume == 0 && OnAudioMuted != null) OnAudioMuted.SendCustomEvent(nameof(OnAudioMuted));
                if (autoDisable) gameObject.SetActive(false);
                _changeVolume = false;
            }
            else
                AudioSource.volume = Mathf.MoveTowards(AudioSource.volume, _targetVolume, changeSpeed * Time.deltaTime);
        }

        public void IncreaseVolume()
        {
            gameObject.SetActive(true);
            _targetVolume = maxVolumeSlider.value;
            _changeVolume = true;
        }

        public void DecreaseVolume()
        {
            gameObject.SetActive(true);
            _targetVolume = 0f;
            _changeVolume = true;
        }

        public void OnToggleChanged()
        {
            if (stateVolume.isOn)
                maxVolumeSlider.value = oldMaxVolume;
            else
            {
                oldMaxVolume = maxVolumeSlider.value;
                maxVolumeSlider.value = 0f;
            }

            AudioSource.volume = maxVolumeSlider.value;
        }

        public void OnSliderChanged() => AudioSource.volume = maxVolumeSlider.value;
        public void ResetVolume() => AudioSource.volume = 0;

#if UNITY_EDITOR && !COMPILER_UDONSHARP
        [CustomEditor(typeof(SmoothVolumeChanger))]
        public class ChangeAudioVolumeInspector : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                var script = (SmoothVolumeChanger)target;
                if (GUILayout.Button(nameof(IncreaseVolume)))
                    script.IncreaseVolume();
                if (GUILayout.Button(nameof(DecreaseVolume)))
                    script.DecreaseVolume();
            }
        }
#endif
    }
}