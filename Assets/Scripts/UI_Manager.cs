using UnityEngine;
using UnityEngine.UIElements;

public class UI_Manager : MonoBehaviour
{
    private void OnEnable() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Slider slider1 = root.Q<Slider>("Slider_artic1");
        Slider slider2 = root.Q<Slider>("Slider_artic2");
        Slider slider3 = root.Q<Slider>("Slider_artic3");
        Slider slider4 = root.Q<Slider>("Slider_artic4");
        Slider slider5 = root.Q<Slider>("Slider_artic5");

        slider1.RegisterValueChangedCallback(v => PrintSliderPosition(v.newValue, "1"));
        slider2.RegisterValueChangedCallback(v => PrintSliderPosition(v.newValue, "2"));
        slider3.RegisterValueChangedCallback(v => PrintSliderPosition(v.newValue, "3"));
        slider4.RegisterValueChangedCallback(v => PrintSliderPosition(v.newValue, "4"));
        slider5.RegisterValueChangedCallback(v => PrintSliderPosition(v.newValue, "5"));
    }

    private void PrintSliderPosition (float sliderPosition, string sliderName = "") {
        Debug.Log("Slider " + sliderName + " position: " + sliderPosition);
    }
}
