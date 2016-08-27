/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tiled2Unity;

/// <summary>
/// 描述：地图创建器，用于动态生成地图
/// <para>创建时间：2016-08-17</para>
/// </summary>
public class MapCreater
{
    private static MapCreater instance;

    public static MapCreater Instance
    {
        get { if(instance == null)  instance = new MapCreater(); return instance;}
    }

    private MapCreater() { }
    // 外部加载委托
    public Action<string , Action<GameObject>> LoadDel; 

    // 场景地图的列数
    public const int MapRow = 10;
    //场景地图的行数
    public const int MapColumn = 10;


    #region -------Private Attribute---------------------------
    //上一次记录的CamPos
    private Vector3 lastMapCamPos;

    private GameObject root;
    //屏幕一半尺寸对应地图Tiled的数量
    private int halfScreenWidthTiledCount, halfScreenHeightTiledCount;

    private WorldSceneData[] lastTileds;

    private List<LandTiled> testLands = new List<LandTiled>();
    #endregion


    #region ----------Public Attribute-------------------

    public static GameObject MapRoot
    {
        get
        {
            if (Instance.root == null)
            {
                GameObject rootObj = new GameObject("_mapRoot");
                rootObj.transform.localScale = Vector3.one;
                rootObj.transform.position = Vector3.zero;

                Canvas mCanvas = rootObj.AddComponent<Canvas>();
                mCanvas.renderMode = RenderMode.WorldSpace;
                mCanvas.planeDistance = 1000;

                Instance.root = rootObj;
            }

            return Instance.root;
        }
    }
    #endregion


    #region -----------Private Method----------------

    /// <summary>
    /// 获得场景Tile的资源名
    /// </summary>
    /// <param name="sceneTileId"></param>
    /// <returns></returns>
    private string getSceneTileAssetName(int sceneTileId)
    {
        return string.Format("Assets/Tiled2Unity/Prefabs/map{0:00}.prefab" , sceneTileId);
    }

    /// <summary>
    /// 估算屏幕内所占格子的数量
    /// </summary>
    private void culScreenTiledCount()
    {
        if (halfScreenWidthTiledCount != 0) return;

        Vector2 mapLeftTop = CoordinationConvert.SceneCamToWorldMapPoint(Vector2.zero);
        Vector2 mapRightTop = CoordinationConvert.SceneCamToWorldMapPoint(new Vector2(Screen.width, 0));
        Vector2 mapLeftButtom = CoordinationConvert.SceneCamToWorldMapPoint(new Vector2(0, Screen.height));

        halfScreenWidthTiledCount = (int)(mapRightTop.x - mapLeftTop.x)/2 + 1;
        halfScreenHeightTiledCount = (int) (mapLeftButtom.y - mapLeftTop.y)/2 + 1;
    }
    #endregion


    #region ----------Public Method---------------------
    /// <summary>
    /// 合并地图
    /// ps:由于每块SceneTiled的位置都是根据下标计算的
    /// </summary>
    /// <param name="tileds">tiled列表</param>
    public void MergeMap(WorldSceneData[] tileds)
    {
        int halfWidth = SceneTiled.MaxSceneWidth / 2;  //每块SceneTiled的宽度的一半
        int halfHeight = SceneTiled.MaxSceneHeight / 2;    //每块SceneTiled的高度的一半

        MapData dataPool = MapData.Instance;
        
        for (int i = 0; i < tileds.Length; i++)
        {
            if (dataPool.HasCacheTiled(tileds[i]))
            {
                SceneTiled cacheTiled = dataPool.SpwanSceneTiled(tileds[i]);
                cacheTiled.Active = true;
                continue;
            }
            SceneTiled tiled = dataPool.SpwanSceneTiled(tileds[i]);
            int tiledIndex = tileds[i].WorldIndex;      //地图世界坐标中的索引
            //计算世界坐标中的位置
            int column = tiledIndex / MapRow;
            float tiledX = halfWidth * column - halfWidth * (tiledIndex % MapRow);
            float tiledY = -halfHeight * column - halfHeight * (tiledIndex % MapRow);

            tiled.WorldPosition = new Vector3(CoordinationConvert.OrginMapPoint.x + tiledX,
                                         CoordinationConvert.OrginMapPoint.y + tiledY);

            //异步加载资源
            string sceneAssetName = getSceneTileAssetName(tileds[i].SceneId);
            LoadDel(sceneAssetName, gObj =>
            {
                if (gObj == null)
                {
                    Debug.LogWarning("<<MergeMap>> Can find Scene tiled ! Name is " + sceneAssetName);
                    return;
                }
                gObj.transform.SetParent(MapRoot.transform);

                SceneTiled sceneTiled = dataPool.GetSceneTiled(tileds[i].WorldIndex);
                sceneTiled.Initlizate(gObj);
            });
        }

        //删除被隐藏的
        if (lastTileds != null)
        {
            for (int i = lastTileds.Length - 1; i >= 0; i--)
            {
                bool isTrue = false;
                for (int j = 0; j < tileds.Length; j++)
                {
                    if (lastTileds[i].WorldIndex == tileds[j].WorldIndex)
                    {
                        isTrue = true;
                        break;
                    }
                }

                //隐藏超出屏幕的
                if (!isTrue)
                {
                    SceneTiled tiled =  dataPool.GetSceneTiled(lastTileds[i].WorldIndex);
                    tiled.Active = false;
                }
            }
        }
        lastTileds = tileds;
    }

