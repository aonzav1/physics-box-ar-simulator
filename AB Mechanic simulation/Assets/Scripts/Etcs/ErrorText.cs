using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(stepToDestroy());
    }
    IEnumerator stepToDestroy()
    {
        Vector3 originalScale =  transform.localScale;
        LeanTween.scale(gameObject, originalScale * 1.4f, 0.12f);
        yield return new WaitForSeconds(0.12f);
        LeanTween.scale(gameObject, originalScale, 0.12f);
        yield return new WaitForSeconds(0.12f);
        LeanTween.scale(gameObject, originalScale*0.75f, 2f);
        LeanTween.alphaText(GetComponent<RectTransform>(), 0, 2f);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
