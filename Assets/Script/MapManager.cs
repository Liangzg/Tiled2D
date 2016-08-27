/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;

/// <summary>
/// 描述：地图管理器
/// <para>创建时间：</para>
/// </summary>
public class MapManager
{

    private static MapManager mInstance;

    private MapManager() { }


    public static MapManager Instance
    {
        get
        {
            if(mInstance == null)   mInstance = new MapManager();
            return mInstance;
        }
    }

    /// <summary>
    /// 地图主相机
    /// </summary>
    public MapCamera CameraMap;
    /// <summary>
    /// 小地图相机
    /// </summary>
    public GameObject MiniMapCamera;


}
