
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using UnityEngine.UI;
using VRC.Udon;

public class SoftShadowInclude : UdonSharpBehaviour
{
    public Light light;
    public Toggle toggle;

    public void ToggleChange()
    {
        if (toggle.isOn) light.shadows = LightShadows.Soft;
        else light.shadows = LightShadows.Hard;
    }
}
