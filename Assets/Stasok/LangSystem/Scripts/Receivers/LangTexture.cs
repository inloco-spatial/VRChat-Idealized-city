using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Stasok.LangSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LangTexture : UdonSharpBehaviour
    {
        [Header("будет установлен в RU, Добавить самому")]
        [Header("использовать 'targetMaterial' или 'targetRawImage', можно сразу оба")]
        [SerializeField] Material targetMaterial;

        [Header("будет установлен в RU, autoGet")]
        [SerializeField] RawImage targetRawImage;

        [Space(2)] /////////////////////////////////////////////////////////////

        [Header("0=RU, 1=EN, 2=JA")]
        [SerializeField] Texture[] langs = new Texture[3];

        private LangSystem _langSystem;
        private bool _inited;
        private Renderer _renderer;

        private void OnEnable()
        {
            if (!_inited) Init();
            if (!GetResource()) return;
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
            if (!targetMaterial)
            {
                _renderer = GetComponent<Renderer>();
                if (_renderer) targetMaterial = _renderer.material;
            }
            if (!targetRawImage)
                targetRawImage = GetComponent<RawImage>();
        }
        private void SetResource(Texture resource)
        {
            if (targetMaterial) targetMaterial.mainTexture = resource;
            if (targetRawImage) targetRawImage.texture = resource;
        }
        private Texture GetResource()
        {
            if (targetMaterial) return targetMaterial.mainTexture;
            else if (targetRawImage) return targetRawImage.mainTexture;
            return null;
        }

        public void ChangeLang()
        {
            if (GetResource() && _langSystem)
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

        private void OnDestroy()
        {
            targetMaterial.mainTexture = langs[0];
            if (_renderer)
                _renderer.sharedMaterial.mainTexture = langs[0];
        }
    }
}