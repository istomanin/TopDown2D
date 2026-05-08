using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{


    [SerializeField] private float _playerSpeed = 5f;

    private Rigidbody2D _rb;
    private float _minMovingSpeed = 0.1f;
    private bool _isRunning = false;
    private Vector2 inputVector;

    public static Player Instance;



    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Instance = this;
    }


    private void Start()
    {
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
    }

    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e)
    {
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }

    private void Update()
    {
        inputVector = GameInput.Instance.GetMovementVector();
    }

    private void FixedUpdate()
    {
        HandleMovment();

    }


    private void HandleMovment()
    {



        _rb.MovePosition(_rb.position + inputVector * (_playerSpeed * Time.fixedDeltaTime));

        if (Mathf.Abs(inputVector.x) > _minMovingSpeed || Mathf.Abs(inputVector.y) > _minMovingSpeed)
        {
            _isRunning = true;
        }
        else
        {
            _isRunning = false;
        }
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
}
