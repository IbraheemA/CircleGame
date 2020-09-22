using UnityEditor;

[CustomEditor(typeof(EntityConfig))]
public class EntityConfigEditor : Editor {

	public override void OnInspectorGUI(){
		base.DrawDefaultInspector();
		EntityConfig config = target as EntityConfig;
	}
}