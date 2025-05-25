using UnityEngine;
using UnityEditor;

public class URPShaderConverter
{
    [MenuItem("Tools/Convert Selected Materials to URP Lit")]
    static void ConvertSelected()
    {
        var mats = Selection.GetFiltered<Material>(SelectionMode.DeepAssets);
        foreach (var mat in mats)
        {
            // 기존에 Standard 셰이더인 경우만
            if (mat.shader.name.Contains("Standard"))
            {
                mat.shader = Shader.Find("Universal Render Pipeline/Lit");
                EditorUtility.SetDirty(mat);
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"Converted {mats.Length} materials to URP Lit.");
    }
}
