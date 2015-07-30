using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class FlightCtrl : MonoBehaviour {
    readonly string GAME_OBJ_NM = "PlaneGameObj";
    readonly string PLUGIN_CLASS_PATH = "com.github.scarviz.flightctrl4unity.ServiceCtrl";

    public static string ACTION_DOWN = "ACTION_DOWN";
    public static string ROLL = "ROLL";
    public static string PITCH = "PITCH";

    private Plane plane;
    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
    {
        Debug.Log("Start");
        StartService();

        plane = GetComponentInChildren<Plane>();
        _processing.Add(Plane.Ctrl.SPEED_UP, false);
        _processing.Add(Plane.Ctrl.ROLL, false);
        _processing.Add(Plane.Ctrl.PITCH, false);
    }

    /// <summary>
    /// アプリケーション一時停止状態の変更時処理
    /// </summary>
    /// <param name="pauseStatus">一時停止状態になったかどうか</param>
    void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log("OnApplicationPause");
        // 一時停止状態になった場合
        if (pauseStatus)
        {
            StopService();
        }
        // 再開された場合
        else
        {
            StartService();
        }
    }

    private Dictionary<Plane.Ctrl, bool> _processing = new Dictionary<Plane.Ctrl, bool>();
    /// <summary>
    /// 処理中かどうか
    /// </summary>
    /// <param name="ctrl">操作</param>
    /// <remarks>処理中かどうか</remarks>
    private bool GetProcessing(Plane.Ctrl ctrl)
    {
        lock (this)
        {
            return _processing[ctrl];
        }
    }
    /// <summary>
    /// 処理状態を設定する
    /// </summary>
    /// <param name="ctrl">操作</param>
    /// <param name="processing">処理状態</param>
    private void SetProcessing(Plane.Ctrl ctrl, bool processing)
    {
        lock (this)
        {
            _processing[ctrl] = processing;
        }
    }

    /// <summary>
    /// Android Nativeプラグインからのコールバック用
    /// </summary>
    /// <param name="mess"></param>
    public void onCallBack(string mess)
    {
        Debug.Log("Call CallbackMess");

        if (String.IsNullOrEmpty(mess)) {
            return;
        }
        
        var paramAry = mess.Trim().Split(',');
        if (paramAry == null || paramAry.Length <= 0)
        {
            Debug.Log("not found split string");
            return;
        }

        var key = paramAry[0];
        var val = 0f;
        if (1 < paramAry.Length) 
        {
            var paramVal = paramAry[1];
            if (!float.TryParse(paramVal, out val)) 
            {
                val = 0f;
            }
        }

        if (ACTION_DOWN == mess && !GetProcessing(Plane.Ctrl.SPEED_UP))
        {
            SetProcessing(Plane.Ctrl.SPEED_UP, true);

            Debug.Log("action down");
            plane.Speed(Plane.Ctrl.SPEED_UP);

            SetProcessing(Plane.Ctrl.SPEED_UP, false);
        }
        if (ROLL == key && !GetProcessing(Plane.Ctrl.ROLL))
        {
            SetProcessing(Plane.Ctrl.ROLL, true);

            Debug.Log("roll");
            var roll = (1 - 1 / (val + 1)) * 4;
            plane.Speed(Plane.Ctrl.ROLL, roll);

            SetProcessing(Plane.Ctrl.ROLL, false);
        }
        if (PITCH == key && !GetProcessing(Plane.Ctrl.PITCH))
        {
            SetProcessing(Plane.Ctrl.PITCH, true);

            Debug.Log("pitch");
            var pitch = (1 - 1 / (val + 1)) * 10;
            plane.Speed(Plane.Ctrl.PITCH, pitch);

            SetProcessing(Plane.Ctrl.PITCH, false);
        }
    }

    /// <summary>
    /// アプリケーション終了時処理
    /// </summary>
    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        StopService();
    }

    /// <summary>
    /// サービス開始処理
    /// </summary>
    private void StartService()
    {
        try
        {
            using (AndroidJavaObject clsPlugin = new AndroidJavaObject(PLUGIN_CLASS_PATH))
            {
                // サービス起動
                clsPlugin.Call("startService", GAME_OBJ_NM);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    /// <summary>
    /// サービス停止処理
    /// </summary>
    private void StopService()
    {
        try
        {
            using (AndroidJavaObject clsPlugin = new AndroidJavaObject(PLUGIN_CLASS_PATH))
            {
                // サービス停止
                clsPlugin.Call("stopService");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
}
