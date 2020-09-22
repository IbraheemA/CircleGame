using UnityEngine;

public class StunDebuff : Buff {

	uint _counter;
	public uint Duration;
	public byte Priority;

	public StunDebuff(uint duration, byte priority = 2){
		base.IsDebuff = true;
		Duration = duration;
		Priority = priority;
	}

	public override void OnApply(Entity entity){
		entity.LockAllActions(Priority);
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
		entity.UnlockAllActions(Priority);
		entity.OnFixedUpdate -= OnFixedUpdate;
	}
}