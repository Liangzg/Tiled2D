/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界地图中的场景数据
/// </summary>
public class WorldSceneData
{
    /// <summary>
    /// 世界地图中的索引
    /// </summary>
    public byte WorldIndex;
    /// <summary>
    /// 场景ID
    /// </summary>
    public byte SceneId;
}


/// <summary>
/// Tiled单元块在场景中的数据
/// </summary>
public class TiledData
{
    public byte SceneIndex;
    /// <summary>
    /// Tiled在Scene中的索引
    /// </summary>
    public int Index;

    public byte TiledId;
}

/// <summary>
/// 地图数据处理
/// </summary>

public class MapData
{
    private WorldSceneData[] worldScenes;

    private static MapData mInstance;

    //场景Tiled缓存池
    public HashSet<int > SceneMap = new HashSet<int>();
    public List<SceneTiled> SceneTilePool = new List<SceneTiled>();

    private ArrayList landTileds = new ArrayList(SceneTiled.SceneColumn * SceneTiled.SceneRow);
    private MapData() { }

    public static int MaxSceneTiledCount = 4;

    public static MapData Instance
    {
        get
        {
            if (mInstance == null) mInstance = new MapData();
            return mInstance;
        }
    }

    /// <summary>
    /// 加载世界地图
    /// </summary>
    /// <param name="path"></param>
    public void LoadWorldMap(string path)
    {
        
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="worldmaps"></param>
    public void Initilize(int[] worldmaps)
    {
        worldScenes = new WorldSceneData[worldmaps.Length];

        for (int i = 0; i < worldScenes.Length; i++)
        {
            WorldSceneData wsd = new WorldSceneData();
            wsd.WorldIndex = (byte)i;
            wsd.SceneId = (byte) worldmaps[i];

            worldScenes[i] = wsd;
        }

        Debug.Log("scene count : "+ worldScenes);
    }

    /// <summary>
    /// 获得指定下标的SceneData
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public WorldSceneData this[int index]
    {
        get
        {
            if (index >= worldScenes.Length)
            {
                Debug.LogWarning("MapData index is out range ! index : " + index);
                return null;
            }
            return worldScenes[index];
        }
    }


    public WorldSceneData[] AllSceneDatas
    {
        get { return worldScenes;}
    }

    #region --------------Scene Map Data-----------------------------

    /// <summary>
    /// 获得一个缓存的SceneTiled对应
    /// </summary>
    /// <param name="sceneData"></param>
    /// <returns></returns>
    public SceneTiled SpwanSceneTiled(WorldSceneData sceneData)
    {
        if (!SceneMap.Contains(sceneData.WorldIndex))
        {
            SceneTiled sceneTile = new SceneTiled(sceneData);
            SceneTilePool.Add(sceneTile);

            if(SceneTilePool.Count > MaxSceneTiledCount)   
                DestroyFirst();

            SceneMap.Add(sceneData.WorldIndex);
        }
        else
        {
            //重新排序
            SceneTiled sceneTiled = null;
            for (int i = SceneTilePool.Count - 1; i >= 0; i--)
            {
                if (SceneTilePool[i].WorldMapIndex == sceneData.WorldIndex)
                {
                    sceneTiled = SceneTilePool[i];
                    SceneTilePool.RemoveAt(i);
                    break;
                }
            }
            SceneTilePool.Add(sceneTiled);
        }

        return SceneTilePool[SceneTilePool.Count - 1];
    }

    /// <summary>
    /// 是否已缓存Tiled
    /// </summary>
    /// <param name="sceneData"></param>
    /// <returns>true表示已存在</returns>
    public bool HasCacheTiled(WorldSceneData sceneData)
    {
        if (sceneData == null) return false;
        
        return SceneMap.Contains(sceneData.WorldIndex);
    }

    /// <summary>
    /// 获取SceneTiled
    /// </summary>
    /// <param name="sceneMapPoint"></param>
    /// <returns></returns>
    public SceneTiled GetSceneTiled(Vector2 sceneMapPoint)
    {
        int index = Math.Abs((int)sceneMapPoint.x*MapCreater.MapRow) + Math.Abs((int)sceneMapPoint.y);
        return GetSceneTiled(index);
    }

    public SceneTiled GetSceneTiled(int sceneWorldIndex)
    {
        if (SceneMap.Contains(sceneWorldIndex))
        {
            for (int i = SceneTilePool.Count - 1; i >= 0; i--)
            {
                if (SceneTilePool[i].WorldMapIndex == sceneWorldIndex)
                    return SceneTilePool[i];
            }
        }
        Debug.LogWarning("<<MapData , GetSceneTiled>> Cant find index ! index : " + sceneWorldIndex);
        return null;
    }

    /// <summary>
    /// 销毁缓存SceneTiled
    /// </summary>
    /// <param name="sceneData"></param>
    public void DestroySceneTiled(WorldSceneData sceneData)
    {
        if(!SceneMap.Contains(sceneData.WorldIndex)) return;

        for (int i = SceneTilePool.Count - 1; i >= 0; i--)
        {
            if (SceneTilePool[i].WorldMapIndex == sceneData.WorldIndex)
            {
                SceneTilePool[i].Destroy();
                SceneTilePool.RemoveAt(i);
                break;
            }
        }
        
        SceneMap.Remove(sceneData.WorldIndex);
    }


    public void DestroyFirst()
    {
        SceneTiled sceneTiled = SceneTilePool[0];
        SceneMap.Remove(sceneTiled.WorldMapIndex);
        sceneTiled.Destroy();
        SceneTilePool.RemoveAt(0);
    }
    #endregion


    #region ----------------------土地数据-------------------------------



    /// <summary>
    /// 获得土地块
    /// </summary>
    /// <param name="screenPos">屏幕坐标系的位置</param>
    /// <returns></returns>
    public LandTiled FindLandTiled(Vector3 screenPos)
    {
        //世界地图坐标
        Vector2 mapPoint = CoordinationConvert.SceneCamToWorldMapPoint(screenPos);
        return FindLandTiledByMapPoint(mapPoint);
    }

    /// <summary>
    /// 根据土地的地图坐标获取LandTiled对象
    /// </summary>
    /// <param name="mapPoint">地图坐标系位置</param>
    /// <returns></returns>
    public LandTiled FindLandTiledByMapPoint(Vector2 mapPoint)
    {
        //父层Scene坐标
        Vector2 sceneMapPoint = CoordinationConvert.TileMapToSceneMapPoint(mapPoint);

        SceneTiled sceneTiled = GetSceneTiled(sceneMapPoint);

        if (sceneTiled == null) return null;
        //相对Scene的坐标
        Vector2 relativeMapPoint = mapPoint - new Vector2((int)sceneMapPoint.x * SceneTiled.SceneRow, (int)sceneMapPoint.y * SceneTiled.SceneColumn);

        int landIndex = (int)(relativeMapPoint.x + relativeMapPoint.y * SceneTiled.SceneColumn);

        return sceneTiled[landIndex];
    }


    #endregion
}
