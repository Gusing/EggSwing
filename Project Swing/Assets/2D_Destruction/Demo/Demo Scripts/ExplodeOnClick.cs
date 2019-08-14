using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Explodable))]
public class ExplodeOnClick : MonoBehaviour {

	private Explodable _explodable;

	void Start()
	{
		_explodable = GetComponent<Explodable>();
	}
	void OnMouseDown()
	{
        GetComponent<enemyHandler>().TakeDamage(100, Random.Range(0, 1000));
		//_explodable.explode();
		//ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
		//ef.doExplosion(transform.position);
	}
}
