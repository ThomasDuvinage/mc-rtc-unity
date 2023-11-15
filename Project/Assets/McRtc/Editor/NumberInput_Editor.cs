using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace McRtc
{
    [CustomEditor(typeof(NumberInput))]
    public class NumberInput_Editor : Element_Editor
    {
        private float data_in;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            float dataIn = ((NumberInput)target).data;
            GUILayout.Label("current value: " + dataIn.ToString());
            data_in = EditorGUILayout.FloatField("Set to: ", data_in);
            if(GUILayout.Button("Set"))
            {
                ((NumberInput)target).data = data_in;
            }
        }
    }
}

