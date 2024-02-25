using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawScript8 : MonoBehaviour
{
    [SerializeField] public int maxInterval = 4;
    [SerializeField] public int minInterval = 2;
    [SerializeField] public float moveSpeed = 1f;
    [SerializeField] public float maxY = 6.8738f;
    [SerializeField] public float minY = 2.42f;

    private BoxCollider col;
    private bool transition = true;
    private bool direction; //0 down, 1 up
    private int interval;
    private float startTime;


    void Start()
    {

        col = GetComponent<BoxCollider>();
        interval = Random.Range(minInterval, maxInterval + 1);
        startTime = Time.time;

    }

    // Update is called once per frame
    void Update()
    {
        if (!transition)
        {
            if (Time.time >= startTime + interval)
            {
                //col.enabled = false;
                transition = true;
                if (transform.position.y == maxY)
                    direction = false;
                else if (transform.position.y == minY)
                    direction = true;
            }
        }
        else
        {


            //down
            if (!direction)
            {
                transform.position -= Vector3.up * Time.deltaTime * moveSpeed;
                if (transform.position.y < minY)
                {
                    //col.enabled = true;
                    var temp = transform.position;
                    temp.y = minY;
                    transform.position = temp;
                    transition = false;
                    interval = Random.Range(minInterval, maxInterval + 1);
                    startTime = Time.time;
                }
            }
            else //up
            {
                transform.position += Vector3.up * Time.deltaTime * moveSpeed;
                if (transform.position.y > maxY)
                {
                    //col.enabled = true;
                    var temp = transform.position;
                    temp.y = maxY;
                    transform.position = temp;
                    transition = false;

                    interval = Random.Range(minInterval, maxInterval + 1);
                    startTime = Time.time;
                }
            }
        }

    }
}
