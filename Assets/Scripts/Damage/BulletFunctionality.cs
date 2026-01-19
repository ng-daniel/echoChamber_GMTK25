using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Tools;
using UnityEngine;

public class BulletFunctionality : MonoBehaviour
{

    Rigidbody2D rb;

    float speed;
    int damage;
    DamageData damageData;
    [SerializeField] string damageDataContext;
    Vector2 direction;
    float deathTimer = 5f;
    [SerializeField] GameObject particle;
    VisualKitManager visKit;

    public void Initialize(Vector2 direction, float angle, float speed, DamageData damage, ToolUserConfig config = null)
    {
        rb = GetComponent<Rigidbody2D>();
        this.rb.rotation = angle;
        this.direction = direction;
        this.speed = speed;

        this.damageData = damage;
        damageData.SetContext(damageDataContext);

        visKit = GetComponentInChildren<VisualKitManager>();
        if (visKit)
        {
            visKit.SelectKit(damage.GetAttacker().tag);
        }
    }

    void Update()
    {
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
        if (collision.gameObject.tag == this.gameObject.tag)
        {
            return;
        }
        bool hitResult = TryHitTarget(collision.gameObject);
        if (hitResult) FizzleOut();
    }
    bool TryHitTarget(GameObject target)
    {
        if (target == damageData.GetAttacker())
            return false;
        if (target.layer == damageData.GetAttackerLayer())
            return false;

        Health hp = target.GetComponent<Health>();
        if (hp != null)
            hp.Damage(new(damageData));

        return true;
    }
    void FizzleOut()
    {
        print("HIT SOMETHING");
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
