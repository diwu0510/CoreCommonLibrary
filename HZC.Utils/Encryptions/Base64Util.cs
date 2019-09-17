using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HZC.Utils
{
    public class Base64Util
    {
        private const string V = @"ABCDEFGHIJKLMNOPQRSTUVWXYZbacdefghijklmnopqrstu_wxyz0123456789*-";

        #region 字段

        protected static Base64Util Sb64 = new Base64Util();

        protected string MCodeTable = V;

        protected string MPad = "v";

        protected Dictionary<int, char> Mt1 = new Dictionary<int, char>();

        protected Dictionary<char, int> Mt2 = new Dictionary<char, int>();

        #endregion

        #region 属性

        /// <summary>
        /// 设置并验证密码表合法性
        /// </summary>
        public string CodeTable
        {
            get => MCodeTable;
            set
            {
                if (value == null)
                {
                    throw new Exception("密码表不能为null");
                }
                if (value.Length < 64)
                {
                    throw new Exception("密码表长度必须至少为64");
                }
                ValidateRepeat(value);
                ValidateEqualPad(value, MPad);
                MCodeTable = value;
                InitDict();
            }
        }

        /// <summary>
        /// 设置并验证补码合法性
        /// </summary>
        public string Pad
        {
            get => MPad;
            set
            {
                if (value == null)
                {
                    throw new Exception("密码表的补码不能为null");
                }
                if (value.Length != 1)
                {
                    throw new Exception("密码表的补码长度必须为1");
                }
                ValidateEqualPad(MCodeTable, value);
                MPad = value;
                InitDict();
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化字典
        /// </summary>
        public Base64Util()
        {
            InitDict();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取具有标准的Base64密码表的加密类
        /// </summary>
        /// <returns>Base64密码表的加密类</returns>
        public static Base64Util GetStandardBase64()
        {
            var b64 = new Base64Util { Pad = "=", CodeTable = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/" };
            return b64;
        }

        /// <summary>
        /// 使用默认的密码表（双向哈西字典）加密字符串
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string input)
        {
            return Sb64.Encode(input);
        }

        /// <summary>
        /// 使用默认的密码表（双向哈西字典）解密字符串
        /// </summary>
        /// <param name="input">需要解密的字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string Decrypt(string input)
        {
            return Sb64.Decode(input);
        }
        #endregion

        #region 受保护的方法

        protected string Encode(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return "";
            }
            var sb = new StringBuilder();
            var tmp = Encoding.UTF8.GetBytes(source);
            var remain = tmp.Length % 3;
            var patch = 3 - remain;
            if (remain != 0)
            {
                Array.Resize(ref tmp, tmp.Length + patch);
            }
            var cnt = (int)Math.Ceiling(tmp.Length * 1.0 / 3);
            for (var i = 0; i < cnt; i++)
            {
                sb.Append(EncodeUnit(tmp[i * 3], tmp[i * 3 + 1], tmp[i * 3 + 2]));
            }
            if (remain != 0)
            {
                sb.Remove(sb.Length - patch, patch);
                for (var i = 0; i < patch; i++)
                {
                    sb.Append(MPad);
                }
            }
            return sb.ToString();
        }

        protected string EncodeUnit(params byte[] unit)
        {
            var obj = new int[4];
            obj[0] = (unit[0] & 0xfc) >> 2;
            obj[1] = ((unit[0] & 0x03) << 4) + ((unit[1] & 0xf0) >> 4);
            obj[2] = ((unit[1] & 0x0f) << 2) + ((unit[2] & 0xc0) >> 6);
            obj[3] = unit[2] & 0x3f;
            var sb = new StringBuilder();
            foreach (var t in obj)
            {
                sb.Append(GetEc(t));
            }
            return sb.ToString();
        }

        protected char GetEc(int code)
        {
            return Mt1[code]; //m_codeTable[code];
        }

        protected string Decode(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return "";
            }
            var list = new List<byte>();
            var tmp = source.ToCharArray();
            var remain = tmp.Length % 4;
            if (remain != 0)
            {
                Array.Resize(ref tmp, tmp.Length - remain);
            }
            var patch = source.IndexOf(MPad, StringComparison.Ordinal);
            if (patch != -1)
            {
                patch = source.Length - patch;
            }
            var cnt = tmp.Length / 4;
            for (var i = 0; i < cnt; i++)
            {
                DecodeUnit(list, tmp[i * 4], tmp[i * 4 + 1], tmp[i * 4 + 2], tmp[i * 4 + 3]);
            }
            for (var i = 0; i < patch; i++)
            {
                list.RemoveAt(list.Count - 1);
            }
            return Encoding.UTF8.GetString(list.ToArray());
        }

        protected void DecodeUnit(List<byte> byteArr, params char[] chArray)
        {
            var res = new int[3];
            var unit = new byte[chArray.Length];
            for (var i = 0; i < chArray.Length; i++)
            {
                unit[i] = FindChar(chArray[i]);
            }
            res[0] = (unit[0] << 2) + ((unit[1] & 0x30) >> 4);
            res[1] = ((unit[1] & 0xf) << 4) + ((unit[2] & 0x3c) >> 2);
            res[2] = ((unit[2] & 0x3) << 6) + unit[3];
            byteArr.AddRange(res.Select(t => (byte)t));
        }

        protected byte FindChar(char ch)
        {
            var pos = Mt2[ch]; //m_codeTable.IndexOf(ch);
            return (byte)pos;
        }

        protected void InitDict()
        {
            Mt1.Clear();
            Mt2.Clear();
            Mt2.Add(MPad[0], -1);
            for (var i = 0; i < MCodeTable.Length; i++)
            {
                Mt1.Add(i, MCodeTable[i]);
                Mt2.Add(MCodeTable[i], i);
            }
        }

        protected void ValidateRepeat(string input)
        {
            for (var i = 0; i < input.Length; i++)
            {
                if (input.LastIndexOf(input[i]) > i)
                {
                    throw new Exception("密码表中含有重复字符：" + input[i]);
                }
            }
        }

        protected void ValidateEqualPad(string input, string pad)
        {
            if (input.IndexOf(pad, StringComparison.Ordinal) > -1)
            {
                throw new Exception("密码表中包含了补码字符：" + pad);
            }
        }
        #endregion
    }
}
