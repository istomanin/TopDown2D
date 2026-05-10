using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    public event EventHandler OnPlayerDeath;

    [SerializeField] private float _playerSpeed = 5f;
    [SerializeField] private int _maxHealth = 10;
    [SerializeField] private float _demegeRecoveryTime = 0.5f;

    public static Player Instance;

    private Rigidbody2D _rb;
    private KnockBack _knockBack;


    private float _minMovingSpeed = 0.1f;
    private bool _isRunning = false;
    private int _currentHealth;


    private Vector2 _inputVector;
    private bool _canTakeDamage;
    private bool _isAlive;




    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _knockBack = GetComponent<KnockBack>();
        Instance = this;
    }


    private void Start()
    {
        _currentHealth = _maxHealth;
        _canTakeDamage = true;
        _isAlive = true;
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
    }

    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e)
    {
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }

    private void Update()
    {
        _inputVector = GameInput.Instance.GetMovementVector();
    }

    private void FixedUpdate()
    {

        if (_knockBack.IsGettingKnockedBack)
        {
            return;
        }
        HandleMovment();

    }

    public bool IsRunning()
    {
        return _isRunning;
    }

    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }


    public void TakeDamage(Transform damegeSource, int damage)
    {
        if (_canTakeDamage && _isAlive)
        {
            _canTakeDamage = false;

            _currentHealth = Mathf.Max(0, _currentHealth -= damage);
            _knockBack.GetKnockBack(damegeSource);
            StartCoroutine(DamageRecoveryRoutine());

        }

        DetectDeath();
    }

    public bool IsAlive()
    {
        return _isAlive;
    }


    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(_demegeRecoveryTime);
        _canTakeDamage = true;
    }


    private void DetectDeath()
    {
        if (_currentHealth == 0 )
        {
            GameInput.Instance.DisableMovment();
            _canTakeDamage = false;
            _knockBack.StopKnockBackMovment();
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
            _isAlive = false;

        }
    }
    private void HandleMovment()
    {



        _rb.MovePosition(_rb.position + _inputVector * (_playerSpeed * Time.fixedDeltaTime));

        if (Mathf.Abs(_inputVector.x) > _minMovingSpeed || Mathf.Abs(_inputVector.y) > _minMovingSpeed)
        {
            _isRunning = true;
        }
        else
        {
            _isRunning = false;
        }
    }


}
