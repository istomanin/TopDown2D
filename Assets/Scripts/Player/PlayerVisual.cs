using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private static readonly int Die = Animator.StringToHash(IsDie);
    private static readonly int Running = Animator.StringToHash(IsRunning);
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private FlashBlink _flashBlink;


    private const string IsRunning = "isRunning";
    private const string IsDie = "isDie";

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
        _animator.SetBool(Die, true);
        _flashBlink.StopBlinking();
    }

    private void Update()
    {
        if (Player.Instance.IsAlive())
        {
            _animator.SetBool(Running, Player.Instance.IsRunning());
            AdjustPlayerFacingDirection();
        }

    }


    private void AdjustPlayerFacingDirection()
    {
        Vector3 mosPos = GameInput.Instance.GetMousePosition();
        Vector3 playerPos = Player.Instance.GetPlayerScreenPosition();

        _spriteRenderer.flipX = mosPos.x < playerPos.x; //поменял эту строчку помошник, я пока не оч понял этот момент
    }

    private void OnDestroy()
    {
        Player.Instance.OnPlayerDeath -= Player_OnPlayerDeath;
    }
}
