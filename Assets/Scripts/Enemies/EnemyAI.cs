using KnightAdvanture.Utils;
using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private State startingState;

    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 2f;
    [SerializeField] private float chasingDistance = 4f;
    [SerializeField] private float chasingSpeedMultiplier = 2f;
    [SerializeField] private float attackingDistance = 2f;
    [SerializeField] private float attackRate = 2f;

    [SerializeField] private bool isChasingEnemy = false;
    [SerializeField] private bool isAttackingEnemy = false;


    private NavMeshAgent _navMeshAgent;

    private State _currentState;

    private Vector3 _roamPosition;
    private Vector3 _startingPosition;
    private Vector3 _lastPosition;

    private float _nextCheckDirectionTime = 0f;
    private float _checkDirectionDuration = 0.1f;

    private float _roamingTimer;
    private float _roamingSpeed;
    private float _chasingSpeed;


    private float _nextAttackTime = 0f;

    public event EventHandler OnEnemyAttack;

    public bool IsRunning => _navMeshAgent.velocity != Vector3.zero; // тоже помошник, но выглядит конечно круто

    private enum State
    {
        Idle,
        Roaming,
        Chasing,
        Attaсking,
        Death
    }


    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;


        _currentState = startingState;
        _roamingSpeed = _navMeshAgent.speed;
        _chasingSpeed = _navMeshAgent.speed * chasingSpeedMultiplier;
    }


    private void Update()
    {
        StateHandler();
        MovementDirectionHendler();
    }

    public void SetDeathState()
    {
        _navMeshAgent.ResetPath();
        _currentState = State.Death;
    }

    private void StateHandler()
    {
        switch (_currentState)
        {
            case State.Roaming:
                _roamingTimer -= Time.deltaTime;

                if (_roamingTimer < 0)
                {
                    Roaming();
                    _roamingTimer = roamingTimerMax;
                }

                CheckCurrentState();
                break;

            case State.Chasing:
                ChasingTarget();
                CheckCurrentState();
                break;
            case State.Attaсking:
                AttackingTarget();
                CheckCurrentState();
                break;
            case State.Death:
                break;
            default:
            case State.Idle:
                break;
        }
    }

    private void AttackingTarget()
    {
        if (Time.time > _nextAttackTime)
        {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);

            _nextAttackTime = Time.time + attackRate;
        }
    }

    public float GetRoamingAnimationSpeed()
    {
        return _navMeshAgent.speed / _roamingSpeed;
    }

    private void CheckCurrentState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);

        State newState = State.Roaming;

        if (isChasingEnemy && distanceToPlayer <= chasingDistance)
        {
            if (Player.Instance.IsAlive())
            {
                newState = State.Chasing;
            }
        }


        if (isAttackingEnemy)
        {
            if (distanceToPlayer <= attackingDistance)
            {
                newState = Player.Instance.IsAlive() ? State.Attaсking : State.Roaming;
            }
        }

        if (newState != _currentState)
        {
            if (newState == State.Chasing)
            {
                _navMeshAgent.ResetPath();
                _navMeshAgent.speed = _chasingSpeed;
            }
            else if (newState == State.Roaming)
            {
                _roamingTimer = 0f; //здесь можно настроить время, через которое объект перейдет в состояние брожения

                _navMeshAgent.speed = _roamingSpeed;
            }
            else if (newState == State.Attaсking)
            {
                _navMeshAgent.ResetPath();
            }

            _currentState = newState;
        }
    }

    private void ChasingTarget()
    {
        _navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    private void Roaming()
    {
        _startingPosition = transform.position;
        _roamPosition = GetRoamingPosition();

        _navMeshAgent.SetDestination(_roamPosition);
    }

    private Vector3 GetRoamingPosition()
    {
        return _startingPosition +
               Utils.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax);
    }


    private void MovementDirectionHendler()
    {
        if (Time.time > _nextCheckDirectionTime)
        {
            if (IsRunning)
            {
                ChangeFaceDirection(_lastPosition, transform.position);
            }
            else if (_currentState == State.Attaсking)
            {
                ChangeFaceDirection(transform.position, Player.Instance.transform.position);
            }

            _lastPosition = transform.position;
            _nextCheckDirectionTime = Time.time + _checkDirectionDuration;
        }
    }

    private void ChangeFaceDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        transform.rotation = sourcePosition.x > targetPosition.x ? Quaternion.Euler(0, -180, 0) : Quaternion.Euler(0, 0, 0);
    }
}