using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    private float timeBtwAttack;
    public float startTimeBtwAttack;


    [SerializeField] private Transform hit;
    [SerializeField] private LayerMask enemy;
    [SerializeField] private int damage = 1;
    [SerializeField] private const float hitRadius = 0.1f;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(timeBtwAttack <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
            }
            timeBtwAttack = startTimeBtwAttack;
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }
    }

    private void Attack()
    {
        anim.SetTrigger("Attack");

        Collider2D[] enemies = Physics2D.OverlapCircleAll(hit.position, hitRadius, enemy);

        foreach (Collider2D enemy in enemies)
        {
            //enemy.GetComponent<Enemy>().TakeDamage(damage);
            Debug.Log("DAmaged");
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(hit.position, hitRadius);
    }
}
