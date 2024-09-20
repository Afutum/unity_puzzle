using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    UIManager ui;

    AudioSource audioSource;

    [SerializeField] AudioClip clickSE;

    // Start is called before the first frame update
    void Start()
    {
        ui = GameObject.Find("UIManager").GetComponent<UIManager>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ui.AllClear();
        ui.GameOver();

        if (Input.GetMouseButtonDown(0))
        {
            audioSource.PlayOneShot(clickSE);

            StartCoroutine(MoveScene());
        }
    }

    IEnumerator MoveScene()
    {
        yield return new WaitForSeconds(1);

        // ƒ^ƒCƒgƒ‹‚É‰æ–Ê‘JˆÚ
        SceneManager.LoadScene("Stage");
    }
}
