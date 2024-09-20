using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearManager : MonoBehaviour
{
    UIManager ui;

    AudioSource audioSource;

    [SerializeField] AudioClip clickSE;

    // Start is called before the first frame update
    void Start()
    {
        ui = GameObject.Find("UIManager").GetComponent<UIManager>();
        audioSource = GetComponent<AudioSource>();

        this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        /*bool isSuccess = NetworkManager.Instance.LoadStageData();

        if (!isSuccess)
        {
            NetworkManager.Instance.SaveStageData(stageSelectManager.stageId);
        }*/
        ui.AllClear();
        ui.Clear();

        if (Input.GetMouseButtonDown(0))
        {
            audioSource.PlayOneShot(clickSE);

            StartCoroutine(MoveScene());
        }
    }

    IEnumerator MoveScene()
    {
        yield return new WaitForSeconds(1);

        this.gameObject.SetActive(false);

        // ƒ^ƒCƒgƒ‹‚É‰æ–Ê‘JˆÚ
        SceneManager.LoadScene("Stage");
    }
}
