
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Footsteps : UdonSharpBehaviour
{
    public Vector3 footLPos;
    public Vector3 footRPos;
    
    public GameObject leftFoot;
    public GameObject rightFoot;
    public float speedTrashhold = 1;
    public float height = -4.1f;
    void Update()
    {
        if (Networking.LocalPlayer.GetVelocity().magnitude > speedTrashhold)
        { 
            footLPos = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftFoot);
            footRPos = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.RightFoot);
            leftFoot.transform.position = footLPos;
            rightFoot.transform.position = footRPos;    
        }
    }
}
