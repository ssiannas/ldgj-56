using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBrain : ScriptableObject
{
	public virtual void Think(EnemyController entity) {  }
}

