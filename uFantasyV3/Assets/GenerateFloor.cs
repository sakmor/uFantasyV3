using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class GenerateFloor : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        NavMeshTriangulation triangulatedNavMesh = NavMesh.CalculateTriangulation();

        Mesh mesh = new Mesh();
        mesh.name = "ExportedNavMesh";
        mesh.vertices = triangulatedNavMesh.vertices;
        mesh.triangles = triangulatedNavMesh.indices;
        gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>();
        // n.AddComponent<MeshFilter>().mesh = mesh;
        // n.AddComponent<MeshRenderer>();
        // string filename = Application.dataPath + "/" + Path.GetFileNameWithoutExtension(EditorApplication.currentScene) + " Exported NavMesh.obj";
        // MeshToFile(mesh, filename);
        // print("NavMesh exported as '" + filename + "'");
        UnityEditor.AssetDatabase.Refresh();
    }


}
