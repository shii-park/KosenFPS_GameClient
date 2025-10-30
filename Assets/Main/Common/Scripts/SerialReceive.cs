using UnityEngine;

public class SerialReceive : MonoBehaviour
{
    public SerialHand serialHand;

    void Start()
    {
        Debug.Log("SerialReceiveスクリプトが開始されました。");
        if (serialHand == null) {
            Debug.LogError("SerialHandがインスペクターに設定されていません！");
            return;
        }
        serialHand.OnDataReceived += OnDataReceived;
        Debug.Log("OnDataReceivedイベントに登録しました。");
    }

    // 受信した信号(message)に対する処理
    void OnDataReceived(string message)
    {
        try
        {
            // 受信したデータをコンソールに表示
            Debug.Log("受信生データ: " + message);

            // カンマで分割して、文字列の配列にする
            string[] values = message.Split(',');

            // 配列の長さが6でなければ、データが不正なので処理を中断
            if (values.Length != 7) {
                Debug.LogWarning("受信データの形式が不正です。要素数: " + values.Length);
                return;
            }

            // 文字列をfloat型に変換
            float ax = float.Parse(values[0]);
            float ay = float.Parse(values[1]);
            float az = float.Parse(values[2]);
            float gx = float.Parse(values[3]);
            float gy = float.Parse(values[4]);
            float gz = float.Parse(values[5]);

            // 変換した値をコンソールに表示して確認
            Debug.Log(string.Format("Accel: (X:{0}, Y:{1}, Z:{2})", ax, ay, az));
            Debug.Log(string.Format("Gyro:  (X:{0}, Y:{1}, Z:{2})", gx, gy, gz));

            // ここで値を使ってオブジェクトを動かすなどの処理を行う
            // transform.rotation = ...

        }
        catch (System.Exception e)
        {
            Debug.LogError("データ処理中にエラー: " + e.Message);
        }
    }
}