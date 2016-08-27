/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using System;
using UnityEngine;

/// <summary>
/// 坐标系转化，用于就单个图标转化成世界地图坐标
/// <para>创建时间：2016-08-17</para>
/// </summary>
public class CoordinationConvert
{
    //地图坐标原点
    public static Vector3 OrginMapPoint = new Vector3(MapCreater.MapRow * SceneTiled.halfSceneWidth, 0);
    //模板场景原点
    public static Vector3 OrginScenePoint = new Vector3(SceneTiled.SceneRow * LandTiled.halfTiledWidth, 0);
    /// <summary>
    /// 屏幕坐标转化为世界地图的位置
    /// </summary>
    /// <param name="sceneX">场景模块地图相对位置X</param>
    /// <param name="sceneY">场景模块地图相对位置Y</param>
    /// <returns></returns>
    public static Vector2 SceneCamToWorldMapPoint(int sceneX, int sceneY)
    {
        return SceneCamToWorldMapPoint(new Vector3(sceneX, sceneY));
    }
    /// <summary>
    /// 屏幕坐标转化为世界地图的位置
    /// </summary>
    public static Vector2 SceneCamToWorldMapPoint(Vector2 screenPoint)
    {
        //真实3D世界坐标
        Vector3 worldPoint = MapCamera.ScreenToWorldPoint(screenPoint);
        
        //相对的场景地图坐标
        Vector2 scenePoint = SceneWorldToMapPoint(worldPoint - Vector3.right * SceneTiled.halfSceneWidth);
        //相对场景地图的世界坐标
        Vector3 sceneWorldPoint = SceneMapToWorld3D(scenePoint);
        //具体地块Tiled的地图坐标
        return TiledWorldToMapPoint(worldPoint , sceneWorldPoint);
    }
    /// <summary>
    /// 屏幕坐标转化为场景地图坐标
    /// </summary>
    /// <param name="screenPoint"></param>
    /// <returns></returns>
    public static Vector2 SceneCamToSceneMapPoint(Vector2 screenPoint)
    {
        //真实3D世界坐标
        Vector3 worldPoint = MapCamera.ScreenToWorldPoint(screenPoint);
        //相对的场景地图坐标
        return SceneWorldToMapPoint(worldPoint - Vector3.right * SceneTiled.halfSceneWidth);
    }
    #region -------------------地图坐标转3D世界坐标-------------------------------------
    /// <summary>
    /// 地图坐标系转换到3D世界坐标系
    /// </summary>
    /// <param name="mapPosX">地图坐标系X</param>
    /// <param name="mapPosY">地图坐标系Y</param>
    /// <returns></returns>
    private static Vector2 mapPosToWorld3D(int mapPosX, int mapPosY , Vector2 relativeOrgin , int halfWidth , int halfHeight)
    {
        int offsetNX = -halfWidth; // N点在像素坐标系X轴的偏移

        //地图坐标系的点计算（Px, Py） ,原点O(x,y)
        int worldX = (int)relativeOrgin.x + halfWidth * mapPosX + offsetNX * mapPosY;
        int worldY = (int)relativeOrgin.y + halfHeight * (mapPosX + mapPosY);
        
        return new Vector2(worldX , -worldY);
    }

    /// <summary>
    /// 场景块转换成大地图坐标
    /// </summary>
    /// <param name="mapPos"></param>
    /// <returns></returns>
    public static Vector2 SceneMapToWorld3D(Vector2 mapPos)
    {
        return mapPosToWorld3D((int)mapPos.x , (int)mapPos.y , OrginMapPoint , SceneTiled.halfSceneWidth , SceneTiled.halfSceneHeight);
    }

    /// <summary>
    /// 土地单元块转换成场景地图块坐标
    /// </summary>
    /// <param name="tiledRelativePos">基于场景模块的相对坐标</param>
    /// <returns></returns>

    public static Vector2 TiledMapToWorld3D(Vector2 tiledMapPos, Vector2 parentMapPos)
    {
        //Scene地图相对世界大地图的坐标位置
        Vector2 sceneOrginPos = SceneMapToWorld3D(parentMapPos);

        //地图块相对Scene地图的坐标
        Vector2 relativeMapPoint = tiledMapPos - new Vector2(parentMapPos.x * SceneTiled.SceneRow , parentMapPos.y * SceneTiled.SceneColumn);
        Vector2 tiledToScenePos = mapPosToWorld3D((int)relativeMapPoint.x, (int)relativeMapPoint.y, OrginScenePoint,
                                                LandTiled.halfTiledWidth, LandTiled.halfTiledHeight);

        return sceneOrginPos + tiledToScenePos;
    }
    #endregion


