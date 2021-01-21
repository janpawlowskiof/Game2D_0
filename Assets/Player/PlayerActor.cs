using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor : Actor
{
    [SerializeField] Vector2 currentSpeed;
    [SerializeField] public LimboBullet limboBullet;
    [SerializeField] int lookDir = 1;
    [SerializeField] float maxSpeed = 90.0f;
    [SerializeField] float variableJumpSpeed = 0.0f;
    [SerializeField] float variableJumpTimer = 0.0f;
    [SerializeField] float coyoteTimer = 0.0f;
    [SerializeField] float jumpBufferTimer = 0.0f;

    private const float VARIABLE_JUMP_TIMER = 0.15f;
    private const float COYOTE_TIMER = 0.1f;
    private const float JUMP_BUFFER_TIMER = 0.15f;
    private const float RUN_ACCEL = 1000.0f / 100.0f;
    private const float JUMP_SPEED = 160.0f;
    private const float MAX_FALL_SPEED = -120.0f;
    private const float GRAVITY = 500.0f / 100.0f;
    
    [Header("Gun variables")] 
    public float bulletSpeed = 512f;

    private PlayerStateMachine psm;
    public Animator anim;
    public float baseGhostTimer = 3.0f;
    public float currentGhostTimer = 0.0f;
    public TextMesh ghostCounterTextMesh;
    
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        psm = new PlayerStateMachine(this);
    }

    void Update()
    {
        var dt = Time.deltaTime;

        int moveDirectionX = Input.GetKey(KeyCode.D).ToInt() - Input.GetKey(KeyCode.A).ToInt();
        int moveDirectionY = Input.GetKey(KeyCode.W).ToInt() - Input.GetKey(KeyCode.S).ToInt();
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            GlobalSettings.Noclip = !GlobalSettings.Noclip;
        }
        
        if (GlobalSettings.Noclip)
        {
            aabb.PositionX += (int)(moveDirectionX * 5);
            aabb.PositionY += (int)(moveDirectionY * 5);
            return;
        }
        
        bool isGrounded = IsGrounded();
        float targetSpeed = moveDirectionX * maxSpeed;

        if (moveDirectionX > 0)
            lookDir = 1;
        else if (moveDirectionX < 0)
            lookDir = -1;

        currentSpeed.x = Mathf.Lerp(currentSpeed.x, targetSpeed, RUN_ACCEL * dt);
        MoveX(currentSpeed.x * dt, OnCollideX);
        MoveY(currentSpeed.y * dt, OnCollideY);
        
        // rendering
        
        if (isGrounded)
        {
            // actor_->ZeroRemainderY();
            coyoteTimer = COYOTE_TIMER;
            currentSpeed.y = 0;
            if (Input.GetKeyDown(KeyCode.Space) || jumpBufferTimer > 0)
            {
                Jump();
            }
        }
        else // airborne
        {
            // coyote jump
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(coyoteTimer > 0)
                    Jump();
                else
                    jumpBufferTimer = JUMP_BUFFER_TIMER;
            }
            // variable jump
            else if (Input.GetKey(KeyCode.Space))
            {
                if (variableJumpTimer > 0)
                {
                    currentSpeed.y = Mathf.Max(currentSpeed.y, variableJumpSpeed);
                }
            }
            else
            {
                variableJumpTimer = 0;
            }

            // gravity
            currentSpeed.y = Mathf.Lerp(currentSpeed.y, MAX_FALL_SPEED, GRAVITY * dt);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (moveDirectionY != 0)
                psm.Shoot(new Vector2(0, moveDirectionY).normalized);
            else
                psm.Shoot(new Vector2(lookDir, 0).normalized);
            
            //8 dir variant
            // if (moveDirectionY != 0)
            //     Shoot(new Vector2(moveDirectionX, moveDirectionY).normalized);
            // else
            //     Shoot(new Vector2(lookDir, 0).normalized);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            psm.TeleportToLimboBullet();
        }

        if (variableJumpTimer > 0)
            variableJumpTimer -= dt;
        if (coyoteTimer > 0)
            coyoteTimer -= dt;
        if (jumpBufferTimer > 0)
            jumpBufferTimer -= dt;

        psm.UpdateGhostTimer();
        ghostCounterTextMesh.text = $"{Mathf.Round(currentGhostTimer * 100)}";
    }


    void OnCollideX()
    {
        
    }
    
    void OnCollideY()
    {
        if (currentSpeed.y > 0)
        {
            currentSpeed.y = 0;
            variableJumpTimer = 0;
        }
    }

    void Jump()
    {
        // to unstick from ground
        MoveY(1, null);
        variableJumpSpeed = currentSpeed.y = JUMP_SPEED;
        variableJumpTimer = VARIABLE_JUMP_TIMER;
        jumpBufferTimer = 0;
    }

    public bool TeleportToAABB(AABB target)
    {
        //Direct teleportation
        if (!CheckCollisionVsSolids(target.PositionX, target.PositionY))
        {
            aabb.PositionX = target.PositionX;
            aabb.PositionY = target.PositionY;
        }
        else if (!CheckCollisionVsSolids(target.PositionX + (target.HalfExtentX - aabb.HalfExtentX), target.PositionY + (target.HalfExtentY - aabb.HalfExtentY)))
        {
            aabb.PositionX = target.PositionX + (target.HalfExtentX - aabb.HalfExtentX);
            aabb.PositionY = target.PositionY + (target.HalfExtentY - aabb.HalfExtentY);
        }
        else if (!CheckCollisionVsSolids(target.PositionX - (target.HalfExtentX - aabb.HalfExtentX), target.PositionY + (target.HalfExtentY - aabb.HalfExtentY)))
        {
            aabb.PositionX = target.PositionX - (target.HalfExtentX - aabb.HalfExtentX);
            aabb.PositionY = target.PositionY + (target.HalfExtentY - aabb.HalfExtentY);
        }
        else if (!CheckCollisionVsSolids(target.PositionX - (target.HalfExtentX - aabb.HalfExtentX), target.PositionY - (target.HalfExtentY - aabb.HalfExtentY)))
        {
            aabb.PositionX = target.PositionX - (target.HalfExtentX - aabb.HalfExtentX);
            aabb.PositionY = target.PositionY - (target.HalfExtentY - aabb.HalfExtentY);
        }
        else if (!CheckCollisionVsSolids(target.PositionX - (target.HalfExtentX + aabb.HalfExtentX), target.PositionY - (target.HalfExtentY - aabb.HalfExtentY)))
        {
            aabb.PositionX = target.PositionX + (target.HalfExtentX - aabb.HalfExtentX);
            aabb.PositionY = target.PositionY - (target.HalfExtentY - aabb.HalfExtentY);
        }
        else
        {
            Debug.Log("Teleport not possible");
            return false;
        }

        return true;
    }
    
    bool IsGrounded()
    {
        return currentSpeed.y <= 0 && CheckCollisionVsSolids(aabb.PositionX, aabb.PositionY - 1);
    }

    public override void OnSquish()
    {
        psm.Damage();
    }
    
    public void Revive()
    {
        psm.Revive();
    }
}
