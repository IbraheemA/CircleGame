using System;

public class GameEvents {
	public event Action<DeathEventData> EntityDied;
	public event Action<DeathEventData> PlayerDied;

	public void TriggerEntityDied(DeathEventData data){
		EntityDied?.Invoke(data);
	}

	public GameEvents(){
		// Register a wrapper function to EntityDied which forwards player deaths to the more specific "PlayerDied"
		// (PlayerDied should never be directly invoked)
		EntityDied += (data) => {
			if(data.DeadEntity.Type == Entity.EntityType.Player){
				PlayerDied?.Invoke(data);
			}
		};
	}
}