    /// <summary>
    /// 主相机移动时的操作
    /// </summary>
    public void CameraChange()
    {
//        this.culScreenTiledCount();
//        Debug.Log("halfTileW:" + halfScreenWidthTiledCount + " , halfTileH:" + halfScreenHeightTiledCount);
        //计算相机中心个点的世界坐标位置
//        Vector2 center = new Vector2(Screen.width / 2 , Screen.height / 2);
        
        //计算TileIndex索引
        Dictionary<int , WorldSceneData> worldSceneDatas = new Dictionary<int, WorldSceneData>();
//        Vector2 mapPos = CoordinationConvert.SceneCamToSceneMapPoint(center);
//        addSceneTiled(mapPos, worldSceneDatas);
        //        //计算横向
        //        for (int i = 0; i < 2; i++)
        //        {
        //            //横向
        //            float nearX =  mapPos.x - (1 - (i % 2) * 2) * halfScreenWidthTiledCount;
        //            float nearY = mapPos.y + (1 - (i % 2) * 2) * halfScreenWidthTiledCount;
        //            Vector2 nearMapPos = new Vector2(nearX , nearY);
        //            addSceneTiled(nearMapPos, worldSceneDatas);
        //
        //            //纵向
        //            nearX = mapPos.x - (1 - (i % 2) * 2) * halfScreenHeightTiledCount;
        //            nearY = mapPos.y - (1 - (i % 2) * 2) * halfScreenHeightTiledCount;
        //            nearMapPos = new Vector2(nearX, nearY);
        //            addSceneTiled(nearMapPos, worldSceneDatas);
        //        }
        //
        //        //计算四个边角
        //        addShowTiled(Vector2.zero , worldSceneDatas);
        //        addShowTiled(new Vector2(Screen.width , 0), worldSceneDatas);
        //        addShowTiled(new Vector2(0 , Screen.height), worldSceneDatas);
        //        addShowTiled(new Vector2(Screen.width , Screen.height), worldSceneDatas);
        
        Vector2 mapLeftTop = CoordinationConvert.SceneCamToWorldMapPoint(Vector2.zero);
        Vector2 mapLeftBottom = CoordinationConvert.SceneCamToWorldMapPoint(new Vector2(0, Screen.height));
        Vector2 mapRightTop = CoordinationConvert.SceneCamToWorldMapPoint(new Vector2(Screen.width, 0));
        Vector2 mapRightBottom = CoordinationConvert.SceneCamToWorldMapPoint(new Vector2(Screen.width, Screen.height));

        Debug.Log("Map LT: " + mapLeftTop + " , LB: " + mapLeftBottom + " , RT: " + mapRightTop + " , RB:" + mapRightBottom);

        int count = (int)(mapRightTop.x - mapLeftTop.x) + 2;
        for (int i = 0; i <= count; i++)
        {
            //上边缘
            Vector2 newMapPoint = mapLeftTop + new Vector2( i +1, -i + 1);
            Vector2 sceneMapPoint = CoordinationConvert.TileMapToSceneMapPoint(newMapPoint);
            addSceneTiled(sceneMapPoint , worldSceneDatas);

            //下边缘
            newMapPoint = mapLeftBottom + new Vector2(i-1, -i);
            sceneMapPoint = CoordinationConvert.TileMapToSceneMapPoint(newMapPoint);
            addSceneTiled(sceneMapPoint, worldSceneDatas);
        }

        //左边缘
        count = (int) (mapLeftTop.x - mapLeftBottom.x) + 1;
        for (int i = 0; i <= count; i++)
        {
            Vector2 newMapPoint = mapLeftTop - new Vector2(i , i - 2);
            Vector2 sceneMapPoint = CoordinationConvert.TileMapToSceneMapPoint(newMapPoint);
            addSceneTiled(sceneMapPoint, worldSceneDatas);
        }

        //右边缘
        count = (int)(mapRightTop.x - mapRightBottom.x) + 2;
        for (int i = 0; i <= count; i++)
        {
            Vector2 newMapPoint = mapRightTop - new Vector2(i - 2 , i);
            Vector2 sceneMapPoint = CoordinationConvert.TileMapToSceneMapPoint(newMapPoint);
            addSceneTiled(sceneMapPoint, worldSceneDatas);
        }

        WorldSceneData[] wsdArr = new WorldSceneData[worldSceneDatas.Count];
        worldSceneDatas.Values.CopyTo(wsdArr , 0);

        this.MergeMap(wsdArr);

        //用于测试地图的可视区域
        //debugViewMap(mapLeftTop , mapLeftBottom , mapRightTop , mapRightBottom);
    }


