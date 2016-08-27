/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System.Collections;
using UnityEngine;

/// <summary>
/// 场景地图，单个的模块地图
/// <para>创建时间：2016-08-17</para>
/// </summary>
public class SceneTiled
{
    // 场景地图的列数
    public const int SceneRow = 20;
    //场景地图的行数
    public const int SceneColumn = 20;
    
    //整个地图的宽高
    public static int MaxSceneWidth = SceneRow * (int)LandTiled.TileRange.x;
    public static int MaxSceneHeight = SceneColumn * (int)LandTiled.TileRange.y;

    public static int halfSceneWidth = MaxSceneWidth / 2;
    public static int halfSceneHeight = MaxSceneHeight / 2 ;
    #region ------Private Attribute---

    private GameObject mainObj;
    /// <summary>
    /// 模块地图对应的tiled图块ID记录信息
    /// </summary>
    private int[] tileds;

    private Vector3 pos;
    //地图相对坐标
    private Vector2 mapPoint;

    private ArrayList landTileds = new ArrayList(SceneColumn * SceneRow);

    #endregion


    #region ---------Public Attribute----------------


    public bool Active
    {
        get { return MainObj.activeInHierarchy; }
        set { MainObj.SetActive(value); }
    }

    public int Width
    {
        get{return MaxSceneWidth;}
    }

    public int Height { get {   return MaxSceneHeight;  }   }
    /// <summary>
    /// 3D世界坐标
    /// </summary>
    public Vector3 WorldPosition
    {
        get { return pos; }

        set
        {
            pos = value;
            if (mainObj != null) mainObj.transform.position = value;
        }   
    }

    /// <summary>
    /// 地图坐标系的坐标
    /// </summary>
    public Vector2 MapPoint { get { return mapPoint; } }
    

    public GameObject MainObj { get { return mainObj;} }

    //Scene在world map中的数据
    public WorldSceneData SceneData { get; private set; }

    //世界地图中的索引
    public int WorldMapIndex { get { return SceneData.WorldIndex; } }

    #endregion

    public SceneTiled(WorldSceneData sceneData)
    {
        this.SceneData = sceneData;
    }

    #region -----------Public Method-----------------

    /// <summary>
    /// 初始化SceneTiled模块地图
    /// </summary>
    /// <param name="gObj">主GameObject</param>
    /// <param name="worldMapIndex">世界地图中的索引</param>
    public void Initlizate(GameObject gObj)
    {
        mainObj = gObj;
        mainObj.name += "_" + WorldMapIndex;

#if UNITY_EDITOR
        if(MapDebug.DebugSceneTiled)
            gObj.AddComponent<SceneTiledGizmo>();
#endif
        gObj.transform.localScale = Vector3.one;
        gObj.transform.position = WorldPosition;

        mapPoint = CoordinationConvert.SceneWorldToMapPoint(WorldPosition);

        //构造场景内的地块信息
        landTileds.Clear();
        int halfWidth = LandTiled.halfTiledWidth;
        int halfHeight = LandTiled.halfTiledHeight;

        //        Vector3 orginPos = WorldPosition + new Vector3(halfSceneWidth - halfWidth,0);
        Vector3 orginPos = WorldPosition + new Vector3(halfSceneWidth , 0);

        for (int i = 0; i < SceneRow; i++)
        {
            int tiledRowIndex = i*SceneRow;
            for (int j = 0; j < SceneColumn; j++)
            {
                int tiledIndex = tiledRowIndex + j;
                //计算世界坐标中的位置
                float tiledX = halfWidth * j - halfWidth * i;
                float tiledY = -halfHeight * j - halfHeight * i;
                
                Vector3 position = new Vector3(tiledX,tiledY);
                position += orginPos;
                //构造记录映射
                TiledData td = new TiledData();
                //td.TiledId = ;
                td.SceneIndex = (byte)WorldMapIndex;
                td.Index = tiledIndex;
                 
                //实例土地
                LandTiled landTiled = new LandTiled(td);
                landTiled.WorldPosition = position;
                landTileds.Add(landTiled);

                MapCreater.Instance.LoadDel("Assets/Prefabs/LandSprite.prefab", obj =>
                {
                    landTiled.Initlizate(obj , this);
                });
                
            }
        }
    }

    /// <summary>
    /// 获得场景中的Tiled的ID
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int GetSceneTiledId(float x, float y)
    {
        //todo
        return 0;
    }

    /// <summary>
    /// 指定TiledID图块中心点的位置
    /// </summary>
    /// <param name="tiledId">图块Tiled的ID</param>
    /// <returns></returns>
    public Vector2 GetScenePosition(int tiledId)
    {
        return Vector2.zero;
    }


    public void Destroy()
    {
        foreach (object landObj in landTileds)
        {
            LandTiled tiled = landObj as LandTiled;
            tiled.OnDestroy();
        }
        landTileds.Clear();

        GameObject.Destroy(mainObj);
    }

    /// <summary>
    /// 获得土地块对象
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public LandTiled this[int index]
    {
        get
        {
            if (index >= landTileds.Count || index < 0)
            {
                Debug.LogWarning("Cant find Land Tiled ! index is out rang . index : " + index);
                return null;
            }
            return landTileds[index] as LandTiled;
        }
    }
    #endregion

}