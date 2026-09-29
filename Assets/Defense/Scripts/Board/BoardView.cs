using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReverseTD.Defense
{
    public class BoardView : MonoBehaviour
    {
        private const int RangeCircleSegments = 48;

        [SerializeField, Tooltip("The stage to build: the single source of truth for the path and the towers.")]
        private StageLayout layout;

        private readonly List<Tower> towers = new List<Tower>();
        private StagePath path;

        public static event Action<BoardView> AllTowersDestroyed;

        public StageLayout Layout => layout;

        public StagePath Path
        {
            get
            {
                if (path == null)
                {
                    if (layout == null)
                    {
                        throw new InvalidOperationException("BoardView has no StageLayout, so it has no path.");
                    }
                    path = new StagePath(layout.Waypoints);
                }
                return path;
            }
        }

        public IReadOnlyList<Tower> Towers => towers;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            AllTowersDestroyed = null;
        }

        internal void HandleTowerDestroyed(Tower tower)
        {
            if (towers.Remove(tower) && towers.Count == 0)
            {
                AllTowersDestroyed?.Invoke(this);
            }
        }

        private void Awake()
        {
            if (layout == null)
            {
                Debug.LogError("BoardView has no StageLayout, so there's no board to build.", this);
                return;
            }

            BuildPath();
            BuildTowers();
        }

        private void BuildPath()
        {
            Transform root = CreateChild("Path");
            IReadOnlyList<Vector2> points = layout.Waypoints;
            for (int i = 1; i < points.Count; i++)
            {
                Vector2 from = points[i - 1];
                Vector2 to = points[i];
                Vector2 delta = to - from;
                float length = delta.magnitude;
                if (length <= 0f)
                {
                    continue;
                }

                var segment = new GameObject($"Segment {i - 1}");
                segment.transform.SetParent(root, false);
                segment.transform.SetPositionAndRotation(
                    (from + to) * 0.5f,
                    Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg));
                segment.transform.localScale = new Vector3(length + layout.PathWidth, layout.PathWidth, 1f);

                var segmentRenderer = segment.AddComponent<SpriteRenderer>();
                segmentRenderer.sprite = ShapeSprites.Square;
                segmentRenderer.color = layout.PathColor;
                segmentRenderer.sortingOrder = SortingOrders.Path;
            }
        }

        private void BuildTowers()
        {
            Transform root = CreateChild("Towers");
            IReadOnlyList<TowerSlot> slots = layout.TowerSlots;
            for (int i = 0; i < slots.Count; i++)
            {
                TowerSlot slot = slots[i];
                if (slot.Definition == null)
                {
                    Debug.LogWarning($"Tower slot {i} in {layout.name} has no TowerDefinition, so it was skipped.", layout);
                    continue;
                }

                var towerObject = new GameObject($"Tower {i}");
                towerObject.SetActive(false);
                towerObject.transform.SetParent(root, false);
                towerObject.transform.position = slot.Position;

                var tower = towerObject.AddComponent<Tower>();
                tower.Initialize(slot.Definition, this);
                towerObject.SetActive(true);
                towers.Add(tower);
            }
        }

        private Transform CreateChild(string childName)
        {
            Transform child = new GameObject(childName).transform;
            child.SetParent(transform, false);
            return child;
        }

        private void OnDrawGizmos()
        {
            if (layout == null)
            {
                return;
            }

            IReadOnlyList<Vector2> points = layout.Waypoints;
            if (points.Count >= 2)
            {
                Gizmos.color = layout.PathColor;
                for (int i = 1; i < points.Count; i++)
                {
                    Gizmos.DrawLine(points[i - 1], points[i]);
                }

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(points[0], 0.2f);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(points[points.Count - 1], 0.2f);
            }

            IReadOnlyList<TowerSlot> slots = layout.TowerSlots;
            for (int i = 0; i < slots.Count; i++)
            {
                TowerDefinition definition = slots[i].Definition;
                if (definition == null)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireCube(slots[i].Position, new Vector3(1f, 1f, 0f));
                    continue;
                }

                Color color = definition.Color;
                Gizmos.color = color;
                Gizmos.DrawWireCube(slots[i].Position, new Vector3(definition.Size, definition.Size, 0f));
                Gizmos.color = new Color(color.r, color.g, color.b, 0.5f);
                DrawCircle(slots[i].Position, definition.Range);
            }
        }

        private static void DrawCircle(Vector2 center, float radius)
        {
            Vector2 previous = center + new Vector2(radius, 0f);
            for (int i = 1; i <= RangeCircleSegments; i++)
            {
                float angle = i * 2f * Mathf.PI / RangeCircleSegments;
                Vector2 next = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                Gizmos.DrawLine(previous, next);
                previous = next;
            }
        }
    }
}
