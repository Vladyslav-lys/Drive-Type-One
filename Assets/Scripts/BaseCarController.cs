using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCarController : MonoBehaviour
{
    public float maxSpeed;
    public float forwardSpeed;
    public float moveX;
    public float turnSpeed;
    public Animator animator;
    protected GameManager Gm;
    protected Rigidbody Rb;
    protected Vector3 ForceDir;
    
    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        ForceDir = new Vector3(0f,0f,forwardSpeed);
    }
    
    protected virtual void Start()
    {
        Gm = GameManager.instance;
    }
    
    protected void FixedUpdate()
    {
        if(!Gm.CanPlay())
            return;
        
        if (Rb.velocity.magnitude < maxSpeed)
        {
            Rb.AddForce(ForceDir, ForceMode.Impulse);
        }
    }

    protected virtual void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            new Vector3(moveX, 0.1f, transform.position.z), turnSpeed * Time.deltaTime);
    }

    public void NullifyVelocity() => Rb.velocity = Vector3.zero;

    public virtual void SetLoseAnim() => animator.SetBool("Lose", true);
    
    public virtual void SetStartAnim(bool isStarted) => animator.SetBool("Start", isStarted);

    public void ReduceSpeed()
    {
        this.maxSpeed = maxSpeed/10f;
        Rb.velocity /= 10f;
    }
    
    public void IncreaseSpeed()
    {
        this.maxSpeed = maxSpeed*10f;
        Rb.velocity *= 10f;
    }

    public void SetAnimator(Animator animator) => this.animator = animator;
}
