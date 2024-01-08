using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RobotJoint[] joints;

    public InverseKinematics inverseKinematics;
    public DrawingManager drawingManager;
    public AutoSliderAnimator[] autoSliderAnimators;
    
    private void Awake() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Initialize sliders (for manual control)
        for (var i = 0; i < joints.Length; i++)
        {
            var index = i;
            Slider slider = root.Q<Slider>("Slider_joint" + index);

            if (slider == null) continue;
            UpdateValueFromSliderPosition(slider.value, index);
            slider.RegisterValueChangedCallback(v => UpdateValueFromSliderPosition(v.newValue, index));
        }

        // Initialize the Line Drawing GUI
        Vector3Field startPosition = root.Q<Vector3Field>("Line_StartPos");
        Vector3Field endPosition = root.Q<Vector3Field>("Line_EndPos");
        Button lineButton = root.Q<Button>("Line_DrawBtn");
        lineButton.RegisterCallback<ClickEvent>(v => drawingManager.DrawLine(startPosition.value, endPosition.value));

        // Initialize the Ellipse Drawing GUI
        Vector2Field semiAxes = root.Q<Vector2Field>("Ellipse_SemiAxes");
        Vector3Field centerPosition = root.Q<Vector3Field>("Ellipse_CenterPosition");
        Vector3Field rotation = root.Q<Vector3Field>("Ellipse_Rotation");
        Button ellipseButton = root.Q<Button>("Ellipse_DrawBtn");
        ellipseButton.RegisterCallback<ClickEvent>(v => drawingManager.DrawEllipse(semiAxes.value, centerPosition.value, rotation.value));

        // Initialize the Line Persistence checkbox
        Toggle linePersistenceToggle = root.Q<Toggle>("LinePersistence");
        drawingManager.trailManager.SetLinePersistence(linePersistenceToggle.value);
        linePersistenceToggle.RegisterValueChangedCallback(v => drawingManager.trailManager.SetLinePersistence(v.newValue));

        // Initialize the "ENSISA" Writing GUI
        Button ensisaButton = root.Q<Button>("Ensisa_DrawBtn");
        ensisaButton.RegisterCallback<ClickEvent>(v => drawingManager.DrawEnsisa());
    }

    private void UpdateValueFromSliderPosition (float sliderPosition, int jointId) {
        if (joints.Length <= jointId || joints[jointId] == null) return;
        
        var jointNewValue = joints[jointId].GetMinValue() +
                            sliderPosition/100 * (joints[jointId].GetMaxValue() - joints[jointId].GetMinValue());
        
        //Debug.Log("Joint " + jointId + " position: " + jointNewValue);
        joints[jointId].SetQ(jointNewValue);
    }

    public void EnableIKMode() {
        inverseKinematics.enabled = true;
        foreach (var animator in autoSliderAnimators)
        {
            animator.active = false;
        }
    }

    public void EnableManualControlMode() {
        inverseKinematics.enabled = false;
        foreach (var animator in autoSliderAnimators)
        {
            animator.active = true;
        }
    }

    void Start() {
        EnableIKMode();

    }
}
