
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace SaccFlightAndVehicles
{
    public class RaceNames : UdonSharpBehaviour
    {
        public string[] raceName;
        public SaccRaceToggleButton RaceToggler;
        public TextMeshProUGUI RaceNameText;

        
        public void RaceChange()
        {
            if (RaceToggler.CurrentCourseSelection == -1) 
            { 
                RaceNameText.text = "Free";
                return;
            }

            if (RaceToggler.CurrentCourseSelection < raceName.Length)
            {
                RaceNameText.text = raceName[RaceToggler.CurrentCourseSelection];
            }
        }
    }
}
