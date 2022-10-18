using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using UnityEditor;
using UnityEngine;

public class RobotMeshBakedTransformation : MonoBehaviour
{
    public Vector3 position;
    public Quaternion rotation;
}

public class McRtcRobot : MonoBehaviour
{
    public string id;
    private Dictionary<string, GameObject> bodies = new Dictionary<string, GameObject>();

    public void ImportMesh(string name, string path, float scale)
    {
        Transform body_tf = this.transform.Find(name);
        if(body_tf)
        {
            bodies[name] = body_tf.gameObject;
            return;
        }
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError($"Cannot load mest at {path} for robot {this.name}");
            bodies[name] = null;
        }
        if (System.IO.File.Exists(path + ".fbx"))
        {
            ImportMesh(name, path + ".fbx", scale);
            return;
        }
        string mesh = System.IO.Path.GetFileName(path);
        string destination_path = $"{Application.dataPath}/McRtc/{this.name}/{mesh}";
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destination_path));
        if (!System.IO.File.Exists(destination_path))
        {
            System.IO.File.Copy(path, destination_path);
            string mesh_path = System.IO.Path.GetDirectoryName(path);
            var textures = System.IO.Directory.EnumerateFiles(mesh_path, "*.*", System.IO.SearchOption.AllDirectories).Where(s => s.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase) || s.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase));
            foreach (var text in textures)
            {
                var text_out = text.Replace(mesh_path, "");
                destination_path = $"{Application.dataPath}/McRtc/{this.name}/{text_out}";
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destination_path));
                if (!System.IO.File.Exists(destination_path))
                {
                    System.IO.File.Copy(text, destination_path);
                }
            }
            AssetDatabase.Refresh();
            ModelImporter importer = (ModelImporter)ModelImporter.GetAtPath($"Assets/McRtc/{this.name}/{mesh}");
            if (importer)
            {
                importer.bakeAxisConversion = true;
                importer.globalScale = scale;
                importer.SaveAndReimport();
                AssetDatabase.Refresh();
            }
        }
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/McRtc/{this.name}/{mesh}", typeof(GameObject));
        if (!obj)
        {
            bodies[name] = null;
        }
        GameObject body = Object.Instantiate(obj, new Vector3(0, 0, 0), Quaternion.identity);
        body.name = name;
        body.transform.parent = transform;
        RobotMeshBakedTransformation baked = body.AddComponent<RobotMeshBakedTransformation>();
        baked.position = obj.transform.position;
        baked.rotation = obj.transform.rotation;
        bodies[name] = body;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateRobot()
    {
    }

    public void UpdateMesh(string name, string path, float scale, float qw, float qx, float qy, float qz, float tx, float ty, float tz)
    {
        if(!bodies.ContainsKey(name))
        {
            ImportMesh(name, path, scale);
        }
        GameObject body = bodies[name];
        if(!body)
        {
            return;
        }
        RobotMeshBakedTransformation baked = body.GetComponent<RobotMeshBakedTransformation>();
        body.transform.localPosition = baked.position;
        body.transform.localRotation = baked.rotation;
        body.transform.Translate(new Vector3(tx, ty, tz));
        body.transform.Rotate(new Quaternion(qx, qy, qz, qw).eulerAngles);
    }

    public void DeleteRobot()
    {
        foreach(var body in bodies)
        {
            GameObject.DestroyImmediate(body.Value);
        }
        bodies.Clear();
    }
}