using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Stasok.LangSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LangSprite : UdonSharpBehaviour
    {
        [Header("будет установлен в RU, autoGet")]
        [Header("использовать 'targetImage' или 'targetSprite', можно сразу оба")]
        [SerializeField] Image targetImage;

        [Header("будет установлен в RU, autoGet")]
        [SerializeField] SpriteRenderer targetSprite;

        [Space(2)] /////////////////////////////////////////////////////////////

        [Header("0=RU, 1=EN, 2=JA")]
        [SerializeField] Sprite[] langs = new Sprite[3];

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
            if (!targetImage)
                targetImage = GetComponent<Image>();
            if (!targetSprite)
                targetSprite = GetComponent<SpriteRenderer>();
        }
        private void SetResource(Sprite resource)
        {
            if (targetImage) targetImage.sprite = resource;
            if (targetSprite) targetSprite.sprite = resource;
        }
        private Sprite GetResource()
        {
            if (targetImage) return targetImage.sprite;
            else if (targetSprite) return targetSprite.sprite;
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
    }
}