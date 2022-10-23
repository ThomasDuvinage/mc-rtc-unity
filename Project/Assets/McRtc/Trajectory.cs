using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using IntPtr = System.IntPtr;

namespace McRtc
{
    [ExecuteAlways]
    public class Trajectory : MonoBehaviour
    {
        public string id;
        private Vector3[] points = new Vector3[0];
        // Start is called before the first frame update
        void Start()
        {
            GetRenderer();
        }

        LineRenderer GetRenderer()
        {
            LineRenderer line = GetComponent<LineRenderer>();
            if (line)
            {
                return line;
            }
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            return lineRenderer;
        }

        public void UpdatePoints(IntPtr data, nuint size)
        {
            points = new Vector3[size];
            float[] point = new float[3];
            for (nuint i = 0; i < size; ++i)
            {
                Marshal.Copy(data, point, 0, 3);
                points[i].x = point[0];
                points[i].z = point[1];
                points[i].y = point[2];
                points[i] = transform.TransformPoint(points[i]);
                data += 3 * sizeof(float);
            }
        }

        public void DeleteTrajectory()
        {
            points = new Vector3[0];
            LineRenderer line = GetRenderer();
            line.positionCount = 0;
            line.SetPositions(points);
        }

        // Update is called once per frame
        void Update()
        {
            LineRenderer line = GetRenderer();
            line.positionCount = points.Length;
            line.SetPositions(points);
        }
    }

}