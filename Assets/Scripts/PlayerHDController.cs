using UnityEngine;

public class PlayerHDController : HealthDamageController
{
    private BoxCollider2D damager;
    private BoxCollider2D damageGetter;

    private void Awake()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        damageGetter = colliders[0];
        damager = colliders[1];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) { return; }
        HealthDamageController enemy = collision.GetComponent<HealthDamageController>();
        if (damager.IsTouching(collision))
        {
            Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 playerPosition = (Vector2)transform.position;
            Vector2 direction = (mousePosition - playerPosition).normalized;
            enemy.TakeDamage(damageAmount, direction);
        }
        else if (damageGetter.IsTouching(collision))
        {
            TakeDamage(enemy.damageAmount, Vector2.zero);
        }
    }
}
