using Stasok.Utils;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Stasok.LangSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LangSystemProgressBar : UdonSharpBehaviour
    {
        [SerializeField] Image progressBar;
        private LangSystem _langSystem;

        private void Start()
        {
            _langSystem = LangSetting.GetLangSystem();
            if (!progressBar)
                progressBar = GetComponent<Image>();
        }

        private void Update()
        {
            if (!_langSystem.ChangeLang) return;

            progressBar.fillAmount = MathUtils.Convert(_langSystem.LangableCount, 0, _langSystem.RecipientsCount, 0, 1);
            if (progressBar.fillAmount >= 1)
                progressBar.fillAmount = 0;
        }
    }
}