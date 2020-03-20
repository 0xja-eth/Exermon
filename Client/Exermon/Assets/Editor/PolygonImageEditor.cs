using UnityEngine;
using UnityEditor.UI;
using UnityEditor;
using UnityEditorInternal;

using UI.Common.Controls.RadarDisplay;

[CustomEditor(typeof(PolygonImage))]
[CanEditMultipleObjects]
public class PolygonImageEditor : GraphicEditor {

    private SerializedProperty m_Texture;
    private SerializedProperty m_DefaultValue;
    private ReorderableList m_ReorderableList;

    protected override void OnEnable() {
        base.OnEnable();
        m_Texture = serializedObject.FindProperty("_texture");
        m_DefaultValue = serializedObject.FindProperty("defaultValue");

        if (m_ReorderableList == null) {
            m_ReorderableList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("_weights"));
            m_ReorderableList.drawElementCallback = DrawEdgeWeight;
            m_ReorderableList.drawHeaderCallback = DrawHeader;
        }
    }

    private void DrawEdgeWeight(Rect rect, int index, bool isActive, bool isFocused) {
        SerializedProperty itemData = m_ReorderableList.serializedProperty.GetArrayElementAtIndex(index);
        EditorGUI.Slider(rect, itemData, 0, 1);
    }

    private void DrawHeader(Rect rect) {
        EditorGUI.LabelField(rect, "边权重");
    }

    protected override void OnDisable() {

    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_Texture);
        EditorGUILayout.PropertyField(m_Color);
        EditorGUILayout.PropertyField(m_Material);
        EditorGUILayout.PropertyField(m_RaycastTarget);
        EditorGUILayout.PropertyField(m_DefaultValue);

        m_ReorderableList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
