using TMPro;
using UdonSharp;
using UnityEditor;
using UnityEngine;

namespace Stasok.LangSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LangTextMeshPro : UdonSharpBehaviour
    {
        [Header("будет установлен в RU, autoGet")]
        [SerializeField] TextMeshProUGUI target;

        [TextArea]
        [Header("0=RU, 1=EN, 2=JA")]
        [SerializeField] string[] langs = { "после старта RU появится тут", "EN", "JA" };

        private LangSystem _langSystem;
        private bool _inited;

        private void OnEnable()
        {
            if (!_inited) Init();
            if (GetResource() == "") return;
            ChangeLang();
        }

        private void Init()
        {
            InitResources();

            if (!_langSystem)
            {
                _langSystem = LangSetting.GetLangSystem();
                _langSystem.AddLangReceiver(this);
            }

            langs[0] = GetResource();
            _inited = true;
        }

        private void InitResources()
        {
            if (!target)
                target = GetComponent<TextMeshProUGUI>();
        }
        private void SetResource(string resource)
        {
            if (target) target.text = resource;
        }
        private string GetResource()
        {
            if (target) return target.text;
            return "";
        }

        public void ChangeLang()
        {
            if (GetResource() != "" && _langSystem)
            {
                if (langs[_langSystem.CurrentLang] == null)
                    SetResource(langs[_langSystem.CurrentLang] = langs[1]);
                else
                    SetResource(langs[_langSystem.CurrentLang]);
            }
            else
                Init();

            this.enabled = false;
        }

#if UNITY_EDITOR && !COMPILER_UDONSHARP
        [CustomEditor(typeof(LangTextMeshPro))]
        public class LangTextMeshProInspector : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                LangTextMeshPro script = (LangTextMeshPro)target;

                if (GUILayout.Button("CopyLangArray"))
                {
                    string arrayString = string.Join("/", script.langs);
                    EditorGUIUtility.systemCopyBuffer = arrayString;
                    Debug.Log("Array copied to clipboard: " + arrayString);
                }

                if (GUILayout.Button("PasteLangArray"))
                {
                    if (!EditorGUIUtility.systemCopyBuffer.Contains("/"))
                    {
                        Debug.Log("clipboard array not valid: " + EditorGUIUtility.systemCopyBuffer);
                        return;
                    }

                    string[] result = script.langs = EditorGUIUtility.systemCopyBuffer.Split('/');
                    EditorUtility.SetDirty(script);
                    Debug.Log("Array received from clipboard: " + result);
                }
            }
        }
#endif
    }
}