using System.Collections;
using UnityEngine;
using NModbus;
using System.Net.Sockets;
using UnityEngine.UI;



public class ModbusClient : MonoBehaviour
{
    private TcpClient client;
    private IModbusMaster modbusMaster;

    [SerializeField] private GameObject LampRed;
    [SerializeField] private GameObject LampYellow;
    [SerializeField] private GameObject LampGreen;
    [SerializeField] private GameObject Sphere;
    [SerializeField] private GameObject Connected;
    [SerializeField] private GameObject Pulley;
    [SerializeField] private GameObject Hinge;
    [SerializeField] private Button btnConnect;
    [SerializeField] private Button btnDisConnect;

    bool isConnected = false;
    bool redValue = false;
    bool yellowValue = false;
    bool greenValue = false;
    bool forward = false;
    bool backward = false;
    bool ccw = false;
    bool cw = false;
    bool h_ccw = false;
    bool h_cw = false;

    ushort[] registers;
    bool[] coils;

    Vector3 velocity = Vector3.zero;
    Quaternion angle = Quaternion.Euler(0, 0, 0);

    public float moveSpeed = 3.0f;
    public float rotationSpeed = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        btnConnect.onClick.AddListener(Connect);
        btnDisConnect.onClick.AddListener(DisConnect);

        StartCoroutine(ReadHoldingRegister());
        StartCoroutine(ReadCoil());
    }

    IEnumerator ReadHoldingRegister()
    {
        while (true)
        {
            if (modbusMaster != null)
            {
                ushort startAddress = 0;
                ushort numRegisters = 10;
                registers = modbusMaster.ReadHoldingRegisters(1, startAddress, numRegisters);

                Debug.Log($"Coils: {string.Join(", ", registers)}");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ReadCoil()
    {
        while (true)
        {
            if (modbusMaster != null)
            {
                ushort startAddress = 0;
                ushort numCoils = 10;
                coils = modbusMaster.ReadInputs(1, startAddress, numCoils);

                redValue = coils[0];
                yellowValue = coils[1];
                greenValue = coils[2];
                forward = coils[3];
                backward = coils[4];
                ccw = coils[5];
                cw = coils[6];
                h_ccw = coils[7];
                h_cw = coils[8];

                //Debug.Log($"Coils: {string.Join(", ", coils)}");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetSphereColor(this.Connected, isConnected ? Color.blue : Color.gray);
        SetSphereColor(this.LampRed, redValue ? Color.red : Color.gray);
        SetSphereColor(this.LampYellow, yellowValue ? Color.yellow : Color.gray);
        SetSphereColor(this.LampGreen, greenValue ? Color.green : Color.gray);

        if (isConnected)
        {
            if (forward == true && backward == false)
            {
                // 선형 이동 두가지 방법 
                Sphere.transform.position = Vector3.SmoothDamp(Sphere.transform.position, new Vector3(-15, 0.5f, 5), ref velocity, 2f);
                // 목표 위치까지 목표 시간 내 이동(속도는 0으로 설정 후 Coroutine마다 속도 알아서 조절)
            }

            else if (backward == true && forward == false)
            {

                Sphere.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
                // 지정한 방향, 지정한 속도로 유니티 내 프레임 시간만큼 이동
                // Coroutien마다 현재 위치에서 moveSpeed * Time.deltaTime만큼 이동함
            }

            if (ccw == true && cw == false)
            {
                Pulley.transform.Rotate(Vector3.right * 40.0f * Time.deltaTime);
            }

            else if (cw == true && ccw == false)
            {
                Pulley.transform.Rotate(Vector3.left * 40.0f * Time.deltaTime);
            }

            if (h_ccw == true && h_cw == false)
            {
                angle = Quaternion.Euler(90, 0, 0);
                HingeRotate(angle);

            }

            else if (h_cw == true && h_ccw == false)
            {
                angle = Quaternion.Euler(-90, 0, 0);
                HingeRotate(angle);
            }
        }
    }
    private void Connect()
    {
        client = new TcpClient("127.0.0.1", 502);
        isConnected = client.Connected;
        var factory = new ModbusFactory();
        modbusMaster = factory.CreateMaster(client);
    }
    private void DisConnect()
    {
        if (client != null && isConnected)
        {
            client.Close();
            isConnected = false;
            modbusMaster = null;

            redValue = false;
            yellowValue = false;
            greenValue = false;

            // 레지스터와 코일 초기화

            for (int i = 0; i < coils.Length; i++)
            {
                coils[i] = false;
            }
        }
    }

    private void SetSphereColor(GameObject sphere, Color color)
    {   
        // 오브젝트의 Renderer 할당
        var renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color; // 오브젝트의 색상 변경
        }
    }

    private void HingeRotate(Quaternion _angle)
    {
        Hinge.transform.rotation = Quaternion.Lerp(Hinge.transform.rotation, _angle, Time.deltaTime * rotationSpeed);
    }
}
