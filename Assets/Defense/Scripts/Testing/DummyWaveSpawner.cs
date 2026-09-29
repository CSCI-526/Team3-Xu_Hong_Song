using System.Collections;
using UnityEngine;

namespace ReverseTD.Defense.Testing
{
    public class DummyWaveSpawner : MonoBehaviour
    {
        [SerializeField, Tooltip("The board whose path the dummies walk.")]
        private BoardView board;

        [SerializeField, Min(1), Tooltip("Dummies per wave.")]
        private int count = 10;

        [SerializeField, Min(0f), Tooltip("Seconds between dummies.")]
        private float interval = 1f;

        [SerializeField, Min(1f), Tooltip("Hit points of each dummy.")]
        private float soldierHp = 60f;

        [SerializeField, Min(0.1f), Tooltip("Walking speed of each dummy, in world units per second.")]
        private float soldierSpeed = 1.5f;

        [SerializeField, Min(0f), Tooltip("Damage each dummy deals per attack on a tower. Dummies attack once per second.")]
        private float soldierDamage = 10f;

        [SerializeField, Min(0f), Tooltip("How close a tower must be for a dummy to attack it, in world units.")]
        private float soldierAttackRange = 2.5f;

        [SerializeField, Range(0.1f, 5f), Tooltip("Game speed while testing. 1 is normal speed.")]
        private float timeScale = 1f;

        private int spawnedCount;

        private void Start()
        {
            SpawnWave();
        }

        private void Update()
        {
            Time.timeScale = timeScale;
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
        }

        [ContextMenu("Spawn Wave")]
        private void SpawnWave()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Spawn Wave only works in Play mode.", this);
                return;
            }

            if (board == null)
            {
                Debug.LogError("DummyWaveSpawner has no board to spawn on.", this);
                return;
            }

            StartCoroutine(SpawnDummies());
        }

        private IEnumerator SpawnDummies()
        {
            for (int i = 0; i < count; i++)
            {
                SpawnDummy();
                yield return new WaitForSeconds(interval);
            }
        }

        private void SpawnDummy()
        {
            var dummyObject = new GameObject($"Dummy {spawnedCount++}");
            dummyObject.SetActive(false);
            dummyObject.transform.SetParent(transform, false);
            dummyObject.AddComponent<DummySoldier>().Initialize(board.Path, soldierHp, soldierSpeed, soldierDamage, soldierAttackRange);
            dummyObject.SetActive(true);
        }
    }
}
