using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    public static Player Instance;
    
    public event EventHandler OnPlayerDeath;
    public event EventHandler OnFlashBlink;

    [SerializeField] private float movingSpeed = 5f;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float damageRecoveryTime = 0.5f;
    
    [Header("Dash Settings")]
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private int dashSpeed = 4;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCoolDownTime = 0.25f;
    
    
    
    

    private Rigidbody2D _rb;
    private KnockBack _knockBack;
    private Camera _mainCamera;


    private readonly float _minMovingSpeed = 0.1f;
    private bool _isRunning = false;
    private bool _isDashing;
    private int _currentHealth;
    private float _initialMovingSpeed;


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
        _initialMovingSpeed = movingSpeed;
        _canTakeDamage = true;
        _isAlive = true;
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
        GameInput.Instance.OnPlayerDash += GameInput_OnPlayerDash;
    }

    private void GameInput_OnPlayerDash(object sender, EventArgs e)
    {
        Dash();
    }

    private void Dash()
    {
        if (!_isDashing)
        {
            StartCoroutine(DashRoutine());  
        }
        
    }

    private IEnumerator DashRoutine()
    {
        _isDashing = true;
        movingSpeed *= dashSpeed;
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashTime);
        
        movingSpeed = _initialMovingSpeed;
        trailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCoolDownTime);
        _isDashing = false;
        
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
        yield return new WaitForSeconds(damageRecoveryTime);
        _canTakeDamage = true;
    }


    private void DetectDeath()
    {
        if (_currentHealth == 0 )
        {
            GameInput.Instance.DisableMovement();
            _canTakeDamage = false;
            _knockBack.StopKnockBackMovement();
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
            _isAlive = false;

        }
    }
    private void HandleMovment()
    {



        _rb.MovePosition(_rb.position + _inputVector * (movingSpeed * Time.fixedDeltaTime));

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
