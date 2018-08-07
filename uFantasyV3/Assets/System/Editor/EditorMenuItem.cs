
using UnityEngine.AI;
using System.IO;
using System.Collections.Generic;
using System.Text;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class EditorMenuItem
{
    [MenuItem("編輯工具/重讀DB _F2")]
    private static void reloadDB()
    {

        AssetDatabase.Refresh();
    }

    [MenuItem("編輯工具/完整執行遊戲 %`")]
    private static void PlayFromPrelaunchScene()
    {
        if (EditorApplication.isPlaying == true)
        {
            EditorApplication.isPlaying = false;
            EditorWindow.GetWindow<OpenPreviousScene>();
        }
        if (EditorApplication.isPlaying == false)
        {
            string openScene = SceneUtility.GetScenePathByBuildIndex(0);
            Scene activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.IsValid() && (activeScene.path != openScene))
            {
                PlayerPrefs.SetString("PreviousScene", activeScene.path);
            }
            else
            {
                PlayerPrefs.SetString("PreviousScene", string.Empty);
            }
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(openScene);
            EditorApplication.isPlaying = true;
        }
    }

    [MenuItem("編輯工具/執行遊戲快捷鍵 _F1")]
    private static void PlayScene()
    {
        if (!Application.isPlaying)
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", false);
        }
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }

    public static float exeTime;
    public static Dictionary<Vector3, GameObject> CleanMap;

    [MenuItem("編輯工具/場景整理 %9")]
    static void SetNavigationBake()
    {
        exeTime = Time.realtimeSinceStartup;

        GetCleanMap();
        SetEveryCubeWalkable();
        BuildNavMesh();
        RemoveHideCube();
        Export();

        Debug.Log("整理完畢 " + (Time.realtimeSinceStartup - exeTime).ToString("F2"));

    }

    static Dictionary<string, Motion> GetMotions(string modelName)
    {
        var MotionArray = Resources.LoadAll("Biology/" + modelName, typeof(AnimationClip));

        Dictionary<string, Motion> Motions = new Dictionary<string, Motion>();
        foreach (var i in MotionArray)
        {
            Motions.Add(i.name, i as Motion);
        }
        return Motions;
    }
    static void GetCleanMap()
    {
        CUBE[] CUBE_list = Object.FindObjectsOfType(typeof(CUBE)) as CUBE[];
        Dictionary<Vector3, GameObject> NewMap = new Dictionary<Vector3, GameObject>();

        // 消除重複取得無重複場景
        foreach (var item in CUBE_list)
        {
            item.name = item.transform.position.ToString("F0");
            if (NewMap.ContainsKey(item.transform.position))
            {
                Object.DestroyImmediate(item.gameObject);
            }
            else
            {
                NewMap.Add(item.transform.position, item.gameObject);
            }
        }
        CleanMap = NewMap;

    }
    private static void RemoveHideCube()
    {

        List<GameObject> DestoryList = new List<GameObject>();
        foreach (var item in CleanMap)
        {
            Vector3 pos = item.Key;
            if (CleanMap.ContainsKey(pos + new Vector3(1, 0, 0))
             && CleanMap.ContainsKey(pos + new Vector3(-1, 0, 0))
             && CleanMap.ContainsKey(pos + new Vector3(0, 1, 0))
             && CleanMap.ContainsKey(pos + new Vector3(0, -1, 0))
             && CleanMap.ContainsKey(pos + new Vector3(0, 0, 1))
             && CleanMap.ContainsKey(pos + new Vector3(0, 0, -1)))
            {
                var temp = item.Value;
                DestoryList.Add(item.Value);
            }
        }
        for (var i = 0; i < DestoryList.Count; i++)
        {
            CleanMap.Remove(DestoryList[i].transform.position);
            Object.DestroyImmediate(DestoryList[i]);
        }
    }

    private static void BuildNavMesh()
    {
        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
    }

    private static void SetEveryCubeWalkable()
    {
        foreach (var item in CleanMap)
        {
            Debug.Log(CleanMap.Count);
            GameObjectUtility.SetNavMeshArea(item.Value, 0);
        }
    }

    static void Export()
    {
        NavMeshTriangulation triangulatedNavMesh = NavMesh.CalculateTriangulation();

        Mesh mesh = new Mesh();
        mesh.name = "ExportedNavMesh";
        mesh.vertices = triangulatedNavMesh.vertices;
        mesh.triangles = triangulatedNavMesh.indices;
        Object.DestroyImmediate(GameObject.Find("Floor"));//fixme:不藥用找名字的啦...
        GameObject n = new GameObject("Floor");//fixme:不藥用找名字的啦...
        n.AddComponent<MeshCollider>().sharedMesh = mesh;
        n.AddComponent<MeshFilter>().mesh = mesh;
        n.AddComponent<MeshRenderer>();
        // n.AddComponent<MeshFilter>().mesh = mesh;
        // n.AddComponent<MeshRenderer>();
        // string filename = Application.dataPath + "/" + Path.GetFileNameWithoutExtension(EditorApplication.currentScene) + " Exported NavMesh.obj";
        // MeshToFile(mesh, filename);
        // print("NavMesh exported as '" + filename + "'");
        AssetDatabase.Refresh();
    }
    static string MeshToString(Mesh mesh)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append(mesh.name).Append("\n");
        foreach (Vector3 v in mesh.vertices)
        {
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in mesh.normals)
        {
            sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in mesh.uv)
        {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        for (int material = 0; material < mesh.subMeshCount; material++)
        {
            sb.Append("\n");
            //sb.Append("usemtl ").Append(mats[material].name).Append("\n");
            //sb.Append("usemap ").Append(mats[material].name).Append("\n");

            int[] triangles = mesh.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
            }
        }
        return sb.ToString();
    }

    static void MeshToFile(Mesh mesh, string filename)
    {
        using (StreamWriter sw = new StreamWriter(filename))
        {
            sw.Write(MeshToString(mesh));
        }
    }
}

[ExecuteInEditMode]
public class OpenPreviousScene : EditorWindow
{
    private float _time;

    private void OnEnable()
    {
        _time = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        if (_time - Time.realtimeSinceStartup > 1.0f)
        {
            string openScene = PlayerPrefs.GetString("PreviousScene");
            if (!string.IsNullOrEmpty(openScene))
            {
                Scene activeScene = EditorSceneManager.GetActiveScene();
                if (activeScene.IsValid() && (activeScene.path != openScene))
                {
                    EditorSceneManager.OpenScene(openScene);
                }
            }
            Close();
        }
    }
}
