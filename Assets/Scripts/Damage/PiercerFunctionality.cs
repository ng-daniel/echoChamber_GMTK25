using UnityEngine;

using Tools;
using VisualKits;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class PiercerFunctionality : MonoBehaviour
{
    const int MAX_LENGTH = 100;
    Rigidbody2D rb;
    [SerializeField] float speed;
    DamageData damageData;
    Vector2 direction;
    float deathTimer = 5f;
    [SerializeField] GameObject particle;
    LayerMask optionalCollisionIgnores = 0;
    VisualKitManager visKit;
    WeaponRailgunStats railgunStats;
    float trailtimer = 0.005f;
    TrailRenderer mytrail;
    public void Initialize(
        Vector2 direction,
        float angle,
        WeaponRailgunStats stats,
        DamageData damage,
        ToolUserConfig config = null)
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        this.rb.rotation = angle;
        this.direction = direction;
        this.damageData = damage;

        visKit = GetComponentInChildren<VisualKitManager>();
        VisualKit myKit = visKit.SelectKit(damage.GetAttacker().tag);
        railgunStats = stats;

        mytrail = myKit.visObj.GetComponent<TrailRenderer>();
        mytrail.enabled = false;

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
        else if (mytrail.enabled == false)
        {
            mytrail.enabled = true;
        }

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
        // if (collision.gameObject.tag == this.gameObject.tag)
        // {
        //     return;
        // }
        // bool hitResult = TryHitTarget(collision.gameObject);
        // if (hitResult) FizzleOut();

        if (((1 << collision.gameObject.layer) & LayerMask.GetMask("Wall")) != 0)
        {
            PreFizzle();
        }
    }
    public void ActivateDamageRaycast(Vector2 direction)
    {
        gameObject.transform.parent = null;
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, direction, MAX_LENGTH, LayerMask.GetMask("Wall"));
        float dist = CalculateDistance();
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, railgunStats.railRadius, direction, dist);
        foreach (RaycastHit2D hit in hits)
        {
            if (this.gameObject.name.Equals(hit.collider.gameObject.name))
            {
                continue;
            }
            bool result = TryHitTarget(hit.collider.gameObject);
            print("AHHHH: " + hit.collider.gameObject.name + " -> " + result);
        }
    }

    float CalculateDistance()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, direction, MAX_LENGTH, LayerMask.GetMask("Wall"));
        float dist = MAX_LENGTH;
        if (hitInfo.collider != null)
        {
            dist = ((Vector2)(hitInfo.collider.gameObject.transform.position - this.gameObject.transform.position)).magnitude; // distance between raycast hit and current pos
        }
        return dist;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, direction.normalized * CalculateDistance());
    }

    bool TryHitTarget(GameObject target)
    {
        if (target == damageData.GetAttacker())
            return false;
        if (target.layer == damageData.GetAttackerLayer())
            return false;
        if (optionalCollisionIgnores != 0 &&
            (((1 << target.layer) & optionalCollisionIgnores.value) != 0)) // is there a layermask override - if so does it apply to the current target?
            return false;

        Health hp = target.GetComponent<Health>();
        if (hp != null)
            hp.Damage(new(damageData));

        return true;
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
}
