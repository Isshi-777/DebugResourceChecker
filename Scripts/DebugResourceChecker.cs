using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace Isshi777
{
    /// <summary>
    /// �����^�C���Ŏg�p���Ă��郊�\�[�X�̐��ƃT�C�Y���v������
    /// </summary>
    /// <typeparam name="T">���\�[�X�̃^�C�v</typeparam>
    public class DebugResourceChecker<T> where T : UnityEngine.Object
    {
        /// <summary>
        /// �ڍ׃N���X
        /// </summary>
        public class Detail
        {
            /// <summary>
            /// ���O
            /// </summary>
            public string Name { private set; get; }

            /// <summary>
            /// �T�C�Y
            /// </summary>
            public long Size { private set; get; }

            /// <summary>
            /// �T�C�Y(�L���o�C�g)
            /// </summary>
            public float SizeKB { get { return ConvertKB(this.Size); } }

            /// <summary>
            /// �T�C�Y(���K�o�C�g)
            /// </summary>
            public float SizeMB { get { return ConvertMB(this.Size); } }

            public Detail(string name, long size)
            {
                this.Name = name;
                this.Size = size;
            }
        }

        /// <summary>
        /// �L���o�C�g
        /// </summary>
        private static float KB = 1024.0f;

        /// <summary>
        /// ���K�o�C�g
        /// </summary>
        private static float MB = 1048576.0f;

        /// <summary>
        /// ���\�[�X��
        /// </summary>
        public int Count { private set; get; }

        /// <summary>
        /// �g�[�^���T�C�Y
        /// </summary>
        public long TotalSize { private set; get; }

        /// <summary>
        /// �g�[�^���T�C�Y(�L���o�C�g)
        /// </summary>
        public float TotalSizeKB { get { return ConvertKB(this.TotalSize); } }

        /// <summary>
        /// �g�[�^���T�C�Y(���K�o�C�g)
        /// </summary>
        public float TotalSizeMB { get { return ConvertMB(this.TotalSize); } }

        /// <summary>
        /// �s�[�N���̃T�C�Y
        /// </summary>
        public long PeakSize { private set; get; }

        /// <summary>
        /// �s�[�N���̃T�C�Y(�L���o�C�g)
        /// </summary>
        public float PeakSizeKB { get { return ConvertKB(this.PeakSize); } }

        /// <summary>
        /// �s�[�N���̃T�C�Y(���K�o�C�g)
        /// </summary>
        public float PeakSizeMB { get { return ConvertMB(this.PeakSize); } }

        /// <summary>
        /// �ڍׂ̃��X�g
        /// </summary>
        public List<Detail> DetailList { private set; get; } = new List<Detail>();


        /// <summary>
        /// �v��
        /// </summary>
        /// <param name="recordDetail">�ڍׂ̃��X�g���쐬���邩</param>
        public void Sample(bool recordDetail)
        {
            // ���X�g������
            if (recordDetail)
            {
                this.DetailList.Clear();
            }

            this.Count = 0;
            this.TotalSize = 0;
            // Unity�������Ŏg�p���Ă��郊�\�[�X�ȊO�̃��X�g
            var list = Resources.FindObjectsOfTypeAll<T>().Where(c => (c.hideFlags & HideFlags.NotEditable) == 0).Where(c => (c.hideFlags & HideFlags.HideAndDontSave) == 0).ToArray();
            foreach (T obj in list)
            {
                this.Count++;
                long size = Profiler.GetRuntimeMemorySizeLong(obj);
                this.TotalSize += size;

                // �ڍ׏��ݒ�
                if (recordDetail)
                {
                    Detail detail = new Detail(obj.name, size);
                    this.DetailList.Add(detail);
                }
            }

            // �\�[�g�i�~���j
            if (recordDetail)
            {
                this.DetailList.Sort((a, b) => CompareDetail(b, a));
            }

            // �s�[�N���̃T�C�Y�X�V
            if (this.PeakSize < this.TotalSize)
            {
                this.PeakSize = this.TotalSize;
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        public void Refresh()
        {
            this.Count = 0;
            this.TotalSize = 0;
            this.PeakSize = 0;
            this.DetailList?.Clear();
        }

        /// <summary>
        /// �L���o�C�g�ϊ�
        /// </summary>
        public static float ConvertKB(long size)
        {
            return (size / KB);
        }

        /// <summary>
        /// ���K�o�C�g�ϊ�
        /// </summary>
        public static float ConvertMB(long size)
        {
            return (size / MB);
        }

        /// <summary>
        /// �ڍ׏��\�[�g�p�̔�r����
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
