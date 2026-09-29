using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReverseTD.Defense
{
    public sealed class StagePath
    {
        private readonly Vector2[] waypoints;
        private readonly float[] distanceAtWaypoint;

        public StagePath(IReadOnlyList<Vector2> points)
        {
            if (points == null || points.Count < 2)
            {
                throw new ArgumentException("A stage path needs at least 2 waypoints.", nameof(points));
            }

            waypoints = new Vector2[points.Count];
            distanceAtWaypoint = new float[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                waypoints[i] = points[i];
                if (i > 0)
                {
                    distanceAtWaypoint[i] = distanceAtWaypoint[i - 1] + Vector2.Distance(waypoints[i - 1], waypoints[i]);
                }
            }

            TotalLength = distanceAtWaypoint[distanceAtWaypoint.Length - 1];
        }

        public IReadOnlyList<Vector2> Waypoints => waypoints;

        public float TotalLength { get; }

        public Vector2 Start => waypoints[0];

        public Vector2 End => waypoints[waypoints.Length - 1];

        public Vector2 GetPointAtDistance(float distance)
        {
            if (distance <= 0f)
            {
                return Start;
            }

            for (int i = 1; i < waypoints.Length; i++)
            {
                if (distance <= distanceAtWaypoint[i])
                {
                    float segmentLength = distanceAtWaypoint[i] - distanceAtWaypoint[i - 1];
                    if (segmentLength <= 0f)
                    {
                        return waypoints[i];
                    }

                    float t = (distance - distanceAtWaypoint[i - 1]) / segmentLength;
                    return Vector2.Lerp(waypoints[i - 1], waypoints[i], t);
                }
            }

            return End;
        }
    }
}
