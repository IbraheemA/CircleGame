using UnityEngine;
using System;
using System.Collections.Generic;

public class PhysicsHandler : ISelfTypeCreator {

	RaycastHit2D[] _raycastResults;
	CollisionEventSet _queuedCollisionEvents;

	ContactFilter2D filter;
	float _skinWidth = 0.1f;
	float _friction = 0.2f;
	float Friction {
		get { return _friction;} 
		set { _friction = Mathf.Max(1, value);}
	}

	public ISelfTypeCreator CreateSelfType(){
		return new PhysicsHandler();
	}

	public PhysicsHandler(){
		_raycastResults = new RaycastHit2D[100];
		filter = new ContactFilter2D(){
			useLayerMask = true,
			layerMask = GameInfo.WallsLayer
		};
		_queuedCollisionEvents = new CollisionEventSet();
	}

	public void Update(Entity entity){
		Vector2 movementVector = entity.Velocity + entity.ExternalVelocity;
		Vector2 originalVector = movementVector;
		ResolveCollision(entity, entity.Position, ref movementVector, out RaycastHit2D? nearestHit);
		Vector2 destination = entity.Position + movementVector;
		entity.MoveTo(destination);
		
		_queuedCollisionEvents.PushAllEventsWithinDistance(movementVector.magnitude);
		float remainingDistance = originalVector.magnitude - movementVector.magnitude;
		if(remainingDistance > 0 && nearestHit.HasValue){
			Vector2 perp = Vector2.Perpendicular(nearestHit.Value.normal);
			float dot = Vector2.Dot(originalVector.normalized, perp);
			Vector2 vectorAlongSurface = (perp * dot) * remainingDistance * (1 - Friction);
			
			ResolveCollision(entity, destination, ref vectorAlongSurface, out RaycastHit2D? nearestHit2);
			entity.MoveTo(destination + vectorAlongSurface);
		}
	}

	public void ResolveCollision(Entity entity, Vector2 origin, ref Vector2 velocityToResolve, out RaycastHit2D? nearestHit){
		nearestHit = null;
		int num = Physics2D.CircleCastNonAlloc(origin, entity.Collider.bounds.extents.x, velocityToResolve.normalized, _raycastResults, velocityToResolve.magnitude, GameInfo.WallsLayer);//entity.Collider?.Cast(entity.velocity.normalized, filter, _raycastResults, entity.velocity.magnitude) ?? 0;
		for(int i = 0; i < num; i++){
			RaycastHit2D hit = _raycastResults[i];
			float dot = Mathf.Abs(Vector2.Dot(velocityToResolve.normalized, hit.normal.normalized));
			float skinWidthToUse = _skinWidth / (dot != 0 ? dot : 1);
			float distanceToCollision = Mathf.Max(hit.distance - skinWidthToUse, 0);

			Entity entityHit = Entity.GetEntityForGameObject(hit.collider.gameObject);
			_queuedCollisionEvents.Add(distanceToCollision, new CollisionEventData(entity, entityHit));

			if(distanceToCollision < velocityToResolve.magnitude){
				velocityToResolve = velocityToResolve.normalized * distanceToCollision;
				nearestHit = hit;
			}
		}
	}

	public void GetMessage(GameInfo.Messages message, object data){

	}

	private class CollisionEventSet {

		SortedSet<Tuple<float, CollisionEventData>> _items;

		public CollisionEventSet(){
			_items = new SortedSet<Tuple<float, CollisionEventData>>(EventDistanceComparer.Instance);
		}

		public void Add(float distance, CollisionEventData data){
			_items.Add(new Tuple<float, CollisionEventData>(distance, data));
		}

		public void PushAllEventsWithinDistance(float distance){
			foreach(Tuple<float, CollisionEventData> pair in _items){
				if(pair.Item1 > distance){ break; }
				CollisionEventData eventData = pair.Item2;
				eventData.MainEntity.ReceiveCollision(eventData);
			}
			_items.Clear();
		}

		private class EventDistanceComparer : IComparer<Tuple<float, CollisionEventData>> {
			static Lazy<EventDistanceComparer> _instance = new Lazy<EventDistanceComparer>(() => { return new EventDistanceComparer();});
			public static EventDistanceComparer Instance {
				get { return _instance.Value; }
			}
			private EventDistanceComparer(){}
			public int Compare(Tuple<float, CollisionEventData> pair1, Tuple<float, CollisionEventData> pair2){
				return pair1.Item1.CompareTo(pair2.Item1);
			}
		}
	}
}