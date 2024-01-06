using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    public InverseKinematics ikManager;
    public TrailManager trailManager;

    public void DrawLine(Vector3 startPosition, Vector3 endPosition, float duration = 2f) {
        StartCoroutine(DrawLineCoroutine(startPosition, endPosition, duration));
    }

    IEnumerator DrawLineCoroutine(Vector3 startPosition, Vector3 endPosition, float duration) {
        trailManager.SetDrawState(false);
        yield return ikManager.MoveTo(startPosition, 1f);
        trailManager.SetDrawState(true);
        yield return ikManager.MoveTo(endPosition, duration);
        trailManager.SetDrawState(false);
    }

    public void DrawEllipse(Vector2 semiAxes, Vector3 centerPosition, Vector3 rotation) {
        StartCoroutine(DrawEllipseCoroutine(semiAxes, centerPosition, rotation));
    }
    IEnumerator DrawEllipseCoroutine(Vector2 semiAxes, Vector3 centerPosition, Vector3 rotation) {
        Quaternion rotationQuaternion = Quaternion.Euler(rotation);
        
        for (float angle = 0; angle <= 2 * Mathf.PI; angle += 0.02f) {
            Vector3 targetPosition;
            targetPosition = new Vector3(semiAxes.x * Mathf.Cos(angle), semiAxes.y * Mathf.Sin(angle), 0f);
            targetPosition = rotationQuaternion * targetPosition;
            targetPosition += centerPosition;

            if (angle == 0) {
                trailManager.SetDrawState(false);
                yield return ikManager.MoveTo(targetPosition, 1f);
                trailManager.SetDrawState(true);
                continue;
            }

            yield return ikManager.MoveTo(targetPosition, 0.01f);

            if (angle == 360) trailManager.SetDrawState(false);
        }
    }
}
