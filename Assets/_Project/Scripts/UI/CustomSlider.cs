using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class CustomSlider : MonoBehaviour
{
	public Text Visualizer;
	public Image Background;

	private Slider _slider;
	private Gradient _gradient;

	void Start()
	{
		_gradient = new Gradient();
		_gradient.SetKeys(new []
		{
			new GradientColorKey(Color.red, 0f),
			new GradientColorKey(Color.white, 0.5f),
			new GradientColorKey(Color.green, 1f),
		}, 
		new []
		{
			new GradientAlphaKey(1f, 1f)
		});

		_slider = GetComponent<Slider>();
		_slider.onValueChanged.AddListener((value) => {  UpdateValue(value); });
		UpdateValue(_slider.value);
	}

	private void UpdateValue(float value)
	{
		var text = "0";
		var progress = 0.5f + (value / _slider.maxValue) * 0.5f;
		
		var color = _gradient.Evaluate(progress);

		if(value < 0f)
		{
			text = "" + value;
		}
		if(value > 0f)
		{
			text = "+" + value;
		}

		Visualizer.text = text;
		Visualizer.color = color;
		Background.color = color;
	}
}
