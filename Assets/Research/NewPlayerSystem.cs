using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerSystem : MonoBehaviour
{
    //Respawn Position
    private Vector2 _respawnPosition;
    
    Vector2 _movementDirection;
    
    [SerializeField] private float _movementSpeed = 5f;
    
    private Rigidbody2D _rigidbody2D;
    //private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        //_animator = GetComponent<Animator>();
        _spriteRenderer = transform.Find("PlayerVisual").GetComponent<SpriteRenderer>();
        
        //Respawn Position
        _respawnPosition = transform.position;
    }
    
    private void Update()
    {
        _movementDirection.x = Input.GetAxisRaw("Horizontal");
        _movementDirection.y = Input.GetAxisRaw("Vertical");
        
        if (_movementDirection.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if (_movementDirection.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        
        //_animator.SetFloat("Speed", _movementDirection.magnitude);
    }
    
    private void FixedUpdate()
    {
        _rigidbody2D.MovePosition(_rigidbody2D.position + _movementDirection * (_movementSpeed * Time.fixedDeltaTime));
    }

    public void DeadZoneCollision()
    {
        Respawn();
    }
    
    //Respawn
    private void Respawn()
    {
        transform.position = _respawnPosition;
    }
}
