using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Stasok.LangSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DropdownLangChanger : UdonSharpBehaviour
    {
        [SerializeField] Dropdown dropdown;
        private LangSystem _langSystem;

        private void Start()
        {
            if (!dropdown)
                dropdown = GetComponent<Dropdown>();

            _langSystem = LangSetting.GetLangSystem();
            _langSystem.AddLangReceiver(this);

            SendCustomEventDelayedSeconds(nameof(ChangeLang), 1f);
        }

        public void ChangeLang()
        {
            dropdown.value = _langSystem.CurrentLang;
        }

        public void DropdownChangeLang()
        {
            if (!_langSystem) return;

            switch (dropdown.value)
            {
                case 0: _langSystem.SetRU(); break;
                case 1: _langSystem.SetEN(); break;
                case 2: _langSystem.SetJA(); break;
                default: break;
            }
        }
    }
}