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
        // カタログ更新処理 最新のカタログ(json)を取得
        var handle = Addressables.UpdateCatalogs();

        yield return handle;

        // ダウンロード実行
        AsyncOperationHandle downloadHandle =
            Addressables.DownloadDependenciesAsync("default",false); // bundleファイルをダウンロード

        // ダウンロード完了するまでスライダーのUIを更新
        while(downloadHandle.Status == AsyncOperationStatus.None)
        {
            // Percentは0～1で取得
            loadingSlider.value = downloadHandle.GetDownloadStatus().Percent * 100;
            yield return null; // 1フレーム待つ
        }
        loadingSlider.value = 100;
        Addressables.Release(downloadHandle);

        // 次のシーンに移動
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
