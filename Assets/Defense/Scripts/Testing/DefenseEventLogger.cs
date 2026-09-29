using UnityEngine;

namespace ReverseTD.Defense.Testing
{
    public class DefenseEventLogger : MonoBehaviour
    {
        private void OnEnable()
        {
            Tower.Destroyed += LogTowerDestroyed;
            BoardView.AllTowersDestroyed += LogAllTowersDestroyed;
        }

        private void OnDisable()
        {
            Tower.Destroyed -= LogTowerDestroyed;
            BoardView.AllTowersDestroyed -= LogAllTowersDestroyed;
        }

        private static void LogTowerDestroyed(Tower tower)
        {
            Debug.Log($"Tower destroyed: {tower.name} at {(Vector2)tower.transform.position}");
        }

        private static void LogAllTowersDestroyed(BoardView board)
        {
            Debug.Log($"All towers destroyed on {board.name}.");
        }
    }
}
