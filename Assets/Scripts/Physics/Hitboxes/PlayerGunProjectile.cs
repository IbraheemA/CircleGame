using UnityEngine;

public class PlayerGunProjectile : Projectile {

	Entity _sourceEntity;
	GameObject _gameObject;
	GameObject GameObject {
		get { return _gameObject; }
		set {
			_gameObject = value;
			_rigidbody = _gameObject.GetComponent<Rigidbody2D>();
		}
	}
	Rigidbody2D _rigidbody;
	float _projectileRadius = 0.2f;
	Vector2 _velocity;
	float _knockbackSpeed;
	LayerMask _layerMask;
	RaycastHit2D[] _hitResults = new RaycastHit2D[100];

	public PlayerGunProjectile(Entity sourceEntity, Vector2 startingPostion, Vector2 velocity, float knockbackSpeed){
		GameObject = Object.Instantiate(_bulletPrefab, startingPostion, Quaternion.identity);
		_rigidbody.position = startingPostion;
		_velocity = velocity;
		_knockbackSpeed = knockbackSpeed;
		_sourceEntity = sourceEntity;
		_layerMask = GameInfo.EnemiesLayer;
	}

	protected override void UpdatePhysics(){
		Vector2 velocityThisFrame = _velocity;
		bool hasHitAnObstacle = false;
		int num1 = Physics2D.CircleCastNonAlloc(_rigidbody.position, _projectileRadius, _velocity.normalized, _hitResults, _velocity.magnitude, GameInfo.HighWallsLayer);
		for(int i = 0; i < num1; i++){
			RaycastHit2D hit = _hitResults[i];
			if(hit.distance < velocityThisFrame.magnitude){
				hasHitAnObstacle = true;
				velocityThisFrame = velocityThisFrame.normalized * hit.distance;
			}
		}
		int num2 = Physics2D.CircleCastNonAlloc(_rigidbody.position, _projectileRadius, velocityThisFrame.normalized, _hitResults, velocityThisFrame.magnitude, _layerMask);
		for(int i = 0; i < num2; i++){
			RaycastHit2D hit = _hitResults[i];
			Entity entity = Entity.GetEntityForGameObject(hit.collider.gameObject);
			if(entity == null){
				continue;
			}
			DamageInstance d = new DamageInstance(_sourceEntity, entity, 3, 0);
			d.OnAfterApplied += (DamageInstance) => {
				entity.ApplyKnockback(_velocity * 0.08f, 8);
				entity.ApplyBuff(new StunDebuff(2, 1));
				Destroy();
			};
			entity.TakeDamage(d);
		}
		if(hasHitAnObstacle){
			Destroy();
			return;
		}
		_rigidbody.MovePosition(_rigidbody.position + velocityThisFrame);
	}

	protected override void Destroy()
	{
		base.Destroy();
		Object.Destroy(GameObject);
	}

	static GameObject _bulletPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/BasicBullet");
}