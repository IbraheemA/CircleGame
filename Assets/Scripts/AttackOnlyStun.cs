using UnityEngine;

public class AttackOnlyStun : Buff {

	uint _counter;
	public uint Duration;

	public AttackOnlyStun(uint duration){
		base.IsDebuff = true;
		Duration = duration;
	}

	public override void OnApply(Entity entity){
		entity.LockActionType(GameInfo.ActionType.Attack);
		entity.Velocity = Vector2.zero;
		entity.OnFixedUpdate += OnFixedUpdate;
		_counter = 0;
	}

	public void OnFixedUpdate(Entity entity){
		if(_counter == Duration){
			OnRemove(entity);
		}
		_counter++;
	}

	public override void Stack(Buff newBuff){
		StunDebuff newStun = newBuff as StunDebuff;
		if(newStun.Duration > Duration - _counter){
			_counter = 0;
			Duration = newStun.Duration;
		}
	}

	public override void OnRemove(Entity entity){
		entity.UnlockAllActions(2);
		entity.OnFixedUpdate -= OnFixedUpdate;
	}
}