using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EnemyType
    {
        Melee,
        Ranged,
        Melee2
    }

public class Enemy_Zombie : MonoBehaviour
{
    private enum EnemyState 
    { 
        Idle,
        Patrol,
        Chase, 
        Attack 
    }

    private EnemyState currentState = EnemyState.Idle;
    [SerializeField] private EnemyType currentType = EnemyType.Melee;

    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;

    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;

    [SerializeField] private float sightRange = 10f;
    [SerializeField] private float attackRange = 2f;

    [SerializeField] private float damage = 20f;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float attackCooldown = 2f;
    private bool isAttacking = false;

    private bool isAlive = true;
    private bool isChasing = false;

    private Transform player;
    private Vector3 targetLocation;
    
    private NavMeshAgent navAgent;
    private SkinnedMeshRenderer render;
    private Animator animator;

    [SerializeField] private GameObject hpBarCanvas;
    [SerializeField] private GameObject hpBarPref;
    private GameObject hpBar;
    private EnemyHpBar hpBarScript;

    private Coroutine damageCoroutine = null;
    public UnityEvent OnDie;

    private AudioSource audioSource;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;

        navAgent = GetComponent<NavMeshAgent>();
        Transform modelBody = transform.GetChild(0).GetChild(2);
        render = modelBody.GetComponent<SkinnedMeshRenderer>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentHP = maxHP;
    }

    private void Start()
    {
        targetLocation = GetRandomLocation();
        navAgent.SetDestination(targetLocation);

        hpBar = Instantiate(hpBarPref);
        hpBarScript = hpBar.GetComponentInChildren<EnemyHpBar>();

        hpBar.transform.SetParent(hpBarCanvas.transform);
        hpBarScript.target = transform;
        hpBarScript.SetMaxValue(maxHP);
        hpBarScript.SetValue(currentHP);
        hpBar.SetActive(false);

        if (currentType == EnemyType.Melee2)
        {
            hpBarScript.isBoss = true;
        }
    }

    private void OnEnable()
    {
        currentHP = maxHP;
        isAlive = true;
        ChangeState(EnemyState.Patrol);
    }

    private void OnDisable()
    {
        ChangeState(EnemyState.Idle);
    }

    private void FixedUpdate()
    {
        if (!isAlive) return;

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Bullet"))
        {
            TakeDamage(player.GetComponent<Player>().GetGunDamage());
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;

        isChasing = true;
        ChangeState(EnemyState.Chase);

        currentHP -= damage;
        hpBarScript.SetValue(currentHP);

        if (damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(OnDamage());
        }

        hpBar.SetActive(true);

        if (currentHP <= 0)
        {
            currentHP = 0;
            isAlive = false;
            Die();
        }

    }

    private void Die()
    {
        animator.SetTrigger("Die");

        navAgent.SetDestination(transform.position);

        OnDie.Invoke();

        Destroy(hpBar, 1f);
        Destroy(gameObject, 1.2f);
    }

    private Vector3 GetRandomLocation()
    {
        float radius = 5f;
        return transform.position + new Vector3(Random.Range(-radius, radius), 0f, Random.Range(-radius, radius));
    }

    private void Patrol()
    {
        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            targetLocation = GetRandomLocation();
            navAgent.SetDestination(targetLocation);
        }

        if (IsPlayerInSight())
        {
            ChangeState(EnemyState.Chase);
            animator.SetBool("ChaseState", true);
        }
    }

    private void Chase()
    {
        navAgent.SetDestination(player.position);

        // 공격 범위에 들어왔을 경우
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            ChangeState(EnemyState.Attack);
            animator.SetBool("isAttacking", true);
        }
        // 공격 받으면 계속 추격
        else if (isChasing)
        {
            return;
        }
        // 시야에서 벗어날 경우
        else if (!IsPlayerInSight())
        {
            ChangeState(EnemyState.Patrol);
            animator.SetBool("ChaseState", false);
        }
    }

    private void Attack()
    {
        transform.LookAt(player);
        
        if (isAttacking == false)
        {
            isAttacking = true;
            // Debug.Log("Zombie: Attack !");

            if (currentType == EnemyType.Melee)
            {
                StartCoroutine(OnMeleeAttack());
            }
            else if (currentType == EnemyType.Ranged)
            {
                StartCoroutine(OnRangedAttack());
            }
            else if (currentType == EnemyType.Melee2)
            {
                StartCoroutine(OnMeleeAttack());
            }
        }
    }

    private IEnumerator OnMeleeAttack()
    {
        yield return new WaitForSeconds(attackDelay);

        if (isAlive && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            // Debug.Log("Zombie: Hit !");
            player.GetComponent<Player>().TakeDamage(damage);
        }
        // else
        // {
        //     Debug.Log("Zombie: 안맞음 !");
        // }

        yield return new WaitForSeconds(attackCooldown - attackDelay);
        ChangeState(EnemyState.Chase);
        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    private IEnumerator OnRangedAttack()
    {
        yield return new WaitForSeconds(attackDelay);
        // 총알 발사 구현
        if (bullet != null)
        {
            Instantiate(bullet, firePoint.position, firePoint.rotation);
        }

        yield return new WaitForSeconds(attackCooldown - attackDelay);
        ChangeState(EnemyState.Chase);
        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    private IEnumerator OnDamage()
    {
        Color curColor = render.material.color;
        render.material.color = Color.red;
        audioSource.Play();
        
        navAgent.SetDestination(transform.position);
        
        yield return new WaitForSeconds(0.3f);

        render.material.color = curColor;

        damageCoroutine = null;
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        
        switch (newState)
        {
            case EnemyState.Patrol:
                navAgent.speed = 1f;
                break;
            case EnemyState.Chase:
                navAgent.speed = 2.5f;
                break;
            case EnemyState.Attack:
                navAgent.speed = 0f;
                if (currentType == EnemyType.Melee2)
                {
                    navAgent.speed = 2f;
                } 
                break;
        }
    }

    private bool IsPlayerInSight()
    {
        if (!(Vector3.Distance(transform.position, player.position) <= sightRange))
            return false;

        navAgent.SetDestination(player.position);

        if (navAgent.remainingDistance >= sightRange)
        {
            navAgent.SetDestination(targetLocation);
            return false;
        }

        return true;
    }
}
