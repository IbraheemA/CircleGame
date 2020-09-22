using UnityEngine;
using System.Collections.Generic;

public class ConeHitbox : Hitbox {
	public Vector2 Origin;
	public Vector2 CenterLine;
	public float Angle;

	public ConeHitbox(Vector2 origin, Vector2 centerLine, float angle, LayerMask layerMask){
		Origin = origin;
		CenterLine = centerLine;
		Angle = angle;
		LayerMask = layerMask;
	}

	public override List<Entity> GetFreshHits(){
		_freshHits.Clear();
		int num = Physics2D.OverlapCircleNonAlloc(Origin, CenterLine.magnitude, _hitResults, LayerMask);
		for(int i = 0; i < num; i++){
			Collider2D col = _hitResults[i];
			Entity entity = Entity.GetEntityForGameObject(col.gameObject);
			if(entity == null || _alreadyHit.Contains(entity) || Vector2.Angle(entity.Position - Origin, CenterLine.normalized) > Angle){
				continue;
			}
			_alreadyHit.Add(entity);
			_freshHits.Add(entity);
		}
		return _freshHits;
	}
}