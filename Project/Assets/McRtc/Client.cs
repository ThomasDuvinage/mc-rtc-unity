using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using IntPtr = System.IntPtr;
using System.Linq;
using System.Runtime.InteropServices;

namespace McRtc
{
    [ExecuteAlways]
    public class Client : ClientBase
    {
        public string host = "localhost";
        private Dictionary<System.Type, Element[]> elements = new Dictionary<System.Type, Element[]>();
        private bool reconnect = false;
        static Client active_instance = null;

        [DllImport("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CreateClient(string host);
        [DllImport("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UpdateClient();
        [DllImport("McRtcPlugin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void StopClient();

        static private void DoOn<T>(string id, System.Action<T> action) where T : Element
        {
            if(active_instance == null)
            {
                return;
            }
            T[] elements = (T[])active_instance.elements[typeof(T)];
            foreach(T item in elements)
            {
                if(item.id == id)
                {
                    action(item);
                    return;
                }
            }
        }

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
            DoOn<Robot>(id, r => r.UpdateRobot());
        }

        static void OnRobotBody(string rid, string body, PTransform X_0_body)
        {
            DoOn<Robot>(rid, r => r.UpdateBody(body, X_0_body));
        }

        static void OnRobotMesh(string rid, string body, string name, string path, float scale, PTransform X_body_visual)
        {
            DoOn<Robot>(rid, r => r.UpdateMesh(body, name, path, scale, X_body_visual));
        }

        static void OnTrajectoryVector3d(string tid, IntPtr data, nuint size)
        {
            DoOn<Trajectory>(tid, t => t.UpdatePoints(data, size));
        }

        static void OnTransform(string tid, bool ro, PTransform pt)
        {
            DoOn<TransformElement>(tid, t => t.UpdateTransform(ro, pt));
        }

        static void OnCheckbox(string cbid, bool state)
        {
          DoOn<Checkbox>(cbid, cb => cb.UpdateState(state));
        }

        static void OnRemoveElement(string id, string type)
        {
            switch (type)
            {
                case "robot":
                    DoOn<Robot>(id, r => r.DeleteRobot());
                    break;
                case "trajectory":
                    DoOn<Trajectory>(id, t => t.DeleteTrajectory());
                    break;
                case "transform":
                    DoOn<TransformElement>(id, t => t.DeleteTransform());
                    break;
                case "checkbox":
                    DoOn<Checkbox>(id, cb => cb.Disconnect());
                    break;
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
            elements[typeof(Robot)] = Object.FindObjectsOfType<Robot>();
            elements[typeof(Trajectory)] = Object.FindObjectsOfType<Trajectory>();
            elements[typeof(TransformElement)] = Object.FindObjectsOfType<TransformElement>();
            elements[typeof(Checkbox)] = Object.FindObjectsOfType<Checkbox>();
            active_instance = this;
            DebugLogCallback(Debug.Log);
            OnRobot(Client.OnRobot);
            OnRobotBody(Client.OnRobotBody);
            OnRobotMesh(Client.OnRobotMesh);
            OnTrajectoryVector3d(Client.OnTrajectoryVector3d);
            OnTransform(Client.OnTransform);
            OnCheckbox(Client.OnCheckbox);
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
