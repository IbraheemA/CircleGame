using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager {
	public GameEvents Events { get; private set; }

	//XXX NOT THE REAL WAY I WILL IMPLEMENT THIS
	List<Entity> _entities =  new List<Entity>();

	public GameManager(){
		SpawnEntityFromConfig(EntityCatalog.TestGetPlayerConfig(), () => { return Vector2.zero; });
		for(int i = 0; i < 10; ++i){
			SpawnEntityFromConfig(EntityCatalog.TestGetEnemyConfig(), ()=>{ return new Vector2(UnityEngine.Random.Range(-5f,5f), UnityEngine.Random.Range(-5f,5f)-15); });
		}
		Events = new GameEvents();
		Events.EntityDied += UntrackDeadEntities;
		uint _spawnCounter = 0;
		OnFixedUpdate += () => {
			if(_spawnCounter == 60){
				SpawnEntityFromConfig(EntityCatalog.TestGetEnemyConfig(), ()=>{ return new Vector2(UnityEngine.Random.Range(-5f,5f), UnityEngine.Random.Range(-5f,5f)-15); });
				_spawnCounter = 0;
			}
			_spawnCounter++;
		};
	}

	private void SpawnEntityFromConfig(EntityConfig config, Func<Vector2> positionCallback, int tolerance = 10){
		Vector2 position = positionCallback();
		Bounds bounds = config.LinkedObjectBounds;
		float size = Mathf.Max(bounds.extents.x, bounds.extents.y);
		int i = 0;
		while(Physics2D.OverlapCircle(position, size, GameInfo.WallsLayer)){
			// if(i = 0)
			position = positionCallback();
		}
		_entities.Add(Entity.CreateFromConfig(config, position));
	}
	
	public void FixedUpdate(){

		OnFixedUpdate?.Invoke();

		foreach(Entity entity in _entities.ToList()){
			entity.FixedUpdate();
		}

		foreach(Entity entity in _entities.ToList()){
			entity.GetCommands();
		}

		foreach(Entity entity in _entities.ToList()){
			entity.ExecuteCommands();
		}

		foreach(Entity entity in _entities.ToList()){
			entity.UpdatePhysics();
		}
		UpdatePhysics?.Invoke();
	}
	
	public void Update(){
		foreach(Entity entity in _entities.ToList()){
			entity.Update();
		}
	}

	public event Action UpdatePhysics;
	public event Action OnFixedUpdate;

	private void UntrackDeadEntities(DeathEventData eventData){
		_entities.Remove(eventData.DeadEntity);
	}
}
