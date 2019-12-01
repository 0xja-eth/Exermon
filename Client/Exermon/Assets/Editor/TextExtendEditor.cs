using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;

[CustomEditor(typeof(TextExtend), true)]
[CanEditMultipleObjects]
public class TextExtendEditor : GraphicEditor {
    
    private static bool m_TextImageTextures = true;

    SerializedProperty m_Text;
    SerializedProperty m_FontData;
    SerializedProperty m_FilterLabels;
    SerializedProperty m_ContentSizeRate;

    //text spacing
    SerializedProperty m_Textures;


    protected override void OnEnable() {
        base.OnEnable();
        
        m_Text = serializedObject.FindProperty("m_Text");
        m_FontData = serializedObject.FindProperty("m_FontData");
        m_FilterLabels = serializedObject.FindProperty("m_FilterLabels");
        m_ContentSizeRate = serializedObject.FindProperty("m_ContentSizeRate");

        //text spacing
        m_Textures = serializedObject.FindProperty("m_QuadImageHandler.m_Textures");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(m_Text);
        EditorGUILayout.PropertyField(m_FontData);
        EditorGUILayout.PropertyField(m_ContentSizeRate);
        AppearanceControlsGUI();
        RaycastControlsGUI();
        addArrayField(m_FilterLabels);
        addArrayField(m_Textures);
        serializedObject.ApplyModifiedProperties();
    }
    void addArrayField(SerializedProperty field) {
        if (EditorGUILayout.PropertyField(field)) {
            // 缩进一级
            EditorGUI.indentLevel++;
            // 设置元素个数
            field.arraySize = EditorGUILayout.DelayedIntField("Size", field.arraySize);
            // 绘制元素
            for (int i = 0, size = field.arraySize; i < size; i++) {
                // 检索属性数组元素
                var element = field.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(element);
            }
            // 重置缩进
            EditorGUI.indentLevel--;
        }
    }
    /*
    private void PlusGUI() {
       EditorUtil.TextImageGUI(m_Textures, ref m_TextImageTextures);
    }*/
}
