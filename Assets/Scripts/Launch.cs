using UnityEngine;

public class Launch {

	uint _counter;
	uint _duration;
	Vector2 _launchVelocity;

	public Launch(Vector2 launchVelocity, uint duration){
		_launchVelocity = launchVelocity;
		_duration = duration;
	}

	public void OnApply(Entity entity){
		entity.ExternalVelocity += _launchVelocity;
		entity.OnFixedUpdate += OnFixedUpdate;
		_counter = 0;
	}

	private void OnFixedUpdate(Entity entity){
		if(_counter == _duration){
			OnRemove(entity);
		}
		_counter++;
	}

	public void OnRemove(Entity entity){
		entity.ExternalVelocity -= _launchVelocity;
		entity.OnFixedUpdate -= OnFixedUpdate;
	}
}