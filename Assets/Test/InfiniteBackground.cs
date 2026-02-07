using UnityEngine;

namespace Test
{
    public class InfiniteBackground : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float parallaxSpeed = 0.5f;
    [Tooltip("If true, background moves based on camera. If false, moves based on player.")]
    [SerializeField] private bool followCamera = true;
    [Tooltip("Loop when this much of background is off-screen. 0.3 = loop when 30% off-screen")]
    [SerializeField] private float loopThreshold = 0.3f;
    
    [Header("Auto-Setup")]
    [SerializeField] private bool autoDetectSize = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    private Transform cameraTransform;
    private Transform playerTransform;
    private float backgroundWidth;
    private Vector3 lastCameraPosition;
    
    void Start()
    {
        cameraTransform = Camera.main.transform;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        if (autoDetectSize)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                Bounds bounds = spriteRenderer.bounds;
                backgroundWidth = bounds.size.x;
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: No SpriteRenderer found! Using manual width.");
            }
        }
        
        if (cameraTransform != null)
        {
            lastCameraPosition = cameraTransform.position;
        }
    }
    
    void LateUpdate()
    {
        if (cameraTransform == null) return;
        
        Vector3 cameraDelta = cameraTransform.position - lastCameraPosition;
        

        Vector3 backgroundMove = new Vector3(
            -cameraDelta.x * parallaxSpeed,
            0,
            0
        );
        
        transform.position += backgroundMove;
        
        float distanceFromCamera = transform.position.x - cameraTransform.position.x;
        
        float loopTriggerDistance = backgroundWidth * loopThreshold;
        
        if (distanceFromCamera > loopTriggerDistance)
        {
            Vector3 newPos = transform.position;
            newPos.x -= backgroundWidth;
            transform.position = newPos;
            
            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name}: Looped LEFT (distance was {distanceFromCamera:F1}, threshold {loopTriggerDistance:F1})");
            }
        }
        else if (distanceFromCamera < -loopTriggerDistance)
        {
            Vector3 newPos = transform.position;
            newPos.x += backgroundWidth;
            transform.position = newPos;
        }
        
        lastCameraPosition = cameraTransform.position;
    }
}
}