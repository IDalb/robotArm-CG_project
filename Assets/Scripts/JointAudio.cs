using UnityEngine;

[RequireComponent(typeof(RobotJoint))]
[RequireComponent(typeof(AudioSource))]

public class JointAudio : MonoBehaviour
{
    private Vector3 _oldLocalPosition;
    private Quaternion _oldLocalRotation;
    private float similirarityThreshold = 0.025f;

    private AudioSource _audio;

    private float _baseVolume = 0.6f;
    private float _volumeLerp;
    private float _volumeTurnOnSpeed = 4;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _volumeLerp = 0;

        _oldLocalPosition = transform.localPosition;
        _oldLocalRotation = transform.localRotation;
    }

    void Update()
    {
        Vector3 newLocalPosition = transform.localPosition;
        Quaternion newLocalRotation = transform.localRotation;

        float positionDiff = Vector3.Distance(newLocalPosition, _oldLocalPosition);
        float rotationDiff = Quaternion.Angle(newLocalRotation, _oldLocalRotation);

        if (positionDiff > similirarityThreshold || rotationDiff > similirarityThreshold * 2 * Mathf.PI)
            _volumeLerp = Mathf.Max(_volumeLerp + _volumeTurnOnSpeed * Time.deltaTime, 1);
        else
            _volumeLerp = Mathf.Max(_volumeLerp - _volumeTurnOnSpeed * Time.deltaTime, 0);

        _oldLocalPosition = newLocalPosition;
        _oldLocalRotation = newLocalRotation;
        
        _audio.volume = _baseVolume * _volumeLerp;
    }
}
