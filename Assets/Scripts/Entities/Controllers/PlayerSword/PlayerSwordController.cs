using UnityEngine;
using System;

public class PlayerSwordController : EntityController, IEntityCallbackUser {

	public InputSource InputSource;
	Action<Entity> _command;
	public uint CooldownTime = 8;
	uint _cooldownCounter;

	public PlayerSwordController(){
		InputSource = new KeyboardInputSource();
		_cooldownCounter = 0;
	}

	public override void LinkCallbacksToEntity(Entity entity){
		base.LinkCallbacksToEntity(entity);
		entity.OnUpdate += OnUpdate;
	}

	protected override void GetCommands(Entity entity){
		if(_cooldownCounter > 0){ 
			_cooldownCounter--;
			return;
		}
		if(InputSource.Attack2Held){
			Vector2 centerLine = InputSource.CursorPosition.normalized * 3;
			float angle = 30;
			_command = (attackingEntity) => {
				ConeHitbox hitbox = new ConeHitbox(attackingEntity.Position, centerLine, angle, GameInfo.EnemiesLayer);
				Debug.DrawRay(attackingEntity.Position, centerLine, Color.red, 1);
				foreach(Entity entityHit in hitbox.GetFreshHits()){
					if(entityHit.IsDead){ continue; }
					DamageInstance inst = new DamageInstance(attackingEntity, entityHit, 5, 0);
					inst.OnAfterApplied += (damageInstance) => {
						if(damageInstance.IsValid){
							damageInstance.AffectedEntity.ApplyKnockback(Behaviours.GetDirectionToTargetEntity(attackingEntity, entityHit), 2);
							entityHit.ApplyBuff(new StunDebuff(6));
						}
					};
					entityHit.TakeDamage(inst);
				}
				_cooldownCounter = CooldownTime;
			};
		}
	}

	protected override void ExecuteCommands(Entity entity){
		ExecuteSingleCommand(ref _command, entity);
	}

	public override void UnlinkCallbacksFromEntity(Entity entity){
		base.UnlinkCallbacksFromEntity(entity);
		entity.OnUpdate -= OnUpdate;
	}

	private void OnUpdate(Entity entity){
		InputSource.UpdateInput();
	}

	public override ISelfTypeCreator CreateSelfType(){
		return new PlayerSwordController();
	}
}