using VRC.Udon;

public class UdonExtension
{
    static public void CallUdonMethod(UdonBehaviour udon, string udonEvent)
    {
        if (udon && udonEvent != "")
            udon.SendCustomEvent(udonEvent);
    }
}
