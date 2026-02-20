using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OffensiveTargeting))]
public class OffensiveTargetingEditor : Editor
{
    SerializedProperty tagPrioritiesProp;
    SerializedProperty defaultTargetProp;
    SerializedProperty defaultTargetTagProp;
    SerializedProperty detectionRadiusProp;
    SerializedProperty aggroMultiplierProp;
    SerializedProperty evaluationIntervalProp;

    void OnEnable()
    {
        tagPrioritiesProp = serializedObject.FindProperty("tagPriorities");
        defaultTargetProp = serializedObject.FindProperty("defaultTarget");
        defaultTargetTagProp = serializedObject.FindProperty("defaultTargetTag");
        detectionRadiusProp = serializedObject.FindProperty("detectionRadius");
        aggroMultiplierProp = serializedObject.FindProperty("aggroMultiplier");
        evaluationIntervalProp = serializedObject.FindProperty("evaluationInterval");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        int size = tagPrioritiesProp.arraySize;
        int newSize = EditorGUILayout.IntField(new GUIContent("Tag Priorities", "Unity tags and priority (0-1). Same tags as GameObject.tag."), size);
        if (newSize != size) tagPrioritiesProp.arraySize = newSize;
        EditorGUI.indentLevel++;
        for (int i = 0; i < tagPrioritiesProp.arraySize; i++)
        {
            SerializedProperty el = tagPrioritiesProp.GetArrayElementAtIndex(i);
            SerializedProperty tagProp = el.FindPropertyRelative("tag");
            SerializedProperty priorityProp = el.FindPropertyRelative("priority");
            string currentTag = tagProp.stringValue;
            string chosenTag = EditorGUILayout.TagField($"Tag {i}", string.IsNullOrEmpty(currentTag) ? "Untagged" : currentTag);
            tagProp.stringValue = chosenTag;
            EditorGUILayout.PropertyField(priorityProp, new GUIContent($"Priority {i}"));
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Default Target", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(defaultTargetProp, new GUIContent("Default Target", "e.g. Caravan Core. Leave empty to use tag below."));
        EditorGUILayout.PropertyField(defaultTargetTagProp, new GUIContent("Default Target Tag", "e.g. HomeBase. Resolved at runtime if Default Target is not set."));

        EditorGUILayout.Space(4);
        EditorGUILayout.PropertyField(detectionRadiusProp);
        EditorGUILayout.PropertyField(aggroMultiplierProp);
        EditorGUILayout.PropertyField(evaluationIntervalProp);

        DrawPropertiesExcluding(serializedObject, "tagPriorities", "defaultTarget", "defaultTargetTag", "detectionRadius", "aggroMultiplier", "evaluationInterval", "m_Script");
        serializedObject.ApplyModifiedProperties();
    }
}
