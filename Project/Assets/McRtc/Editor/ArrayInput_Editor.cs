using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace McRtc
{
    [CustomEditor(typeof(ArrayInput))]
    public class ArrayInput_Editor : Editor
    {
        private bool editing = false;
        private float[] data = null;
        public override bool RequiresConstantRepaint()
        {
            return !editing;
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (editing)
            {
                ArrayInput input = (ArrayInput)target;
                for (int i = 0; i < data.Length; ++i)
                {
                    float d = EditorGUILayout.FloatField(input.labels[i], data[i]);
                    if (d != data[i])
                    {
                        data[i] = d;
                    }
                }
                if (GUILayout.Button("Send"))
                {
                    input.data = data;
                    editing = false;
                }
            }
            else
            {
                ArrayInput input = (ArrayInput)target;
                for (int i = 0; i < input.data.Length; ++i)
                {
                    EditorGUILayout.LabelField(input.labels[i], input.data[i].ToString());
                }
                if (GUILayout.Button("Edit"))
                {
                    editing = true;
                    data = input.data;
                }
            }
        }
    }
}