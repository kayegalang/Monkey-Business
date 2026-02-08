using UnityEngine;

public class StatPopupSpawner : MonoBehaviour
{
    public static StatPopupSpawner Instance { get; private set; }

    public enum PopupType
    {
        Health,
        Damage,
        APS,
        DPS,
        Speed,
        Range,
        ClassUp
    }

    [Header("Prefabs (assign in order of PopupType enum)")]
    [SerializeField] private GameObject[] popupPrefabs = new GameObject[7];

    [Header("Spawn Settings")]
    [SerializeField] private Vector2 spawnOffset = new Vector2(0f, 40f); // offset from button
    [SerializeField] private Transform canvasParent; // your Canvas or a child panel

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Call this from anywhere: StatPopupSpawner.Instance.Spawn(PopupType.Health, buttonTransform.position);
    /// </summary>
    public void Spawn(PopupType type, Vector3 worldPosition)
    {
        int index = (int)type;
        if (index < 0 || index >= popupPrefabs.Length || popupPrefabs[index] == null)
        {
            Debug.LogWarning($"StatPopupSpawner: No prefab assigned for {type}");
            return;
        }

        Transform parent = canvasParent != null ? canvasParent : transform;
        GameObject popup = Instantiate(popupPrefabs[index], parent);
        popup.transform.position = worldPosition + (Vector3)spawnOffset;
    }
}