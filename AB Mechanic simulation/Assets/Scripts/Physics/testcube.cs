using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testcube : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rbClick();
        }
    }
    public void rbClick()
    {
        rb.AddForce(new Vector3(0, 100, 0));
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

        }
    }
}
