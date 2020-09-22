using UnityEngine;

public class TrackingCircle : IEntityCallbackUser {

	public int TimeBetweenFlashes;
	float _currentRadius;
	public float CurrentRadius{
		get {
			return _currentRadius;
		}
		set {
			_currentRadius = Mathf.Max(value, InnerRadius);
		}
	}
	public float InnerRadius;

	int _counter;
	Vector2 _postLastTick;
	Vector2 _posTwoTicksAgo;
	SpriteRenderer _outerSpriteRenderer;
	SpriteRenderer _innerSpriteRenderer;
	GameObject _outerCircleObject;
	GameObject _innerCircleObject;

	public TrackingCircle(GameObject outerCirclePrefab, GameObject innerCirclePrefab){
		_outerCircleObject = Object.Instantiate(outerCirclePrefab);
		_innerCircleObject = Object.Instantiate(innerCirclePrefab);
		_outerSpriteRenderer = _outerCircleObject.GetComponent<SpriteRenderer>();
		_innerSpriteRenderer = _innerCircleObject.GetComponent<SpriteRenderer>();
		_counter = 1;
		TimeBetweenFlashes = 15;
		InnerRadius = 2;
		CurrentRadius = InnerRadius;

		Color tmp = _outerSpriteRenderer.color;
		tmp.a = 0;
		_outerSpriteRenderer.color = tmp;

		Color tmp2 = _innerSpriteRenderer.color;
		tmp2.a = 0;
		_innerSpriteRenderer.color = tmp2;
	}

	public void LinkCallbacksToEntity(Entity entity){
		entity.OnFixedUpdate += OnFixedUpdate;
		entity.OnUpdate += OnUpdate;
		entity.OnDeath += RemoveOnDeath;
		_postLastTick = _posTwoTicksAgo = entity.Position;
	}

	private void OnFixedUpdate(Entity entity){
		if(_counter == 0){
			Color tmp = _outerSpriteRenderer.color;
			tmp.a = 1;
			_outerSpriteRenderer.color = tmp;

			Color tmp2 = _innerSpriteRenderer.color;
			tmp2.a = 1;
			_innerSpriteRenderer.color = tmp2;

			_innerCircleObject.transform.localScale = InnerRadius * 2 * Vector2.one;
			_outerCircleObject.transform.localScale = CurrentRadius * 2 * Vector2.one;

			Vector2 newPosition = _posTwoTicksAgo;
			_outerCircleObject.transform.position = newPosition;
			_innerCircleObject.transform.position = newPosition;
			_posTwoTicksAgo = _postLastTick;
			_postLastTick = entity.Position;

			CircleHitbox c = new CircleHitbox(newPosition, CurrentRadius, GameInfo.PlayersLayer);
			if(c.GetFreshHits().Contains(entity)){
				if((entity.Position - newPosition).magnitude <= InnerRadius){
					CurrentRadius += 0.6f;
				}
				entity.TakeDamage(new DamageInstance(entity, entity, 2));
			}
			else {
				CurrentRadius -= 0.3f;
			}
		}
		_counter = (_counter + 1) % TimeBetweenFlashes;
	}

	public void UnlinkCallbacksFromEntity(Entity entity){
		entity.OnFixedUpdate -= OnFixedUpdate;
		entity.OnUpdate -= OnUpdate;
		entity.OnDeath -= RemoveOnDeath;
	}

	private void OnUpdate(Entity entity){
		Color tmp = _outerSpriteRenderer.color;
		tmp.a -= Time.deltaTime * 60 * 0.05f;
		_outerSpriteRenderer.color = tmp;

		Color tmp2 = _innerSpriteRenderer.color;
		tmp2.a -= Time.deltaTime * 60 * 0.05f;
		_innerSpriteRenderer.color = tmp2;
	}

	private void RemoveOnDeath(DeathEventData data){
		Object.Destroy(_outerCircleObject);
		Object.Destroy(_innerCircleObject);
	}
}