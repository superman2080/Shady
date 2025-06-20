using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SliderValue
{
    public float val;
    public float maxVal;
    public float recoverVal;

    public SliderValue(float val, float maxVal, float recoverVal)
    {
        this.val = val;
        this.maxVal = maxVal;
        this.recoverVal = recoverVal;
    }
}

public class StatBar : MonoBehaviour
{
    public Slider slider;
    private SliderValue sliderValue;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sliderValue.val = sliderValue.maxVal;
    }

    // Update is called once per frame
    void Update()
    {
        if(sliderValue.maxVal > 0)
        {
            slider.value = sliderValue.val / sliderValue.maxVal;
        }
    }

    public void SetStatValue(SliderValue value)
    {
        sliderValue = value;
    }
}
