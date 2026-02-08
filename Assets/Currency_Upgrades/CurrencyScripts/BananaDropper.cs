// BananaDropper.cs
using UnityEngine;

public class BananaDropper : MonoBehaviour
{
    [SerializeField] private BananaWallet wallet;
    [SerializeField] private int dropAmount = 1;

    // call this when the enemy dies
    public void DropBananas()
    {
        wallet.Add(dropAmount);
        Debug.Log($"Dropped {dropAmount} bananas!");
    }
}