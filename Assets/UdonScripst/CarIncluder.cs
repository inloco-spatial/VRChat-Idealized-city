
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using UnityEngine.UI;

namespace SaccFlightAndVehicles
{
    public class CarIncluder : UdonSharpBehaviour
    {
        public SaccEntity EntityControl;

        [UdonSynced] public bool carInlude;
        public TextMeshProUGUI showButton;
        public TextMeshProUGUI hideButton;
        public GameObject car;
        private VRCPlayerApi localPlayer;


        void Start()
        {
            if (carInlude)
            {
                showButton.color = Color.yellow;
                car.SetActive(true);
            }
            else
            {
                hideButton.color = Color.yellow;
                car.SetActive(false);
            }
            if (Networking.IsOwner(gameObject))
                RequestSerialization();
        }
        public override void OnDeserialization()
        {
            valueChanged();
        }

            public void CarOn()
        {
            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(localPlayer, gameObject);
            carInlude = true;
            valueChanged();
            RequestSerialization();
        }

        public void CarOff()
        {
            
            if (!EntityControl.Occupied) 
            {
                if (!Networking.IsOwner(gameObject))
                    Networking.SetOwner(localPlayer, gameObject);
                carInlude = false;
                valueChanged();
                RequestSerialization();
            }
            
        }
        
        public void valueChanged()
        {
            

            if (carInlude)
            {
                showButton.color = Color.yellow;
                hideButton.color = Color.white;
                car.SetActive(true);
            }
            else
            {
                hideButton.color = Color.yellow;
                showButton.color = Color.white;
                car.SetActive(false);
            }
        }
    }
}
