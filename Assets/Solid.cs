using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;


[RequireComponent(typeof(AABB))]
public class Solid : MonoBehaviour, IResettable
{
    float _xRemainder;
    float _yRemainder;
    public AABB aabb;
    private SolidSnapshot _snapshot;

    [SerializeField] private List<Actor> ridingActors = new List<Actor>();

    public void Move(float x, float y)
    {
        _xRemainder += x; 
        _yRemainder += y;
        int moveX = Mathf.RoundToInt(_xRemainder);
        int moveY = Mathf.RoundToInt(_yRemainder);

        if (moveX != 0 || moveY != 0)
        {
            UpdateRidingActors();
            enabled = false;

            if (moveX != 0)
            {
                _xRemainder -= moveX;
                aabb.PositionX += moveX;

                foreach(var actor in PlatPhysics.Main.actors)
                {
                    if (!actor.enabled)
                        continue;
                    if (PlatPhysics.CheckAABBVsAABB(aabb, actor.aabb))
                    {
                        if (moveX > 0)
                            actor.MoveX(aabb.Right - actor.aabb.Left, actor.OnSquish);
                        else
                            actor.MoveX(aabb.Left - actor.aabb.Right, actor.OnSquish);
                    }
                    else if (ridingActors.Contains(actor))
                    {
                        actor.MoveX(moveX, null);
                    }
                }
            }

            if (moveY != 0)
            {
                _yRemainder -= moveY;
                aabb.PositionY += moveY;

                foreach(var actor in PlatPhysics.Main.actors)
                {
                    if (!actor.enabled)
                        continue;
                    if (PlatPhysics.CheckAABBVsAABB(aabb, actor.aabb))
                    {
                        if (moveY > 0)
                            actor.MoveY(aabb.Top - actor.aabb.Bottom, actor.OnSquish);
                        else
                            //NOTE: This is an error probably
                            actor.MoveY(aabb.Bottom - actor.aabb.Top, actor.OnSquish);
                    }
                    else if (ridingActors.Contains(actor))
                    {
                        actor.MoveY(moveY, null);
                    }
                }
            }

            enabled = true;
        }
    }
    
    public void UpdateRidingActors()
    {
        ridingActors.Clear();
        foreach (var actor in PlatPhysics.Main.actors)
        {
            if(!actor.enabled)
                continue;
            if (actor.IsRiding(this))
                ridingActors.Add(actor);
        }
    }
    
    protected virtual void Start()
    {
        aabb = GetComponent<AABB>();
        PlatPhysics.Main.solids.Add(this);
    }

    void OnDestroy()
    {
        PlatPhysics.Main.solids.Remove(this);
    }

    public void CreateSnapshot()
    {
        aabb.CreateSnapshot();
        _snapshot = new SolidSnapshot
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
    
    private struct SolidSnapshot
    {
        public float XRemainder;
        public float YRemainder;
    }
}
