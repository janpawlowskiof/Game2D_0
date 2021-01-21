using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;


[RequireComponent(typeof(AABB))]
public abstract class Actor: MonoBehaviour, IResettable
{
    float _xRemainder = 0;
    float _yRemainder = 0;
    public AABB aabb;
    private ActorSnapshot _snapshot;
    
    public int MoveX(float amount, Action callback)
    {
        _xRemainder += amount;
        int move = Mathf.RoundToInt(_xRemainder);

        _xRemainder -= move;
        int dir = (int)Mathf.Sign(move);
        while (move != 0)
        {
            if (!PlatPhysics.Main.CheckCollisionVsSolids (aabb.PositionX + dir, aabb.PositionY, aabb))
            {
                aabb.PositionX += dir;
                move -= dir;
            }
            else
            {
                callback?.Invoke();
                return move;
            }
        }
        return 0;
    }
    
    public int MoveY(float amount, Action callback)
    {
        _yRemainder += amount;
        int move = Mathf.RoundToInt(_yRemainder);

        _yRemainder -= move;
        int dir = (int)Mathf.Sign(move);
        while (move != 0)
        {
            if (!PlatPhysics.Main.CheckCollisionVsSolids (aabb.PositionX, aabb.PositionY + dir, aabb))
            {
                aabb.PositionY += dir;
                move -= dir;
            }
            else
            {
                callback?.Invoke();
                return move;
            }
        }
        return 0;
    }

    protected virtual void Start()
    {
        aabb = GetComponent<AABB>();
        PlatPhysics.Main.actors.Add(this);
    }

    void OnDestroy()
    {
        PlatPhysics.Main.actors.Remove(this);
    }

    void ZeroRemainderX() { _xRemainder = 0; }
    void ZeroRemainderY() { _yRemainder = 0; }

    public void UpdateJumpThroughCollisionMask()
    {

    }

    public bool CheckCollisionVsSolids(int positionX, int positionY)
    {
        return PlatPhysics.Main.CheckCollisionVsSolids(positionX, positionY, aabb);
    }

    public virtual bool IsRiding(Solid solid)
    {
        return PlatPhysics.Main.CheckAABBVsAABB(solid.aabb, solid.aabb.PhysicsPosition2Int, aabb, aabb.PhysicsPosition2Int + Vector2Int.down);
    }

    public abstract void OnSquish();
    public void CreateSnapshot()
    {
        aabb.CreateSnapshot();
        _snapshot = new ActorSnapshot
        {
            XRemainder = _xRemainder,
            YRemainder = _yRemainder
        };
    }

    public void ReloadSnapshot()
    {
        aabb.ReloadSnapshot();
        _xRemainder = _snapshot.XRemainder;
        _yRemainder = _snapshot.YRemainder;
    }
    
    private struct ActorSnapshot
    {
        public float XRemainder;
        public float YRemainder;
    }
}
