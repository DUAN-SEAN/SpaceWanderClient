using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

/// <summary>
/// 差值信息
/// </summary>
public struct DumpInfo
{
    public Vector3 Position;

    public Vector3 Forward;

    public float Dump;

}


public class MoveController : MonoBehaviour
{
    private Vector3 _endPos;
    private Vector3 _endForwoard;
    private Vector3 _startPos;
    private Vector3 _startForwoard;
    /// <summary>
    /// 动态差值时间
    /// </summary>
    private float _intervalTime = 0.2f;
    /// <summary>
    /// 差值起始时间
    /// </summary>
    private float _startTime;
    private long _interval = 1000000L;//0.1s
    /// <summary>
    /// 当前差值时间
    /// </summary>
    private long _currentDumpTime;
    /// <summary>
    /// 下一次获取队列时间
    /// </summary>
    private long _nextQueueTime;
    /// <summary>
    /// 差值信息队列
    /// </summary>
    private Queue<DumpInfo> _dumpInfoQue;
    /// <summary>
    /// 默认差值时间
    /// </summary>
    private const float _defaultDumpInterval = 0.2f;

    // Start is called before the first frame update
    void Awake()
    {

        _currentDumpTime = DateTime.Now.Ticks;

        _nextQueueTime = (DateTime.Now.Ticks) + _interval;

        _startPos = _endPos = transform.position;
        _startForwoard = _endForwoard = transform.forward;

        _dumpInfoQue = new Queue<DumpInfo>();
    }

    void Start()
    {

        _currentDumpTime = DateTime.Now.Ticks;

        _nextQueueTime = (DateTime.Now.Ticks) + _interval;

        _startPos = _endPos = transform.position;
        _startForwoard = _endForwoard = transform.forward;


    }

    // 每一帧调用
    void Update()
    {
        if (_dumpInfoQue == null) return;
        _currentDumpTime = DateTime.Now.Ticks;
        if (_currentDumpTime > _nextQueueTime)
        {
            Vector3 tempPos = transform.position;
            Vector3 tempForward = transform.forward;
            float tempDumpTime = _defaultDumpInterval;//0.2s
            if (_dumpInfoQue.Count > 0)
            {
                var dump = _dumpInfoQue.Dequeue();
                tempPos = dump.Position;
                tempForward = dump.Forward;
                tempDumpTime = dump.Dump;
            }
            
            SetPosition(tempPos);
            SetForward(tempForward);
            SetIntervalTime(tempDumpTime);
            _nextQueueTime += (long)(tempDumpTime*10000000f);
            //Debug.Log(_currentDumpTime);
        }

        _startTime += Time.deltaTime;
        if (_startTime > _intervalTime) return;
        var newPos = Vector3.Lerp(_startPos, _endPos, _startTime / _intervalTime);
        var newForwoard = Vector3.Slerp(_startForwoard, _endForwoard, _startTime / _intervalTime);
        transform.forward = newForwoard;
        transform.position = newPos;
    }

    

    /// <summary>
    /// 添加差值项
    /// </summary>
    /// <param name="dumpInfo"></param>
    public void AddDumpInfo(DumpInfo dumpInfo)
    {
        if (_dumpInfoQue.Count >= 5)
        {
            Debug.Log("Move队列被清空");
            _dumpInfoQue.Clear();
        }
        _dumpInfoQue?.Enqueue(dumpInfo);
    }
    /// <summary>
    /// 设置位置差值
    /// </summary>
    /// <param name="pos"></param>
    public void SetPosition(Vector3 pos)
    {
        _endPos = pos;
        _startPos = transform.position;
    }
    /// <summary>
    /// 设置朝向差值
    /// </summary>
    /// <param name="forward"></param>
    public void SetForward(Vector3 forward)
    {
        _endForwoard = forward;
        _startForwoard = transform.forward;
    }
    /// <summary>
    /// 设置差值区间间隔
    /// </summary>
    /// <param name="interval"></param>
    public void SetIntervalTime(float interval)
    {
        _intervalTime = interval;
        _startTime = 0f;
    }
    //清空队列
    public void Clear()
    {
        _dumpInfoQue?.Clear();
    }
}
