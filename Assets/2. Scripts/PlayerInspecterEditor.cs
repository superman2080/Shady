using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PlayerCtrl))]
public class PlayerInspecterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (PlayerCtrl)target;

        EditorGUILayout.LabelField("Width Curve (0~1 range)");
        DrawDefaultInspector();
        script.dashSpeed = EditorGUILayout.CurveField("Dash Speed", script.dashSpeed, Color.red, new Rect(0, 0, 1, 1));
        EditorUtility.SetDirty(script);
    }
}
#endif
