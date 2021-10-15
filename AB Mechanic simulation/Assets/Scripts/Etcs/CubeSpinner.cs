using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpinner : MonoBehaviour
{
    public float spinInterval = 5;
    public Vector2 spinRange;
    public Vector3 spinVector;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(intervalSpin());
    }
    private void Update()
    {
        if (this.isActiveAndEnabled)
        {
            transform.Rotate(spinVector*Time.deltaTime);
        }
    }

    IEnumerator intervalSpin()
    {
        while (this.isActiveAndEnabled)
        {
            float spinpower = Random.Range(spinRange.x,spinRange.y);
            spinVector = new Vector3(spinpower * Random.Range(1, 10), spinpower * Random.Range(0, 10), spinpower * Random.Range(1, 10));
            yield return new WaitForSeconds(spinInterval);
        }
    }
}
