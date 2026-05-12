using UnityEngine;

public class SwordVisual : MonoBehaviour
{
    private static readonly int AttackHash = Animator.StringToHash(Attack);
    private Animator animator;

    [SerializeField] private Sword sword;
    private const string Attack = "Attack";




    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void Start()
    {
        sword.OnSwordSwing += Sword_OnSwordSwing;
    }

    private void Sword_OnSwordSwing(object sender, System.EventArgs e)
    {
        animator.SetTrigger(AttackHash);
    }



    public void TriggerEndAttackdAnimation()
    {
        sword.AttackColliderTurnOff();
    }

    private void OnDestroy()
    {
        sword.OnSwordSwing -= Sword_OnSwordSwing;
    }
}
