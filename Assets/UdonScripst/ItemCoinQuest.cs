
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ItemCoinQuest : UdonSharpBehaviour
{
    public CustomCoinQuest MainScript;
    public AudioSource sound;
    public AudioClip soundClip;
    public ParticleSystem pS;
    public SphereCollider colliderS;
    public GameObject rendMesh;
    
    void Start()
    {
        
    }
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player == Networking.LocalPlayer)
        {
            MainScript.CoinGrabbed();
            sound.PlayOneShot(soundClip);
            pS.Play();
            colliderS.enabled = false;
            rendMesh.SetActive(false);
        }
    }
}
