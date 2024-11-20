
using Stasok.Utils;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Stasok.Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerTrigger : UdonSharpBehaviour
    {
        [SerializeField] GameObject[] toggleable;
        [SerializeField] bool toggleableStartState = true;
        [SerializeField] bool toggleableActivate = true;
        [SerializeField] bool toggleableDeactivate = true;
        
        [Space(10)]

        [Header("Enter")]
        [SerializeField] UdonBehaviour udonEnter;
        [SerializeField] string udonEnterEvent;
        [SerializeField] AudioSource audioEnter;

        [Space(5)]

        [Header("Exit")]
        [SerializeField] UdonBehaviour udonExit;
        [SerializeField] string udonExitEvent;
        [SerializeField] AudioSource audioExit;
        [SerializeField] float lifetimeOnExit = 2f;

        private BoxCollider _bc;

        private void Start()
        {
            _bc = GetComponent<BoxCollider>();
            _bc.isTrigger = true;
            SendCustomEventDelayedSeconds(nameof(StartBySwitch), 2);
        }

        public void StartBySwitch() => ArrayUtils.SetActive(toggleable, toggleableStartState);

        private void PlayAudio(AudioSource audio)
        {
            if (audio && !audio.isPlaying) audio.Play();
        }

        public override void OnPlayerTriggerEnter(VRCPlayerApi player) => HandleTrigger(player, true, toggleableActivate, udonEnter, udonEnterEvent, audioEnter);
        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            SendCustomEventDelayedSeconds(nameof(Deactivate), lifetimeOnExit);
        }

        public void Deactivate()
        {
            if (_bc.bounds.Contains(Networking.LocalPlayer.GetPosition())) return;
            HandleTrigger(Networking.LocalPlayer, false, toggleableDeactivate, udonExit, udonExitEvent, audioExit);
        }

        private void HandleTrigger(VRCPlayerApi player, bool transitionState, bool toggleableAction, UdonBehaviour udon, string udonEvent, AudioSource audio)
        {
            if (!player.isLocal) return;

            if (toggleableAction) ArrayUtils.SetActive(toggleable, transitionState);
            UdonExtension.CallUdonMethod(udon, udonEvent);
            PlayAudio(audio);
        }
    }
}
