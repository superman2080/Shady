using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PlayerCtrl))]
public class PlayerInspecterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (PlayerCtrl)target;

        DrawDefaultInspector();
        EditorGUILayout.LabelField("Width Curve (0~1 range)");
        script.dashSpeed = EditorGUILayout.CurveField("Dash Speed", script.dashSpeed, Color.red, new Rect(0, 0, 1, 1));
        EditorUtility.SetDirty(script);
    }
}
#endif
