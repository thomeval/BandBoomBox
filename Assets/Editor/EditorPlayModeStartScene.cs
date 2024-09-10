using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


[InitializeOnLoad]
public class EditorPlayModeStartScene : MonoBehaviour
{
    static EditorPlayModeStartScene()
    {
        var scenePath = "Assets/Scenes/InitialLoadScene.unity";
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
    }
}
