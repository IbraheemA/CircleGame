using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[CreateAssetMenu(menuName = "CircleGame/Entities/EntityConfig")]
public class EntityConfig : ScriptableObject, ISerializationCallbackReceiver {
	public EntityConfig OptionalParentConfig;


	[Header("Basic Info")]
	public string Name;
	public Entity.EntityType EntityType;

	[Header("Components")]
	[SerializeField]
	private List<ControllerConfig> ControllerConfigs = new List<ControllerConfig>();
	public GameObject LinkedObject;

	public Bounds LinkedObjectBounds { get; private set; }

	[Serializable]
	public class FloatNullable {
		public bool HasValue = true;
		public float Value;
	}
	[Header("Stats")]
	public FloatNullable MaxHealth;
	public FloatNullable MoveSpeed;

	Dictionary<GameInfo.Stats, float> _statDict;
	public ReadOnlyDictionary<GameInfo.Stats, float> StatDict;

	public List<IEntityCallbackUser> CreateControllers(){
		if(ControllerConfigs.Count == 0){
			return null;
		}
		List<IEntityCallbackUser> controllers = new List<IEntityCallbackUser>();
		foreach(ControllerConfig config in ControllerConfigs){
			if(config == null){ continue; }
			controllers.Add(config.CreateController());
		}
		return controllers;
	}

	private void OnEnable(){
		GameObject g = UnityEngine.Object.Instantiate(LinkedObject);
		LinkedObjectBounds = g.GetComponent<Collider2D>().bounds;
		UnityEngine.Object.DestroyImmediate(g);
	}
	
	public void OnBeforeSerialize(){
	}

	public void OnAfterDeserialize(){
		_statDict = new Dictionary<GameInfo.Stats, float>();
		StatDict = new ReadOnlyDictionary<GameInfo.Stats, float>(_statDict);
		if(MaxHealth.HasValue){_statDict.Add(GameInfo.Stats.MaxHealth, MaxHealth.Value);}
		if(MoveSpeed.HasValue){_statDict.Add(GameInfo.Stats.MoveSpeed, MoveSpeed.Value);}
	}
}