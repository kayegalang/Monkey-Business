// BananaTestDebug.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class BananaTestDebug : MonoBehaviour
{
    [SerializeField] private BananaWallet wallet;
    [SerializeField] private int testAmount = 10;

    private void Update()
    {
        // press B to add bananas
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            wallet.Add(testAmount);
            Debug.Log($"Added {testAmount} bananas! Total: {wallet.Bananas}");
        }

        // press R to reset wallet
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            wallet.Reset();
            Debug.Log("Wallet reset!");
        }
    }
}