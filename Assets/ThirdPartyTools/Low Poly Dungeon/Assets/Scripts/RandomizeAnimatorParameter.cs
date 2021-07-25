using UnityEngine;
using System.Collections;

public class RandomizeAnimatorParameter : MonoBehaviour {

	public string parameter;
	public int minValue;
	public int maxValue;
	public float minTime;
	public float maxTime;
	private float leftTime;
	private Animator anim;
	private int value;
	void Start()
	{
		anim = GetComponent<Animator> ();
	}
	// Update is called once per frame
	void Update () {
		leftTime -= Time.deltaTime;
		if (leftTime <= 0) {
			leftTime = Random.Range (minTime, maxTime);
			int prevValue = value;
			while (value == prevValue) {
				value = Random.Range (minValue, maxValue+1);
			}
			anim.SetInteger (parameter, value);
		}

	}
}
