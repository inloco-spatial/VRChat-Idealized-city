
using UdonSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Stasok.LangSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ScrollAudioSync : UdonSharpBehaviour
    {
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] AudioSource audioSource;
        [SerializeField] float offset = 3f;
        [Header("min: -0.5 | max: 0.5 | stable: 0")]
        [SerializeField] AnimationCurve speedOffset = AnimationCurve.Linear(0, 0, 1, 0);
        [Header("Read Only")]
        [SerializeField] float currentTimeCurve;
        [SerializeField] Vector2[] curveInitSetting;

        private void Start()
        {
            if (!scrollRect) scrollRect = GetComponent<ScrollRect>();
            if (!audioSource) audioSource = GetComponentInChildren<AudioSource>(true);
        }

        private void OnEnable()
        {
            audioSource.time = Networking.GetServerTimeInMilliseconds() % audioSource.clip.length;
            audioSource.Play();
        }

        private void Update()
        {
            currentTimeCurve = Mathf.Round(audioSource.time * 100f) / 100f;
            float t = currentTimeCurve / audioSource.clip.length;
            scrollRect.verticalNormalizedPosition = 1 - (t + speedOffset.Evaluate(currentTimeCurve));
        }

        private void AudioDurationToCurve()
        {
#if UNITY_EDITOR && !COMPILER_UDONSHARP
            Start();
            speedOffset.keys = new Keyframe[0];
            for (int i = 0; i < curveInitSetting.Length; i++)
                speedOffset.AddKey(curveInitSetting[i].x, curveInitSetting[i].y);

            speedOffset.AddKey(audioSource.clip.length, 0);
            EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR && !COMPILER_UDONSHARP
        [CustomEditor(typeof(ScrollAudioSync))]
        public class ScrollAudioSyncInspector : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                ScrollAudioSync script = (ScrollAudioSync)target;
                if (GUILayout.Button(nameof(AudioDurationToCurve)))
                    script.AudioDurationToCurve();
            }
        }
#endif
    }
}