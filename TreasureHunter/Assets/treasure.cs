using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treasure : MonoBehaviour
{
    public int X;
    public int Z;
    public int height;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void move() {
        int x = Random.Range(0, X);
        int y = Random.Range(1, height-3);
        int z = Random.Range(0, Z);

        this.transform.position = new Vector3(x, y, z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
