using System;
using UnityEngine;

static class Behaviours {
	
	// A "sandbox" class which provides methods usable to assemble behaviour
	
	public static Vector2 GetVectorToTargetEntity(Entity source, Entity target){
		return (target.Position - source.Position);
	}
	
	public static Vector2? GetVectorToTargetEntityIfInSight(Entity source, Entity target){
		if(!CheckIfEntityInSight(source, target)) { return null; }
		return (target.Rigidbody.position - source.Rigidbody.position);
	}

	public static bool CheckIfEntityInSight(Entity source, Entity target){
		Vector2 vec = GetVectorToTargetEntity(source, target);
		RaycastHit2D hit = Physics2D.Raycast(source.Rigidbody.position, vec.normalized, vec.magnitude, GameInfo.HighWallsLayer);
		return hit.collider == null;
	}

	public static Vector2 GetDirectionToTargetEntity(Entity source, Entity target){
		return GetVectorToTargetEntity(source, target).normalized;
	}

	public static Action<Entity> GetMoveToTargetEntityCommand(Entity source, Entity target, byte priority = 0){
		Vector2 vec = GetVectorToTargetEntity(source, target);
		vec = vec.normalized * Mathf.Min(vec.magnitude, source.GetStat(GameInfo.Stats.MoveSpeed));
		return GetBasicMoveCommand(vec, priority);
	}

	public static Action<Entity> GetEntityMoveCommandFromInput(Entity entity, InputSource input, byte priority = 0){
		Vector2 direction = input.MoveVector.normalized;
		return GetBasicMoveCommand(direction * entity.GetStat(GameInfo.Stats.MoveSpeed), priority);
	}

	public static Action<Entity> GetBasicMoveCommand(Vector2 direction, byte priority = 0){
		return (entity) => {
			if(!entity.CanExecuteActionOfType(GameInfo.ActionType.Move,priority)) {return;}
			entity.Velocity = direction;
		};
	}

	public static Action<Entity> GetStopCommand(byte priority = 0){
		return GetBasicMoveCommand(Vector2.zero, priority);
	}

	public static Entity SeekTargetWithRadiusAndCallbackNonAlloc(Entity entity, float seekRadius, Func<Entity, Entity, bool> callback, Collider2D[] seekResults){
		int numHits = Physics2D.OverlapCircleNonAlloc(entity.Rigidbody.position, seekRadius, seekResults, GameInfo.PlayersLayer);
		for(int i = 0; i < numHits; i++){
			Collider2D c = seekResults[i];
			Entity e = Entity.GetEntityForGameObject(c.gameObject);
			if(e == entity) {continue;}
			if(callback != null && e != null && callback(entity, e)){
				return e;
			};
		}
		return null;
	}

	public static Action<Entity> MoveToTargetIfInRange(Entity entity, Entity target, float aggroRange, byte priority = 0){
		if(target != null){
			float distance = Behaviours.GetVectorToTargetEntity(entity, target).magnitude;
			if(distance < aggroRange){
				return Behaviours.GetMoveToTargetEntityCommand(entity, target, priority);
			}
		}
		return null;
	}

	public static Action<Entity> MoveToTargetIfInRangeAndSight(Entity entity, Entity target, float aggroRange, byte priority = 0){
		if(target != null){
			Vector2? vec = Behaviours.GetVectorToTargetEntityIfInSight(entity, target);
			if(!vec.HasValue){ return null; }
			float distance = vec.Value.magnitude;
			if(distance < aggroRange){
				return Behaviours.GetMoveToTargetEntityCommand(entity, target, priority);
			}
		}
		return null;
	}
}