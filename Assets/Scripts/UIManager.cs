using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RobotJoint[] joints;
    
    private void OnEnable() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        for (var i = 0; i < joints.Length; i++)
        {
            var index = i;
            Slider slider = root.Q<Slider>("Slider_joint" + index);
            slider.RegisterValueChangedCallback(v => PrintSliderPosition(v.newValue, index));
        }
    }

    private void PrintSliderPosition (float sliderPosition, int jointId) {
        if (joints.Length <= jointId || joints[jointId] == null) return;
        
        var jointNewValue = joints[jointId].GetMinValue() +
                              sliderPosition/100 * (joints[jointId].GetMaxValue() - joints[jointId].GetMinValue());
        
        Debug.Log("Joint " + jointId + " position: " + jointNewValue);
        joints[jointId].SetQ(jointNewValue);
    }
}
