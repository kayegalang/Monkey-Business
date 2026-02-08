using UnityEngine;

public class DebugCheats : MonoBehaviour
{
    [SerializeField] private BananaWallet wallet;
    [SerializeField] private int addAmount = 99999;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            wallet.Add(addAmount);
            Debug.Log($"Added {addAmount} bananas. Total: {wallet.Bananas}");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            wallet.Reset();
            Debug.Log("Wallet reset.");
        }
    }
}