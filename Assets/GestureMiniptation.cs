using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Kinect;
using ParticlePlayground;

public class GestureMiniptation : MonoBehaviour {

	public enum InputPhase
	{
		Up, down, movingStart, Moving, MovingEnd
	}

	public Transform ManObject;
	Transform[] votexObj = new Transform[2];
	//public ManipulatorObjectC[] Repellent;
	//public ManipulatorObjectC[] Attacher;
	List<ManipulatorObjectC> Manipulators = new List<ManipulatorObjectC>();
	List<ManipulatorObjectC> VotexManipulator = new List<ManipulatorObjectC> ();
	List<ManipulatorObjectC> DefaultRepellentMani = new List<ManipulatorObjectC> ();
	List<ManipulatorObjectC> ImplueseRepellent = new List<ManipulatorObjectC> ();

	#region integral Values
	static Vector3 _integralDetlaPosition;
	public Vector3 IntegralDeltaPosition {
		get { return  _integralDetlaPosition;}
		set { _integralDetlaPosition = value; }
	}

	float _integralMomentum;
	public float IntegralMomentum
	{
		get{ return _integralMomentum; }
		set{ _integralMomentum = value; }
	}

	Vector3 _integralDirection;
	public Vector3 IntegralDirection{
		get{ return _integralDirection; }
		set{ _integralDirection = value;}
	}

	#endregion

	public PlaygroundParticlesC[] particles;
	public float MoveTargetSpeed;
	float standradVotexStrength;

	private InputPhase MouseInputPhase;

	// Use this for initialization
	void Start () {
		//particles = GetComponent<PlaygroundParticlesC>();
		particles[1].manipulators = particles[0].manipulators;
		Manipulators.AddRange (particles [0].manipulators);
		Manipulators.AddRange (particles [1].manipulators);
		foreach (ManipulatorObjectC mu in Manipulators) {
			if (mu.type == MANIPULATORTYPEC.Repellent) {
				if (mu.transform.transform.tag == "ImpluseAnchor") {
					ImplueseRepellent.Add (mu);
					continue;
				}
				DefaultRepellentMani.Add (mu);
			} else if (mu.type == MANIPULATORTYPEC.Vortex) {
				VotexManipulator.Add (mu);
				standradVotexStrength = mu.strength;
			}
		}
		_integralDetlaPosition = Vector3.zero;
		Application.targetFrameRate = 90;
		isActiveManipulator (false, 0);

		votexObj [0] = ManObject.FindChild ("Votex1");
		votexObj [1] = ManObject.FindChild ("Votex2");
	}


	Vector3 detlaPosition = Vector2.zero;
	Vector3 _lastPosition = Vector2.zero;
	// Update is called once per frame
	void Update () {
		
		if (Input.GetMouseButton (0)) {			
			//man.strength = 60f;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			//isActiveManupulator (true);
			if (Physics.Raycast (ray, out hit, 1000f)) {

				if (hit.collider.tag == "Plane") {
					if (ManObject != null)
						ManObject.position = hit.point;
					Debug.DrawLine (Camera.main.ScreenToWorldPoint (Input.mousePosition), hit.point, Color.red);
				}
			}

			//Check the status of the mouse input
			if (detlaPosition.sqrMagnitude > 0.1) {
				if (MouseInputPhase == InputPhase.down || MouseInputPhase == InputPhase.MovingEnd) {
					MouseInputPhase = InputPhase.movingStart;
				} else if (MouseInputPhase == InputPhase.movingStart) {
					MouseInputPhase = InputPhase.Moving;
				}
			} else if (MouseInputPhase == InputPhase.Moving){
				MouseInputPhase = InputPhase.MovingEnd;
				isActiveManipulator (false, 0);
			}

			if (MouseInputPhase != InputPhase.Up) {
				//Do something at Moving

				float DirX = _integralDirection.x;
				float DirY = _integralDirection.y;

				//Apply upper votex manipulator position
				float HorzionUpper = DirX == 0f ? -(DirY * 2.2f / 0.01f) : -(DirY * 2.2f / DirX);
				float vertiUpper = DirX > 0 ? -1f : 1f;
				Vector3 calUpperVotex = (new Vector3(HorzionUpper, 2.2f * vertiUpper, 0f).normalized) * 2.2f;
				Vector3 UppderVotexPosition = calUpperVotex + transform.localPosition;
				votexObj [0].localPosition = Vector3.Lerp (votexObj [0].localPosition, UppderVotexPosition, Time.deltaTime * 6f);

				//Apply lower votex manipulator position
				float HorzionLower = DirX == 0f ? -(DirY * -2.2f / 0.01f) : -(DirY * -2.2f / DirX);
				float vertiLower = DirX > 0 ? 1f : -1f;
				Vector3 calLowerVotex = (new Vector3 (HorzionLower, 2.2f * vertiLower, 0f).normalized) * 2.2f;
				Vector3 LowerVotexPosition = calLowerVotex + transform.localPosition;
				votexObj[1].localPosition = Vector3.Lerp (votexObj[1].localPosition, LowerVotexPosition, Time.deltaTime * 6f);
				//Debug.Log ("InputPhase.Moving "+detlaPosition.sqrMagnitude);
			}

		} else {
			if (MouseInputPhase != InputPhase.Up)
				isActiveManipulator (false, 0);

			MouseInputPhase = InputPhase.Up;
		}
		RecogBeheavior(_integralMomentum, Veclocity, _integralDetlaPosition, _integralDirection);
	}

