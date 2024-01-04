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

            if (slider == null) continue;
            PrintSliderPosition(slider.value, index);
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

    public InverseKinematics inverseKinematics;
    public AutoSliderAnimator[] autoSliderAnimators;

    public void EnableIKMode()
    {
        inverseKinematics.enabled = true;
        foreach (var animator in autoSliderAnimators)
        {
            animator.active = false; 
        }
    }

    public void EnableManualControlMode()
    {
        inverseKinematics.enabled = false;
        foreach (var animator in autoSliderAnimators)
        {
            animator.active = true;  
        }
    }

    void Start()
    {
        if (inverseKinematics == null)
        {
            Debug.LogError("InverseKinematics script not assigned in UIManager.");
        }

        if (autoSliderAnimators == null || autoSliderAnimators.Length == 0)
        {
            Debug.LogError("AutoSliderAnimators not assigned in UIManager.");
        }

        EnableIKMode();  
    }


}
