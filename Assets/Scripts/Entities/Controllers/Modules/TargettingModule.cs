using System;
using UnityEngine;

public class TargettingModule {
	
	public Entity Target { get; private set; }

	Collider2D[] _seekResults = new Collider2D[100];
	public float SeekRadius { get; set; } = 7;
	public Func<Entity, Entity, bool> CheckEntityCallback;
	public ContactFilter2D ContactFilter;

	public void CopyConfigToModule(TargettingModule otherModule){
		otherModule.SeekRadius = SeekRadius;
		otherModule.CheckEntityCallback = CheckEntityCallback;
	}

	public Entity SeekAndSetTarget(Entity callingEntity){
		return Target = Behaviours.SeekTargetWithRadiusAndCallbackNonAlloc(callingEntity, SeekRadius, CheckEntityCallback, _seekResults);
	}

	public Entity SeekAndSetTargetIfNone(Entity callingEntity){
		return Target = Target ?? SeekAndSetTarget(callingEntity);
	}

	public enum Technique {
		ByEntityType
	}
}