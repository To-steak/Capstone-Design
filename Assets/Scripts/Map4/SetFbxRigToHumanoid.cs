using UnityEditor;
using UnityEngine;
using System.IO;

public class SetFbxRigToHumanoid : EditorWindow
{
    private string folderPath = "Assets/Animations";

    [MenuItem("Tools/Set FBX Rig to Humanoid")]
    public static void ShowWindow()
    {
        GetWindow<SetFbxRigToHumanoid>("FBX Rig Converter");
    }

    private void OnGUI()
    {
        GUILayout.Label("FBX Rig 설정 자동 변경기", EditorStyles.boldLabel);
        folderPath = EditorGUILayout.TextField("폴더 경로", folderPath);

        if (GUILayout.Button("Humanoid로 변환"))
        {
            ConvertFbxRigsToHumanoid(folderPath);
        }
    }

    private void ConvertFbxRigsToHumanoid(string path)
    {
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { path });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;

            if (importer != null && importer.animationType != ModelImporterAnimationType.Human)
            {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;

                Debug.Log($"변환: {assetPath}");
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }
        }

        Debug.Log("변환 완료!");
    }
}
