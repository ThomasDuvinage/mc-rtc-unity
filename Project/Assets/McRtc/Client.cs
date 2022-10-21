using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Action = System.Action;
using IntPtr = System.IntPtr;
using System.Linq;
using System.Runtime.InteropServices;

namespace McRtc
{
    [ExecuteAlways]
    public class Client : MonoBehaviour
    {
        public string host = "localhost";
        private Robot[] robots;
        private Trajectory[] trajectories;
        private bool reconnect = false;
        static Client active_instance = null;

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

        // This will be called to place a robot's body in the scene
        private delegate void OnRobotBodyCallback(string id, string body, PTransform X_0_body);
        [DllImport("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void OnRobotBody(OnRobotBodyCallback cb);

        // This will be called to place a robot's body's mesh in the scene
        private delegate void OnRobotMeshCallback(string id, string body, string name, string path, float scale, PTransform X_body_visual);
        [DllImport("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void OnRobotMesh(OnRobotMeshCallback cb);

        // This will be called when a robot is removed from the scene
        private delegate void OnRemoveElementCallback(string id, string type);
        [DllImport("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void OnRemoveElement(OnRemoveElementCallback cb);

        // This will be called when a trajectory should be visible in the scene
        private delegate void OnTrajectoryVector3dCallback(string id, IntPtr data, nuint size);
        [DllImport("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void OnTrajectoryVector3d(OnTrajectoryVector3dCallback cb);

        void Start()
        {
            CreateClient(host);
            if (active_instance != this)
            {
                active_instance = this;
            }
        }

        static void OnRobot(string id)
        {
            if (!active_instance)
            {
                return;
            }
            foreach (Robot robot in active_instance.robots)
            {
                if (robot.id == id)
                {
                    robot.UpdateRobot();
                }
            }
        }

        static void OnRobotBody(string rid, string body, PTransform X_0_body)
        {
            if (!active_instance)
            {
                return;
            }
            foreach (Robot robot in active_instance.robots)
            {
                if (robot.id == rid)
                {
                    robot.UpdateBody(body, X_0_body);
                }
            }
        }

        static void OnRobotMesh(string rid, string body, string name, string path, float scale, PTransform X_body_visual)
        {
            if (!active_instance)
            {
                return;
            }
            foreach (Robot robot in active_instance.robots)
            {
                if (robot.id == rid)
                {
                    robot.UpdateMesh(body, name, path, scale, X_body_visual);
                    return;
                }
            }
        }

        static void OnTrajectoryVector3d(string tid, IntPtr data, nuint size)
        {
            if(!active_instance)
            {
                return;
            }
            foreach(Trajectory trajectory in active_instance.trajectories)
            {
                if(trajectory.id == tid)
                {
                    trajectory.UpdatePoints(data, size);
                    return;
                }
            }
        }

        static void OnRemoveElement(string id, string type)
        {
            if (!active_instance)
            {
                return;
            }
            if (type == "robot")
            {
                foreach (Robot robot in active_instance.robots)
                {
                    if (robot.id == id)
                    {
                        robot.DeleteRobot();
                    }
                }
            }
            if(type == "trajectory")
            {
                foreach(Trajectory traj in active_instance.trajectories)
                {
                    if (traj.id == id)
                    {
                        traj.DeleteTrajectory();
                    }
                }
            }
        }

        public void Reconnect()
        {
            reconnect = true;
        }

        void OnDestroy()
        {
            if (active_instance == this)
            {
                active_instance = null;
            }
            StopClient();
        }

        void Awake()
        {
            if (Application.IsPlaying(gameObject))
            {
                SetupCallbacks();
            }
        }

        void SetupCallbacks()
        {
            if (active_instance != this)
            {
                active_instance = this;
            }
            robots = Object.FindObjectsOfType<Robot>();
            trajectories = Object.FindObjectsOfType<Trajectory>();
            active_instance = this;
            OnRobot(Client.OnRobot);
            OnRobotBody(Client.OnRobotBody);
            OnRobotMesh(Client.OnRobotMesh);
            OnTrajectoryVector3d(Client.OnTrajectoryVector3d);
            OnRemoveElement(Client.OnRemoveElement);
        }

        void Update()
        {
            if (reconnect)
            {
                reconnect = false;
                CreateClient(host);
            }
            if (!Application.IsPlaying(gameObject))
            {
                SetupCallbacks();
            }
            UpdateClient();
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
}