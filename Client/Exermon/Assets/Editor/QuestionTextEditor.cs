using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;

using UI.Common.Controls.SystemExtend.QuestionText;

[CustomEditor(typeof(QuestionText), true)]
[CanEditMultipleObjects]
public class QuestionTextEditor : UnityEditor.UI.TextEditor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_embedImage"));
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_textures"), true);
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_imagePrefab"));
        /*
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_onImageLinkClick"));
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_imageContainer"));
            */
        serializedObject.ApplyModifiedProperties();
    }
    
}
