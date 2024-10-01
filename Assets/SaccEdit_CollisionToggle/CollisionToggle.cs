
using SaccFlightAndVehicles;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using UnityEngine.UI;
using VRC.Udon;
using TMPro;

#if !COMPILER_UDONSHARP && UNITY_EDITOR

using UnityEngine.SceneManagement;
using VRC.SDKBase.Editor.Api;
using UdonSharpEditor;
using UnityEditor;
using System.Collections.Generic;
#endif

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class CollisionToggle : UdonSharpBehaviour
{

  [UdonSynced]
  public bool CollisionsActive = true;
  public Renderer Indicator = null;
  public Toggle newIndicator = null;
    public MasterObjectIncluder fly;

  public TMPro.TextMeshProUGUI Mastertext = null;
  
  [Tooltip("For when Vehicles have Collision. This is the layer that players not inside the vehicle see the vehicle as")]
  public int CollisionOutsideLayer = 17;
  [Tooltip("For when Vehicles have Collision. This is the layer that players inside the vehicle see the vehicle as")]
  public int CollisionOnboardLayer = 31;
  
  [Tooltip("For when Vehicles do not have Collision. This is the layer that players not inside the vehicle see the vehicle as")]
  public int NoCollisionOutsideLayer = 30;
  [Tooltip("For when Vehicles do not have Collision. This is the layer that players inside the vehicle see the vehicle as")]
  public int NoCollisionOnboardLayer = 30;
  public SaccGroundVehicle[] VehicleList;

  private void OnEnable()
  {
    UpdateMaster();
  }

  private void Start()
  {
  
    OnToggle();
  }

  
  public override void Interact()
  {
        if (!Networking.IsMaster)
        {
            newIndicator.isOn = CollisionsActive;
            return;            
        }
    

    if (!Networking.IsOwner(gameObject))
      Networking.SetOwner(Networking.LocalPlayer, gameObject);

    CollisionsActive = !CollisionsActive;
    OnToggle();

    RequestSerialization();
  }

  /////////////////////////////////
  /// Master Text functions

  public override void OnPlayerLeft(VRCPlayerApi player)
  {
    UpdateMaster();
  }

  // Only allow the master to own this so we can check master by checking this object's owner
  public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
  {
    return false;
  }

  void UpdateMaster()
  {
//#if !UNITY_EDITOR
            // We know the owner of this will always be the master so just get the owner and update the name
            if (Mastertext)
            {
                VRCPlayerApi owner = Networking.GetOwner(gameObject);
                if (owner != null && owner.IsValid())
                   Mastertext.text = "Current Master: " + Networking.GetOwner(gameObject).displayName;
            fly.owner = owner;
            }
//#endif
  }

  ///
  /////////////////////////////////

  //ondeserialize 
  //fire the OnToggle Event w/ the current Collision settings

  public override void OnDeserialization()
  {
    if (!Networking.IsOwner(gameObject))
    {
      OnToggle();
    }
  }

  //OnToggle
  // for each ground vehicle in the map fire the collisionlayerset function
  // Also set the outside/onboard layers on SaccGroundVehicle
  void OnToggle()
  {
    int NewOnboardLayer;
    int NewOutsideLayer;

    if (CollisionsActive)
    {
      NewOnboardLayer = CollisionOnboardLayer;
      NewOutsideLayer = CollisionOutsideLayer;

      Indicator.material.color = Color.green;
            newIndicator.isOn = true;
    }
    else
    {
      NewOnboardLayer = NoCollisionOnboardLayer;
      NewOutsideLayer = NoCollisionOutsideLayer;

      Indicator.material.color = Color.red;
            newIndicator.isOn = false;
        }

    foreach (var v in VehicleList)
    {

      v.OnboardVehicleLayer = NewOnboardLayer;
      v.OutsideVehicleLayer = NewOutsideLayer;
      if (v.IsOwner && v.Occupied)
        v.SetCollidersLayer(NewOnboardLayer);
      else
        v.SetCollidersLayer(NewOutsideLayer);
    }
  }
    
}

#if !COMPILER_UDONSHARP && UNITY_EDITOR
[CustomEditor(typeof(CollisionToggle))]
public class CollisionToggleEditor : Editor
{
  public override void OnInspectorGUI()
  {
    // Draws the default convert to UdonBehaviour button, program asset field, sync settings, etc.
    if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

    base.OnInspectorGUI();

    if (GUILayout.Button("Add All GroundVehicles to Vehicle List"))
    {
      var parent = (CollisionToggle)this.target;

      var ls = new List<SaccGroundVehicle>();
      foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
      {
        var objs = g.GetComponentsInChildren<SaccGroundVehicle>(true);
        ls.AddRange(objs);
      }
      parent.VehicleList = ls.ToArray();
    }
  }
}

#endif
