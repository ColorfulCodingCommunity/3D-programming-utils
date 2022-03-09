using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles : MonoBehaviour
{

    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        if (!Selection.activeObject)
        {
            Debug.LogWarning("No prefab selected!");
            return;
        }

        string assetBundleDirectory = "AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        Debug.Log("Cleaning folder...");

        ClearFolder(assetBundleDirectory);

        Debug.Log("Starting build...");

        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        string assetBundleName = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject)).assetBundleName;
        var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);


        //assetBundleDirectory = EditorUtility.SaveFilePanel("Save Resources", assetBundleDirectory, assetBundleName, "");

        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = assetBundleName;
        build.assetNames = assetPaths;

        builds.Add(build);


        string winPath = assetBundleDirectory + "/win";
        if (!Directory.Exists(winPath))
        {
            Directory.CreateDirectory(winPath);
        }

        string androidPath = assetBundleDirectory + "/android";
        if (!Directory.Exists(androidPath))
        {
            Directory.CreateDirectory(androidPath);
        }

        string iosPath = assetBundleDirectory + "/ios";
        if (!Directory.Exists(iosPath))
        {
            Directory.CreateDirectory(iosPath);
        }

        BuildPipeline.BuildAssetBundles(winPath, builds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        BuildPipeline.BuildAssetBundles(androidPath, builds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.Android);
        BuildPipeline.BuildAssetBundles(iosPath, builds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.iOS);

        Debug.Log("Postprocessing...");
        CleanupAfterBuild();
        Debug.Log("Build finished");
    }

    private static void ClearFolder(string dirPath)
    {
        DirectoryInfo dir = new DirectoryInfo(dirPath);

        foreach (FileInfo fi in dir.GetFiles())
        {
            fi.Delete();
        }

        foreach (DirectoryInfo di in dir.GetDirectories())
        {
            ClearFolder(di.FullName);
            di.Delete();
        }
    }

    //[MenuItem("Assets/Cleanup After Build")]
    static void CleanupAfterBuild()
    {
        if (!Selection.activeObject)
        {
            Debug.LogWarning("No prefab selected!");
            return;
        }

        string assetBundleDirectory = "AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Debug.LogWarning("No build present!");
            return;
        }

        string winPath = assetBundleDirectory + "/win";
        if (!Directory.Exists(winPath))
        {
            Debug.LogWarning("Windows build not present!");
        }

        string androidPath = assetBundleDirectory + "/android";
        if (!Directory.Exists(androidPath))
        {
            Debug.LogWarning("Android build not present!");
        }

        string iosPath = assetBundleDirectory + "/ios";
        if (!Directory.Exists(iosPath))
        {
            Debug.LogWarning("IOS build not present!");
        }

        string filename = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject)).assetBundleName;

        File.Move(winPath + "/" + filename, assetBundleDirectory + "/" + filename + "-Win");
        File.Move(androidPath + "/" + filename, assetBundleDirectory + "/" + filename + "-Android");
        File.Move(iosPath + "/" + filename, assetBundleDirectory + "/" + filename + "-IOS");

        Directory.Delete(winPath, true);
        Directory.Delete(androidPath, true);
        Directory.Delete(iosPath, true);

    }
}
