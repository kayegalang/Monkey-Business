using UnityEngine;

public class BananaDrop : MonoBehaviour
{
    [SerializeField] private float despawnTime = 1f;

    private void Start()
    {
        Destroy(gameObject, despawnTime);
    }
}