using ActUtlType64Lib;
using UnityEngine;
using System.Collections;




public class TowerControl : MonoBehaviour
{

    private ActUtlType64 Plc;


    //GameObject LampRed;
    //GameObject LampYellow;
    //GameObject LampGreen;
    //GameObject Sphere;

    [SerializeField] private GameObject LampRed;
    [SerializeField] private GameObject LampYellow;
    [SerializeField] private GameObject LampGreen;
    [SerializeField] private GameObject Sphere;

    int redValue = 0;
    int yellowValue = 0;
    int greenValue = 0;
    int forward = 0;
    int backward = 0;

    Vector3 velocity = Vector3.zero;

    public float moveSpeed = 3.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //this.LampRed = GameObject.Find("LampRed");
        //this.LampYellow = GameObject.Find("LampYellow");
        //this.LampGreen = GameObject.Find("LampGreen");
        //this.Sphere = GameObject.Find("Sphere");


        Plc = new ActUtlType64();
        Plc.ActLogicalStationNumber = 0;
        Plc.Open();
        StartCoroutine(ReadPlcData());

    }

    IEnumerator ReadPlcData()
    {
        while (true) // 무한 반복
        {

            Plc.GetDevice("M0", out redValue);
            Plc.GetDevice("M1", out yellowValue);
            Plc.GetDevice("M2", out greenValue);
            Plc.GetDevice("M3", out forward);
            Plc.GetDevice("M4", out backward);

            //SetSphereColor(this.LampRed, redValue == 1 ? Color.red : Color.gray);
            //SetSphereColor(this.LampYellow, yellowValue == 1 ? Color.yellow : Color.gray);
            //SetSphereColor(this.LampGreen, greenValue == 1 ? Color.green : Color.gray);
            //if (forward == 1 && backward == 0)
            //{
            //    Sphere.transform.position = Vector3.SmoothDamp(Sphere.transform.position, new Vector3(-15, 0.5f, 5), ref velocity, 4f);
            //    Sphere.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            //}

            //else if (backward == 1 && forward == 0)
            //{
            //    Sphere.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            //}
            // 100ms(0.1초) 대기
            yield return new WaitForSeconds(0.1f);
        }
    }



    // Update is called once per frame
    void Update()
    {

        SetSphereColor(this.LampRed, redValue == 1 ? Color.red : Color.gray);
        SetSphereColor(this.LampYellow, yellowValue == 1 ? Color.yellow : Color.gray);
        SetSphereColor(this.LampGreen, greenValue == 1 ? Color.green : Color.gray);

        if (forward == 1 && backward == 0)
        {
            Sphere.transform.position = Vector3.SmoothDamp(Sphere.transform.position, new Vector3(-15, 0.5f, 5), ref velocity, 5f);
            Sphere.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }

        else if (backward == 1 && forward == 0)
        {

            Sphere.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            //_rbsphere = this.sphere.GetComponent<Rigidbody>();
            //_rbsphere.AddForce(Vector3.left * 5.0f);
        }
    }
    void OnApplicationQuit()
    {
        // 프로그램 종료 시 PLC 연결 해제
        Plc.Close();
    }

    private void SetSphereColor(GameObject sphere, Color color)
    {
        var renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }
}

