using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{

    LineRenderer lineRenderer;
    Rigidbody2D rb;

    [Header("Laser Config Settings")]
    [SerializeField] float maxDistance; // to completely ensure no infinite cast BS
    [SerializeField] float laserWidth;
    [SerializeField] LayerMask stopLayers;
    [SerializeField] LayerMask damageLayers;
    [SerializeField] DamageData damageData;
    [SerializeField] string context;

    public void Initialize(GameObject attacker, int damage)
    {
        this.damageData = new(attacker, damage);
        damageData.SetContext(context);
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = laserWidth;
    }

    void Start()
    {
        Initialize(this.gameObject, 1);
    }

    void Update()
    {
        FireLaser(Vector2.right + Vector2.down);
    }

    void FireLaser(Vector2 aimDirection)
    {
        // raycast to gague distance
        RaycastHit2D hitRay = Physics2D.Raycast(this.transform.position, aimDirection, maxDistance, stopLayers);
        Vector2 hitPosition = hitRay ? hitRay.point : aimDirection * maxDistance;

        // circlecast to collide and damage
        RaycastHit2D[] hitCircle = Physics2D.CircleCastAll(this.transform.position, laserWidth / 2, aimDirection, maxDistance, damageLayers);
        foreach (RaycastHit2D hit in hitCircle)
        {
            Health objectHealth = hit.collider.gameObject.GetComponent<Health>();
            if (objectHealth == null) continue;

            print("object: " + objectHealth.gameObject);
            objectHealth.Damage(damageData);
        }

        // linerenderer to show cool effects
        lineRenderer.SetPosition(0, this.transform.position);
        lineRenderer.SetPosition(1, hitPosition);
    }
}
