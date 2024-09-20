using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    StageSelectManager stageSelectManager;

    
    [SerializeField] GameObject gameOver;

    GameDirector gameDirector;
    UIManager ui;

    // Start is called before the first frame update
    void Start()
    {
        //stageSelectManager = GameObject.Find("StageSelectManager").GetComponent<StageSelectManager>();

        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();

        ui = GameObject.Find("UIManeger").GetComponent<UIManager>();

        gameOver.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        /*bool isSuccess = NetworkManager.Instance.LoadStageData();

        if (!isSuccess)
        {
            NetworkManager.Instance.SaveStageData(stageSelectManager.stageId);
        }*/

        if (Input.GetMouseButtonDown(0))
        {
            // ƒ^ƒCƒgƒ‹‚É‰æ–Ê‘JˆÚ
            SceneManager.LoadScene("Stage");
        }
    }
}
