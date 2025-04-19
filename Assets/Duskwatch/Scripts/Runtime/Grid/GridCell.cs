using System;
using UnityEngine;
using UnityEngine.Profiling;

[Serializable]
public struct GridCell : IEquatable<GridCell>
{
    public const int WorldSize = 512;
    public const int GridBounds = 256; // WorldSize / GridSize
    public const int HalfGridBounds = 128;
    public const int GridSize = 2;
    
    public int x;
    public int y;

    public GridCell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public bool Overlaps(GridCell pos, float radius)
    {
        Profiler.BeginSample("Overlaps");
        Vector2 center = new Vector2(pos.x, pos.y);
        radius /= GridSize;
        
        float closestX = Math.Max(x, Math.Min(center.x, x+GridSize));
        float closestY = Math.Max(y, Math.Min(center.y, y+GridSize));
        
        float distanceX = center.x - closestX;
        float distanceY = center.y - closestY;
        
        float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
        Profiler.EndSample();
        return distanceSquared <= (radius * radius);
    }

    public bool Overlaps(Vector3 pos, float radius)
    {
        Vector2 center = new Vector2((pos.x / GridSize)+HalfGridBounds, (pos.z / GridSize)+HalfGridBounds);
        radius /= GridSize;
        
        float closestX = Math.Max(x, Math.Min(center.x, x+GridSize));
        float closestY = Math.Max(y, Math.Min(center.y, y+GridSize));
        
        float distanceX = center.x - closestX;
        float distanceY = center.y - closestY;
        
        float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
        return distanceSquared <= (radius * radius);
    }
    
    public override string ToString()
    {
        return $"[{x},{y}]";
    }

    public static GridCell FromWorldPos(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(Mathf.Clamp((worldPos.x / GridSize)+HalfGridBounds, 0, GridBounds));
        int y = Mathf.RoundToInt(Mathf.Clamp((worldPos.z / GridSize)+HalfGridBounds, 0, GridBounds));
        return new GridCell(x, y);
    }
    
    public static GridCell FromWorldPos(Vector2 pos)
    {
        int x = Mathf.RoundToInt(Mathf.Clamp((pos.x / GridSize)+HalfGridBounds, 0, GridBounds));
        int y = Mathf.RoundToInt(Mathf.Clamp((pos.y / GridSize)+HalfGridBounds, 0, GridBounds));
        return new GridCell(x, y);
    }

    public static GridCell operator +(GridCell a, GridCell b) => new (a.x + b.x, a.y + b.y);
    public static GridCell operator -(GridCell a, GridCell b) => new (a.x - b.x, a.y - b.y);

    #region Equality
    public bool Equals(GridCell other)
    {
        return x == other.x && y == other.y;
    }

    public override bool Equals(object obj)
    {
        return obj is GridCell other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }
    #endregion
}
