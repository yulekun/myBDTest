using System.IO;
using UnityEditor;
using UnityEngine;
using LitJson;
class VersionText {
	public string version;
	public BundleItemInfo[] assetBds;
}

class BundleItemInfo {
	public string BDName; // bd名称
	public int hashCode; // hash
	public string[] subResName; // 包含的资源
	public string[] dependences; // 依赖bd
}

public class CreateBdEditor {
	[MenuItem ("Assets/BuildBD")]
	static void BuildAllAssetBundles () {
		string dir = "Assets/AssetBundles";
		if (Directory.Exists (dir)) {
			Directory.Delete (dir, true);
		}
		Directory.CreateDirectory (dir);
		ChangeRawImagePackTag ();
		Debug.Log ("------------------->修改rawResources图标packTag完成");
		NameAssetBundle ("Assets/Resources");
		Debug.Log ("------------------->Resources资源打包完成");
		NameAssetBundle ("Assets/RawResources/common", "UIRawCommon");
		Debug.Log ("------------------->common图集资源打包完成");
		if (!Directory.Exists (dir)) {
			Directory.CreateDirectory (dir);
		}

		BuildPipeline.BuildAssetBundles (dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
		Debug.Log ("------------------->bd打包完成");
		CreateVersionText ();
	}

	// 修改每个目录中的图片资源packTag
	static void ChangeRawImagePackTag () {
		DirectoryInfo dicInfo = new DirectoryInfo ("Assets/RawResources");
		foreach (DirectoryInfo dire in dicInfo.GetDirectories ()) {
			foreach (FileInfo file in dire.GetFiles ()) {
				if (file.Name.IndexOf (".meta") < 0) {
					string path = file.FullName.Substring (file.FullName.IndexOf ("Assets"));
					TextureImporter tex = AssetImporter.GetAtPath (path) as TextureImporter;
					tex.spritePackingTag = dire.Name;
					tex.textureType = TextureImporterType.Sprite;
					tex.SaveAndReimport ();
				}
			}
		}
	}

	// 设置bundle名称
	static void NameAssetBundle (string path, string bundleName = "") {
		if (!Directory.Exists (path)) {
			return;
		}
		DirectoryInfo dicInfo = new DirectoryInfo (path);
		foreach (FileInfo file in dicInfo.GetFiles ()) {
			if (file.Name.IndexOf (".meta") < 0) {
				string path2 = file.FullName.Substring (file.FullName.IndexOf ("Assets"));
				AssetImporter tex = AssetImporter.GetAtPath (path2);
				if (tex) {
					string name = bundleName == "" ? Path.GetFileNameWithoutExtension (file.Name) : bundleName;
					tex.assetBundleName = name;
				}
			}
		}
	}

	// 创建版本控制文本
	static void CreateVersionText () {
		VersionText verText = new VersionText ();
		verText.version = "1.0.0";
		AssetBundle ab = AssetBundle.LoadFromFile ("Assets/AssetBundles/AssetBundles");
		AssetBundleManifest am = ab.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
		string[] bdles = am.GetAllAssetBundles ();
		verText.assetBds = new BundleItemInfo[bdles.Length];
		int i = 0;
		foreach (var item in bdles) {
			BundleItemInfo bi = new BundleItemInfo ();
			bi.BDName = item; // 名称

			AssetBundle subAB = AssetBundle.LoadFromFile ("Assets/AssetBundles/" + item);
			bi.hashCode = subAB.GetHashCode ();
			bi.dependences = am.GetAllDependencies (item);
			verText.assetBds[i] = bi;
			i++;
		}
		string json = JsonMapper.ToJson (verText);
		string dirPath = Path.Combine (Application.dataPath, "Vertion");
		string filePath = Path.Combine (dirPath, "version.txt");
		if (!Directory.Exists (dirPath)) {
			Directory.CreateDirectory (dirPath);
		}
		if (File.Exists (filePath)) {
			File.Delete (filePath);
		}
		File.WriteAllText (filePath, json);
		Debug.Log ("----------------->版本控制文本完成");
	}
}