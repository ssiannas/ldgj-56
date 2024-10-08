using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBrain : ScriptableObject
{
	[SerializeField] public List<Sound> enemySounds;

	public virtual void Think(EnemyController entity) {  }

	public virtual float GetEyesightRange() { return 1.0f; }

	public virtual void FlipColliders(EnemyController entity) { }

	public virtual List<Sound> GetEnemySounds() { return null; }

	public virtual void OnTaunt(EnemyController entity, int tauntCount) { }
}

