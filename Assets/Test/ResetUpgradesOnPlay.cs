using UnityEngine;

public class ResetUpgradesOnPlay : MonoBehaviour
{
    [SerializeField] private BananaWallet wallet;
    [SerializeField] private UpgradeStat[] upgrades;

    private void Awake()
    {
        wallet.Reset();
    }
}