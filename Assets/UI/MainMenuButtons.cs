using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void PlayGame()
    {
        // Start a completely new game
        PlayerPrefs.DeleteKey("SavedCheckpointID");
        PlayerPrefs.Save();

        Debug.Log("NEW GAME STARTED");

        SceneManager.LoadScene("SampleScene");
    }

    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("SavedCheckpointID"))
        {
            int checkpointID = PlayerPrefs.GetInt("SavedCheckpointID");

            Debug.Log("CONTINUING FROM CHECKPOINT: " + checkpointID);

            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Debug.Log("NO PREVIOUS DATA - PLEASE START A NEW GAME");
        }
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}