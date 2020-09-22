using UnityEngine;
using System.Collections.Generic;

public class CircleHitbox : Hitbox {
	public Vector2 Origin;
	public float Radius;

	public CircleHitbox(Vector2 origin, float radius, LayerMask layerMask){
		Origin = origin;
		Radius = radius;
		LayerMask = layerMask;
	}

	public override List<Entity> GetFreshHits(){
		_freshHits.Clear();
		int num = Physics2D.OverlapCircleNonAlloc(Origin, Radius, _hitResults, LayerMask);
		for(int i = 0; i < num; i++){
			Collider2D col = _hitResults[i];
			Entity entity = Entity.GetEntityForGameObject(col.gameObject);
			if(entity == null || _alreadyHit.Contains(entity)){
				continue;
			}
			_alreadyHit.Add(entity);
			_freshHits.Add(entity);
		}
		return _freshHits;
	}
}