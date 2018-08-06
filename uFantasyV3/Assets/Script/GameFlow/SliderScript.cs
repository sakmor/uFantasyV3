using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderScript : MonoBehaviour
{
	public UnityEngine.UI.Slider Slider;
	internal float Value;
	internal bool IsDone;

	// Use this for initialization
	private void Start()
	{

	}

	// Update is called once per frame
	private void Update()
	{
		if (IsDone) { return; }

		Slider.value = Mathf.Lerp(Slider.value, Value, 0.05f);
		IsDone = Slider.value >= 1 ? true : false;
	}

	internal void Rest()
	{
		Slider.value = Value = 0;
		IsDone = false;
	}
}
