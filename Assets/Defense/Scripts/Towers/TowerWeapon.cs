using ReverseTD.Shared;
using UnityEngine;

namespace ReverseTD.Defense
{
    public class TowerWeapon : MonoBehaviour
    {
        private TowerDefinition definition;
        private TowerTargeting targeting;
        private float cooldown;

        public void Initialize(TowerDefinition towerDefinition, TowerTargeting towerTargeting)
        {
            definition = towerDefinition;
            targeting = towerTargeting;
        }

        private void Update()
        {
            cooldown -= Time.deltaTime;
            if (cooldown > 0f)
            {
                return;
            }

            IDamageable target = targeting.FindTarget();
            if (target == null)
            {
                cooldown = 0f;
                return;
            }

            Projectile.Launch(transform.position, target, definition.Damage, definition.ProjectileSpeed);

            cooldown += 1f / definition.FireRate;
        }
    }
}
