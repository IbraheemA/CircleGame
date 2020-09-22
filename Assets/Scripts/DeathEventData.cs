public class DeathEventData {
	public Entity DeadEntity;
	public bool IsValid;
	public DeathEventData(Entity deadEntity){
		DeadEntity = deadEntity;
		IsValid = true;
	}
}