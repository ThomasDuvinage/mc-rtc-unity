using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = System.Action;
using IntPtr = System.IntPtr;
using System.Linq;
using System.Runtime.InteropServices;

[ExecuteAlways]
public class McRtcClient : MonoBehaviour
{
    public string host = "localhost";
    private McRtcRobot[] robots;

    [DllImport("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void CreateClient(string host);
    [DllImport("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void UpdateClient();
    [DllImport("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void StopClient();

    // This will be called when a new robot is seen by the GUI
    private delegate void OnRobotCallback(string id);
    [DllImport("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void OnRobot(OnRobotCallback cb);

    // This will be called to place a robot mesh in the scene
    private delegate void OnRobotMeshCallback(string id, string name, string path, float scale, float qw, float qx, float qy, float qz, float tx, float ty, float tz);
    [DllImport("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void OnRobotMesh(OnRobotMeshCallback cb);

    // This will be called when a robot is removed from the scene
    private delegate void OnRemoveRobotCallback(string id);
    [DllImport("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void OnRemoveRobot(OnRemoveRobotCallback cb);

    void Start()
    {
        CreateClient(host);
    }

    void OnRobot(string id)
    {
        foreach (McRtcRobot robot in robots)
        {
            if(robot.id == id)
            {
                robot.UpdateRobot();
            }
        }
    }

    void OnRobotMesh(string id, string name, string path, float scale, float qw, float qx, float qy, float qz, float tx, float ty, float tz)
    {
        foreach (McRtcRobot robot in robots)
        {
            if (robot.id == id)
            {
                robot.UpdateMesh(name, path, scale, qw, qx, qy, qz, tx, ty, tz);
            }
        }
    }

    void OnRemoveRobot(string id)
    {
        foreach(McRtcRobot robot in robots)
        { 
            if(robot.id == id)
            {
                robot.DeleteRobot();
            }
        }
    }

    void OnValidate()
    {
        CreateClient(host);
    }

    void Update()
    {
        robots = Object.FindObjectsOfType<McRtcRobot>();
        OnRobot((string id) => OnRobot(id));
        OnRobotMesh((string id, string name, string path, float scale, float qw, float qx, float qy, float qz, float tx, float ty, float tz) => OnRobotMesh(id, name, path, scale, qw, qx, qy, qz, tx, ty, tz));
        OnRemoveRobot((string id) => OnRemoveRobot(id));
        UpdateClient();
    }

    void OnDestroy()
    {
        StopClient();
    }

    // Force the scene to update frequently in editor mode
    // Based on https://forum.unity.com/threads/solved-how-to-force-update-in-edit-mode.561436/
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
