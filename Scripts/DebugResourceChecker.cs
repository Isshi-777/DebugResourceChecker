using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace Isshi777
{
    /// <summary>
    /// ランタイムで使用しているリソースの数とサイズを計測する
    /// </summary>
    /// <typeparam name="T">リソースのタイプ</typeparam>
    public class DebugResourceChecker<T> where T : UnityEngine.Object
    {
        /// <summary>
        /// 詳細クラス
        /// </summary>
        public class Detail
        {
            /// <summary>
            /// 名前
            /// </summary>
            public string Name { private set; get; }

            /// <summary>
            /// サイズ
            /// </summary>
            public long Size { private set; get; }

            /// <summary>
            /// サイズ(キロバイト)
            /// </summary>
            public float SizeKB { get { return ConvertKB(this.Size); } }

            /// <summary>
            /// サイズ(メガバイト)
            /// </summary>
            public float SizeMB { get { return ConvertMB(this.Size); } }

            public Detail(string name, long size)
            {
                this.Name = name;
                this.Size = size;
            }
        }

        /// <summary>
        /// キロバイト
        /// </summary>
        private static float KB = 1024.0f;

        /// <summary>
        /// メガバイト
        /// </summary>
        private static float MB = 1048576.0f;

        /// <summary>
        /// リソース数
        /// </summary>
        public int Count { private set; get; }

        /// <summary>
        /// トータルサイズ
        /// </summary>
        public long TotalSize { private set; get; }

        /// <summary>
        /// トータルサイズ(キロバイト)
        /// </summary>
        public float TotalSizeKB { get { return ConvertKB(this.TotalSize); } }

        /// <summary>
        /// トータルサイズ(メガバイト)
        /// </summary>
        public float TotalSizeMB { get { return ConvertMB(this.TotalSize); } }

        /// <summary>
        /// ピーク時のサイズ
        /// </summary>
        public long PeakSize { private set; get; }

        /// <summary>
        /// ピーク時のサイズ(キロバイト)
        /// </summary>
        public float PeakSizeKB { get { return ConvertKB(this.PeakSize); } }

        /// <summary>
        /// ピーク時のサイズ(メガバイト)
        /// </summary>
        public float PeakSizeMB { get { return ConvertMB(this.PeakSize); } }

        /// <summary>
        /// 詳細のリスト
        /// </summary>
        public List<Detail> DetailList { private set; get; } = new List<Detail>();


        /// <summary>
        /// 計測
        /// </summary>
        /// <param name="recordDetail">詳細のリストを作成するか</param>
        public void Sample(bool recordDetail)
        {
            // リスト初期化
            if (recordDetail)
            {
                this.DetailList.Clear();
            }

            this.Count = 0;
            this.TotalSize = 0;
            // Unityが内部で使用しているリソース以外のリスト
            var list = Resources.FindObjectsOfTypeAll<T>().Where(c => (c.hideFlags & HideFlags.NotEditable) == 0).Where(c => (c.hideFlags & HideFlags.HideAndDontSave) == 0).ToArray();
            foreach (T obj in list)
            {
                this.Count++;
                long size = Profiler.GetRuntimeMemorySizeLong(obj);
                this.TotalSize += size;

                // 詳細情報設定
                if (recordDetail)
                {
                    Detail detail = new Detail(obj.name, size);
                    this.DetailList.Add(detail);
                }
            }

            // ソート（降順）
            if (recordDetail)
            {
                this.DetailList.Sort((a, b) => CompareDetail(b, a));
            }

            // ピーク時のサイズ更新
            if (this.PeakSize < this.TotalSize)
            {
                this.PeakSize = this.TotalSize;
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Refresh()
        {
            this.Count = 0;
            this.TotalSize = 0;
            this.PeakSize = 0;
            this.DetailList?.Clear();
        }

        /// <summary>
        /// キロバイト変換
        /// </summary>
        public static float ConvertKB(long size)
        {
            return (size / KB);
        }

        /// <summary>
        /// メガバイト変換
        /// </summary>
        public static float ConvertMB(long size)
        {
            return (size / MB);
        }

        /// <summary>
        /// 詳細情報ソート用の比較処理
        /// </summary>
        private static int CompareDetail(Detail a, Detail b)
        {
            if (a.Size < b.Size)
            {
                return -1;
            }
            else if (a.Size > b.Size)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