	void FixedUpdate()
	{
		//calcute the delta position from the device data
		if (Input.GetMouseButton (0)) {
			if (MouseInputPhase == InputPhase.Up)
				MouseInputPhase = InputPhase.down;
			
			detlaPosition = Input.mousePosition - _lastPosition;
			_lastPosition = Input.mousePosition;
		}else {
			_lastPosition = Vector3.zero;
			detlaPosition = Vector3.zero;

		}

		_IntegralDetlaPosition (Input.mousePosition, ref _integralDetlaPosition, ref _integralMomentum);
		_IntegralDirection (_integralDetlaPosition, ref _integralDirection);
	}

	int integralCount = 0;
	Vector3 SumDelta = Vector3.zero;
	float SumMomentum = 0f;
	float Veclocity = 0; //Veclocity = (momentun - last momentun / time)
	void _IntegralDetlaPosition(Vector3 Position, ref Vector3 _deltaPosition, ref float _deltaMomentum)
	{
		if (MouseInputPhase == InputPhase.Moving) {
			integralCount ++;
			SumDelta += detlaPosition;
			SumMomentum += _deltaPosition.sqrMagnitude;
			if (integralCount != 0) { //prevent devid by zero
				_deltaPosition = SumDelta / integralCount;
				Veclocity = ((SumMomentum / integralCount) - _deltaMomentum);
				_deltaMomentum = SumMomentum / integralCount;
			}
			//Debug.Log ("_DeltaPosition: "+_deltaPosition.sqrMagnitude + "integralCount: "+integralCount + "Veclocity: "+ Veclocity);
		}
		else
		{
			_deltaPosition = Vector3.zero;
			SumDelta = Vector3.zero;
			SumMomentum = 0f;
			integralCount = 0;
			Veclocity = 0;
		}
	}

	List<Vector3> SumDirPos = new List<Vector3>();
	Vector3 _AveangeDirPos = Vector3.zero;
	void _IntegralDirection(Vector3 DeltaPosition, ref Vector3 direction)
	{
		if (MouseInputPhase == InputPhase.Moving) {
			SumDirPos.Add (DeltaPosition);
			_AveangeDirPos += DeltaPosition;
			if (SumDirPos.Count > 10) {
				SumDirPos.RemoveAt (0);
				_AveangeDirPos -= SumDirPos [0];
			}

			direction = (_AveangeDirPos / SumDirPos.Count).normalized;
			Debug.Log ("Direction "+ direction+"Angle: " + Vector3.Angle (direction, Vector3.left));
		} else if (MouseInputPhase == InputPhase.Up) {
			if (SumDirPos.Count > 0) {
				SumDirPos.Clear ();
				_AveangeDirPos = Vector3.zero;
			}
		}

	}

	/// <summary>
	/// switch the manipulators active state.
	/// </summary>
	/// <param name="active">If set to <c>true</c> active.</param>
	/// <param name="mask">Mask for each type of manipulatior <c>-1</c></param>
	void isActiveManipulator(bool active, int mask)
	{
		//Debug.Log ("Mask: "+ mask +" handled mask >> 2 "+(mask >> 2)+" handle (mask >> 2) % 2 "+(mask >> 1) % 2);
		for (int i = 0; i < Manipulators.Count; i++) {
			
			if (((mask >> 1) & 1 /*% 2*/) == 1 &&
			    Manipulators [i].type == MANIPULATORTYPEC.Repellent &&
			    Manipulators [i].transform.transform.tag != "ImpluseAnchor") {
				continue;

			} else if (((mask >> 2) & 1) == 1 && Manipulators [i].type == MANIPULATORTYPEC.Vortex) {
				continue;
			} else if (((mask >> 3) & 1) == 1 &&
			           Manipulators [i].type == MANIPULATORTYPEC.Repellent &&
			           Manipulators [i].transform.transform.tag == "ImpluseAnchor") {
				continue;
			}

			Manipulators [i].enabled = active;
		}
	}

	float tempCounter = 0f;
	void RecogBeheavior (float Momentum, float veclocity, Vector3 deltaPosition, Vector3 direction)
	{
		if (InImpluse) {
			tempCounter += Time.deltaTime;
			if (tempCounter > 1f) {
				InImpluse = false;
			}
			return;
		}

		if (MouseInputPhase == InputPhase.Moving) {
			if (Mathf.Abs (veclocity / Time.deltaTime) > 40000f) {
				Debug.Log ("Mathf.Abs (veclocity / Time.deltaTime): " + Mathf.Abs (veclocity / Time.deltaTime));
				ImpluseMovement (Momentum / 5000f);
				Debug.Log ("Impluse");
			} else  {
				Debug.Log ("Mathf.Abs (veclocity / Time.deltaTime): " + Mathf.Abs (veclocity / Time.deltaTime));
				StandardMovement (Momentum);
				Debug.Log ("smooth");
			}
		}
	}

	void StandardMovement(float Momentum)
	{
		isActiveManipulator (true, 8);
		//Debug.Log (Momentum);
		float VotexStrength = Mathf.Clamp (standradVotexStrength * Momentum / 300f, 0.5f, standradVotexStrength * 1.7f);
		//Debug.Log ("Votex Strength:" + VotexStrength + " standradVotexStrength: " + standradVotexStrength);
		foreach (ManipulatorObjectC VMu in VotexManipulator) {
			VMu.strength = VotexStrength;
		}
	}

	bool InImpluse = false;
	void ImpluseMovement(float strength)
	{
		//Repellent.strength = 40f * strength;
		foreach (ManipulatorObjectC RMu in ImplueseRepellent) {
			RMu.strength = Mathf.Clamp(strength * 30f, 30f, 60f);
		}
		isActiveManipulator (true, 6);
		InImpluse = true;
	}
}
