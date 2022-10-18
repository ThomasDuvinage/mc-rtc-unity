using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

using Action = System.Action;
using IntPtr = System.IntPtr;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class BakedTransformation : MonoBehaviour
{
    public Vector3 position;
    public Quaternion rotation;
}

public class McRtcWindow : MonoBehaviour
{
    /*[MenuItem("McRtc/GUI")]
    public static void ShowExample()
    {
        var wnd = GetWindow<McRtcWindow>();
        wnd.titleContent = new GUIContent("McRtcGUI");
    }

    private static GameObject GetRobotRoot(string id)
    {
        var ids = id.Split('\n');
        GameObject root = null;
        foreach(GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if(obj.name == ids[0])
            {
                root = obj;
                break;
            }
        }
        if(!root)
        {
            root = new GameObject(ids[0]);
        }
        for(uint i = 1; i < ids.Length; ++i)
        {
            Transform child = root.transform.Find(ids[i]);
            if(child)
            {
                root = child.gameObject;
            }
            else
            {
                GameObject nroot = new GameObject(ids[i]);
                nroot.transform.parent = root.transform;
                root = nroot;
            }
        }
        return root;
    }

    private static void OnRobot(string id)
    {
    }

    private static void OnRobotMesh(string id, string name, string path, float scale, float qw, float qx, float qy, float qz, float tx, float ty, float tz)
    {
        work.Enqueue(() => {
            GameObject robot = GetRobotRoot(id);
            GameObject body = RobotMesh.Import(robot, name, path, scale);
            if(!body)
            {
                return;
            }
            BakedTransformation baked = body.GetComponent<BakedTransformation>();
            body.transform.position = baked.position;
            body.transform.rotation = baked.rotation;
            body.transform.Translate(new Vector3(tx, ty, tz));
            body.transform.Rotate(new Quaternion(qx, qy, qz, qw).eulerAngles);
        });
    }

    private static void OnRemoveRobot(string id)
    {
        work.Enqueue(() => {
            GameObject robot = GetRobotRoot(id);
            if(robot)
            {
                GameObject.DestroyImmediate(robot);
            }
        });
    }

    public void OnDisable()
    {
        StopClient();
        while(work.Count > 0)
        {
            work.Dequeue()();
        }
    }

    public void Update()
    {
        while(work.Count > 0)
        {
            work.Dequeue()();
        }
    }

    public void CreateGUI()
    {
    }*/
}
