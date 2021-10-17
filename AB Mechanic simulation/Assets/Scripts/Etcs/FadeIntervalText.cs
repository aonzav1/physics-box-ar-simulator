using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIntervalText : MonoBehaviour
{
    public float fadeInterval;
    public float fadeDuration;
    RectTransform rect;

    // Start is called before the first frame update
    void Awake()
    {
        rect = transform.GetComponent<RectTransform>();
        StartCoroutine(intervalFade());
    }
    IEnumerator intervalFade()
    {
        while (true)
        {
            LeanTween.alphaText(rect, 0, fadeDuration);
            yield return new WaitForSeconds(fadeInterval);
            LeanTween.alphaText(rect, 1, fadeDuration);
            yield return new WaitForSeconds(fadeInterval);
        }
    }
}
