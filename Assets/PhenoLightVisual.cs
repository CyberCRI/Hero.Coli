using UnityEngine;
using System.Collections;

public class PhenoLightVisual : MonoBehaviour {
	
	public float minRad = 0;
	public float maxRad = 10;
	public float fadeSpeed = 5;
	public float rad {
		get{return _rad;}
		set{
			_rad = Mathf.Clamp(value, minRad, maxRad);
			CapsuleCollider col = (CapsuleCollider)collider;
			col.radius = _rad;
			Light li = light;
			li.range = _rad;
			_rad = value;
		}
	}
	
	[SerializeField]
	private float _rad = 5;
	private bool _fading = false;
	
	public void Start(){
		rad = 5;	
	}
	
	public void Update(){
		
	}
	
}
