using System.Collections;
using UnityEngine;

public class DrawingScene : MonoBehaviour
{
    public Camera mainCamera;
    public Light spotlightLeft;
    public Light spotlightRight; 
    public AudioSource soundEffect; 
    public AudioClip spotlightSound; 
    public AudioClip drawingSound; 
    public InverseKinematics inverseKinematics;

    private void Start()
    {
        StartCoroutine(StartDrawingSequence());
    }

    IEnumerator StartDrawingSequence()
    {
        PositionAndOrientCameraAndSpotlights();
        soundEffect.PlayOneShot(spotlightSound);
        StartCoroutine(FadeInLight(spotlightLeft, 5f)); 
        StartCoroutine(FadeInLight(spotlightRight, 5f)); 
        yield return new WaitForSeconds(spotlightSound.length);

        // TODO:Must be looped while drawing
        soundEffect.loop = true;
        soundEffect.clip = drawingSound;
        soundEffect.Play();
    }

    void PositionAndOrientCameraAndSpotlights()
    {
        // Camera setup
        mainCamera.transform.position = new Vector3(0, 5, -10);
        mainCamera.transform.LookAt(new Vector3(0, 0, 0));

        // Spotlight setup
        spotlightLeft.transform.position = new Vector3(-5, 5, -5);
        spotlightRight.transform.position = new Vector3(5, 5, -5);
        spotlightLeft.transform.LookAt(new Vector3(0, 0, 0));
        spotlightRight.transform.LookAt(new Vector3(0, 0, 0));
    }


    IEnumerator FadeInLight(Light light, float duration)
    {
        float time = 0;
        float initialIntensity = light.intensity;
        while (time < duration)
        {
            light.intensity = Mathf.Lerp(0, initialIntensity, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }
}

