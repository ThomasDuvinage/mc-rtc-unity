using Codice.CM.Common;
using UnityEditor;
using UnityEngine;

namespace McRtc
{
    [CustomEditor(typeof(Element), true)]
    public class Element_Editor : Editor
    {
        private int i = 0;
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
        public override void OnInspectorGUI()
        {
            Element e = (Element)target;
            if (e.connected)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontStyle = FontStyle.Bold;
                GUILayout.Label("Online", style);
                EditorGUILayout.LabelField("Update delay:", $"{e.last_update_delay * 1000:0.0}ms ago");
            }
            else
            {
                EditorGUILayout.LabelField("Offline");
            }
            DrawDefaultInspector();
        }
    }
}