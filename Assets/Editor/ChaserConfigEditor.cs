using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChaserControllerConfig))]
public class ChaserConfigEditor : Editor {

	public override void OnInspectorGUI(){
		base.DrawDefaultInspector();
		ChaserControllerConfig config = target as ChaserControllerConfig;
		if(config.targettingType == TargettingModule.Technique.ByEntityType){
			GUILayout.BeginHorizontal();
			GUILayout.Label("Target Entity Type");
			config.targetEntityType = (Entity.EntityType)EditorGUILayout.EnumPopup(config.targetEntityType);
			GUILayout.EndHorizontal();
		}
	}
}