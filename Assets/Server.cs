using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;



public class Server : MonoBehaviour {

    [SerializeField]
    private GameObject LocalPlayer;
    [SerializeField]
    private GameObject remotePlayer;
    [SerializeField]
    private int ReceivePort = 20000;
    [SerializeField]
    private int SendPort = 20001;
    [SerializeField]
    private string ip;
    [SerializeField]
    private Text resultText;
    [SerializeField]
    private bool ServerMode;

    

    private Move LocalMoveScript;
    private Move RemoteMoveScript;
    private UdpClient sender;


    private bool gameRunning;
    private bool standby;
    private bool Gamestarted;
    private bool EndGame;
    private string winner;

    private int BuffDirection = -1;

    // Use this for initialization
    void Start () {
        RemoteMoveScript = remotePlayer.GetComponent<Move>();
        LocalMoveScript = LocalPlayer.GetComponent<Move>();
        gameRunning = false;
        standby = true;
        ServerMode = false;
        Gamestarted = false;
        EndGame = false;
        ReceivePort = 0;
        SendPort = 0;
        ip = "";
        resultText.text =  winner = "";
        Application.runInBackground = true;
    }
	
	// Update is called once per frame
	void Update () {

        if(standby)
            return;

        if (gameRunning )
        {
            if (!Gamestarted)
            {
                initGame();
                Gamestarted = true;
                if(ServerMode)
                    NewtworkClientSend(-1);
            }

            if (ServerMode)
            {
                if (LocalMoveScript.isPlayerDead())
                {
                    standby = true;
                    EndOfGame(2);
                    LocalMoveScript.EndGame();
                    RemoteMoveScript.EndGame();
                    resultText.text = winner;
                }
                else if (RemoteMoveScript.isPlayerDead())
                {
                    standby = true;
                    EndOfGame(1);
                    LocalMoveScript.EndGame();
                    RemoteMoveScript.EndGame();
                    resultText.text = winner;

                }
            }
            else if(EndGame)
            {
                resultText.text = winner;
                LocalMoveScript.EndGame();
                RemoteMoveScript.EndGame();
                EndGame = false;
            }
        }
        else if(ServerMode)
        {
            Debug.Log("Waiting For client");          
        }
        else
        {
            //NewtworkClientSend(-1);
                
        }

    }

    private void EndOfGame(int player)
    {
        Debug.Log("Player " + player + "Win");
        winner = "Player " + player + " Win";
        if (ServerMode)    
            NewtworkClientSend(player + 3);
        EndGame = true;
    }
    public void UpdateMove(int direction)
    {
        NewtworkClientSend(direction);
    }

    // -----------------------------Network Actions bellow-----------------------------

    private  void NetworkUpdate(IAsyncResult ar)
    {
        
        UdpClient c = (UdpClient)ar.AsyncState;
        IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Byte[] receivedBytes = c.EndReceive(ar, ref receivedIpEndPoint);

        // Convert data to ASCII 
        string receivedText = ASCIIEncoding.ASCII.GetString(receivedBytes);

        if (!gameRunning)
            gameRunning = true;

        Debug.Log(receivedIpEndPoint + ": " + receivedText + Environment.NewLine);
        decodeMessage(receivedText);
        c.BeginReceive(NetworkUpdate, ar.AsyncState);
    }

    private void NetworkInitServer()
    {
        UdpClient receiver = new UdpClient(ReceivePort);

        // Display some information
        Debug.Log("Starting Upd receiving on port: " + ReceivePort);
        Debug.Log("Press any key to quit.");
        Debug.Log("-------------------------------\n");

        // Start async receiving
        receiver.BeginReceive(NetworkUpdate, receiver);
    }

    private void NewtworkClientSend(int direction)
    {
        // 0 up, 1 right, 2 down, 3 left

        Byte[] buff = Encoding.ASCII.GetBytes(direction.ToString());     
        sender.Send(buff, buff.Length, ip, SendPort);
        if(direction != -1 && direction < 4)
        {
            BuffDirection = direction;
        }

    }
    // ----------------------------- End of Network Actions-----------------------------
    public void setReceivePort(string port)
    {
        ReceivePort = int.Parse(port);
    }
    public void setSendPort(string port)
    {
         SendPort = int.Parse(port);
    } 
    public void setServerMode(bool mod)
    {
        ServerMode = mod;
    }
    public void setIp(string a_ip)
    {
        ip = a_ip;
    }
    public void initNetwork()
    {
        if (ReceivePort != 0 && SendPort != 0 && ip.Length > 1)
        {
            //--------------------------------- Network COnnection------------------------

            NetworkInitServer();
            sender = new UdpClient();
            //--------------------------------- End of network connection -----------------

            Debug.Log("server mode is :" + ServerMode);
            standby = false;
            NewtworkClientSend(-1);
        }
        else
            Debug.Log("Port no initialized");
        
    }

    void initGame()
    {
        if (ServerMode)
        {
            RemoteMoveScript.setRemote(true);
            LocalMoveScript.startMoving();
            RemoteMoveScript.startMoving();
            //Debug.Log("init server mode");
        }
        else
        {
            //Debug.Log("init client mode");
            Move temp = RemoteMoveScript;
            RemoteMoveScript = LocalMoveScript;
            LocalMoveScript = temp;
            RemoteMoveScript.setRemote(true);
            LocalMoveScript.startMoving();
            RemoteMoveScript.startMoving();
        }
    }

    private void decodeMessage(string receivedText)
    {
        
        int message = -2;
        try
        {
            message = int.Parse(receivedText);

        }
        catch (Exception ex)
        {
            // Debug.Log("Parse Error");
        }

        switch (message)
        {
            case -2:
                Debug.Log("parse message error");
                break;
            case -1:
                // Ping, do nothing
                break;
            case 0:
                if (!standby)
                {
                    RemoteMoveScript.remoteUpdate(message);
                    NewtworkClientSend(6);
                }
                break;
            case 1:
                if (!standby)
                {
                    RemoteMoveScript.remoteUpdate(message);
                    NewtworkClientSend(6);
                }
                break;
            case 2:
                if (!standby)
                {
                    RemoteMoveScript.remoteUpdate(message);
                    NewtworkClientSend(6);
                }
                break;
            case 3:
                if (!standby)
                {
                    RemoteMoveScript.remoteUpdate(message);
                    NewtworkClientSend(6);
                }
                break;
            case 4:
                //Debug.Log("Player 1 Win");
                EndOfGame(message - 3);
                break;
            case 5:
                //Debug.Log("Player 2 win");
                EndOfGame(message - 3);
                break;
            case 6: // move ACK
                if(!ServerMode)
                    LocalMoveScript.validateMove(BuffDirection);
                break;
                
        }
    }
}




