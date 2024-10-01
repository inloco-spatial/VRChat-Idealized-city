
using Stasok.Utils;
using UdonSharp;
using UnityEngine;

namespace Stasok.AudioPlayer
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AudioPlayer : UdonSharpBehaviour 
    {
        public bool RemoveTriggerRenderers = true;
        [Space(8)]

        [Header("Время плавного перехода")]
        [SerializeField] private float maxTransitionTime = 3f;
        [Space(8)]
        [SerializeField] private bool useCurve = false;
        [SerializeField] private AnimationCurve curve;

        [Space(20)]
        [Range(1, 10)]
        [SerializeField] private float receiverStartDelay = 1f;
        public Color GizmosAutoPlayTriggerColor = new Color(0.5f, 1f, 0f, 0.5f);
        [HideInInspector]
        public UdonSharpBehaviour ReceiverStartPlay;

        private AudioSource[] _audioList = new AudioSource[_chunkSize];
        private AudioSource _targetAudio;
        private float _targetVolume = 1f;
        private float _startVolume = 0f;
        private float[] _startVolumes = new float[_chunkSize];
        private float _accumulatedTime = float.MaxValue;
        private int _isRemoveTargetAudio = 1;
        private int _audioCount = 0;
        private float _t;
        const int _chunkSize = 50;

        private void Start()
        {
            if (maxTransitionTime <= 0) maxTransitionTime = 0.01f;

            if (ReceiverStartPlay)
                ReceiverStartPlay.SendCustomEventDelayedSeconds(nameof(AudioTrigger.RunPlay), receiverStartDelay);
        }

        public void AddAudio(AudioSource newAudio)
        {
            ArrayUtils.AddToNext(ref _audioList, newAudio, ref _audioCount, _chunkSize);

            if (_audioCount >= _startVolumes.Length)
                ArrayUtils.IncreaseSize(ref _startVolumes, _chunkSize);
        }

        public void Play(AudioSource targetAudio, float targetVolume, bool reset, bool allVolumeChange)
        {
            _targetAudio = targetAudio;
            _targetVolume = targetVolume;
            _startVolume = _targetAudio.volume;
            _isRemoveTargetAudio = allVolumeChange ? 0 : 1;

            if (reset && _targetAudio.volume == 0f)
            {
                _targetAudio.Stop();
                _targetAudio.time = 0;
                _targetAudio.Play();
            }

            if (!_targetAudio.isPlaying) _targetAudio.Play();

            for (int i = 0; i < _audioCount; i++)
                if (_audioList[i] == targetAudio)
                {
                    _audioList[i] = _audioList[_audioCount - 1];
                    _audioList[_audioCount - 1] = targetAudio;
                    break;
                }

            for (int i = 0; i < _audioCount; i++)
                _startVolumes[i] = _audioList[i].volume;

            _accumulatedTime = 0f;
        }

        private void Update()
        {
            if (_accumulatedTime > maxTransitionTime) return;

            _accumulatedTime += Time.deltaTime;
            _t = _accumulatedTime / maxTransitionTime;
            if (useCurve) _t = curve.Evaluate(_t);
            if (_targetAudio.volume < 1f && _isRemoveTargetAudio == 1)
                ToAudio(_targetAudio, _startVolume, _targetVolume, _t);

            for (int i = 0; i < _audioCount - _isRemoveTargetAudio; i++)
            {
                if (_audioList[i].volume > 0f)
                {
                    ToAudio(_audioList[i], _startVolumes[i], 0f, _t);
                    if (_audioList[i].volume == 0f)
                        _audioList[i].Pause();
                }
            }
        }

        private void ToAudio(AudioSource target, float transitionVolume, float targetVolume, float t)
        {
            target.volume = Mathf.Lerp(transitionVolume, targetVolume, t);
        }
    }
}