    private void debugViewMap(Vector2 mapLeftTop , Vector2 mapLeftBottom , Vector2 mapRightTop , Vector2 mapRightBottom)
    {
        foreach (LandTiled tiled in testLands)
        {
            if (tiled == null) continue;

            tiled.SetTiledState(ELandStatus.None);
        }
        testLands.Clear();

        int count = (int)(mapRightTop.x - mapLeftTop.x) + 2;
        for (int i = 0; i <= count; i++)
        {
            //上边缘
            Vector2 newMapPoint = mapLeftTop + new Vector2(i + 1, -i + 1);
            LandTiled tile = MapData.Instance.FindLandTiledByMapPoint(newMapPoint);
            if (tile != null)
            {
                tile.SetTiledState(ELandStatus.Select);
                testLands.Add(tile);
            }

            //下边缘
            newMapPoint = mapLeftBottom + new Vector2(i - 1, -i);
            tile = MapData.Instance.FindLandTiledByMapPoint(newMapPoint);
            if (tile != null)
            {
                tile.SetTiledState(ELandStatus.Self);
                testLands.Add(tile);
            }
        }

        //左边缘
        count = (int)(mapLeftTop.x - mapLeftBottom.x) + 1;
        for (int i = 0; i <= count; i++)
        {
            Vector2 newMapPoint = mapLeftTop - new Vector2(i, i - 2);
            LandTiled tile = MapData.Instance.FindLandTiledByMapPoint(newMapPoint);
            if (tile != null)
            {
                tile.SetTiledState(ELandStatus.Friendly);
                testLands.Add(tile);
            }
        }

        //右边缘
        count = (int)(mapRightTop.x - mapRightBottom.x) + 2;
        for (int i = 0; i <= count; i++)
        {
            Vector2 newMapPoint = mapRightTop - new Vector2(i - 2, i);
            LandTiled tile = MapData.Instance.FindLandTiledByMapPoint(newMapPoint);
            if (tile != null)
            {
                tile.SetTiledState(ELandStatus.Enemy);
                testLands.Add(tile);
            }
        }
    }

    private void addShowTiled(Vector2 screenPos , Dictionary<int, WorldSceneData> worldSceneDatas)
    {
        Vector2 mapPos = CoordinationConvert.SceneCamToSceneMapPoint(screenPos);
        addSceneTiled(mapPos , worldSceneDatas);
    }

    private void addSceneTiled(Vector2 mapPos, Dictionary<int, WorldSceneData> worldSceneDatas)
    {
        int tileIndex = Math.Abs((int)mapPos.x * MapRow) + Math.Abs((int)mapPos.y);
        WorldSceneData wsd = MapData.Instance[tileIndex];

        if (wsd == null) return ;

        if (!worldSceneDatas.ContainsKey(tileIndex))
        {
            worldSceneDatas.Add(tileIndex, wsd);
        }
    }

    #endregion
}
