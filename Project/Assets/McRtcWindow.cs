using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

using Action = System.Action;
using IntPtr = System.IntPtr;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[ExecuteAlways]
public class TestB : MonoBehaviour
{
    public float velocity = 0.01f;
    private float dir = 1.0f;
    void Update()
    {
        if(transform.position.z > 1.0 || transform.position.z < -1.0)
        {
            dir *= -1;
        }
        transform.Translate(new Vector3(0, 0, dir * velocity));
    }
   void OnDrawGizmos()
   {
      // Your gizmo drawing thing goes here if required...
#if UNITY_EDITOR
      // Ensure continuous Update calls.
      if (!Application.isPlaying)
      {
         UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
         UnityEditor.SceneView.RepaintAll();
      }
#endif
   }
}

public class BakedTransformation : MonoBehaviour
{
    public Vector3 position;
    public Quaternion rotation;
}

public class RobotMesh
{
    static public GameObject Import(GameObject parent, string name, string path, float scale)
    {
        if(!System.IO.File.Exists(path))
        {
            throw new System.Exception($"Cannot load mesh at {path} for robot {parent.name}");
        }
        if(System.IO.File.Exists(path + ".fbx"))
        {
            return Import(parent, name, path + ".fbx", scale);
        }
        string mesh = System.IO.Path.GetFileName(path);
        string destination_path = $"{Application.dataPath}/McRtc/{parent.name}/{mesh}";
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destination_path));
        if(!System.IO.File.Exists(destination_path))
        {
            System.IO.File.Copy(path, destination_path);
            string mesh_path = System.IO.Path.GetDirectoryName(path);
            var textures = System.IO.Directory.EnumerateFiles(mesh_path, "*.*", System.IO.SearchOption.AllDirectories).Where(s => s.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase) || s.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase));
            foreach(var text in textures)
            {
                var text_out = text.Replace(mesh_path, "");
                destination_path = $"{Application.dataPath}/McRtc/{parent.name}/{text_out}";
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destination_path));
                if(!System.IO.File.Exists(destination_path))
                {
                    System.IO.File.Copy(text, destination_path);
                }
            }
            AssetDatabase.Refresh();
            ModelImporter importer = (ModelImporter)ModelImporter.GetAtPath($"Assets/McRtc/{parent.name}/{mesh}");
            if(importer)
            {
                importer.bakeAxisConversion = true;
                importer.globalScale = scale;
                importer.SaveAndReimport();
                AssetDatabase.Refresh();
            }
        }
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/McRtc/{parent.name}/{mesh}", typeof(GameObject));
        if(!obj)
        {
            return null;
        }
        Transform body_tf = parent.transform.Find(name);
        if(body_tf)
        {
            return body_tf.gameObject;
        }
        else
        {
            GameObject body = Object.Instantiate(obj, new Vector3(0, 0, 0), Quaternion.identity);
            body.name = name;
            body.transform.parent = parent.transform;
            BakedTransformation baked = body.AddComponent<BakedTransformation>();
            baked.position = obj.transform.position;
            baked.rotation = obj.transform.rotation;
            return body;
        }
    }
}

public class TabBar : VisualElement
{
    private VisualElement header;
    private VisualElement content;

    public TabBar() : base()
    {
        header = new VisualElement();
        Add(header);
        content = new VisualElement();
        Add(content);
        header.style.flexDirection = FlexDirection.Row;
    }

    public void AddTab(string title)
    {
        var btn = new Button() { text = title };
        header.Add(btn);
        var cnt = new ScrollView();
        content.Add(cnt);
        cnt.Add(new Label($"This is the tab for {title}"));
        foreach(VisualElement child in content.Children())
        {
          child.style.display = DisplayStyle.None;
        }
        cnt.style.display = DisplayStyle.Flex;
        btn.clicked += () =>
        {
            foreach(VisualElement child in content.Children())
            {
                if(cnt != child)
                {
                    child.style.display = DisplayStyle.None;
                }
            }
            cnt.style.display = DisplayStyle.Flex;
        };
        var closeClickable = new Clickable((evt) => {
            header.Remove(btn);
            content.Remove(cnt);
        });
        closeClickable.activators.Add(new ManipulatorActivationFilter {button = MouseButton.MiddleMouse});
        btn.AddManipulator(closeClickable);
    }
}

public class McRtcWindow : EditorWindow
{
    private uint ntabs = 0;
    private static Queue<Action> work = new Queue<Action>();
    private IntPtr gui_handle;
    [MenuItem("McRtc/GUI")]
    public static void ShowExample()
    {
        var wnd = GetWindow<McRtcWindow>();
        wnd.titleContent = new GUIContent("McRtcGUI");
    }

    [DllImport ("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void StartClient();
    [DllImport ("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void StopClient();

    // This will be called when a new robot is seen by the GUI
    private delegate void OnRobotCallback(string id);

    // This will be called to place a robot mesh in the scene
    private delegate void OnRobotMeshCallback(string id, string name, string path, float scale, float qw, float qx, float qy, float qz, float tx, float ty, float tz);

    // This will be called when a robot is removed from the scene
    private delegate void OnRemoveRobotCallback(string id);

    // This method lets us register our callback
    [DllImport ("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void RegisterCallbacks(OnRobotCallback robot_callback,
                                                 OnRobotMeshCallback robot_mesh_callback,
                                                 OnRemoveRobotCallback remove_robot_callback);

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
        work.Enqueue(() => { GetRobotRoot(id); });
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
        StartClient();
        RegisterCallbacks(OnRobot, OnRobotMesh, OnRemoveRobot);
        {
            var btn = new Button() { text = "Stop client" };
            rootVisualElement.Add(btn);
            btn.clicked += () =>
            {
                StopClient();
            };
        }
        {
            var btn = new Button();
            btn.text = "Add tab";
            rootVisualElement.Add(btn);
            var tabRoot = new TabBar();
            rootVisualElement.Add(tabRoot);
            btn.clicked += () =>
            {
                ntabs += 1;
                tabRoot.AddTab($"Tab {ntabs}");
            };
        }
    }
}
