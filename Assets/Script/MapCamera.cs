/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 描述：地图主相机
/// <para>创建时间：2016-08-17</para>
/// </summary>
[RequireComponent(typeof(Camera))]  
public class MapCamera : MonoBehaviour
{
    public static MapCamera Instance;
     


    private Camera mapCamera;

    //自动移动到目标点时的移动
    public float Speed = 10;
    //自动移动到目标点的最大时间量
    public float maxTime = 2;
    //目标位置
    public Vector2 targetPos;

    //移动完成时的回调
    public List<Action> Finish = new List<Action>();
    //移动改变时的委托
    public Action MoveChangeDel;

    public Vector3 depath;

    private Transform cacheTrans;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        mapCamera = this.GetComponent<Camera>();
        cacheTrans = this.transform;

        depath = this.gameObject.transform.position.z * Vector3.back;
    }

	// Use this for initialization
	void Start ()
	{
        depath = this.gameObject.transform.position.z * Vector3.back;
    }

	// Update is called once per frame
	void Update ()
	{


	}

    /// <summary>
    /// 移动一个相对距离
    /// </summary>
    /// <param name="offset">相对偏移量</param>
    public void Move(Vector3 offset)
    {
        this.gameObject.transform.Translate(offset , Space.World);

        if (MoveChangeDel != null) MoveChangeDel.Invoke();
    }

    /// <summary>
    /// 移动到指定位置
    /// </summary>
    /// <param name="pos">指定位置3D世界坐标位置</param>
    public void MoveTo(Vector3 pos)
    {
        this.targetPos = pos;

        this.StartCoroutine(moveToTarget());
    }


    private IEnumerator moveToTarget()
    {
        float distance = float.MaxValue;
        while (distance < 0.01f)
        {
            Vector3 curPosition = this.gameObject.transform.position;
            this.gameObject.transform.position = Vector3.Lerp(curPosition, targetPos, Speed * Time.deltaTime);

            distance = Vector3.Distance(curPosition, targetPos);
            yield return null;
        }        

        CallFinish();
    }
    
    /// <summary>
    /// 执行回调
    /// </summary>
    public void CallFinish()
    {
        foreach (Action action in Finish)
        {
            if(action != null)  action.Invoke();
        }
    }

    /// <summary>
    /// 屏幕坐标转换成世界坐标
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector3 ScreenToWorldPoint(Vector3 pos)
    {
        Vector3 depth = Instance.depath;
        Ray ray = Instance.mapCamera.ScreenPointToRay(pos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
           Vector3 viewPos = MapMainCamera.WorldToViewportPoint(hit.transform.position);
           depth = Vector3.forward* viewPos.z;
        }
        return Instance.mapCamera.ScreenToWorldPoint(pos + depth);
    }


    public static Camera MapMainCamera { get { return Instance.mapCamera;} }
}
