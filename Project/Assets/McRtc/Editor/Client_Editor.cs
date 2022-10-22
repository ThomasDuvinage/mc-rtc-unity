using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace McRtc
{
    [CustomEditor(typeof(Client))]
    public class Client_Inspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            // Add default fields
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            var btn = new Button();
            btn.text += "Connect";
            btn.clicked += () => ((Client)target).Reconnect();
            root.Add(btn);

            return root;
        }
    }

}