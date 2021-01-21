using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlatPhysics : MonoBehaviour
{
    public List<Actor> actors = new List<Actor>();
    public List<Solid> solids = new List<Solid>();
    public static PlatPhysics Main = null;
    public static LayerMask LayerNormal;
    public static LayerMask LayerGhost;

    public bool CheckCollisionVsSolids(int positionX, int positionY, AABB aabb)
    {
        return CheckCollisionVsSolids(new Vector2Int(positionX, positionY), aabb);
    }

    public bool CheckCollisionVsSolids(Vector2Int position, AABB aabb)
    {
        foreach (var solid in solids)
        {
            // if (!solid.enabled || (layers_mask >= 0 && !(solid->layers_ & layers_mask)) || (collision_mask && !(*collision_mask)[solid_index]))
            if (!solid.enabled)
                continue;
            if (CheckAABBVsAABB(aabb, position, solid.aabb, solid.aabb.PhysicsPosition2Int))
                return true;
        }

        return false;
    }

    public bool CheckCollisionVsActors(AABB aabb)
    {
        foreach (var actor in actors)
        {
            if (!actor.enabled)
                continue;
            if (CheckAABBVsAABB(aabb, actor.aabb))
                return true;
        }

        return false;
    }

    public bool CheckAABBVsAABB(AABB a, Vector2Int aPosition, AABB b, Vector2Int bPosition)
    {
        if (aPosition.x + a.HalfExtentX <= bPosition.x  - b.HalfExtentX || aPosition.x - a.HalfExtentX >= bPosition.x + b.HalfExtentX)
            return false;
        if (aPosition.y + a.HalfExtentY <= bPosition.y - b.HalfExtentY || aPosition.y - a.HalfExtentY >= bPosition.y + b.HalfExtentY)
            return false;
        return true;
    }

    public static bool CheckAABBVsAABB(AABB a, AABB b)
    {
        if (a.Right <= b.Left || a.Left >= b.Right)
            return false;
        if (a.Top <= b.Bottom || a.Bottom >= b.Top)
            return false;
        return true;
    }

    void Awake()
    {
        if (Main != null)
        {
            Debug.LogError("There is already another platPhysics!");
        }

        Main = this;
        Debug.Log($"Instance is {Main}");
            
        LayerNormal = LayerMask.NameToLayer("LayerNormal");
        LayerGhost = LayerMask.NameToLayer("LayerGhost");
    }

    void Update()
    {
        foreach (var actor in actors)
        {
            actor.UpdateJumpThroughCollisionMask();
        }
    }
    
    void LateUpdate()
    {
        if (GlobalSettings.DrawColliders)
        {
            foreach (var solid in solids)
            {
                solid.aabb.Draw(Color.blue);
            }
        }

        if (GlobalSettings.DrawActorColliders)
        {
            foreach (var actor in actors)
            {
                actor.aabb.Draw(Color.green);
            }
        }
    }

    private void OnDestroy()
    {
        Main = null;
    }
}
