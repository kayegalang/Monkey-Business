// UpgradeButtonUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private UpgradeStat upgrade;
    [SerializeField] private BananaWallet wallet;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button button;

    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
        wallet.OnBananasChanged += OnBananasChanged;
        upgrade.OnUpgraded += OnUpgraded;
        RefreshUI();
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
        wallet.OnBananasChanged -= OnBananasChanged;
        upgrade.OnUpgraded -= OnUpgraded;
    }

    private void OnClick()
    {
        upgrade.TryUpgrade(wallet);
    }

    private void OnBananasChanged(int _) => RefreshUI();
    private void OnUpgraded(UpgradeStat _) => RefreshUI();

    private void RefreshUI()
    {
        nameText.text = upgrade.upgradeName;
        levelText.text = $"Lv {upgrade.CurrentLevel}/{upgrade.MaxLevel}";

        if (upgrade.IsMaxed)
        {
            costText.text = "MAX";
            button.interactable = false;
        }
        else
        {
            int cost = upgrade.CurrentCost;
            costText.text = $"{cost}";

            // grey out if can't afford
            button.interactable = wallet.Bananas >= cost;
        }
    }
}