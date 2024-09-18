using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AssetLoader : MonoBehaviour
{
    [SerializeField] Slider loadingSlider;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(loading());
    }

    IEnumerator loading()
    {
        // �J�^���O�X�V���� �ŐV�̃J�^���O(json)���擾
        var handle = Addressables.UpdateCatalogs();

        yield return handle;

        // �_�E�����[�h���s
        AsyncOperationHandle downloadHandle =
            Addressables.DownloadDependenciesAsync("default",false); // bundle�t�@�C�����_�E�����[�h

        // �_�E�����[�h��������܂ŃX���C�_�[��UI���X�V
        while(downloadHandle.Status == AsyncOperationStatus.None)
        {
            // Percent��0�`1�Ŏ擾
            loadingSlider.value = downloadHandle.GetDownloadStatus().Percent * 100;
            yield return null; // 1�t���[���҂�
        }
        loadingSlider.value = 100;
        Addressables.Release(downloadHandle);

        // ���̃V�[���Ɉړ�
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
