using UnityEditor;
using UnityEngine;
/// <summary>
/// 土地块信息调试
/// </summary>
public  class LandTiledGizmo : MonoBehaviour
{
    public int TiledWidth , TiledHeight;

    public LandTiled mainTiled;
    public SceneTiled rootScene;

    Vector3 pos_w;
    Vector3 topLeft, topRight, bottomRight, bottomLeft;

    private Vector2 mapPoint, worldPoint;

    private string cooditions="";
    void Start()
    {
        pos_w = mainTiled.WorldPosition - Vector3.right * LandTiled.halfTiledWidth;
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

        if (MapDebug.IsMapPoint)
        {
            mapPoint = CoordinationConvert.TiledWorldToMapPoint(this.transform.position, rootScene.WorldPosition);
            cooditions = string.Format("({0},{1})", mapPoint.x, mapPoint.y);
        }
            
        if (MapDebug.IsWorldPoint)
        {
            worldPoint = CoordinationConvert.TiledMapToWorld3D(this.mapPoint , rootScene.MapPoint);
            cooditions += string.Format("/({0},{1})", worldPoint.x, worldPoint.y);
        }
    }

    public float WidthInScaled()
    {
        return TiledWidth * this.transform.lossyScale.x;
    }

    public float HeightInScaled()
    {
        return TiledHeight * this.transform.lossyScale.y;
    }

     void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    private void OnDrawGizmos()
    {
//        pos_w = mainTiled.WorldPosition - Vector3.right * LandTiled.halfTiledWidth;
//        topLeft = Vector3.zero + pos_w;
//        topRight = new Vector3(WidthInScaled(), 0) + pos_w;
//        bottomRight = new Vector3(WidthInScaled(), -HeightInScaled()) + pos_w;
//        bottomLeft = new Vector3(0, -HeightInScaled()) + pos_w;
//
//        // To make gizmo visible, even when using depth-shader shaders, we decrease the z depth by the number of layers
//        float depth_z = -1.0f * 3;//this.NumLayers
//        pos_w.z += depth_z;
//        topLeft.z += depth_z;
//        topRight.z += depth_z;
//        bottomRight.z += depth_z;
//        bottomLeft.z += depth_z;

        
//        Vector2 worldPos = CoordinationConvert.TiledToWorld3DPoint(mapPos);
        GUI.color = Color.red;
        float x = (topLeft + (topRight - topLeft) * 0.25f).x;
        float y = (topLeft + (bottomLeft - topLeft) * 0.25f).y;

        Handles.Label(new Vector3(x, y) , cooditions);

    }
}
