using UnityEngine;
using System;

public class WanderingModule {

	public enum CycleStage {
		Start,
		WaitingToStop,
		StopPoint,
		WaitingToCycle
	}

	public bool IsControllingVelocity = false;
	public uint StartingCounter { get; set; }
	public uint FramesUntilStop { get; set; }
	public uint WanderingFramesVariance { get; set; }
	public uint StillFramesAfterStop { get; set; }
	public float SpeedMultiplierVariance { get; set; } = 0.3f;
	
	uint _counter;
	uint _nextCycleBreakpoint;
	uint _cycleLength {
		get {
			return (FramesUntilStop + StillFramesAfterStop);
		}
	}
	Vector2 _velocity;
	Vector2 _seedDirection = Vector2.zero;

	public WanderingModule(){
		ConfigureDefault();
	}

	public void ConfigureDefault(){
		FramesUntilStop = 120;
		StillFramesAfterStop = 60;
		WanderingFramesVariance = 0;
		StartingCounter = 0;
		_counter = 0;
	}

	public void CopyConfigToModule(WanderingModule otherModule){
		otherModule.FramesUntilStop = FramesUntilStop;
		otherModule.StillFramesAfterStop = StillFramesAfterStop;
		otherModule.WanderingFramesVariance = WanderingFramesVariance;
		otherModule.StartingCounter = StartingCounter;
	}

	public Vector2 CycleAndGetVelocity(float baseSpeed = 1){
		if(_counter == 0){
			IsControllingVelocity = true;
			CalculateNextCycleBreakpoint();
			RecalculateVelocity(baseSpeed);
		} else if (_counter == _nextCycleBreakpoint){
			IsControllingVelocity = true;
			_velocity = Vector2.zero;
		}
		_counter = (_counter + 1) % _cycleLength;
		return _velocity;
	}

	public Action<Entity> CycleAndGetCommand(float baseSpeed = 1){
		return Behaviours.GetBasicMoveCommand(CycleAndGetVelocity(baseSpeed));
	}

	private void CalculateNextCycleBreakpoint(){
		int varianceToUse = (int)WanderingFramesVariance;
		_nextCycleBreakpoint = (uint)Mathf.Min(
			Mathf.Max(
				0,
				(int)(FramesUntilStop + UnityEngine.Random.Range(-varianceToUse, varianceToUse + 1))
			),
			(int)_cycleLength
		);
	}

	public void RecalculateVelocity(float baseSpeed = 1){
		float speedMultiplierToUse = baseSpeed * (1 + UnityEngine.Random.Range(-SpeedMultiplierVariance, SpeedMultiplierVariance));
		_velocity = (new Vector2(UnityEngine.Random.Range(-1f,1f), UnityEngine.Random.Range(-1f,1f)) - _seedDirection).normalized * speedMultiplierToUse;
		_seedDirection = _velocity.normalized;
	}

	public void ResetCycle(){
		_counter = StartingCounter;
	}

	public void SetCycleStage(CycleStage stage, int offset = 0){
		switch(stage){
			case CycleStage.Start:
				_counter = (uint)Mathf.Min(Mathf.Max(0, offset), (int)_nextCycleBreakpoint);
				break;
			case CycleStage.WaitingToStop:
				_counter = (uint)Mathf.Min(Mathf.Max(1, 1 + offset), (int)_nextCycleBreakpoint);
				break;
			case CycleStage.StopPoint:
				CalculateNextCycleBreakpoint();
				_counter = (uint)Mathf.Min(Mathf.Max(0, _nextCycleBreakpoint + offset), (int)_cycleLength);
				break;
			case CycleStage.WaitingToCycle:
				CalculateNextCycleBreakpoint();
				_counter = (uint)Mathf.Min(Mathf.Max(_nextCycleBreakpoint + 1, _nextCycleBreakpoint + offset), (int)_cycleLength);
				break;
			default:
				ResetCycle();
				break;
		}
	}

	public CycleStage CurrentCycleStage { get {
			if(_counter == 0){
				return CycleStage.Start;
			}
			else if(_counter < _nextCycleBreakpoint){
				return CycleStage.WaitingToStop;
			}
			else if(_counter == _nextCycleBreakpoint){
				return CycleStage.StopPoint;
			}
			else{
				return CycleStage.WaitingToCycle;
			}
		}
	}

	public void ForceVelocity(Vector2 velocity){
		_velocity = velocity;
		IsControllingVelocity = false;
	}

	public void SetSeedDirection(Vector2 velocity){
		_seedDirection = velocity.normalized;
	}
}