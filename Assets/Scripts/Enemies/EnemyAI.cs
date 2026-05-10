using KnightAdvanture.Utils;
using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    [SerializeField] private State _startingState;

    [SerializeField] private float _roamingDistanceMax = 7f;
    [SerializeField] private float _roamingDistanceMin = 3f;
    [SerializeField] private float _roamingTimerMax = 2f;
    [SerializeField] private float _chasingDistance = 4f;
    [SerializeField] private float _chasingSpeedMultipier = 2f;
    [SerializeField] private float _attackingDistance = 2f;
    [SerializeField] private float _attackRate = 2f;

    [SerializeField] private bool _isChaisingEnemy = false;
    [SerializeField] private bool _isAttackingEnemy = false;


   



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

    public bool isRunning
    {
        get
        {
            if (_navMeshAgent.velocity == Vector3.zero)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

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



        _currentState = _startingState;
        _roamingSpeed = _navMeshAgent.speed;
        _chasingSpeed = _navMeshAgent.speed * _chasingSpeedMultipier;
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
                    _roamingTimer = _roamingTimerMax;
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

            _nextAttackTime = Time.time+_attackRate;
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

        if (_isChaisingEnemy && distanceToPlayer <= _chasingDistance)
        {
            newState = State.Chasing;
        }


        if (_isAttackingEnemy)
        {
            if (distanceToPlayer <= _attackingDistance)
            {
                newState = State.Attaсking;
            }
        }

        if (newState != _currentState)
        {
            if (newState == State.Chasing)
            {
                _navMeshAgent.ResetPath();
                _navMeshAgent.speed = _chasingSpeed;
            }
            else if( newState == State.Roaming)
            {

                _roamingTimer = 0f; //здесь можно настроить время, через которое объект перейдет в состояние брожения

                _navMeshAgent.speed = _roamingSpeed;
            }
            else if(newState == State.Attaсking)
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
        return _startingPosition + Utils.GetRandomDir() * UnityEngine.Random.Range(_roamingDistanceMin, _roamingDistanceMax);
    }



    private void MovementDirectionHendler()
    {
        if (Time.time > _nextCheckDirectionTime)
        {
            if (isRunning)
            {
                ChangeFaceDirection(_lastPosition, transform.position);
            }else if (_currentState == State.Attaсking)
            {
                ChangeFaceDirection(transform.position, Player.Instance.transform.position);
            }

            _lastPosition=transform.position;
            _nextCheckDirectionTime = Time.time + _checkDirectionDuration;
        }
    }

    private void ChangeFaceDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

    }

}
