using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFunctionality : MonoBehaviour
{

    Rigidbody2D rb;

    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    Vector2 direction;
    LayerMask targetLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction, LayerMask targetLayer)
    {
        this.direction = direction;
        this.targetLayer = targetLayer;
    }

    void Update()
    {
        rb.velocity = direction.normalized * speed;

        rb.rotation += rotationSpeed * Time.deltaTime;
        rb.rotation = rb.rotation >= 360 ? 0 : rb.rotation;
        rb.rotation = rb.rotation < 0 ? 360 : rb.rotation;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // check if collision obj's layer is part of target layermask via bitwise operators
        if (0 != (targetLayer & (1 << collision.gameObject.layer)))
        {
            print("hit target!");
            DamageTarget(collision.gameObject);
            FizzleOut();
        }
    }
    void DamageTarget(GameObject target)
    {

    }
    void FizzleOut()
    {
        Destroy(this.gameObject);
    }
}
