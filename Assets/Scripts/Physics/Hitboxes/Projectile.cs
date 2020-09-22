public abstract class Projectile {

	public Projectile(){
		MainManager.GameManager.UpdatePhysics += UpdatePhysics;
	}

	protected abstract void UpdatePhysics();

	protected virtual void Destroy(){
		MainManager.GameManager.UpdatePhysics -= UpdatePhysics;
	}
}