using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyAI : MonoBehaviour
{
    public enum States { Idle = 0, Move, Attack, Death }

    #region States
    private IdleState idleState;
    private MoveState moveState;
    private AttackState attackState;
    #endregion

    [SerializeField] private float m_health;
    [SerializeField] private float m_moveSpeed;

    [SerializeField] private float m_detectRange;
    [SerializeField] private Transform m_attackPoint;
    [SerializeField] private float m_attackRange;
    [SerializeField] private float m_attackDistance;

    [SerializeField] private float m_followRadius;

    [SerializeField] private LayerMask playersLayer;

    private Player player;

    StateMachine _stateMachine;
    Animator animator;
    Rigidbody2D rigidbody;

    #region MonoBehavior

    public void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();

        _stateMachine = GetComponent<StateMachine>();

        idleState = new IdleState(this, _stateMachine);
        moveState = new MoveState(this, _stateMachine);
        attackState = new AttackState(this, _stateMachine);

        _stateMachine.Initialize(idleState);
    }

    public void Update()
    {
        if (m_health > 0)
        {
            _stateMachine.CurrentState.HandleInput();
            _stateMachine.CurrentState.LogicUpdate();
        }
    }

    public void FixedUpdate()
    {
        if (m_health > 0)
            _stateMachine.CurrentState.PhysicsUpdate();
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.name);
        if (collider.tag == "Player")
            Debug.Log("Nice");
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(m_attackPoint.transform.position, m_attackRange);
        Gizmos.DrawWireSphere(transform.position, m_detectRange);
        Gizmos.DrawWireSphere(transform.position, m_followRadius);
    }

    #endregion

    #region Methods

    public void SetState(States state)
    {
        switch (state)
        {
            case States.Idle:
                _stateMachine.ChangeState(idleState);
                break;
            case States.Move:
                _stateMachine.ChangeState(moveState);
                break;
            case States.Attack:
                _stateMachine.ChangeState(attackState);
                break;
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
        Collider2D[] overlapedColliders = Physics2D.OverlapCircleAll(m_attackPoint.transform.position, m_attackRange, playersLayer);
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
        Collider2D[] overlapedColliders = Physics2D.OverlapCircleAll(transform.position, m_detectRange, playersLayer);
        foreach (var collider in overlapedColliders)
        {
            if (collider.tag == "Player")
            {
                player = collider.gameObject.GetComponent<Player>();
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

    virtual public void TakeDamage(int damage)
    {
        m_health -= damage;

        if (m_health <= damage)
            animator.Play("Hit");
        else
            animator.Play("Death");
    }

    public bool CanAttack()
    {
        if (Vector2.Distance(player.transform.position, gameObject.transform.position) < m_attackDistance)
            return true;

        return false;
    }

    public void PlayAnimation(string name)
    {
        animator.Play(name);
    }

    public bool IsAnimationPlaying(string name, int layer = 0)
    {
        if (animator.GetCurrentAnimatorStateInfo(layer).IsName(name) && animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1.0f)
            return true;

        return false;
    }

    public void StopMoving()
    {
        rigidbody.velocity *= Vector2.up;
    }

    #endregion 
}