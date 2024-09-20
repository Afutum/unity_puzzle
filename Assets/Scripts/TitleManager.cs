using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    //Audio
    AudioSource audioSource;

    [SerializeField] AudioClip clickSE;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            audioSource.PlayOneShot(clickSE);

            bool isSuccess = NetworkManager.Instance.LoadUserData();
            if (!isSuccess)
            {
                // ユーザーデータが保存されていない場合
                StartCoroutine(NetworkManager.Instance.RegistUser(
                    Guid.NewGuid().ToString(),      // 名前
                    result => {                     // 登録終了後の処理
                        SceneManager.LoadScene("Stage");
                    }));
            }
            else
            {
                SceneManager.LoadScene("Stage");
            }
        }
    }
}
