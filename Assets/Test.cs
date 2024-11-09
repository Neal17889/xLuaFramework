using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        AssetBundleCreateRequest request1 = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "ui/prefab/uilogin.prefab.ab"));
        yield return request1;

        AssetBundleCreateRequest request2 = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "ui/res/common_bg_02.png.ab"));
        yield return request2;

        AssetBundleRequest assetBundleRequest = request1.assetBundle.LoadAssetAsync("Assets/BuildResources/UI/Prefab/UILogin.prefab");
        yield return assetBundleRequest;

        GameObject go = Instantiate(assetBundleRequest.asset) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
