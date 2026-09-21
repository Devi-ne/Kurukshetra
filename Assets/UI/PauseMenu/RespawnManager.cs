using UnityEngine;
using UnityEngine.InputSystem;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    [SerializeField] private Transform checkpoint01;
    [SerializeField] private GameObject player;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            FakeDeath();
        }
    }

    private void FakeDeath()
    {
        Debug.Log("PLAYER DIED - TEST");

        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        int checkpointID = PlayerPrefs.GetInt("SavedCheckpointID", 0);

        if (checkpointID == 1 && checkpoint01 != null && player != null)
        {
            player.transform.position = checkpoint01.position;
            player.transform.rotation = checkpoint01.rotation;

            Debug.Log("PLAYER RESPAWNED AT CHECKPOINT 1");
        }
        else
        {
            Debug.Log("NO SAVED CHECKPOINT");
        }
    }
}