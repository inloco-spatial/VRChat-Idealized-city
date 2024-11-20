
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components.Video;
using Stasok.Volume;

namespace Stasok.LocalVideoPlayer
{
    [RequireComponent(typeof(VRCAVProVideoPlayer))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LocalVideoPlayer : UdonSharpBehaviour
    {
        public bool RemoveTriggerRenderers = true;
        [Space(8)]
        public SmoothVolumeChanger SmoothVolumeChanger;

        public VideoPlayerTrigger lastPlayerTrigger;

        [Space(8)]
        [SerializeField] private bool sync = false;
        [SerializeField] private bool loop = true;
        [SerializeField] private float maxTimeReload = 8f;
        [Space(8)]
        public GameObject ErrAccessDenied;
        [SerializeField] private GameObject errRateLimited;

        private VRCAVProVideoPlayer _videoPlayer;
        private VRCUrl _videoURL = new VRCUrl("");
        private bool resetPlaying = true;

        private bool IsURLChanged(VRCUrl url) => _videoURL.Get() != url.Get();
        private bool ValidateVideo() => _videoPlayer.IsReady && _videoPlayer.GetDuration() != 0;

        private void Start()
        {
            _videoPlayer = GetComponent<VRCAVProVideoPlayer>();
            _videoPlayer.Loop = loop;
            SetActiveErr(null, false);
        }

        public void TryLoad()
        {
            if (ValidateVideo()) OnVideoReady();
            else Load();
        }

        private void TryPlay()
        {
            if (ValidateVideo()) Play();
            else Load();
        }

        public void Load()
        {
            _videoPlayer.Stop();
            resetPlaying = true;
            _videoPlayer.LoadURL(_videoURL);
        }

        public void Play() 
        {
            _videoPlayer.Play();
            OnVideoPlay();
        }

        public void PlayURL(VRCUrl url)
        {
            if (IsURLChanged(url))
            {
                _videoURL = url;
                Load();
            }
            else
                TryPlay();
        }

        public void Pause() => _videoPlayer.Pause();
        public void Stop() => _videoPlayer.Stop();
        public void OnAudioMuted() => Pause();
        public override void OnVideoLoop() => TryLoad();

        public void SmoothPause()
        {
            if (SmoothVolumeChanger) SmoothVolumeChanger.DecreaseVolume();
            else Pause();
        }

        public override void OnVideoReady()
        {
            if (!ValidateVideo())
            {
                Load();
                return;
            }

            SetActiveErr(null, false);

            if (sync)
            {
                float targetTime = (float)Networking.GetServerTimeInSeconds() % _videoPlayer.GetDuration();
                _videoPlayer.SetTime(targetTime);
            }

            Play();
        }

        private new void OnVideoPlay()
        {
            if (SmoothVolumeChanger)
            {
                if (resetPlaying)
                {
                    SmoothVolumeChanger.ResetVolume();
                    resetPlaying = false;
                }

                SmoothVolumeChanger.IncreaseVolume();
            }
        }

        public override void OnVideoError(VideoError videoError)
        {
            if (videoError.ToString() == "AccessDenied") SetActiveErr(ErrAccessDenied, true);
            else if (videoError.ToString() == "RateLimited") SetActiveErr(errRateLimited, true);

            SendCustomEventDelayedSeconds(nameof(TryLoad), Random.Range(maxTimeReload / 4f, maxTimeReload));
        }

        private void SetActiveErr(GameObject err, bool state)
        {
            if (err) err.SetActive(state);

            ErrDeactivate(err, ErrAccessDenied);
            ErrDeactivate(err, errRateLimited);
        }

        private void ErrDeactivate(GameObject err, GameObject currentErr)
        {
            if (currentErr)
                if (err != currentErr) currentErr.SetActive(false);
        }
    }
}

