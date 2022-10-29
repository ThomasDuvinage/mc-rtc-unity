using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace McRtc
{
    [CustomEditor(typeof(Checkbox))]
    public class Checkbox_Editor : Element_Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            bool stateIn = ((Checkbox)target).state;
            if(stateIn != EditorGUILayout.Toggle("State", stateIn))
            {
                ((Checkbox)target).state = !stateIn;
            }
        }
    }
}