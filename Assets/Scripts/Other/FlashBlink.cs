using System;
using UnityEngine;

public class FlashBlink : MonoBehaviour
{



    [SerializeField] private MonoBehaviour damageableObject;
    [SerializeField] private Material blinkMaterial;
    [SerializeField] private float blinkDuration = 0.2f;


    private float _blinkTimer;
    private Material _defaultMaterial;
    private SpriteRenderer _spriteRenderer;
    private bool _isBlinking;


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultMaterial = _spriteRenderer.material;
        _isBlinking = true;


      
    }

    public void Start()
    {
        if (damageableObject is Player player)
        {
           player.OnFlashBlink += DamageableObject_OnFlashBlink;
        }
    }

    private void DamageableObject_OnFlashBlink(object sender, EventArgs e)
    {
        SetBlinkingMaterial();
    }

    private void Update()
    {
        if (_isBlinking)
        {
            _blinkTimer -= Time.deltaTime;
            if (_blinkTimer < 0)
            {
                SetDefaultMaterial();
            }
        }
    }

    public void StopBlinking()
    {
        SetDefaultMaterial();
        _isBlinking = false;
    }

    private void SetDefaultMaterial()
    {
        _spriteRenderer.material = _defaultMaterial;
    }


    private void SetBlinkingMaterial()
    {
        _blinkTimer = blinkDuration;
        _spriteRenderer.material = blinkMaterial;
    }

    private void OnDestroy()
    {
        if(damageableObject is Player player)
        {
            player.OnFlashBlink -= DamageableObject_OnFlashBlink;
        }
    }
}
