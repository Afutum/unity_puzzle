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
    private int userID = 0;       // �����̃��[�U�[ID
    private string userName = ""; // �����̃��[�U�[��
    private int stageId = 0;

    // get�v���p�e�B�ŌĂяo�������񎞂ɃC���X�^���X��������static�ŕێ�
    private static NetworkManager instance;

    public static NetworkManager Instance
    {
        get
        {
            if(instance == null)
            {// null�̂Ƃ�
                // GameObject�𐶐����āANetworkManager�R���|�[�l���g��ǉ�
                GameObject gameObj = new GameObject("NetworkManager");
                instance = gameObj.AddComponent<NetworkManager>();

                // �I�u�W�F�N�g���V�[���J�ڎ��ɍ폜���Ȃ��悤�ɂ���
                DontDestroyOnLoad(gameObj);
            }

            return instance;
        }
    }

    // ���[�U�[�o�^���� (Action<bool>�^=>bool�������ɂ����֐�������^)
    public IEnumerator RegistUser(string name,Action<bool> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g�𑗐M
        RegistUserRequest requestData = new RegistUserRequest();
        requestData.Name = name;
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request =
            UnityWebRequest.Post(API_BASE_URL + "users/store", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        bool isSuccess = false;
        if(request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // ���X�|���X�{�f�B(json)�̕�������擾
            string jsonResult = request.downloadHandler.text;
            // json���f�V���A���C�Y
            RegistUserResponse response = JsonConvert.DeserializeObject<RegistUserResponse>(jsonResult);

            // �t�@�C���Ƀ��[�U�[ID��ۑ�
            this.userName = name;
            this.userID = response.UserID;
            SaveUserData();
            isSuccess = true;
        }

        result?.Invoke(isSuccess); // �����ŌĂт�������result�������Ăяo��
    }

    private void SaveUserData()
    {
        // ���[�U�[����ۑ�����
        SaveData saveData = new SaveData();
        saveData.UserName = this.userName;
        saveData.UserID = this.userID;
        string json = JsonConvert.SerializeObject(saveData);
        // StreamWriter�N���X�Ńt�@�C����json��ۑ�
        // persistentDataPath�̓A�v���̕ۑ��t�@�C����u���ꏊ�BOS���ɕς��Ă����B
        var writer = new StreamWriter(Application.persistentDataPath + "/saveData.json");
        writer.Write(json);
        writer.Flush();
        writer.Close();
    }

    // ���[�U�[����ǂݍ���
    public bool LoadUserData()
    {
        if(!File.Exists(Application.persistentDataPath + "/saveData.json"))
        {
            return false;
        }

        var reader = new StreamReader(Application.persistentDataPath + "/saveData.json");
        string json = reader.ReadToEnd();
        reader.Close();
        // ���[�J���t�@�C�������烆�[�U�[���ƃ��[�U�[ID���擾
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        this.userID = saveData.UserID;
        this.userName = saveData.UserName;
        // �ǂݍ��񂾂��ǂ���
        return true;
    }

    public IEnumerator GetStage(Action<StageResponse[]> result)
    {
        // �X�e�[�W�擾API�����s
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "stages/index/" + this.userID);
        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // ���X�|���X�{�f�B(json)�̕�������擾
            string jsonResult = request.downloadHandler.text;
            // json���f�V���A���C�Y
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
        // ���[�U�[����ۑ�����
        SaveData saveData = new SaveData();
        saveData.StageID = stageId;
        string json = JsonConvert.SerializeObject(saveData);
        // StreamWriter�N���X�Ńt�@�C����json��ۑ�
        // persistentDataPath�̓A�v���̕ۑ��t�@�C����u���ꏊ�BOS���ɕς��Ă����B
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
        // ���[�J���t�@�C�������烆�[�U�[���ƃ��[�U�[ID���擾
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        this.stageId = saveData.StageID;
        // �ǂݍ��񂾂��ǂ���
        return true;
    }
}
