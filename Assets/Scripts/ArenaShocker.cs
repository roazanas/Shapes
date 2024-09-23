using System.Collections.Generic;
using UnityEngine;

public class ArenaShocker : MonoBehaviour
{
    private CircleCollider2D trigger;
    private List<HealthDamageController> entitiesOutArena = new();

    private void Start()
    {
        trigger = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (trigger == null || collision == null) { return; }
        if (!collision.CompareTag("Player") && !collision.CompareTag("Enemy")) { return; }

        HealthDamageController entity = collision.GetComponent<HealthDamageController>();
        if (entity != null)
        {
            entitiesOutArena.Remove(entity);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (trigger == null || collision == null) { return; }
        if (!collision.CompareTag("Player") && !collision.CompareTag("Enemy")) { return; }

        HealthDamageController entity = collision.GetComponent<HealthDamageController>();
        if (entity != null)
        {
            entitiesOutArena.Add(entity);
            Vector2 direction = -(Vector2)collision.transform.position.normalized * 2f;
            entity.GetShocked(direction, 3f);
        }
    }

    private void Update()
    {
        for (int i = entitiesOutArena.Count - 1; i >= 0; i--)
        {
            HealthDamageController entity = entitiesOutArena[i];

            if (entity == null || !entity.gameObject.activeInHierarchy)
            {
                entitiesOutArena.RemoveAt(i);
                continue;
            }

            if (!trigger.IsTouching(entity.GetComponent<Collider2D>()))
            {
                entity.GetShocked(-(Vector2)entity.transform.position.normalized * 8f, 12f);
            }
        }
    }
}
