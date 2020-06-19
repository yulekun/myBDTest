using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;



public class ShowPanel : MonoBehaviour {
	public Text progressUI;
	// Use this for initialization
	void Start () {
		string assetPaht = Application.dataPath;

		AssetBundle ab = AssetBundle.LoadFromFile (assetPaht + "/AssetBundles/AssetBundles");
		AssetBundleManifest ma = ab.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
		string[] depenencies = ma.GetAllDependencies ("Panel22");
		for (int i = 0; i < depenencies.Length; ++i) {
			AssetBundle.LoadFromFile (assetPaht + "/AssetBundles/" + depenencies[i]);
		}
		AssetBundle panelab = AssetBundle.LoadFromFile (assetPaht + "/AssetBundles/Panel22");
		GameObject panelObj = panelab.LoadAsset<GameObject> ("Panel22");
		GameObject panel = GameObject.Instantiate (panelObj);
		panel.transform.parent = transform;
		panel.transform.position = Vector3.one * 300;

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			string url = "http://dlied5.qq.com/2DPocket/InTest/Android/Res/T-Ball_3.15.0.422.json";
			StartCoroutine (DownDB (url, "T-Ball_3.15.0.422.json"));
		}
	}

	IEnumerator DownDB (string url, string fileName) {
		UnityWebRequest uw = UnityWebRequest.Get (url);
		uw.SendWebRequest ();

		if (uw.isNetworkError || uw.isHttpError) {
			progressUI.text = "错误：" + uw.error;
		} else {
			while (!uw.isDone) {
				progressUI.text = uw.downloadProgress * 100 + "/100";
				yield return null;
			}
			var File = uw.downloadHandler.data;
			FileStream fs = new FileStream (Application.persistentDataPath + "/" + fileName, FileMode.OpenOrCreate);
			fs.Write (File, 0, File.Length);
			fs.Close ();
		}
	}
}