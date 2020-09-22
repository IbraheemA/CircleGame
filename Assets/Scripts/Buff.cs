public abstract class Buff {
	public bool IsDebuff = false;
	public abstract void OnApply(Entity entity);
	public abstract void OnRemove(Entity entity);
	public abstract void Stack(Buff newBuff);
}