using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class Gmanager : NetworkBehaviour
{
    List<GameObject> car;
    [SyncVar]
    public List<string> list = new List<string>();
    public bool startGame = false;
    private float timer = 0;
    private int originNum = 4;
    public Text txt;
    private bool use=true;
    [SyncVar]
    private int non = 0;
    public void CmdchangeNo(int nt)
    {
        non = nt;
    }
    public int getno()
    {
        return non;
    }

    // Start is called before the first frame update
    void Start()
    {
        car = new List<GameObject>();
        txt.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (car.Count != 0&&use)
        {
            int readyCount = 0;
            foreach (var ca in car)
            {
                if (ca.GetComponent<CarMoveController>().isReady)
                {
                    readyCount++;
                }
            }
            if (readyCount == car.Count)
            {
                txt.enabled = true;
                timer += Time.deltaTime;
                if (timer >= 1)
                {
                    timer = 0;
                    originNum -= 1;
                }
                txt.text = originNum.ToString();
                if (originNum == 0)
                {
                    txt.text = "开始比赛";
                    Invoke("close", 1f);
                    startGame = true;
                }

            }
        }
        
    }

    public void addCar(GameObject c)
    {
        car.Add(c);
    }
    private void close()
    {
        txt.enabled = false;
        use = false;
    }
    public bool inList(string s)
    {
        return list.Contains(s);
    }
    public void addList(string s)
    {
        list.Add(s);
    }
}
