using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

//color
//1: green
//2: yellow
//3: none

//agent
//1: collector
//2: master_g
//3: master_y


public class agent : Agent
{
    Rigidbody rBody;
    // Start is called before the first frame update
    public int color;

    void Start()
    {        
        rBody = GetComponent<Rigidbody>();
    }
    
    public override void Initialize() {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin() {
        if (this.name == "master_g") 
            GameObject.Find("Env").GetComponent<env>().env_Reset();
        //GameObject.Find("Env").GetComponent<env>().env_Reset();
        color = 3;
    }

    public void OnCollisionStay(Collision collision) {
        GameObject obj = collision.gameObject;
        
        if (this.name[0] == 'c') {
            if (obj.name == "master_g" && this.color == 1) {
                this.color = 3;
                this.GetComponent<Renderer>().material.color = Color.grey;
            }
            else if (obj.name == "master_y" && this.color == 2) {
                this.color = 3;
                this.GetComponent<Renderer>().material.color = Color.grey;
            }
            else if (obj.tag == "Treasure" && this.color == 3) {
                if (obj.name[0] == 'g') {
                    this.color = 1;
                    this.GetComponent<Renderer>().material.color = Color.green;
                }
                else {
                    this.color = 2;
                    this.GetComponent<Renderer>().material.color = Color.yellow;
                }
                GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");
                foreach(GameObject tmp in agents) 
                    tmp.GetComponent<agent>().AddReward(100f);
                obj.GetComponent<treasure>().move();
            }
            else if (obj.name[0] == 'c')
                AddReward(-100f);
        }
        else if (this.name == "master_g") {
            if (obj.name[0] == 'c' && obj.GetComponent<agent>().color == 1) {
                GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");
                foreach(GameObject tmp in agents) 
                    tmp.GetComponent<agent>().AddReward(100f);
            }
        }
        else if (this.name == "master_y") {
            if (obj.name[0] == 'c' && obj.GetComponent<agent>().color == 2) {
                GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");
                foreach(GameObject tmp in agents) 
                    tmp.GetComponent<agent>().AddReward(100f);
            }
        }
    }   

    static int Compare(KeyValuePair<float, GameObject> u, KeyValuePair<float, GameObject> v) {
        return u.Key.CompareTo(v.Key);
    }

    // need to change
    public override void CollectObservations(VectorSensor sensor) {
        GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");
        GameObject[] treasures = GameObject.FindGameObjectsWithTag("Treasure");

        List<KeyValuePair<float, GameObject>> agents_dist = new List<KeyValuePair<float, GameObject>>();
        List<KeyValuePair<float, GameObject>> treasures_dist = new List<KeyValuePair<float, GameObject>>();
        
        for(int i = 0; i<agents.Length; ++i)
            if (agents[i].name != this.name) 
                agents_dist.Add(new KeyValuePair<float, GameObject>(Vector3.Distance(this.transform.position, agents[i].transform.position), agents[i]));
            
        for(int i = 0; i<treasures.Length; ++i)
            treasures_dist.Add(new KeyValuePair<float, GameObject>(Vector3.Distance(this.transform.position, treasures[i].transform.position), treasures[i]));

        agents_dist.Sort(Compare);
        treasures_dist.Sort(Compare);



        sensor.AddObservation(this.transform.position);
        sensor.AddObservation(rBody.velocity);
        if (this.name[0] == 'c') {

            if (color == 1) sensor.AddObservation(new Vector2(1f, 0f));
            else if (color == 2) sensor.AddObservation(new Vector2(0f, 1f));
            else sensor.AddObservation(new Vector2(0f, 0f));
        }
        foreach(KeyValuePair<float, GameObject> dist in agents_dist) {
            GameObject tmp = dist.Value;
            sensor.AddObservation(this.transform.position - tmp.transform.position);
            sensor.AddObservation(tmp.GetComponent<Rigidbody>().velocity);
            if (tmp.name[0] == 'c') {
                sensor.AddObservation(new Vector2(0f, 0f));
                
                if (color == 1)      sensor.AddObservation(new Vector2(1f, 0f));
                else if (color == 2) sensor.AddObservation(new Vector2(0f, 1f));
                else                 sensor.AddObservation(new Vector2(0f, 0f));
            }
            else {
                if (tmp.name == "master_g") sensor.AddObservation(new Vector2(1f, 0f));
                else                        sensor.AddObservation(new Vector2(0f, 1f));
                
                sensor.AddObservation(new Vector2(0f, 0f));
            }
        }
        foreach(KeyValuePair<float, GameObject> dist in treasures_dist) {
            GameObject tmp = dist.Value;
            sensor.AddObservation(this.transform.position - tmp.transform.position);
            if (tmp.name[0] == 'g') sensor.AddObservation(new Vector2(1f, 0f));
            else                    sensor.AddObservation(new Vector2(0f, 1f));
        }
        if (this.name[0] != 'c') 
            sensor.AddObservation(new Vector2(0f, 0f));
    }



    void cal_dist_penalty() {

        float dist = 0f;
        if (this.name[0] == 'c') {
            if (color == 3) {
                GameObject[] treasures = GameObject.FindGameObjectsWithTag("Treasure");
                List<float> treasureDist = new List<float>();

                foreach(GameObject obj in treasures) 
                    treasureDist.Add(Vector3.Distance(this.transform.position, obj.transform.position));
                
                treasureDist.Sort();
                dist = treasureDist[0];
            }
            else if (color == 1) {
                GameObject obj = GameObject.Find("master_g");
                dist = Vector3.Distance(this.transform.position, obj.transform.position);
            }
            else {
                GameObject obj = GameObject.Find("master_y");
                dist = Vector3.Distance(this.transform.position, obj.transform.position);
            }
        }
        else if (this.name == "master_g") {
            GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");
            List<float> agentDist = new List<float>();

            foreach(GameObject obj in agents) 
                if (obj.GetComponent<agent>().color == 1)
                    agentDist.Add(Vector3.Distance(this.transform.position, obj.transform.position));
            agentDist.Sort();
            if (agentDist.Count != 0)
                dist = agentDist[0];
            else {
                Vector3 allDist = new Vector3(0f, 0f, 0f);
                foreach(GameObject obj in agents) {
                    if (obj.name[0] == 'c') 
                        allDist = allDist + this.transform.position - obj.transform.position;
                }
                allDist = allDist / (agents.Length - 2);
                dist = Vector3.Distance(new Vector3(0f, 0f, 0f), allDist);
            }
        }
        else if (this.name == "master_y") {
            GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");
            List<float> agentDist = new List<float>();

            foreach(GameObject obj in agents) 
                if (obj.GetComponent<agent>().color == 2)
                    agentDist.Add(Vector3.Distance(this.transform.position, obj.transform.position));
            agentDist.Sort();
            if (agentDist.Count != 0)
                dist = agentDist[0];
            else {
                Vector3 allDist = new Vector3(0f, 0f, 0f);
                foreach(GameObject obj in agents) {
                    if (obj.name[0] == 'c') 
                        allDist = allDist + this.transform.position - obj.transform.position;
                }
                allDist = allDist / (agents.Length - 2);
                dist = Vector3.Distance(new Vector3(0f, 0f, 0f), allDist);
            }
        }
        AddReward(-0.1f * dist);
    }

    public override void OnActionReceived(float[] vectorAction) {
        int movement = Mathf.FloorToInt(vectorAction[0]);
        float dX = 0f, dY = 0f, dZ = 0f;
        if (movement == 0) { dX = 1f; }
        else if (movement == 1) { dZ = 1f; }
        else if (movement == 2) { dX = -1f; }
        else if (movement == 3) { dZ = -1f; }

        else if (movement == 4) { dY = 1f; }
        else if (movement == 5) { dY = -1f; }

        this.transform.Translate(
            new Vector3(
                dX*0.1f, dY*0.1f, dZ*0.1f
            )
        );

//        this.GetComponent<Rigidbody>().AddForce(
//            new Vector3(
//                dX * 10f, dY * 10f, dZ * 10f
//            )
//        );

        cal_dist_penalty();
    }

    public override void Heuristic(float[] actionsOut) {
        
        if (Input.GetKey("w"))
            actionsOut[0] = 1;
        else if(Input.GetKey("d"))
            actionsOut[0] = 3;
        else if(Input.GetKey("s"))
            actionsOut[0] = 5;
        else if(Input.GetKey("a"))
            actionsOut[0] = 7;
        else if(Input.GetKey("q"))
            actionsOut[0] = 9;
        else if(Input.GetKey("e"))
            actionsOut[0] = 10;
    }
}