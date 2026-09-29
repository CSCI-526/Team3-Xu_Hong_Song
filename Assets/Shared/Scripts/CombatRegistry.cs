using System.Collections.Generic;
using UnityEngine;

namespace ReverseTD.Shared
{
    public static class CombatRegistry
    {
        private static readonly List<IDamageable> attackers = new List<IDamageable>();
        private static readonly List<IDamageable> defenders = new List<IDamageable>();

        private static readonly System.Predicate<IDamageable> isDestroyed = IsDestroyed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            attackers.Clear();
            defenders.Clear();
        }

        public static void Register(IDamageable unit)
        {
            if (unit == null)
            {
                return;
            }

            List<IDamageable> list = ListFor(unit.Team);
            if (!list.Contains(unit))
            {
                list.Add(unit);
            }
        }

        public static void Unregister(IDamageable unit)
        {
            if (unit == null)
            {
                return;
            }

            attackers.Remove(unit);
            defenders.Remove(unit);
        }

        public static void GetAlive(Team team, List<IDamageable> results)
        {
            results.Clear();

            List<IDamageable> list = ListFor(team);
            list.RemoveAll(isDestroyed);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsAlive)
                {
                    results.Add(list[i]);
                }
            }
        }

        public static bool IsAlive(IDamageable unit)
        {
            return unit != null && !IsDestroyed(unit) && unit.IsAlive;
        }

        private static List<IDamageable> ListFor(Team team)
        {
            return team == Team.Attacker ? attackers : defenders;
        }

        private static bool IsDestroyed(IDamageable unit)
        {
            return unit is Object unityObject && unityObject == null;
        }
    }
}
