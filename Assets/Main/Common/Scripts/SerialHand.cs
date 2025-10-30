using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Collections.Concurrent; 

public class SerialHand : MonoBehaviour
{
    public delegate void SerialDataReceivedEventHandler(string message);
    public event SerialDataReceivedEventHandler OnDataReceived;

    public string portName = "COM7";
    public int baudRate = 115200;

    private SerialPort serialPort_;
    private Thread thread_;
    private bool isRunning_ = false;

    private ConcurrentQueue<string> messageQueue_ = new ConcurrentQueue<string>();

    void Awake()
    {
        Open();
    }

    void Update()
    {

        if (messageQueue_.Count > 0) 
        {
            Debug.Log("キューに " + messageQueue_.Count + " 件のデータがあります。処理します。");
        }

        while (messageQueue_.TryDequeue(out string message))
        {
            OnDataReceived?.Invoke(message);
        }
    }

    void OnDestroy()
    {
        Close();
    }

    private void Open()
    {
        try 
        {
            serialPort_ = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            serialPort_.ReadTimeout = 100; // タイムアウトを設定して固まらないようにする
            serialPort_.Open();

            isRunning_ = true;

            thread_ = new Thread(Read);
            thread_.Start();
        }
        catch (System.Exception e)
        {
            // Open時のエラーはメインスレッドなのでDebug.LogでOK
            Debug.LogError("ポートを開けませんでした: " + e.Message);
        }
    }

    private void Close()
    {
        isRunning_ = false;

        if (thread_ != null && thread_.IsAlive)
        {
            thread_.Join();
        }

        if (serialPort_ != null && serialPort_.IsOpen)
        {
            serialPort_.Close();
            serialPort_.Dispose();
        }
    }

    private void Read()
    {
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen)
        {
            try
            {

                string message = serialPort_.ReadLine();
                messageQueue_.Enqueue(message);
            }
            catch (System.TimeoutException)
            {
                // データが来ていない時のタイムアウトは正常なので、何もしない
            }
            catch (System.Exception e)
            {

                if (isRunning_)
                {
                    // エラーがわかるように文字列を加工してキューに入れる
                    messageQueue_.Enqueue("READ_ERROR: " + e.Message);
                }
            }
        }
    }

    public void Write(string message)
    {
        if (serialPort_ == null || !serialPort_.IsOpen) return;
        try
        {
            serialPort_.Write(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}