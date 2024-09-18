using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    const string API_BASE_URL = "http://localhost:8000/api/";
    private int userID = 0;       // 自分のユーザーID
    private string userName = ""; // 自分のユーザー名
    private int stageId = 0;

    // getプロパティで呼び出した初回時にインスタンス生成してstaticで保持
    private static NetworkManager instance;

    public static NetworkManager Instance
    {
        get
        {
            if(instance == null)
            {// nullのとき
                // GameObjectを生成して、NetworkManagerコンポーネントを追加
                GameObject gameObj = new GameObject("NetworkManager");
                instance = gameObj.AddComponent<NetworkManager>();

                // オブジェクトをシーン遷移時に削除しないようにする
                DontDestroyOnLoad(gameObj);
            }

            return instance;
        }
    }

    // ユーザー登録処理 (Action<bool>型=>boolを引数にした関数を入れる型)
    public IEnumerator RegistUser(string name,Action<bool> result)
    {
        // サーバーに送信するオブジェクトを送信
        RegistUserRequest requestData = new RegistUserRequest();
        requestData.Name = name;
        // サーバーに送信するオブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request =
            UnityWebRequest.Post(API_BASE_URL + "users/store", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        bool isSuccess = false;
        if(request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // レスポンスボディ(json)の文字列を取得
            string jsonResult = request.downloadHandler.text;
            // jsonをデシリアライズ
            RegistUserResponse response = JsonConvert.DeserializeObject<RegistUserResponse>(jsonResult);

            // ファイルにユーザーIDを保存
            this.userName = name;
            this.userID = response.UserID;
            SaveUserData();
            isSuccess = true;
        }

        result?.Invoke(isSuccess); // ここで呼びだし元のresult処理を呼び出す
    }

    private void SaveUserData()
    {
        // ユーザー情報を保存する
        SaveData saveData = new SaveData();
        saveData.UserName = this.userName;
        saveData.UserID = this.userID;
        string json = JsonConvert.SerializeObject(saveData);
        // StreamWriterクラスでファイルにjsonを保存
        // persistentDataPathはアプリの保存ファイルを置く場所。OS毎に変えてくれる。
        var writer = new StreamWriter(Application.persistentDataPath + "/saveData.json");
        writer.Write(json);
        writer.Flush();
        writer.Close();
    }

    // ユーザー情報を読み込む
    public bool LoadUserData()
    {
        if(!File.Exists(Application.persistentDataPath + "/saveData.json"))
        {
            return false;
        }

        var reader = new StreamReader(Application.persistentDataPath + "/saveData.json");
        string json = reader.ReadToEnd();
        reader.Close();
        // ローカルファイル名からユーザー名とユーザーIDを取得
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        this.userID = saveData.UserID;
        this.userName = saveData.UserName;
        // 読み込んだかどうか
        return true;
    }

    public IEnumerator GetStage(Action<StageResponse[]> result)
    {
        // ステージ取得APIを実行
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "stages/index/" + this.userID);
        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // レスポンスボディ(json)の文字列を取得
            string jsonResult = request.downloadHandler.text;
            // jsonをデシリアライズ
            StageResponse[] response = JsonConvert.DeserializeObject<StageResponse[]>(jsonResult);

            result.Invoke(response);
        }
        else
        {
            result.Invoke(null);
        }
    }

    public void SaveStageData(int stageId)
    {
        // ユーザー情報を保存する
        SaveData saveData = new SaveData();
        saveData.StageID = stageId;
        string json = JsonConvert.SerializeObject(saveData);
        // StreamWriterクラスでファイルにjsonを保存
        // persistentDataPathはアプリの保存ファイルを置く場所。OS毎に変えてくれる。
        var writer = new StreamWriter(Application.persistentDataPath + "/saveData.json");
        writer.Write(json);
        writer.Flush();
        writer.Close();
    }

    public bool LoadStageData()
    {
        if (!File.Exists(Application.persistentDataPath + "/saveData.json"))
        {
            return false;
        }

        var reader = new StreamReader(Application.persistentDataPath + "/saveData.json");
        string json = reader.ReadToEnd();
        reader.Close();
        // ローカルファイル名からユーザー名とユーザーIDを取得
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        this.stageId = saveData.StageID;
        // 読み込んだかどうか
        return true;
    }
}
