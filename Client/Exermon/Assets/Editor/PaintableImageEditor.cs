using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;

using UI.Common.Controls.SystemExtend.PaintableImage;

[CustomEditor(typeof(PaintableImage), true)]
[CanEditMultipleObjects]
public class PaintableImageEditor : UnityEditor.UI.ImageEditor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("lineColor"));
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("thickness"));
        /*
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_onImageLinkClick"));
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_imageContainer"));
            */
        serializedObject.ApplyModifiedProperties();
    }
    
}
