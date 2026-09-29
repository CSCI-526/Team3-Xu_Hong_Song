using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAgain : MonoBehaviour
{
    [SerializeField] private OffenseData offenseData;

    public void GoToStart()
    {
        if (offenseData != null)
        {
            offenseData.ResetGame();
        }

        SceneManager.LoadScene("StartGame");
    }
}