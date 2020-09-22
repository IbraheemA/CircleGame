using UnityEngine;
using System;
using System.Collections.Generic;

public class Entity : ISelfTypeCreator {

	public string Name = "Entity";
	public EntityType Type { get; set; }

	public Vector2 Velocity;
	public Vector2 ExternalVelocity = Vector2.zero;

	public Entity(){
		_damageTaken = 0;
		IsInvincible = false;
		_invincibilityTimer = 0;
		OnDamaged += GetInvincibilityFromDamage;
		for(int i = 0; i < GameInfo.StatsLength; i++){
			_stats[i] = _baseStats[i] = _statModifiers[i] = _statMultipliers[i] = 0;
		}
		RecalculateAllStats();
	}

	public void FixedUpdate(){
		if(_invincibilityTimer == 0){
			IsInvincible = false;
		}
		else {
			_invincibilityTimer--;
		}
		OnFixedUpdate?.Invoke(this);
	}

	public void GetCommands(){
		GetCommandsHandler?.Invoke(this);
	}

	public void ExecuteCommands(){
		ExecuteCommandsHandler?.Invoke(this);
	}

	public void UpdatePhysics(){
		PhysicsHandler.Update(this);
	}

	public void MoveTo(Vector2 destination){
		Rigidbody.MovePosition(destination);
	}

	public void TakeDamage(DamageInstance damageInstance){
		if(IsInvincible){ damageInstance.IsValid = false; }
		OnBeforeDamage?.Invoke(damageInstance);
		if(damageInstance.IsValid){
			damageInstance.Amount -= GetStat(GameInfo.Stats.FlatDamageReduction);
			CurrentHealth -= damageInstance.Amount;
			OnDamaged?.Invoke(damageInstance);
			damageInstance.DoOnAfterApplied();
		}
		if(CurrentHealth <= 0){
			TriggerDeath();
		}
	}

	public void ApplyKnockback(Vector2 launchVelocity, uint duration){
		Launch l = new Launch(launchVelocity, duration);
		l.OnApply(this);
	}

	public void ReceiveCollision(CollisionEventData eventData){
		OnCollision?.Invoke(eventData);
	}

	public void ApplyBuff(Buff buff, bool trackWithOtherBuffs = true){
		if(trackWithOtherBuffs){ _activeBuffs.Add(buff); }
		buff.OnApply(this);
	}

	public float GetStat(GameInfo.Stats stat){
		return _stats[(int)stat];
	}

	public void SetBaseStat(GameInfo.Stats stat, float value){
		_baseStats[(int)stat] = value;
		RecalculateStat(stat);
	}

	public void IncrementStatModifier(GameInfo.Stats stat, float value){
		_statModifiers[(int)stat] += value;
		RecalculateStat(stat);
	}

	public void IncrementStatMultiplier(GameInfo.Stats stat, float value){
		_statMultipliers[(int)stat] += value;
		RecalculateStat(stat);
	}

	private void RecalculateStat(GameInfo.Stats stat){
		_stats[(int)stat] = _statFormulas[(int)stat](_baseStats[(int)stat], _statModifiers[(int)stat], _statMultipliers[(int)stat]);
	}

	private void RecalculateAllStats(){
		for(int i = 0; i < GameInfo.StatsLength; i++){
			RecalculateStat((GameInfo.Stats)i);
		}
	}

	public void GetInvincibilityFromDamage(DamageInstance damageInstance){
		if(damageInstance.Amount > 0 && damageInstance.InvincibilityDuration > 0){
			IsInvincible = true;
			_invincibilityTimer = damageInstance.InvincibilityDuration;
		}
	}

	public void TriggerDeath(){
		DeathEventData deathEventData = new DeathEventData(this);
		OnBeforeDeath?.Invoke(deathEventData);
		if(!deathEventData.IsValid){ return; }
		OnDeath?.Invoke(deathEventData);
		foreach(KeyValuePair<Type, IEntityCallbackUser> entry in _controllers){
			entry.Value.UnlinkCallbacksFromEntity(this);
			// _controllers.Remove(entry.Key);
		}
		IsDead = true;
		MainManager.GameManager.Events.TriggerEntityDied(deathEventData);
		UnityEngine.Object.Destroy(GameObject);
	}

	public event Action<Entity> GetCommandsHandler;
	public event Action<Entity> ExecuteCommandsHandler;

	public event Action<Entity> OnFixedUpdate;
	public event Action<Entity> OnUpdate;
	public event Action<CollisionEventData> OnCollision;
	public event Action<Entity, GameInfo.ActionType, byte> OnInterrupt;
	public event Action<DamageInstance> OnBeforeDamage;
	public event Action<DamageInstance> OnDamaged;
	public event Action<DeathEventData> OnBeforeDeath;
	public event Action<DeathEventData> OnDeath;

	public bool CanExecuteActionOfType(GameInfo.ActionType actionType, byte priority = 0){
		if(priority == 0){return _actionLocks[(int)actionType] == 0;}
		return _actionLocks[(int)actionType] < priority;
	}

	public void LockAllActions(byte priority){
		for(int i = 0; i < _actionLocks.Length; i++){
			if(_actionLocks[i] > priority){ continue; }
			_actionLocks[i] = priority;
			OnInterrupt?.Invoke(this, (GameInfo.ActionType)i, priority);
		}
	}

