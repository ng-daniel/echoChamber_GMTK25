using UnityEngine;

using Tools;
using VisualKits;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class PiercerFunctionality : MonoBehaviour, IDamaging
{
    const int MAX_LENGTH = 500;
    Rigidbody2D rb;
    [SerializeField] float speed;
    DamageData damageData;
    Vector2 direction;
    float deathTimer = 5f;
    [SerializeField] GameObject particle;
    [SerializeField] LayerMask rayStoppingLayers;
    LayerMask optionalCollisionIgnores = 0;
    VisualKitManager visKit;
    WeaponRailgunStats railgunStats;
    float trailtimer = 0.005f;
    TrailRenderer mytrail;
    Vector2 startingPosition;
    bool initialized = false;
    public void Initialize(
        Vector2 direction,
        float angle,
        WeaponRailgunStats stats,
        DamageData damage,
        ToolUserConfig config = null)
    {
        startingPosition = this.gameObject.transform.position;

        rb = this.gameObject.GetComponent<Rigidbody2D>();
        this.rb.rotation = angle;
        this.direction = direction;
        this.damageData = damage;

        visKit = GetComponentInChildren<VisualKitManager>();
        VisualKit myKit = visKit.SelectKit(damage.GetAttacker().tag);
        railgunStats = stats;

        mytrail = myKit.visObj.GetComponent<TrailRenderer>();
        mytrail.enabled = false;

        initialized = true;
        ActivateDamageRaycast(direction);
    }
    public void OptionalCollisionIgnores(LayerMask layers)
    {
        this.optionalCollisionIgnores = layers;
    }
    void Update()
    {
        if (trailtimer > 0)
        {
            trailtimer -= Time.deltaTime;
        }
        // else if (mytrail.enabled == false)
        // {
        //     mytrail.enabled = true;
        // }

        rb.linearVelocity = direction.normalized * speed;

        rb.rotation = rb.rotation >= 360 ? 0 : rb.rotation;
        rb.rotation = rb.rotation < 0 ? 360 : rb.rotation;

        deathTimer -= Time.deltaTime;
        if (deathTimer < 0)
        {
            FizzleOut();
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if ((rayStoppingLayers & (1 << collision.gameObject.layer)) != 0)
        {
            PreFizzle();
        }
    }
    public void ActivateDamageRaycast(Vector2 direction)
    {
        // RaycastHit2D hitInfo = Physics2D.Raycast(startingPosition, direction, MAX_LENGTH, LayerMask.GetMask("Wall"));
        float dist = CalculateDistance();
        RaycastHit2D[] hits = Physics2D.CircleCastAll(startingPosition, railgunStats.railRadius, direction, dist);
        foreach (RaycastHit2D hit in hits)
        {
            if (this.gameObject.name.Equals(hit.collider.gameObject.name))
            {
                continue;
            }
            if (this.gameObject.CompareTag(hit.collider.gameObject.tag))
            {
                Health otherHealth = hit.collider.gameObject.GetComponent<Health>();
                if (otherHealth != null)
                {
                    otherHealth.KillNoRegard();
                }
            }
            bool result = TryHitTarget(hit.collider.gameObject);
            print("AHHHH: " + hit.collider.gameObject.name + " -> " + result);
        }
    }

    float CalculateDistance()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(startingPosition, direction, MAX_LENGTH, LayerMask.GetMask("Wall"));
        float dist = MAX_LENGTH;
        if (hitInfo.collider != null)
        {
            dist = (hitInfo.point - startingPosition).magnitude; // distance between raycast hit and current pos
        }
        return dist;
    }

    public void OnDrawGizmos()
    {
        float dist = CalculateDistance();
        Gizmos.color = Color.red;
        Gizmos.DrawRay(startingPosition, direction.normalized * dist);
        Gizmos.DrawSphere(startingPosition + (direction.normalized * dist), railgunStats.railRadius);
    }

    bool TryHitTarget(GameObject target)
    {
        return IDamaging.TryHitTarget(target, damageData, optionalCollisionIgnores);
    }
    void PreFizzle()
    {
        rb.linearVelocity = Vector2.zero;
    }
    void FizzleOut()
    {
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
    public DamageData GetDamageData()
    {
        return damageData;
    }
}
