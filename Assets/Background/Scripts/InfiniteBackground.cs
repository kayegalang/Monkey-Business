using UnityEngine;

namespace Background.Scripts
{
    public class InfiniteBackground : MonoBehaviour
{
    private Transform bg1;
    private Transform bg2;
    private GameObject bg2Object;
    private float backgroundWidth;
    private Transform cam;
    
    void Start()
    {
        cam = Camera.main.transform;
        bg1 = transform;
        
        // Get background width
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            backgroundWidth = sr.bounds.size.x;
        }
        else
        {
            Debug.LogError($"{gameObject.name}: No SpriteRenderer found!");
            return;
        }
        
        // Create second background
        CreateStationaryCopy();
        
        // Position them
        SetupPositions();
    }
    
    void CreateStationaryCopy()
    {
        // Duplicate this background (keeps Animator for animation!)
        bg2Object = Instantiate(gameObject, transform.parent);
        bg2Object.name = $"{gameObject.name}_Copy";
        
        // Remove THIS script from copy (only original needs it)
        InfiniteBackground copyScript = bg2Object.GetComponent<InfiniteBackground>();
        if (copyScript != null)
        {
            Destroy(copyScript);
        }
        
        bg2 = bg2Object.transform;
    }
    
    void SetupPositions()
    {
        Vector3 bg1Pos = bg1.position;
        
        // Position BG2 to the LEFT (for left-scrolling camera)
        Vector3 bg2Pos = bg1Pos;
        bg2Pos.x -= backgroundWidth;
        bg2.position = bg2Pos;
    }
    
    void LateUpdate()
    {
        if (cam == null || bg2 == null) return;
        
        float camX = cam.position.x;
        
        // Check if camera has passed the threshold point of BG1
        float bg1Center = bg1.position.x;
        if (camX < bg1Center - (backgroundWidth * 1f))
        {
            // Camera has passed the threshold of BG1
            // Reposition BG1 to the LEFT of BG2
            
            Vector3 newPos = bg2.position;
            newPos.x -= backgroundWidth;
            bg1.position = newPos;
        }
        
        // Check if camera has passed the threshold point of BG2
        float bg2Center = bg2.position.x;
        if (camX < bg2Center - (backgroundWidth * 1f))
        {
            // Camera has passed the threshold of BG2
            // Reposition BG2 to the LEFT of BG1
            
            Vector3 newPos = bg1.position;
            newPos.x -= backgroundWidth;
            bg2.position = newPos;
        }
    }
}
}