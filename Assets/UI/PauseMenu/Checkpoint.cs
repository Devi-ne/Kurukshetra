using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetInt("SavedCheckpointID", checkpointID);
            PlayerPrefs.Save();

            Debug.Log("CHECKPOINT REACHED: " + checkpointID);
        }
    }
}