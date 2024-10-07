
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;

namespace Stasok.AudioPlayer
{
    [RequireComponent(typeof(BoxCollider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AudioTrigger : UdonSharpBehaviour 
    {
        [SerializeField] private AudioPlayer audioPlayer;
        [Space(15)]
        [SerializeField] private AudioSource volumeable;
        [Range(0.01f, 1.0f)]
        [SerializeField] private float maxVolume = 1f;
        [Space(15)]
        [SerializeField] private bool resetPlaying = false;
        [SerializeField] private bool muteOnExit = false;

        private void Start()
        {
            if (audioPlayer.RemoveTriggerRenderers)
            {
                Renderer r = GetComponent<Renderer>();
                if (r) Destroy(r);
            }

            if (!volumeable) volumeable = GetComponent<AudioSource>();
            if (audioPlayer) audioPlayer.AddAudio(volumeable);
            volumeable.volume = 0;
        }

        public void RunPlay() => OnPlayerTriggerEnter(Networking.LocalPlayer);

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            audioPlayer.Play(volumeable, maxVolume, resetPlaying, false);
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (!player.isLocal || !muteOnExit) return;

            audioPlayer.Play(volumeable, 0.01f, resetPlaying, true);
        }

#if UNITY_EDITOR && !COMPILER_UDONSHARP

        private void OnDrawGizmos()
        {
            Gizmos.color = audioPlayer.GizmosAutoPlayTriggerColor;
            if (audioPlayer && audioPlayer.ReceiverStartPlay == this)
                Gizmos.DrawCube(transform.position, transform.localScale * 1.3f);
        }

        [CustomEditor(typeof(AudioTrigger))]
        public class AudioTriggerInspector : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                var script = (AudioTrigger)target;

                if (GUILayout.Button("SetStartPlay"))
                {
                    script.audioPlayer.ReceiverStartPlay = script;
                    EditorUtility.SetDirty(script.audioPlayer);
                }
            }
        }
#endif
    }
}