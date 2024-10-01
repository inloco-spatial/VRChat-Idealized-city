
using Stasok.LangSystem;
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class LangChanger : UdonSharpBehaviour
{
    private LangSystem _langSystem;

    private void Start()
    {
        _langSystem = LangSetting.GetLangSystem();
        this.enabled = false;
    }

    public void SetRU() { if (_langSystem) _langSystem.SetRU(); }
    public void SetEN() { if (_langSystem) _langSystem.SetEN(); }
    public void SetJA() { if (_langSystem) _langSystem.SetJA(); }
}
