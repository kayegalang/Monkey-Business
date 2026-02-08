using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClassUpButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BananaWallet wallet;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button button;
    
    private int evolutionCount = 0; // Track how many times evolved
    
    void Start()
    {
        if (button != null)
            button.onClick.AddListener(OnClick);
        
        if (wallet != null)
            wallet.OnBananasChanged += OnBananasChanged;
        
        if (MonkeyStatsManager.Instance != null)
        {
            MonkeyStatsManager.Instance.OnStatsChanged += RefreshUI;
            MonkeyStatsManager.Instance.OnMonkeyEvolved += OnEvolved;
        }
        
        Invoke(nameof(RefreshUI), 0.1f);
    }
    
    void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClick);
        
        if (wallet != null)
            wallet.OnBananasChanged -= OnBananasChanged;
        
        if (MonkeyStatsManager.Instance != null)
        {
            MonkeyStatsManager.Instance.OnStatsChanged -= RefreshUI;
            MonkeyStatsManager.Instance.OnMonkeyEvolved -= OnEvolved;
        }
    }
    
    void OnClick()
    {
        if (MonkeyStatsManager.Instance == null) return;
        
        if (MonkeyStatsManager.Instance.TryEvolve())
        {
            Debug.Log("🎉 CLASS UP!");
        }
    }
    
    void OnBananasChanged(int _) => RefreshUI();
    
    void OnEvolved()
    {
        evolutionCount++;
        RefreshUI();
    }
    
    void RefreshUI()
    {
        if (MonkeyStatsManager.Instance == null) return;
        
        int cost = MonkeyStatsManager.Instance.GetEvolutionCost();
        int bananas = wallet != null ? wallet.Bananas : 0;
        
        // Get current form to show as "level"
        string currentForm = MonkeyStatsManager.Instance.GetCurrentFormName();
        string[] forms = { "The Monkee", "Chiller Monkee", "Lit Monkee", "Battle Kong", "Golden Kong", "Big Chungus Kong" };
        int formIndex = System.Array.IndexOf(forms, currentForm);
        int maxEvolutions = 5;
        
        bool isMaxed = formIndex >= maxEvolutions;
        
        // Update name
        if (nameText != null)
        {
            nameText.text = "Class Up";
        }
        
        // Update level (show current form as "level")
        if (levelText != null)
        {
            if (isMaxed)
            {
                levelText.text = "MAX";
            }
            else
            {
                levelText.text = $"Lv {formIndex}/{maxEvolutions}";
            }
        }
        
        // Update cost and button state
        if (isMaxed)
        {
            if (costText != null)
            {
                costText.text = "MAX";
            }
            
            if (button != null)
            {
                button.interactable = false;
            }
        }
        else
        {
            if (costText != null)
            {
                costText.text = $"{cost} 🍌";
            }
            
            if (button != null)
            {
                button.interactable = bananas >= cost;
            }
        }
        
        Debug.Log($"Class Up UI refreshed: Form {formIndex}/{maxEvolutions}, Cost:{cost}, Can afford:{bananas >= cost}");
    }
}