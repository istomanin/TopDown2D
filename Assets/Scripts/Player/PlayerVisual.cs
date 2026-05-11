using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private FlashBlink _flashBlink;


    private const string IS_RUNNING = "isRunning";
    private const string IS_DIE = "isDie";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _flashBlink = GetComponent<FlashBlink>();
       
    }

    private void Start()
    {
        Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
    }

    private void Player_OnPlayerDeath(object sender, System.EventArgs e)
    {
        _animator.SetBool(IS_DIE, true);
        _flashBlink.StopBlinking();
    }

    private void Update()
    {
        if (Player.Instance.IsAlive())
        {
            _animator.SetBool(IS_RUNNING, Player.Instance.IsRunning());
            AdjustPlayerFacingDirection();
        }

    }


    private void AdjustPlayerFacingDirection()
    {
        Vector3 mosPos = GameInput.Instance.GetMousePosition();
        Vector3 playerPos = Player.Instance.GetPlayerScreenPosition();

        if (mosPos.x < playerPos.x)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
    }
}
