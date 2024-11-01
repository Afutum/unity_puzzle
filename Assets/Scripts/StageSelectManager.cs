using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] Button[] buttons;

    public static int stageId = 0;

    //Audio
    AudioSource audioSource;

    [SerializeField] AudioClip clickSE;

    UIManager ui;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(NetworkManager.Instance.GetStage(stages =>
        {
            int maxStageId = 0;

            for (int i = 0; i < stages.Length; i++)
            {
                if (stages[i].StageClearCount >= 1 && maxStageId < stages[i].StageID)
                {
                    maxStageId = stages[i].StageID;
                }
            }

            for (int n = 0; n < maxStageId + 1; n++)
            {
                buttons[n].interactable = true;
            }
        }));
    }

    public void PushSelectButton(int selectNum)
    {
        StartCoroutine(WaitLoadScene(selectNum));

        audioSource.PlayOneShot(clickSE);

        stageId = selectNum;
    }

    public IEnumerator WaitLoadScene(int loadNum)
    {
        yield return new WaitForSeconds(3);

        switch (loadNum)
        {
            case 1:
                SceneManager.LoadScene("Stage1");
                break;

            case 2:
                SceneManager.LoadScene("Stage2");
                break;

            case 3:
                SceneManager.LoadScene("Stage3");
                break;

            case 4:
                SceneManager.LoadScene("Stage4");
                break;

            case 5:
                SceneManager.LoadScene("Stage5");
                break;

            case 6:
                SceneManager.LoadScene("Stage6");
                break;

            case 7:
                SceneManager.LoadScene("Stage7");
                break;

            case 8:
                SceneManager.LoadScene("Stage8");
                break;

            case 9:
                SceneManager.LoadScene("Stage9");
                break;

            case 10:
                SceneManager.LoadScene("Stage10");
                break;

            case 11:
                SceneManager.LoadScene("Raid");
                break;
        }
    }
}
