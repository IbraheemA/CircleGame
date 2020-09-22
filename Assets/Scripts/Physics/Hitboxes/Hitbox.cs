using UnityEngine;
using System.Collections.Generic;

public abstract class Hitbox {

	public LayerMask LayerMask;
	protected List<Entity> _freshHits = new List<Entity>();
	protected List<Entity> _alreadyHit = new List<Entity>();
	protected Collider2D[] _hitResults = new Collider2D[100];

	public abstract List<Entity> GetFreshHits();
}