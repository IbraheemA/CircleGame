using UnityEngine;
using System;

public class SlashController : EntityController {

	public SlashController(){
		_targettingModule = new TargettingModule();
		_targettingModule.SeekRadius = 2;
		_targettingModule.CheckEntityCallback = (callingEntity, entityBeingChecked) => {
			if(entityBeingChecked.IsDead){ return false; }
			if(!Behaviours.CheckIfEntityInSight(callingEntity,entityBeingChecked)){ return false; }
			return entityBeingChecked.Type == Entity.EntityType.Player;
		};
	}

	protected override void GetCommands(Entity entity){
		if(!entity.CanExecuteActionOfType(GameInfo.ActionType.Attack,1)){ return; }
		Entity target = _targettingModule.SeekAndSetTarget(entity);
		if(target != null){
			_command = (attackingEntity) => {
				attackingEntity.Velocity = Vector2.zero;
				attackingEntity.LockActionType(GameInfo.ActionType.Attack, 1);
				attackingEntity.LockActionType(GameInfo.ActionType.Move, 1);
				Vector2 hitboxCenterLine = Behaviours.GetDirectionToTargetEntity(attackingEntity, _targettingModule.Target).normalized * 4;
				ushort counter = 0;
				ushort attackThreshold = 30;
				ushort finishAttackThreshold = 50;
				ushort endThreshold = 60;
				ConeHitbox hitbox = null;
				Action<Entity> OnFixedUpdate = null;
				Action<Entity, GameInfo.ActionType, byte> OnInterrupt = null;
				OnFixedUpdate = (e) => {
					if(counter == attackThreshold){
						hitbox = new ConeHitbox(e.Position, hitboxCenterLine, 40, GameInfo.PlayersLayer);
						Debug.DrawRay(e.Position, hitboxCenterLine.Rotate(-40), UnityEngine.Color.magenta,0.33f);
						Debug.DrawRay(e.Position, hitboxCenterLine.Rotate(40), UnityEngine.Color.magenta,0.33f);
					}
					else if(counter == finishAttackThreshold){
						hitbox = null;
					}
					else if(counter == endThreshold){
						e.UnlockActionType(GameInfo.ActionType.Attack, 1);
						e.UnlockActionType(GameInfo.ActionType.Move, 1);
						hitbox = null;
						e.OnFixedUpdate -= OnFixedUpdate;
						e.OnInterrupt -= OnInterrupt;
						return;
					}
					if(hitbox != null){
						hitbox.Origin = e.Position;
						foreach(Entity entityHit in hitbox.GetFreshHits()){
							if(entityHit.IsDead){ continue; }
							DamageInstance inst = new DamageInstance(e, entityHit, 5, 12);
							inst.OnAfterApplied += (damageInstance) => {
								if(damageInstance.IsValid){
									damageInstance.AffectedEntity.ApplyKnockback(Behaviours.GetDirectionToTargetEntity(e, entityHit), 2);
								}
							};
							entityHit.TakeDamage(inst);
						}
					}
					counter++;
				};
				entity.OnFixedUpdate += OnFixedUpdate;
				OnInterrupt = (e, actionType, priority) => {
					if(priority > 1 && actionType == GameInfo.ActionType.Attack){
						e.OnFixedUpdate -= OnFixedUpdate;
					}
				};
				entity.OnInterrupt += OnInterrupt;
			};
		}
	}

	protected override void ExecuteCommands(Entity entity){
		_command?.Invoke(entity);
		_command = null;
	}

	TargettingModule _targettingModule;
	Action<Entity> _command;

	public override ISelfTypeCreator CreateSelfType(){
		return new SlashController();
	}
}