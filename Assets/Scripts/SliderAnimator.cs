using UnityEngine;
using UnityEngine.UIElements;

public class AutoSliderAnimator : MonoBehaviour
{
    // This script automatically increases the value of a slider for demonstration purposes

    public bool active = true;
    [SerializeField] private int sliderId;

    [Space]
    [SerializeField] private float speed = 1;

    private Slider _slider;

    void Awake() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        _slider = root.Q<Slider>("Slider_joint" + sliderId);
    }

    void Update()
    {
        if (_slider == null || !active) return;
        _slider.value = (_slider.value + Time.deltaTime * speed) % 100;
    }
}
