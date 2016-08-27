using UnityEditor;
using UnityEngine;

public class SceneTiledGizmo : MonoBehaviour
{
    Vector3 pos_w;
    Vector3 topLeft, topRight, bottomRight, bottomLeft;

    public float WidthInScaled()
    {
        return SceneTiled.MaxSceneWidth * this.transform.lossyScale.x;
    }

    public float HeightInScaled()
    {
        return SceneTiled.MaxSceneHeight * this.transform.lossyScale.y;
    }

    private void OnDrawGizmosSelected()
    {
        pos_w = this.gameObject.transform.position;
        topLeft = Vector3.zero + pos_w;
        topRight = new Vector3(WidthInScaled(), 0) + pos_w;
        bottomRight = new Vector3(WidthInScaled(), -HeightInScaled()) + pos_w;
        bottomLeft = new Vector3(0, -HeightInScaled()) + pos_w;

        // To make gizmo visible, even when using depth-shader shaders, we decrease the z depth by the number of layers
        float depth_z = -1.0f * 3;//this.NumLayers
        pos_w.z += depth_z;
        topLeft.z += depth_z;
        topRight.z += depth_z;
        bottomRight.z += depth_z;
        bottomLeft.z += depth_z;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    private void OnDrawGizmos()
    {
        pos_w = this.gameObject.transform.position;
        topLeft = Vector3.zero + pos_w;
        topRight = new Vector3(WidthInScaled(), 0) + pos_w;
        bottomRight = new Vector3(WidthInScaled(), -HeightInScaled()) + pos_w;
        bottomLeft = new Vector3(0, -HeightInScaled()) + pos_w;

        // To make gizmo visible, even when using depth-shader shaders, we decrease the z depth by the number of layers
        float depth_z = -1.0f * 3;//this.NumLayers
        pos_w.z += depth_z;
        topLeft.z += depth_z;
        topRight.z += depth_z;
        bottomRight.z += depth_z;
        bottomLeft.z += depth_z;

        Vector2 mapPos = CoordinationConvert.SceneWorldToMapPoint(this.transform.position);
        Vector2 worldPos = CoordinationConvert.SceneMapToWorld3D(mapPos);
        GUI.color = Color.red;
        float x = (topLeft + (topRight - topLeft) * 0.25f).x;
        float y = (topLeft + (bottomLeft - topLeft) * 0.25f).y;
        Handles.Label(new Vector3(x, y),
                      string.Format("({0},{1})/({2},{3})", mapPos.x, mapPos.y, worldPos.x, worldPos.y));

    }
}