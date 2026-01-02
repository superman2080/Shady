using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using PlayerNameSpace;

[CustomEditor(typeof(Player))]
public class PlayerInspecterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (Player)target;

        DrawDefaultInspector();
        EditorGUILayout.LabelField("Width Curve (0~1 range)");
        //script.dashSpeed = EditorGUILayout.CurveField("Dash Speed", script.dashSpeed, Color.red, new Rect(0, 0, 1, 1));
        EditorUtility.SetDirty(script);
    }
}
#endif
