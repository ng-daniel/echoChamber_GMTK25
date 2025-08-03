using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BulletFunctionality : MonoBehaviour
{

    Rigidbody2D rb;

    float speed;
    [SerializeField] float rotationSpeed;
    int damage;
    DamageData damageData;
    [SerializeField] string damageDataContext;
    Vector2 direction;
    float deathTimer = 5f;
    [SerializeField] GameObject particle;

    public void Initialize(Vector2 direction, float angle, float speed, DamageData damage)
    {
        rb = GetComponent<Rigidbody2D>();
        this.direction = direction;
        this.damageData = damage;
        damageData.SetContext(damageDataContext);
        this.speed = speed;
        this.rb.rotation = angle;
    }

    void Update()
    {
        rb.velocity = direction.normalized * speed;

        rb.rotation += rotationSpeed * Time.deltaTime;
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
        if (target == damageData.GetAttacker()) return false;
        Health hp = target.GetComponent<Health>();
        if (hp == null) return true;
        print(damageData.GetDamage());
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
