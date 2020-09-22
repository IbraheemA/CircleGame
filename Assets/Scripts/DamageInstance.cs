
using System;

public class DamageInstance {

	public Entity CausingEntity;
	public Entity AffectedEntity;
	public float Amount;
	public uint InvincibilityDuration;
	public event Action<DamageInstance> OnAfterApplied;
	public bool IsValid;

	public DamageInstance(Entity causingEntity, Entity affectedEntity, float amount, uint invincibilityDuration = 0){
		CausingEntity = causingEntity;
		AffectedEntity = affectedEntity;
		Amount = amount;
		InvincibilityDuration = invincibilityDuration;
		IsValid = true;
	}

	public void DoOnAfterApplied(){
		OnAfterApplied?.Invoke(this);
	}
}