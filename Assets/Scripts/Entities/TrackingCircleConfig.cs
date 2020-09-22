using UnityEngine;

[CreateAssetMenu(menuName = "CircleGame/Controllers/TrackingCircleConfig")]
public class TrackingCircleConfig : ControllerConfig {

	public override IEntityCallbackUser CreateController(){
		TrackingCircle c = new TrackingCircle(outerCirclePrefab, innerCirclePrefab);
		return c;
	}

	[Header("Basic")]
	public GameObject outerCirclePrefab;
	public GameObject innerCirclePrefab;
}