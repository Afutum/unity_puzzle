using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bool isSuccess = NetworkManager.Instance.LoadUserData();
            if (!isSuccess)
            {
                // ���[�U�[�f�[�^���ۑ�����Ă��Ȃ��ꍇ
                StartCoroutine(NetworkManager.Instance.RegistUser(
                    Guid.NewGuid().ToString(),      // ���O
                    result => {                     // �o�^�I����̏���
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
