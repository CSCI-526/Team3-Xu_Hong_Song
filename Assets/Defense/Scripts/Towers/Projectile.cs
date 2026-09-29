using ReverseTD.Shared;
using UnityEngine;

namespace ReverseTD.Defense
{
    public class Projectile : MonoBehaviour
    {
        private const float Size = 0.2f;
        private const float HitDistance = 0.05f;
        private const float MaxLifetime = 5f;

        private static readonly Color projectileColor = new Color(0.6f, 0.95f, 1f);

        private IDamageable target;
        private Vector3 aimPoint;
        private float damage;
        private float speed;
        private float age;

        public static Projectile Launch(Vector3 origin, IDamageable target, float damage, float speed)
        {
            var projectileObject = new GameObject("Projectile");
            projectileObject.transform.position = origin;
            projectileObject.transform.localScale = new Vector3(Size, Size, 1f);

            var spriteRenderer = projectileObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = ShapeSprites.Circle;
            spriteRenderer.color = projectileColor;
            spriteRenderer.sortingOrder = SortingOrders.Projectile;

            var projectile = projectileObject.AddComponent<Projectile>();
            projectile.target = target;
            projectile.aimPoint = target.Position;
            projectile.damage = damage;
            projectile.speed = speed;
            return projectile;
        }

        private void Update()
        {
            bool targetAlive = CombatRegistry.IsAlive(target);
            if (targetAlive)
            {
                aimPoint = target.Position;
            }

            transform.position = Vector3.MoveTowards(transform.position, aimPoint, speed * Time.deltaTime);
            if ((transform.position - aimPoint).sqrMagnitude <= HitDistance * HitDistance)
            {
                if (targetAlive)
                {
                    target.TakeDamage(damage);
                }
                Destroy(gameObject);
                return;
            }

            age += Time.deltaTime;
            if (age >= MaxLifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
