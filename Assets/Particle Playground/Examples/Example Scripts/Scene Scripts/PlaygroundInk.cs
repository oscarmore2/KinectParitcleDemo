using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class PlaygroundInk : MonoBehaviour {

	public Transform controlTransform;
	ManipulatorObjectC Repellent;
	ManipulatorObjectC Attacher;
	public PlaygroundParticlesC particles;

	// Use this for initialization
	void Start () {
		//particles = GetComponent<PlaygroundParticlesC>();
		if (particles.manipulators.Count > 0) {
			Repellent = particles.manipulators [0];
			Attacher = particles.manipulators [1];
		}
		Attacher.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		Repellent.strength = 10f;
		Attacher.enabled = false;
		Repellent.enabled = false;
		if (Input.GetMouseButton (0)) {
			//man.strength = 60f;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			Attacher.enabled = true;
			Repellent.enabled = true;
			if (Physics.Raycast (ray, out hit, 1000f)) {
				/*particles.Emit (
					Mathf.RoundToInt (4000*Time.deltaTime), 
					hit.point-new Vector3(.2f,.2f,.2f), 
					hit.point+new Vector3(.2f,.2f,.2f), 
					new Vector3(-1f,-1f,-1f), 
					new Vector3(1f,1f,1f), 
					Color.white
				);*/
				if (hit.collider.tag == "Plane") {
					if (controlTransform != null)
						controlTransform.position = hit.point;
					Debug.DrawLine (Camera.main.ScreenToWorldPoint (Input.mousePosition), hit.point, Color.red);
				}
			}
		}
	}
}