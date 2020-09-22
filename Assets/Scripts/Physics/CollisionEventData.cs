public class CollisionEventData {

	public Entity MainEntity;
	public Entity EntityHit;

	public CollisionEventData(Entity mainEntity, Entity entityHit){
		MainEntity = mainEntity;
		EntityHit = entityHit;
	}
}