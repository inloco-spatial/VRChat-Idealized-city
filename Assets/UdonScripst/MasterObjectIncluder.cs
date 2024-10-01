
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using UnityEngine.UI;
using VRC.Udon;

public class MasterObjectIncluder : UdonSharpBehaviour
{
    public GameObject target;
    public VRCPlayerApi owner;

    [UdonSynced] public bool currentValue;
    public Toggle toggle;

    public void Start()
    {
        toggle.isOn = currentValue;
        target.SetActive(toggle.isOn);
    }
    public void ToggleChange()
    {

        if (owner == Networking.LocalPlayer)
        {
            toggle.isOn = !currentValue;
            currentValue = toggle.isOn;
            target.SetActive(toggle.isOn);
        }
        else
        {
            toggle.isOn = currentValue;
        }
        RequestSerialization();
    }
    public override void OnDeserialization()
    {
        toggle.isOn = currentValue; 
        target.SetActive(toggle.isOn);
    }
}
