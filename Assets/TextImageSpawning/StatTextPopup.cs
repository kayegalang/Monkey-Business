using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class StatTextPopup : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float floatSpeed = 80f;
    [SerializeField] private float lifetime = 1.2f;
    [SerializeField] private float fadeStartPercent = 0.4f; // start fading at 40% through lifetime

    private CanvasGroup canvasGroup;
    private float timer;
    private float fadeStartTime;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        timer = 0f;
        fadeStartTime = lifetime * fadeStartPercent;
        canvasGroup.alpha = 1f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Float up
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out after fadeStartTime
        if (timer >= fadeStartTime)
        {
            float fadeProgress = (timer - fadeStartTime) / (lifetime - fadeStartTime);
            canvasGroup.alpha = 1f - Mathf.Clamp01(fadeProgress);
        }

        // Self-destruct
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}