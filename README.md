# DebugResourceChecker
ランタイムでのリソースの数やメモリ使用量を知る  

## 使用について
こちらの機能はあくまで実機（端末）用の機能であり、Unityエディタでの使用を想定していません


## 使用例
以下はTexture2Dアセットの計測を１秒ごとに行い表示する例  
```cs
using Isshi777;
using System.Collections;
using UnityEngine;

public class Example : MonoBehaviour
{
    private DebugResourceChecker<Texture2D> texture2DChecker = new DebugResourceChecker<Texture2D>();

    private IEnumerator Start()
    {
        while (true)
        {
            // １秒ごとに計測
            yield return new WaitForSeconds(1f);
            this.SampleAll();
        }
    }

    private void SampleAll()
    {
        this.texture2DChecker.Sample(true);
    }

    private void OnGUI()
    {
        this.DisplayAll();
    }

    private void DisplayAll()
    {
        this.Display("Texture2D", this.texture2DChecker);
    }

    private void Display<T>(string title, DebugResourceChecker<T> checker) where T : UnityEngine.Object
    {
        GUILayout.Label(title);
        GUILayout.Label(string.Format(" Count     : {0} ", checker.Count));
        GUILayout.Label(string.Format(" TotalSize : {0:F2}MB ", checker.TotalSizeMB));
        GUILayout.Label(string.Format(" PeakSize  : {0:F2}MB ", checker.PeakSizeMB));

        // サイズが大きい10件のみ表示
        int count = Mathf.Min(10, checker.DetailList.Count);
        for (int i = 0; i < count; i++)
        {
            var detail = checker.DetailList[i];
            string str = detail.Name + " : " + (detail.SizeMB).ToString("0.00") + "MB";
            GUILayout.Label(str);
        }
    }
}
```