using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(SizeScales))]
public class SizeScalesInspector : Editor {

    public override VisualElement CreateInspectorGUI() {
        VisualElement inspector = new VisualElement();
        PropertyField sizesProperty = new PropertyField(this.serializedObject.FindProperty("sizesF"));

        sizesProperty.schedule.Execute(() => {
            IntegerField sizeField = sizesProperty.Q<IntegerField>();
            sizeField.SetEnabled(false);
        });

        inspector.Add(sizesProperty);

        return inspector;
    }

}
