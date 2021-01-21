using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : PlayerState
{
    private PlayerState _state;
    public PlayerState State
    {
        get => _state;
        set
        {
            _state = value;
            _state.OnEnter();
        }
    }

    public readonly PlayerNormalSt PlayerNormalSt;
    public readonly PlayerGhostSt PlayerGhostSt;


    public PlayerStateMachine(PlayerActor player) : base(player, null)
    {
        PlayerNormalSt = new PlayerNormalSt(player, this);
        PlayerGhostSt = new PlayerGhostSt(player, this);
        State = PlayerNormalSt;
    }

    public override void Damage() { State.Damage(); }
    public override void Revive() { State.Revive(); }

    public override void Shoot(Vector2 direction) { State.Shoot(direction); }

    public override void TeleportToLimboBullet() { State.TeleportToLimboBullet(); }
    public override void UpdateGhostTimer() { State.UpdateGhostTimer(); }
    public void SoftReset()
    {
        State = PlayerNormalSt;
    }
    
    public void HardReset()
    {
        State = PlayerNormalSt;
    }

    public override void OnEnter() { }
}
