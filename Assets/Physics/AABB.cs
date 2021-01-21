using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;


public class AABB : MonoBehaviour, IResettable
{
    private AABBSnapshot _snapshot;
    
    public int PositionX
    {
        get => (int)Math.Floor(transform.position.x);
        set
        {
            var transformPosition = transform.position;
            transformPosition.x = value;
            transform.position = transformPosition;
        }
    }

    public int PositionY
    {
        get => (int)Math.Floor(transform.position.y);
        set
        {
            var transformPosition = transform.position;
            transformPosition.y = value;
            transform.position = transformPosition;
        }
    }

    public Vector2Int halfExtentsInt = new Vector2Int(16, 16);
    public Vector2 startPosition;
    public int HalfExtentX => halfExtentsInt.x;
    public int HalfExtentY => halfExtentsInt.y;

    public Vector3 PhysicsPosition3 => new Vector3(PositionX, PositionY, 0);
    public Vector2 PhysicsPosition2 => new Vector2(PositionX, PositionY);
    public Vector2Int PhysicsPosition2Int => new Vector2Int(PositionX, PositionY);
    public int W => halfExtentsInt.x * 2;
    public int H => halfExtentsInt.y * 2;
    public int Right => PositionX + halfExtentsInt.x;
    public int Left => PositionX - halfExtentsInt.x;
    public int Top => PositionY + halfExtentsInt.y;
    public int Bottom => PositionY - halfExtentsInt.y;
    public Vector2Int BottomLeftInt => new Vector2Int(Left, Bottom);
    public Vector2Int TopLeftInt => new Vector2Int(Left, Top);
    public Vector2Int BottomRightInt => new Vector2Int(Right, Bottom);
    public Vector2Int TopRightInt => new Vector2Int(Right, Top);
    public Vector2 BottomLeft => new Vector2(Left, Bottom);
    public Vector2 TopLeft => new Vector2(Left, Top);
    public Vector2 BottomRight => new Vector2(Right, Bottom);
    public Vector2 TopRight => new Vector2(Right, Top);

    void Awake()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        var position = transform.position;
        transform.position = new Vector3((int)Math.Floor(position.x) , (int) Math.Floor(position.y), position.z);
        transform.localScale = new Vector3(W, H, 0);
    }

    public void Draw()
    {
        Draw(Color.cyan);
    }

    public void Draw(Color color)
    {
        Debug.DrawLine(TopRight, TopLeft, color);
        Debug.DrawLine(TopRight, BottomRight, color);
        Debug.DrawLine(TopLeft, BottomLeft, color);
        Debug.DrawLine(BottomRight, BottomLeft, color);
    }

    public void CreateSnapshot()
    {
        _snapshot = new AABBSnapshot
        {
            PositionX = PositionX,
            PositionY = PositionY,
            HalfExtentX = HalfExtentX,
            HalfExtentY = HalfExtentY
        };
    }

    public void ReloadSnapshot()
    {
        PositionX = _snapshot.PositionX;
        PositionY = _snapshot.PositionY;
        halfExtentsInt.x = _snapshot.HalfExtentX;
        halfExtentsInt.y = _snapshot.HalfExtentY;
    }

    private struct AABBSnapshot
    {
        public int PositionX;
        public int PositionY;
        public int HalfExtentX;
        public int HalfExtentY;
    }
}