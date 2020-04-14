using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_Constants : MonoBehaviour
{
    public class Damage {
		public GameObject Damager;
		public int DamageAmount;
		public Vector3 DamagePoint; //default = Vector3.zero
		public Vector3 DamageNormal; //default = Vector3.zero

		public Damage(int d, Vector3 p, GameObject gO = null) {
			DamageAmount = d;
			DamagePoint = p;
			Damager = gO;
		}


		public Damage(int d, Vector3 p, Vector3 n, GameObject gO = null) {
			DamageAmount = d;
			DamagePoint = p;
			DamageNormal = n;
			Damager = gO;
		}

		public Damage(int d) {
			DamageAmount = d;
			DamagePoint = Vector3.zero;
			DamageNormal = Vector3.zero;
			Damager = null;
		}
	}
}
