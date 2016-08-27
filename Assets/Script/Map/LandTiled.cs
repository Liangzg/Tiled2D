/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using Tiled2Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public enum ELandStatus { Self , Friendly , Enemy , Select , None}

/// <summary>
/// 单一地块逻辑
/// <para>创建时间：2016-08-23</para>
/// </summary>
public class LandTiled : ITiledTrigger
{
    //Tiled块的宽高
    public static Vector2 TileRange = new Vector2(64, 32);

    public static int halfTiledWidth = (int)TileRange.x/2;
    public static int halfTiledHeight = (int) TileRange.y/2;

    //上层地图与图块之间的映射
    private TiledData tileMap;

    //状态贴图
    private Image imgStatus;
    //当前的土地状态
    private ELandStatus curState;
    private GameObject mainObj;

    #region ----------------Public Attribute-------------------------
    /// 是否激活显示
    public bool Active { get; set; }
    //世界坐标
    public Vector3 WorldPosition { get; set; }
    /// <summary>
    /// 地图系内的坐标
    /// </summary>
    public Vector2 MapPoint
    {
        get { return Vector2.zero;}
    }
    /// <summary>
    /// 当前土地的状态
    /// </summary>
    public ELandStatus CurrentState { get { return curState;} }

    public TiledData Tiled { get { return tileMap;} }

    public int TiledIndex { get { return tileMap.Index; } }
    #endregion

    public LandTiled(TiledData td)
    {
        this.tileMap = td;
    }

    #region --------------Public Method-----------------------
    /// <summary>
    /// 初始化地表
    /// </summary>
    public void Initlizate(GameObject mainGO , SceneTiled parent)
    {
        mainObj = mainGO;
        mainObj.name += "_" + TiledIndex;
        mainObj.transform.localScale = Vector3.one;
        mainObj.transform.position = WorldPosition - Vector3.up * halfTiledHeight;
        mainObj.transform.SetParent(parent.MainObj.transform);

        imgStatus = mainObj.GetComponent<Image>();
        this.SetTiledState(ELandStatus.None);
#if UNITY_EDITOR
        //debug
        if (MapDebug.DebugLandTiled)
        {
            LandTiledGizmo ltg = mainObj.AddComponent<LandTiledGizmo>();
            ltg.mainTiled = this;
            ltg.TiledWidth = (int)TileRange.x;
            ltg.TiledHeight = (int)TileRange.y;
            ltg.rootScene = parent;
        }
#endif
    }

    public void OnTiledEnter(ITiledTrigger tiled)
    {
        throw new System.NotImplementedException();
    }

    public void OnTiledStay(ITiledTrigger tiled)
    {
        throw new System.NotImplementedException();
    }

    public void OnTiledExit(ITiledTrigger tiled)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 设置当前地块的状态
    /// </summary>
    /// <param name="state"></param>
    public void SetTiledState(ELandStatus state)
    {
        if (state == ELandStatus.Self)
            imgStatus.color = Color.green;
        else if (state == ELandStatus.Friendly)
            imgStatus.color = Color.blue;
        else if (state == ELandStatus.Enemy)
            imgStatus.color = Color.red;
        else if (state == ELandStatus.Select)
            imgStatus.color = Color.yellow;
        else
        {
            imgStatus.color = Color.white;
        }

#if UNITY_EDITOR
        if(!MapDebug.DebugLandTiled)
            imgStatus.enabled = state != ELandStatus.None;
#endif
        curState = state;
    }

    public void OnDestroy()
    {
        GameObject.Destroy(mainObj);
    }
    #endregion
}

