using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    public int stageId = 0;

    // apiŽcŠ[
    /*private void Start()
    {
        StartCoroutine(NetworkManager.Instance.GetStage(starges =>
        {
            
        }));
    }*/

    public void PushSelectButton(int selectNum)
    {
        StartCoroutine(WaitLoadScene(selectNum));

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
        }
    }
}
