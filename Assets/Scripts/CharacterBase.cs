using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    // value equals to Mathf.Sin(22.5 degree)
    const float k_inputCriterionSin = 0.38268343236f;
    [SerializeField] float speed;
    protected Rigidbody2D rigidBody;

    protected Vector2 inputDirection;
    protected bool isDoneMoving; //this character has reached its ending point.
    protected int stageNum;

    protected ParticleSystem _deadParticleSystem;
    protected ParticleSystem _winParticleSystem;

    protected SpriteRenderer _spriteRenderer;
    
    private bool _isAtEndingPoint = false;

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        //_deadParticleSystem = GetComponentInChildren<ParticleSystem>();
        _deadParticleSystem = transform.Find("Dead Particle System").GetComponent<ParticleSystem>();
        _winParticleSystem = transform.Find("Win Particle System").GetComponent<ParticleSystem>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        GameManager.Instance.clearedCount = 0;
    }

    protected virtual void Update()
    {
        if(!isDoneMoving ) Move();
        else rigidBody.velocity = Vector2.zero;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (CheckDeadly(other)) {
            return;
        }
        if (other.tag == "EndingPoint" && !_isAtEndingPoint)
        {
            Debug.Log("end");
            isDoneMoving = true;
            _isAtEndingPoint = true;
            Debug.Log("end " + this);
            StartCoroutine(StageClearCoroutine());
        }
    }

    public void SetStageNum(int stage)
    {
        stageNum = stage;
    }
    private void Move()
    {
        var velocity = inputDirection;
        velocity.x = Mathf.Abs(velocity.x) > k_inputCriterionSin ? Mathf.Sign(velocity.x) : 0;
        velocity.y = Mathf.Abs(velocity.y) > k_inputCriterionSin ? Mathf.Sign(velocity.y) : 0;
        rigidBody.velocity = velocity.normalized * speed;
    }
 
    protected virtual bool CheckDeadly(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone"))
        {
            Debug.Log("dead");
            isDoneMoving = true;
            StartCoroutine(StageFailCoroutine());
            return true;
        }
        return false;
    }

    protected void StageFail()
    {
        GameManager.Instance.StageFail();
    }

    protected IEnumerator StageFailCoroutine()
    {
        _deadParticleSystem.Play();
        _spriteRenderer.enabled = false;
        yield return new WaitForSeconds(1f);
        StageFail();
    }

    protected IEnumerator StageClearCoroutine()
    {
        _winParticleSystem.Play();
        _spriteRenderer.enabled = false;
        yield return new WaitForSeconds(1f);
        GameManager.Instance.OneOfStagesCleared();
    }
}
