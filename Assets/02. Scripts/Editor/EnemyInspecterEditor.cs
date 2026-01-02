using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Enemy), true)]
public class EnemyInspecterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (Enemy)target;
        string state = script.stateMachine.CurState != null ? script.stateMachine.CurState.ToString() : "State is null";
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField($"State: { state}", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        DrawDefaultInspector();
        EditorUtility.SetDirty(script);
    }
}
#endif