    #region -------------3D世界坐标转地图坐标------------------------


    /// <summary>
    /// 3D世界坐标系转换到地图坐标系
    /// </summary>
    /// <param name="worldX">3D世界坐标系X</param>
    /// <param name="worldY">3D世界坐标系X</param>
    /// <returns></returns>
    private static Vector2 worldToMapPoint(float worldX, float worldY , Vector2 relativeOrgin, int halfWidth, int halfHeight)
    {
        //新的坐标系M,N
//        int halfWidth = SceneTiled.MaxSceneWidth / 2; // M点在像素坐标系X轴的偏移
//        int halfHeight = SceneTiled.MaxSceneHeight / 2;  // 像素坐标系Y轴的偏移
//        int offsetNX = -offsetMX; // N点在像素坐标系X轴的偏移
//
//        int offsetHeight = (int)((worldY - OrginMapPoint.y) / offsetY);
//        int mapY = (int)(worldX - OrginMapPoint.x - offsetMX * offsetHeight)/(offsetNX - offsetMX);
//        int mapX = (int)offsetHeight - mapY;

        worldX -= relativeOrgin.x;
        worldY -= relativeOrgin.y; 

        int mapX = (int)((worldX / halfWidth - worldY/halfHeight) * 0.5f);
        int mapY = -(int)((worldY / halfHeight + worldX / halfWidth) * 0.5f);
        
        return new Vector2(mapX, mapY);
    }

    public static Vector2 SceneWorldToMapPoint(Vector3 worldPos)
    {
        return worldToMapPoint(worldPos.x , worldPos.y , OrginMapPoint , SceneTiled.halfSceneWidth , SceneTiled.halfSceneHeight);
    }

    /// <summary>
    /// 地图块转换成世界大地图坐标
    /// </summary>
    /// <param name="tiledWorldPos">地图块相对场景地图的坐标</param>
    /// <returns></returns>
    public static Vector2 TiledWorldToMapPoint(Vector3 tiledWorldPos, Vector3 rootSceneWorldPos)
    {
        Vector3 relativeScenePos = tiledWorldPos - rootSceneWorldPos;
        //相对Scene的坐标
        Vector2 sceneMapWorldPoint = worldToMapPoint(relativeScenePos.x, relativeScenePos.y, OrginScenePoint, LandTiled.halfTiledWidth, LandTiled.halfTiledHeight);
        //Scene相对世界大地图的坐标
        Vector2 mapPoint = SceneWorldToMapPoint(rootSceneWorldPos);

        return new Vector2(mapPoint.x * SceneTiled.SceneRow + sceneMapWorldPoint.x,
                           mapPoint.y * SceneTiled.SceneColumn + sceneMapWorldPoint.y);
    }

    /// <summary>
    /// 土地块地图坐标转为上层场景坐标系
    /// </summary>
    /// <param name="tiledMapPoint"></param>
    /// <returns></returns>
    public static Vector2 TileMapToSceneMapPoint(Vector2 tiledMapPoint)
    {
        return new Vector2((int)tiledMapPoint.x / SceneTiled.SceneRow , (int)tiledMapPoint.y / SceneTiled.SceneColumn);
    }


    #endregion

    /// <summary>
    /// 屏幕坐标转换成Tile的下标
    /// </summary>
    /// <param name="screenPoint">屏幕坐标位置</param>
    /// <returns>x-SceneMap的Index , y - Tile 相对SceneMap的Index</returns>
    public static Vector2 ConvertTiledIndex(Vector3 screenPoint)
    {
        //世界地图坐标
        Vector2 mapPoint = SceneCamToWorldMapPoint(screenPoint);
        //父层Scene坐标
        Vector2 sceneMapPoint = TileMapToSceneMapPoint(mapPoint);

        int sceneIndex = Math.Abs((int)sceneMapPoint.x * MapCreater.MapRow) + Math.Abs((int)sceneMapPoint.y);
        //相对Scene的坐标
        Vector2 relativeMapPoint = mapPoint - new Vector2((int)sceneMapPoint.x * SceneTiled.SceneRow, (int)sceneMapPoint.y * SceneTiled.SceneColumn);

        int landIndex = (int)(relativeMapPoint.x + relativeMapPoint.y * SceneTiled.SceneColumn);

        return new Vector2(sceneIndex, landIndex);
    }
}
