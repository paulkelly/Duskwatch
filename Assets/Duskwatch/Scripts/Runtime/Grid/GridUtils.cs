using UnityEngine;

public static class GridUtils
{
    public static Vector3 GetCenterPosition(Vector3 position, Bounds bounds)
    {
        int sizeX = Mathf.CeilToInt(bounds.size.x / GridCell.GridSize);
        int sizeY = Mathf.CeilToInt(bounds.size.z / GridCell.GridSize);
            
        int halfGridSize = (int)(GridCell.GridSize / 2); 
        Vector3 worldPos = GetWorldPositionFromCell(GridCell.FromWorldPos(position));
            
        bool xEven = sizeX % 2 == 0;
        bool yEven = sizeY % 2 == 0;
        if (xEven)
        {
            if (worldPos.x > position.x)
            {
                worldPos.x -= halfGridSize;
            }
            else
            {
                worldPos.x += halfGridSize;
            }
        }

        if (yEven)
        {
            if (worldPos.z > position.z)
            {
                worldPos.z -= halfGridSize;
            }
            else
            {
                worldPos.z += halfGridSize;
            }
        }

        //float worldY = SampleTerrainHeight(new Vector3(worldX, 0, worldZ));
        float worldY = 0;
        return new Vector3(worldPos.x, worldY, worldPos.z);
    }
    
    public static Vector3 GetWorldPositionFromCell(GridCell cell)
    {
        float halfGrid = GridCell.GridSize / 2f;
        float worldX = ((cell.x-GridCell.HalfGridBounds) * GridCell.GridSize)+halfGrid;
        float worldZ = ((cell.y-GridCell.HalfGridBounds) * GridCell.GridSize)+halfGrid;
        //float worldY = SampleTerrainHeight(new Vector3(worldX, 0, worldZ));
        float worldY = 0;
        return new Vector3(worldX, worldY, worldZ);
    }
}
