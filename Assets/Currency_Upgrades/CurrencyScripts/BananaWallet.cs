// BananaWallet.cs
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "BananaWallet", menuName = "Game/Banana Wallet")]
public class BananaWallet : ScriptableObject
{
    [SerializeField] private int bananas;

    // anyone can listen for changes (UI, audio, etc.)
    public event Action<int> OnBananasChanged;

    public int Bananas => bananas;

    public void Add(int amount)
    {
        bananas += amount;
        OnBananasChanged?.Invoke(bananas);
    }

    public bool TrySpend(int amount)
    {
        if (bananas < amount)
            return false;

        bananas -= amount;
        OnBananasChanged?.Invoke(bananas);
        return true;
    }

    public void Reset()
    {
        bananas = 0;
        OnBananasChanged?.Invoke(bananas);
    }
    
    // For getting banana count
    public int GetCurrency() => bananas;

    // For UI button events (alias for OnBananasChanged)
    public event Action<int> OnCurrencyChanged
    {
        add => OnBananasChanged += value;
        remove => OnBananasChanged -= value;
    }
}