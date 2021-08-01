using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class env : MonoBehaviour 
{
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject block_no_rigid;
    public GameObject block_rigid;
    public GameObject agent;
    public GameObject treasure;

    public int X;
    public int Z;
    public int height;
    public int num_drones;
    public int num_treasures;

    GameObject[] agents, treasures, obstacles;

    // This script will simply instantiate the Prefab when the game starts.
    void Start()
    {        
        env_Make();
        agents = GameObject.FindGameObjectsWithTag("Agent");
        treasures = GameObject.FindGameObjectsWithTag("Treasure");
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
    }

    public void env_Reset() {

        // make obst
        float x, y, z;
        foreach(GameObject obj in obstacles) {
            x = Random.Range(0.5f, X-0.5f);
            y = Random.Range(0, height-3f);
            z = Random.Range(0.5f, Z-0.5f);
            obj.transform.position = new Vector3(x, 0, z);
            obj.transform.localScale = new Vector3(1, 2f*y+1f, 1);
        }

        foreach(GameObject obj in agents) {
            x = Random.Range(1f, X-1f);
            y = Random.Range(1f, height-3f);
            z = Random.Range(1f, Z-1f);
            obj.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            obj.transform.position = new Vector3(x, y, z);
            if (obj.name[0] == 'c')
                obj.GetComponent<Renderer>().material.color = Color.grey;
        }

        foreach(GameObject obj in treasures) {
            x = Random.Range(0.3f, X-0.3f);
            y = Random.Range(1f, height-3);
            z = Random.Range(0.3f, Z-0.3f);
            obj.transform.position = new Vector3(x, y, z);
        }
    }

    public void env_Make() {
        // make floor
        Renderer color;
        GameObject obj;

        float newX = X/2f - 0.5f;
        float newZ = Z/2f - 0.5f;
        newX = (float)System.Math.Round(newX, 1);
        newZ = (float)System.Math.Round(newZ, 1);
        obj = Instantiate(block_no_rigid, new Vector3(newX, 0, newZ), Quaternion.identity);
        obj.GetComponent<Renderer>().material.color = Color.white;
        obj.transform.localScale = new Vector3(X, 1, Z);
        obj.tag = "Base";

        // make clear wall
        obj = Instantiate(block_no_rigid, new Vector3(0, 0, 0), Quaternion.identity);
        obj.tag = "Base";
        obj.transform.localScale = new Vector3(X, height, 1);
        obj.transform.position = new Vector3(newX, height/2f, -1);
        color = obj.GetComponent<Renderer>();
        color.enabled = false;

        obj = Instantiate(block_no_rigid, new Vector3(0, 0, 0), Quaternion.identity);
        obj.tag = "Base";
        obj.transform.localScale = new Vector3(X, height, 1);
        obj.transform.position = new Vector3(newX, height/2f, Z);
        color = obj.GetComponent<Renderer>();
        color.enabled = false;

        obj = Instantiate(block_no_rigid, new Vector3(0, 0, 0), Quaternion.identity);
        obj.tag = "Base";
        obj.transform.localScale = new Vector3(1, height, Z);
        obj.transform.position = new Vector3(-1, height/2f, newZ);
        color = obj.GetComponent<Renderer>();
        color.enabled = false;

        obj = Instantiate(block_no_rigid, new Vector3(0, 0, 0), Quaternion.identity);
        obj.tag = "Base";
        obj.transform.localScale = new Vector3(1, height, Z);
        obj.transform.position = new Vector3(X, height/2f, newZ);
        color = obj.GetComponent<Renderer>();
        color.enabled = false;

        obj = Instantiate(block_no_rigid, new Vector3(0, 0, 0), Quaternion.identity);
        obj.tag = "Base";
        obj.transform.localScale = new Vector3(X, 1, Z);
        obj.transform.position = new Vector3(newX, height, newZ);
        color = obj.GetComponent<Renderer>();
        color.enabled = false;
        

        // make obst
        int num_obst = 0;
        float x, y, z;
        for(int i = 0; i<num_obst; ++i) {
            x = Random.Range(0.5f, X-0.5f);
            y = Random.Range(0, height-3f);
            z = Random.Range(0.5f, Z-0.5f);
            obj = Instantiate(block_rigid, new Vector3(x, 0, z), Quaternion.identity);
            obj.transform.localScale = new Vector3(1, 2f*y +1f, 1);
            obj.tag = "Obstacle";
            obj.GetComponent<Renderer>().material.color = Color.red;
        }

        // make drone
        int count = num_drones + 2;
        int idx = 0;
        for(int i = 0; i<num_drones + 2; ++i) {
            x = Random.Range(1f, X-1f);
            y = Random.Range(1f, height-3f);
            z = Random.Range(1f, Z-1f);
            obj = Instantiate(agent, new Vector3(x, y, z), Quaternion.identity);
            obj.tag = "Agent";
            Renderer c = obj.GetComponent<Renderer>();

            if (count == 2) {
                c.material.color = Color.green;
                obj.name = "master_g";
            }
            else if (count == 1) {
                c.material.color = Color.yellow;
                obj.name = "master_y";
            }
            else {
                c.material.color = Color.grey;
                obj.name = "collector" + System.Convert.ToString(idx);
                obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                ++idx;
            }
            --count;
        }

        // make treasure
        bool flag = false;
        int g_idx = 0;
        int y_idx = 0;
        for (int i = 0; i<num_treasures; ++i) {
            x = Random.Range(0.3f, X-0.3f);
            y = Random.Range(1f, height-3);
            z = Random.Range(0.3f, Z-0.3f);
            obj = Instantiate(treasure, new Vector3(x, y, z), Quaternion.identity);
            obj.tag = "Treasure";
            Renderer c = obj.GetComponent<Renderer>();
            if (flag) {
                c.material.color = Color.green;
                obj.name = "green" + System.Convert.ToString(g_idx);
                ++g_idx;
            }
            else {
                c.material.color = Color.yellow;
                obj.name = "yellow" + System.Convert.ToString(y_idx);
                ++y_idx;
            }
            flag = !flag;
        }
    }
}
