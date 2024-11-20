
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Stasok.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SmoothCanvasTransparency : UdonSharpBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] float speed = 1f;
        [SerializeField] bool startState;
        [SerializeField] bool viewOnEnabe = true;
        [SerializeField] bool autoDisable = true;

        private bool _inited;
        private bool _change;
        private float _targetAlpha;

        void Start()
        {
            if (_inited) return;
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
            _inited = true;
            if (!startState) gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (viewOnEnabe) View();
            else canvasGroup.alpha = 0;
        }

        private void OnDisable() => canvasGroup.alpha = 0;

        public void View() => ToAlpha(1);
        public void Hide() => ToAlpha(0);
        private void ToAlpha(float alpha)
        {
            Start();
            _targetAlpha = alpha;
            _change = true;
        }

        private void Update()
        {
            if (!_change) return;
            if (canvasGroup.alpha == _targetAlpha) AlphaApplied();
            else canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, _targetAlpha, speed * Time.deltaTime);
        }

        private void AlphaApplied()
        {
            _change = false;
            if (autoDisable && _targetAlpha == 0) gameObject.SetActive(false);
        }

#if UNITY_EDITOR && !COMPILER_UDONSHARP
        [CustomEditor(typeof(SmoothCanvasTransparency))]
        public class SmoothCanvasTransparencyInspector : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                var script = (SmoothCanvasTransparency)target;
                if (GUILayout.Button(nameof(View)))
                    script.View();
                if (GUILayout.Button(nameof(Hide)))
                    script.Hide();
            }
        }
#endif
    }
}