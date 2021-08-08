using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector3 m_velocity = Vector3.zero;

    [SerializeField] protected int m_health;
    [SerializeField] protected float m_moveSpeed;
    [SerializeField] protected int m_attackDamage;

    [SerializeField] protected GameObject attackPoint;
    [SerializeField] protected float m_attackRadius;
    [SerializeField] protected float m_attackDistance;

    [SerializeField] protected float m_detectRadius;
    [SerializeField] protected float m_followRadius;

    [SerializeField] protected LayerMask playerLayerMask;

    private Animator animator;
    private Rigidbody2D rigidbody;

    private GameObject player;

    private bool m_isDetected;

    virtual public void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        m_isDetected = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));
    }

    virtual public void Update()
    {
        if (m_isDetected)
        {
            LookAtPlayer();
        }
    }

    virtual public void FixedUpdate()
    {
        m_isDetected = IsDetected();

        if (m_isDetected || _Follow())
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                if (Vector2.Distance(player.transform.position, gameObject.transform.position) > m_attackDistance)
                {
                    Move();
                }
                else
                {
                    animator.Play("Attack");
                }
            }
        }
        else
        {
            animator.Play("Idle");
        }
    }

    virtual public void Move()
    {
        float move = m_moveSpeed * Time.deltaTime * transform.localScale.x * 10;

        Vector3 targetVelocity = new Vector2(move, rigidbody.velocity.y);
        rigidbody.velocity = targetVelocity;

        animator.Play("Run");
    }

    virtual public void Attack()
    {
        Collider2D[] overlapedColliders = Physics2D.OverlapCircleAll(attackPoint.transform.position, m_attackRadius, playerLayerMask);
        foreach (var collider in overlapedColliders)
        {
            if (collider.tag == "Player")
            {
                //player.GetComponent<Player>().TakeHit(m_attackDamage);
                return;
            }
        }
    }

    virtual public bool _Follow()
    {
        if (player != null)
        {
            if (Vector2.Distance(transform.position, player.transform.position) < m_followRadius)
                return true;
        }

        return false;
    }

    virtual public bool IsDetected()
    {
        Collider2D[] overlapedColliders = Physics2D.OverlapCircleAll(transform.position, m_detectRadius, playerLayerMask);
        foreach (var collider in overlapedColliders)
        {
            if (collider.tag == "Player")
            {
                player = collider.gameObject;
                return true;
            }
        }

        return false;
    }

    virtual public void LookAtPlayer()
    {
        Vector3 scale = transform.localScale;
        if (player.transform.position.x < transform.position.x)
            scale.x = -1f;
        else
            scale.x = 1f;

        transform.localScale = scale;
    }

    public void TakeDamage(int damage)
    {
        m_health -= damage;

        if (m_health <= damage)
            animator.Play("Hit");
        else
            animator.Play("Death");
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.tag == "Player")
            Debug.Log("Nice");
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, m_attackRadius);
        Gizmos.DrawWireSphere(transform.position, m_detectRadius);
        Gizmos.DrawWireSphere(transform.position, m_followRadius);
    }
}
