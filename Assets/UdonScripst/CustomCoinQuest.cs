
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CustomCoinQuest : UdonSharpBehaviour
{
    public GameObject[] coins;
    public int currentCoin;
    public int lastCoin;
    void Start()
    {
        for (int i = 0;i < coins.Length ;i++)
        {
            coins[i].SetActive(false);
        }
        coins[0].SetActive(true);
    }
    public void CoinGrabbed()
    {
        lastCoin = currentCoin;
        if (currentCoin < coins.Length-1)
        {
            currentCoin++;
            coins[currentCoin].SetActive(true);
        }
        SendCustomEventDelayedSeconds("LateCoinHide", 1);
    }
    public void LateCoinHide()
    {
        coins[lastCoin].SetActive(false);
    }
}
