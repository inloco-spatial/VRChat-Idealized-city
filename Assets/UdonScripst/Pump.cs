
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Pump : UdonSharpBehaviour
{
    public AudioSource soundSource;
    public AudioClip soundClip;
    public Animator pumpAnim;
    public string pumpAnimName;
    public Animator baloon;
    public string baloonAnimName;

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"PublicPump");
    }
    
    public void PublicPump()
    {
        soundSource.PlayOneShot(soundClip);
        pumpAnim.SetTrigger(pumpAnimName);
        baloon.SetTrigger(baloonAnimName);

    }

}
