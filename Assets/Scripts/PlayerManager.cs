using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public PlayerController player;
    public Camera mainCam;

    void OnValidate()
    {
        if (player == null) player = FindFirstObjectByType<PlayerController>();
        if (mainCam == null) mainCam = player.transform.GetComponentInChildren<Camera>();
    }

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this.gameObject);
        }
        
        // Variables
        if (player == null) player = FindFirstObjectByType<PlayerController>();
        if (mainCam == null) mainCam = player.transform.GetComponentInChildren<Camera>();
    }

    void Update()
    {
        
    }
}
