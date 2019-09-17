using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HZC.Utils
{
    public class HtmlUtil
    {
        #region 获取图片
        /// <summary>
        /// 获取HTML文本的图片地址
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>/
        /// 
        public static List<string> GetImageUrl(string html)
        {
            var resultStr = new List<string>();
            var r = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);//忽视大小写
            var mc = r.Matches(html);

            foreach (Match m in mc)
            {
                resultStr.Add(m.Groups["imgUrl"].Value.ToLower());
            }

            if (resultStr.Count > 0)
            {
                return resultStr;
            }
            return resultStr;
        }
        #endregion

        /// <summary>
        /// 获取内容摘要
        /// </summary>
        /// <returns></returns>
        public static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return "";
            }

            html = html
                .Replace("\n", "")
                .Replace(" ", "")
                .Replace("&nbsp;", " ")
                .Replace("<br>", "\n");
            var regex = new Regex(@"<[^>]+>|</[^>]+>");
            var strOutput = regex.Replace(html, "");
            return strOutput;
        }

        #region 提取HTML的摘要
        /// <summary>
        /// 提取HTML的摘要
        /// </summary>
        /// <param name="content">要提取的HTML</param>
        /// <param name="length">要提取的字数</param>
        /// <param name="stripHtml">是否清除HTML代码 </param>
        /// <returns></returns>
        public static string GetContentSummary(string content, int length, bool stripHtml)
        {
            if (string.IsNullOrEmpty(content) || length == 0)
            {
                return "";
            }

            if (stripHtml)
            {
                var re = new Regex("<[^>]*>");
                content = re.Replace(content, "");
                content = content.Replace("　", "").Replace(" ", "");
                if (content.Length <= length)
                {
                    return content;
                }
                return content.Substring(0, length) + "……";
            }

            if (content.Length <= length)
            {
                return content;
            }

            var pos = 0;
            var size = 0;
            bool firstStop = false, noTr = false, noLi = false;

            var sb = new StringBuilder();
            while (true)
            {
                if (pos >= content.Length)
                {
                    break;
                }

                var cur = content.Substring(pos, 1);
                if (cur == "<")
                {
                    var next = content.Substring(pos + 1, 3).ToLower();
                    int nPos;
                    if (next.IndexOf("p", StringComparison.Ordinal) == 0 && next.IndexOf("pre", StringComparison.Ordinal) != 0)
                    {
                        nPos = content.IndexOf(">", pos, StringComparison.Ordinal) + 1;
                    }
                    else if (next.IndexOf("/p", StringComparison.Ordinal) == 0 && next.IndexOf("/pr", StringComparison.Ordinal) != 0)
                    {
                        nPos = content.IndexOf(">", pos, StringComparison.Ordinal) + 1;

                        if (size < length)
                        {
                            sb.Append("<br/>");
                        }
                    }
                    else if (next.IndexOf("br", StringComparison.Ordinal) == 0)
                    {
                        nPos = content.IndexOf(">", pos, StringComparison.Ordinal) + 1;
                        if (size < length)
                        {
                            sb.Append("<br/>");
                        }
                    }
                    else if (next.IndexOf("img", StringComparison.Ordinal) == 0)
                    {
                        nPos = content.IndexOf(">", pos, StringComparison.Ordinal) + 1;
                        if (size < length)
                        {
                            sb.Append(content.Substring(pos, nPos - pos));
                            size += nPos - pos + 1;
                        }
                    }
                    else if (next.IndexOf("li", StringComparison.Ordinal) == 0 || next.IndexOf("/li", StringComparison.Ordinal) == 0)
                    {
                        nPos = content.IndexOf(">", pos, StringComparison.Ordinal) + 1;
                        if (size < length)
                        {
                            sb.Append(content.Substring(pos, nPos - pos));
                        }
                        else
                        {
                            if (!noLi && next.IndexOf("/li", StringComparison.Ordinal) == 0)
                            {
                                sb.Append(content.Substring(pos, nPos - pos));
                                noLi = true;
                            }
                        }
                    }
                    else if (next.IndexOf("tr", StringComparison.Ordinal) == 0 || next.IndexOf("/tr", StringComparison.Ordinal) == 0)
                    {
                        nPos = content.IndexOf(">", pos, StringComparison.Ordinal) + 1;
                        if (size < length)
                        {
                            sb.Append(content.Substring(pos, nPos - pos));
                        }
                        else
                        {
                            if (!noTr && next.IndexOf("/tr", StringComparison.Ordinal) == 0)
                            {
                                sb.Append(content.Substring(pos, nPos - pos)); noTr = true;
                            }
                        }
                    }
                    else if (next.IndexOf("td", StringComparison.Ordinal) == 0 || next.IndexOf("/td", StringComparison.Ordinal) == 0)
                    {
                        nPos = content.IndexOf(">", pos, StringComparison.Ordinal) + 1;
                        if (size < length)
                        {
                            sb.Append(content.Substring(pos, nPos - pos));
                        }
                        else
                        {
                            if (!noTr)
                            {
                                sb.Append(content.Substring(pos, nPos - pos));
                            }
                        }
                    }
                    else
                    {
                        nPos = content.IndexOf(">", pos, StringComparison.Ordinal) + 1;
                        sb.Append(content.Substring(pos, nPos - pos));
                    }
                    if (nPos <= pos)
                        nPos = pos + 1;
                    pos = nPos;
                }
                else
                {
                    if (size < length)
                    {
                        sb.Append(cur);
                        size++;
                    }
                    else
                    {
                        if (!firstStop)
                        {
                            sb.Append("……");
                            firstStop = true;
                        }
                    }
                    pos++;
                }
            }
            return sb.ToString();
        } 
        #endregion
    }
}
