using UnityEngine;
using System.Collections;
using System;

public class FlightCtrl : MonoBehaviour {
    readonly string GAME_OBJ_NM = "PlaneGameObj";
    readonly string PLUGIN_CLASS_PATH = "com.github.scarviz.flightctrl4unity.ServiceCtrl";
    
    public static string ACTION_DOWN = "ACTION_DOWN";

    private Plane plane;
    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
    {
        Debug.Log("Start");
        StartService();

        plane = GetComponentInChildren<Plane>();
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

    /// <summary>
    /// Android Nativeプラグインからのコールバック用
    /// </summary>
    /// <param name="mess"></param>
    public void onCallBack(string mess)
    {
        Debug.Log("Call CallbackMess");

        if (ACTION_DOWN == mess)
        {
            Debug.Log("action down");
            plane.Speed(Plane.Ctrl.SPEED_UP);
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
