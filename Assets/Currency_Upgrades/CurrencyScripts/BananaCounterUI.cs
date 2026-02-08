// BananaCounterUI.cs
using UnityEngine;
using TMPro;

public class BananaCounterUI : MonoBehaviour
{
    [SerializeField] private BananaWallet wallet;
    [SerializeField] private TextMeshProUGUI bananaText;

    private void OnEnable()
    {
        wallet.OnBananasChanged += UpdateDisplay;
        UpdateDisplay(wallet.Bananas);
    }

    private void OnDisable()
    {
        wallet.OnBananasChanged -= UpdateDisplay;
    }

    private void UpdateDisplay(int amount)
    {
        bananaText.text = $"{amount}";
    }
}