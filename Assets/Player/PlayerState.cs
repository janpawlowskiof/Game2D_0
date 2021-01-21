using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    public PlayerActor Player;
    public PlayerStateMachine psm;
    public static readonly int Alive = Animator.StringToHash("Alive");
    
    public PlayerState(PlayerActor player, PlayerStateMachine psm)
    {
        this.Player = player;
        this.psm = psm;
    }
    public abstract void Damage();
    public abstract void Revive();
    public abstract void Shoot(Vector2 direction);
    public abstract void TeleportToLimboBullet();
    public abstract void OnEnter();
    public abstract void UpdateGhostTimer();

}

public class PlayerNormalSt : PlayerState
{
    public PlayerNormalSt(PlayerActor player, PlayerStateMachine psm) : base(player, psm) { }

    public override void Damage()
    {
        psm.State = psm.PlayerGhostSt;
    }

    public override void Revive()
    {
        Debug.Log("Reviving already alive player");
    }

    public override void Shoot(Vector2 direction)
    {
        Debug.Log("Shooting Bullet");
        Player.limboBullet.Recall();
        Player.limboBullet.Shoot(Player.transform.position, direction * Player.bulletSpeed);
    }

    public override void TeleportToLimboBullet()
    {
        if (Player.limboBullet.isRecalled)
            return;

        if (Player.TeleportToAABB(Player.limboBullet.aabb))
        {
            Player.limboBullet.Recall();
            psm.State = psm.PlayerGhostSt;
        }
    }

    public override void OnEnter()
    {
        Player.gameObject.layer = PlatPhysics.LayerNormal;
        Player.anim.SetBool(Alive, true);
        Player.currentGhostTimer = Player.baseGhostTimer;
    }

    public override void UpdateGhostTimer()
    {
        if (Player.currentGhostTimer > Player.baseGhostTimer)
        {
            Player.currentGhostTimer -= Time.deltaTime;
            Player.currentGhostTimer = Mathf.Max(Player.currentGhostTimer, Player.baseGhostTimer);
        }
    }
}

public class PlayerGhostSt : PlayerState
{
    public PlayerGhostSt(PlayerActor player, PlayerStateMachine psm) : base(player, psm) { }

    public override void Damage()
    {
        psm.SoftReset();
    }

    public override void Revive()
    {
        Debug.Log("Reviving a dead player");
        psm.State = psm.PlayerNormalSt;
    }

    public override void Shoot(Vector2 direction)
    {
        Debug.Log("Shooting not allowed while in ghost state");
    }

    public override void TeleportToLimboBullet()
    {
        Debug.Log("Teleporting not allowed while in ghost state");
    }

    public override void OnEnter()
    {
        Player.gameObject.layer = PlatPhysics.LayerGhost;
        Player.anim.SetBool(Alive, false);
    }

    public override void UpdateGhostTimer()
    {
        Player.currentGhostTimer -= Time.deltaTime;
        if (Player.currentGhostTimer < 0)
        {
            psm.SoftReset();
        }
    }
}