using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Stasok.LangSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LangAudio : UdonSharpBehaviour
    {
        [Header("будет установлен в RU, autoGet")]
        [SerializeField] AudioSource target;

        [Header("0=RU, 1=EN, 2=JA")]
        [SerializeField] AudioClip[] langs = new AudioClip[3];

        private LangSystem _langSystem;
        private bool _inited;

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
            if (!target)
                target = GetComponent<AudioSource>();
        }
        private void SetResource(AudioClip resource)
        {
            if (target) target.clip = resource;
        }
        private AudioClip GetResource()
        {
            if (target) return target.clip;
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

                target.Play();
            }
            else
                Init();

            this.enabled = false;
        }
    }
}