	public void UnlockAllActions(byte priority){
		for(int i = 0; i < _actionLocks.Length; i++){
			if(_actionLocks[i] <= priority){
				_actionLocks[i] = 0;
			}
		}
	}

	public void LockActionType(GameInfo.ActionType actionType, byte priority = 1){
		if(_actionLocks[(int)actionType] < priority){
			_actionLocks[(int)actionType] = priority;
			OnInterrupt?.Invoke(this, actionType, priority);
		}
	}

	public void UnlockActionType(GameInfo.ActionType actionType, byte priority){
		if(_actionLocks[(int)actionType] <= priority){
			_actionLocks[(int)actionType] = 0;
		}
	}

	// NOTE: do not use this for game actions! only for cosmetic things like graphics, etc.
	public void Update(){
		OnUpdate?.Invoke(this);
	}

	public void OverwriteControllers(IEnumerable<IEntityCallbackUser> newControllers){
		foreach(IEntityCallbackUser controller in _controllers.Values){
			controller.UnlinkCallbacksFromEntity(this);
		}
		_controllers.Clear();
		if(newControllers == null){ return; }
		foreach(IEntityCallbackUser controller in newControllers){
			ProvideController(controller);
		}
	}

	public void ProvideController(IEntityCallbackUser controller){
		controller.LinkCallbacksToEntity(this);
		_controllers.Add(controller.GetType(), controller);
	}

	public T GetController<T>() where T : class, IEntityCallbackUser {
		_controllers.TryGetValue(typeof(T), out IEntityCallbackUser controller);
		return controller as T;
	}

	Dictionary<Type, IEntityCallbackUser> _controllers = new Dictionary<Type, IEntityCallbackUser>();
	public PhysicsHandler PhysicsHandler;
	public Transform Transform { get; private set; }
	public Rigidbody2D Rigidbody { get; private set; }
	public Collider2D Collider { get; private set; }
	public Vector2 Position { get { return Rigidbody.position; }}
	private GameObject _gameObject;
	public GameObject GameObject{
		get {
			return _gameObject;
		}
		set {
			// if(value == null){ return; }
			if(_gameObject != null){
				_entitiesByGameObjects.Remove(_gameObject);
			}
			_gameObject = value;
			_entitiesByGameObjects.Add(_gameObject, this);
			Transform = _gameObject.transform;
			Collider = _gameObject.GetComponent<Collider2D>();
			Rigidbody = _gameObject.GetComponent<Rigidbody2D>();
		}
	}

	public bool IsDead { get; private set; }
	public bool IsInvincible { get; set; }
	private uint _invincibilityTimer;

	public float CurrentHealth {
		get {
			return GetStat(GameInfo.Stats.MaxHealth) - _damageTaken;
		}
		private set {
			_damageTaken = Mathf.Max(0,GetStat(GameInfo.Stats.MaxHealth) - value);
		}
	}

	private float _damageTaken;
	private float[] _stats = new float[GameInfo.StatsLength];
	private float[] _baseStats = new float[GameInfo.StatsLength];
	private float[] _statModifiers = new float[GameInfo.StatsLength];
	private float[] _statMultipliers = new float[GameInfo.StatsLength];
	static Func<float, float, float, float>[] _statFormulas = new Func<float, float, float, float>[GameInfo.StatsLength];

	public enum EntityType {
		Undefined,
		Player,
		Enemy
	}

	private List<Buff> _activeBuffs = new List<Buff>();
	byte[] _actionLocks = new byte[GameInfo.ActionTypesLength];

	private static Dictionary<GameObject, Entity> _entitiesByGameObjects = new Dictionary<GameObject, Entity>();
	
	public static Entity GetEntityForGameObject(GameObject gameObject){
		if(_entitiesByGameObjects.TryGetValue(gameObject, out Entity e)){
			return e;
		}
		return null;
	}

	public static Entity CreateFromConfig(EntityConfig config, Vector2? location = null){
		Entity en = new Entity();
		en.GameObject = UnityEngine.Object.Instantiate(config.LinkedObject, location ?? Vector2.zero, Quaternion.identity);
		if(!String.IsNullOrEmpty(config.Name)){
			en.Name = config.Name;
		}
		en.Type = config.EntityType;
		en.OverwriteControllers(config.CreateControllers());
		en.PhysicsHandler = SelfTypeCreatorFactory<PhysicsHandler>.Create<PhysicsHandler>();
		foreach(KeyValuePair<GameInfo.Stats,float> entry in config.StatDict){
			en.SetBaseStat(entry.Key, entry.Value);
		}
		en.RecalculateAllStats();
		return en;
	}

	private static float DefaultStatFormula(float baseStat, float flatModifier, float multiplier){
		return (baseStat + flatModifier) * (1 + multiplier);
	}

	static Entity(){
		for(int i = 0; i < GameInfo.StatsLength; i++){
			_statFormulas[i] = DefaultStatFormula;
		}
		_statFormulas[(int)GameInfo.Stats.MoveSpeed] = (baseStat, flatModifier, multiplier) => {return Mathf.Max((baseStat + flatModifier) * (1 + multiplier), 0.1f);};
	}

	public ISelfTypeCreator CreateSelfType(){
		return new Entity();
	}
}
