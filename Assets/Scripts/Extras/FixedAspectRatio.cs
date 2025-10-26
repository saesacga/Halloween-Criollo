using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedAspectRatio : MonoBehaviour
{
    private readonly float _targetAspect = 16f / 9f; // 16:9
    private Camera _cam;

    void Start()
    {
        _cam = GetComponent<Camera>();
        float windowAspect = (float)Screen.width / (float)Screen.height;
        
        if (windowAspect >= _targetAspect)
        {
            float scaleWidth = _targetAspect / windowAspect;
            Rect rect = _cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            _cam.rect = rect;
        }
        else
        {
            float scaleHeight = windowAspect / _targetAspect;
            Rect rect = _cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            _cam.rect = rect;
        }
    }
}