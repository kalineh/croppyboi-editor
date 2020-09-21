using UnityEngine;

public class PlotProxy
    : MonoBehaviour
{
    public int xCount;
    public int yCount;

    public void DrawGizmos()
    {
        for (int y = 0; y < yCount; ++y)
        {
            for (int x = 0; x < xCount; ++x)
            {
                var xOffset = 1.0f;
                var yOffset = 1.0f;

                var position = transform.position + new Vector3(xOffset * x, yOffset * y, 0.0f) + Vector3.one * 0.25f;
                var size = Vector3.one * 0.5f;

                Gizmos.DrawCube(position, size);
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        DrawGizmos();    
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        DrawGizmos();
    }


}
