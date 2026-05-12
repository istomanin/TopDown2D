using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    public event EventHandler OnPlayerDeath;
    public event EventHandler OnFlashBlink;

    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float demegeRecoveryTime = 0.5f;

    public static Player Instance;

    private Rigidbody2D _rb;
    private KnockBack _knockBack;
    private Camera _mainCamera;


    private readonly float _minMovingSpeed = 0.1f;
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
        _mainCamera = Camera.main;
    }


    private void Start()
    {
        _currentHealth = maxHealth;
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
        Vector3 playerScreenPosition = _mainCamera.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }


    public void TakeDamage(Transform damageSource, int damage)
    {
        if (_canTakeDamage && _isAlive)
        {
            _canTakeDamage = false;
            OnFlashBlink?.Invoke(this, EventArgs.Empty);
            _currentHealth = Mathf.Max(0, _currentHealth -= damage);
            _knockBack.GetKnockBack(damageSource);
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
        yield return new WaitForSeconds(demegeRecoveryTime);
        _canTakeDamage = true;
    }


    private void DetectDeath()
    {
        if (_currentHealth == 0 )
        {
            GameInput.Instance.DisableMovment();
            _canTakeDamage = false;
            _knockBack.StopKnockBackMovement();
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
            _isAlive = false;

        }
    }
    private void HandleMovment()
    {



        _rb.MovePosition(_rb.position + _inputVector * (playerSpeed * Time.fixedDeltaTime));

        if (Mathf.Abs(_inputVector.x) > _minMovingSpeed || Mathf.Abs(_inputVector.y) > _minMovingSpeed)
        {
            _isRunning = true;
        }
        else
        {
            _isRunning = false;
        }
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPlayerAttack -= GameInput_OnPlayerAttack;
    }


}
