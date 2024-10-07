
using UdonSharp;
using UnityEditor;
using UnityEngine;

namespace Stasok.AudioPlayer
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AudioInteract : UdonSharpBehaviour 
    {
        [SerializeField] private AudioPlayer audioPlayer;

        [Space(15)]
        [SerializeField] private AudioSource volumeable;

        [Range(0.01f, 1.0f)]
        [SerializeField] private float maxVolume = 1f;

        [Space(15)]
        [SerializeField] private bool resetPlaying = false;

        private void Start()
        {
            if (!volumeable) volumeable = GetComponent<AudioSource>();
            if (audioPlayer) audioPlayer.AddAudio(volumeable);
            volumeable.volume = 0;
        }

        public void RunPlay() => Interact();

        public override void Interact()
        {
            audioPlayer.Play(volumeable, maxVolume, resetPlaying, false);
        }

#if UNITY_EDITOR && !COMPILER_UDONSHARP

        private void OnDrawGizmos()
        {
            Gizmos.color = audioPlayer.GizmosAutoPlayTriggerColor;
            if (audioPlayer && audioPlayer.ReceiverStartPlay == this)
                Gizmos.DrawCube(transform.position, transform.localScale * 1.3f);
        }

        [CustomEditor(typeof(AudioInteract))]
        public class AudioInteractInspector : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                var script = (AudioInteract)target;

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