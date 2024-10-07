using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    public int stageId = 0;

    //Audio
    AudioSource audioSource;

    [SerializeField] AudioClip clickSE;

    UIManager ui;

    // apiŽcŠ[
    /*private void Start()
    {
        StartCoroutine(NetworkManager.Instance.GetStage(starges =>
        {
            
        }));
    }*/

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
                SceneManager.LoadScene("Game1");
                break;

            case 2:
                SceneManager.LoadScene("Game2");
                break;

            case 3:
                SceneManager.LoadScene("Game3");
                break;

            case 4:
                SceneManager.LoadScene("Game4");
                break;

            case 5:
                SceneManager.LoadScene("Game5");
                break;

            case 6:
                SceneManager.LoadScene("Game6");
                break;

            case 7:
                SceneManager.LoadScene("Game7");
                break;

            case 8:
                SceneManager.LoadScene("Game8");
                break;

            case 9:
                SceneManager.LoadScene("Game9");
                break;

            case 10:
                SceneManager.LoadScene("Game10");
                break;
        }
    }
}
