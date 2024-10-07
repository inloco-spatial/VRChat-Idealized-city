
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class ObjectInclude : UdonSharpBehaviour
{
    public GameObject GO;
    public Toggle toggle;
    void Start()
    {
        GO.SetActive(toggle.isOn);
    }
    public void ToggleChange()
    {
        GO.SetActive(toggle.isOn);
    }
}
