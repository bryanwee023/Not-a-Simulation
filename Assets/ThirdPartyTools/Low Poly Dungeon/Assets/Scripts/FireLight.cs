using UnityEngine;
using System.Collections;
/// <summary>
/// Script used to make the light flicker each frame (fire effect)
/// </summary>
public class FireLight : MonoBehaviour {
	public float minIntensity = 0.5f;
	public float maxIntensity = 1.5f;
	public float maxVariationX = 0.1f;
	public float maxVariationZ = 0.1f;
	public float secondsRate = 0.3f;
	private new Light light = null;
	private float initialX = 0f;
	private float initialZ = 0f;
	private Vector3 newPosition = Vector3.zero;


	void Start () {
		light = GetComponent <Light> ();
		initialX = light.transform.localPosition.x;
		initialZ = light.transform.localPosition.z;
		StartCoroutine(flickerLight());
	}


	private IEnumerator flickerLight()
	{
		while(true)
		{
			// randomize the intensity of the light
			light.intensity = Random.Range (minIntensity, maxIntensity);
			// randomize the X,Z position of the light
			newPosition = new Vector3(initialX+(Random.Range (-maxVariationX,maxVariationX)),
			                          light.transform.localPosition.y,
			                          initialZ+(Random.Range (-maxVariationZ,maxVariationZ)));
			light.transform.localPosition = newPosition;
			yield return new WaitForSeconds(secondsRate);
		}
	}
}
