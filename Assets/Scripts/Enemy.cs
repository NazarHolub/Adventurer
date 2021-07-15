using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector3 m_velocity = Vector3.zero;

    [SerializeField] protected float m_moveSpeed;
    [SerializeField] protected int m_attackDamage;
    [SerializeField] protected int m_health;

    [SerializeField] protected GameObject attackPoint;
    [SerializeField] protected float m_attackRadius;
    [SerializeField] protected float m_attackDistance;

    [SerializeField] protected float m_detectRadius;
    [SerializeField] protected float m_followRadius;

    private Animator animator;
    private Rigidbody2D rigidbody;

    private GameObject playerTarget;

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
        if (!m_isDetected || IsFollowing())
            m_isDetected = IsDetected();

        if (m_isDetected)
        {
            LookAtPlayer();
        }
    }

    virtual public void FixedUpdate()
    {
        if (m_isDetected)
        {
            if (Vector2.Distance(playerTarget.transform.position, gameObject.transform.position) > m_attackDistance && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                Move();
            else if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                animator.Play("Attack");
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
        Collider2D[] overlaped = Physics2D.OverlapCircleAll(attackPoint.transform.position, m_attackRadius);
        foreach (var player in overlaped)
        {
            if (player.tag == "Player")
            {
                //player.GetComponent<Player>().TakeHit(m_attackDamage);
                return;
            }
        }
    }

    virtual public bool IsFollowing()
    {
        Collider2D[] overlaped = Physics2D.OverlapCircleAll(transform.position, m_followRadius);
        foreach (var player in overlaped)
        {
            if (player.tag == "Player")
            {
                playerTarget = player.gameObject;
                return false;
            }
        }

        animator.Play("Idle");
        return true;
    }

    virtual public bool IsDetected()
    {
        Collider2D[] overlaped = Physics2D.OverlapCircleAll(transform.position, m_detectRadius);
        foreach (var player in overlaped)
        {
            if (player.tag == "Player")
            {
                playerTarget = player.gameObject;
                return true;
            }
        }

        return false;
    }

    virtual public void LookAtPlayer()
    {
        Vector3 scale = transform.localScale;
        if (playerTarget.transform.position.x < transform.position.x)
            scale.x = -1f;
        else
            scale.x = 1f;

        transform.localScale = scale;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, m_attackRadius);
        Gizmos.DrawWireSphere(transform.position, m_detectRadius);
        Gizmos.DrawWireSphere(transform.position, m_followRadius);
    }
}
