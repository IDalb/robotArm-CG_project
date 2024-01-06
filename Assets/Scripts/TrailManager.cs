using UnityEngine;

public class TrailManager : MonoBehaviour
{
    public TrailRenderer drawingTrail;
    public Light drawSourceLight;
    public ParticleSystem drawSourceParticles;

    private float originalTrailLifetime = 3f;

    private void Awake() {
        if (drawingTrail == null) drawingTrail = FindObjectOfType<TrailRenderer>();
        if (drawingTrail == null) enabled = false;
        originalTrailLifetime = FindObjectOfType<TrailRenderer>().time;

        SetDrawState(false);
    }

    public void SetDrawState(bool state) {
        drawingTrail.emitting = state;

        if (drawSourceLight != null) drawSourceLight.enabled = state;
        if (drawSourceParticles != null) {
            ParticleSystem.EmissionModule emissionModule = drawSourceParticles.emission;
            emissionModule.enabled = state;
        }
    }

    public void SetLinePersistence(bool persistent) { drawingTrail.time = persistent ? 3600f : originalTrailLifetime; }
}
