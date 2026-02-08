// BananaTestDebug.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class BananaTestDebug : MonoBehaviour
{
    [SerializeField] private BananaWallet wallet;
    [SerializeField] private int testAmount = 10;

    private void Start()
    {
        Debug.Log("=== DEBUG TESTER ACTIVE ===");
        Debug.Log("B - Add bananas");
        Debug.Log("R - Reset wallet");
        Debug.Log("P - Toggle pause");
        Debug.Log("T - Print wallet total");
    }

    private void Update()
    {
        // simulate enemy drop
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            wallet.Add(testAmount);
            Debug.Log($"Added {testAmount} bananas! Total: {wallet.Bananas}");
        }

        // reset wallet only
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            wallet.Reset();
            Debug.Log("Wallet reset!");
        }

        // test pause toggle
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            var gsm = GameSceneManager.Instance;
            if (gsm.state == GameSceneManager.State.Pause)
            {
                gsm.ChangeState(GameSceneManager.State.Battle);
                Debug.Log("Resumed!");
            }
            else
            {
                gsm.ChangeState(GameSceneManager.State.Pause);
                Debug.Log("Paused!");
            }
        }

        // print total
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            Debug.Log($"Current bananas: {wallet.Bananas}");
        }
    }
}