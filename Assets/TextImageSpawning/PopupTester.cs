using UnityEngine;
using UnityEngine.InputSystem;

public class PopupTester : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Trying to spawn popup...");
            
            if (StatPopupSpawner.Instance == null)
            {
                Debug.LogError("StatPopupSpawner.Instance is NULL!");
                return;
            }

            StatPopupSpawner.Instance.Spawn(
                StatPopupSpawner.PopupType.Health, 
                new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
            );
            
            Debug.Log("Popup spawned!");
        }
    }
}