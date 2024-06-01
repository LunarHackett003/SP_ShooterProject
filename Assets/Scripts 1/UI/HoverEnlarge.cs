using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverEnlarge : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float enlargeSpeed = 2;
    public float maxSize = 1.2f;
    public float time;
    /// <summary>
    /// Enlarges or shrinks the UI Element
    /// </summary>
    /// <param name="enlarge">If true, the UI Element will get bigger.</param>
    /// <returns></returns>
    IEnumerator Resize(bool enlarge)
    {
        float targetSize = enlarge ? maxSize : 1;
        float startsize = enlarge ? 1 : maxSize;
        time = 0;
        while (time <= 1)
        {
            time += 0.01f * enlargeSpeed;
            transform.localScale = Vector3.one * Mathf.Lerp(startsize, targetSize, time);
            yield return new WaitForSecondsRealtime(0.01f);
        }
        transform.localScale = Vector3.one * targetSize;
        yield break;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(Resize(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(Resize(false));
    }
}
