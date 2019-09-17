using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace HZC.Utils
{
    public class VerificationCodeUtil
    {
        /// <summary>  
        /// 该方法用于生成指定位数的随机数  
        /// </summary>  
        /// <param name="codeLength">参数是随机数的位数</param>  
        /// <returns>返回一个随机数字符串</returns>  
        private string GetRandomCode(int codeLength)
        {
            //验证码可以显示的字符集合  
            const string vChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,P,P,Q,R,S,T,U,V,W,X,Y,Z";
            var vcArray = vChar.Split(new[] { ',' });  //拆分成数组   
            var code = "";  //产生的随机数  
            var temp = -1;  //记录上次随机数值，尽量避避免生产几个一样的随机数  

            var rand = new Random();
            //采用一个简单的算法以保证生成随机数的不同  
            for (var i = 1; i < codeLength + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));  //初始化随机类  
                }
                var t = rand.Next(61);  //获取随机数  
                if (temp != -1 && temp == t)
                {
                    return GetRandomCode(codeLength);  //如果获取的随机数重复，则递归调用  
                }
                temp = t;  //把本次产生的随机数记录起来  
                code += vcArray[t];  //随机数的位数加一  
            }
            return code;
        }

        /// <summary>  
        /// 该方法是将生成的随机数写入图像文件  
        /// </summary>  
        /// <param name="code">code是一个随机数</param>
        /// <param name="numbers">生成位数（默认4位）</param>  
        public MemoryStream Create(out string code, int numbers = 4)
        {
            code = GetRandomCode(numbers);
            Graphics g;
            var random = new Random();

            //验证码颜色集合  
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };

            //验证码字体集合
            string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };


            //定义图像的大小，生成图像的实例  
            var img = new Bitmap(code.Length * 18, 32);

            g = Graphics.FromImage(img);//从Img对象生成新的Graphics对象    

            g.Clear(Color.White);//背景设为白色  

            //在随机位置画背景点  
            for (var i = 0; i < 100; i++)
            {
                var x = random.Next(img.Width);
                var y = random.Next(img.Height);
                g.DrawRectangle(new Pen(Color.LightGray, 0), x, y, 1, 1);
            }
            //验证码绘制在g中  
            for (var i = 0; i < code.Length; i++)
            {
                //随机颜色索引值 
                var colorIndex = random.Next(7);
                //随机字体索引值
                var fontIndex = random.Next(5);
                //字体
                var f = new Font(fonts[fontIndex], 15, FontStyle.Bold);
                //颜色
                Brush b = new SolidBrush(c[colorIndex]);  
                var ii = 4;
                //控制验证码不在同一高度
                if ((i + 1) % 2 == 0)  
                {
                    ii = 2;
                }
                //绘制一个验证字符
                g.DrawString(code.Substring(i, 1), f, b, 3 + (i * 12), ii);  
            }
            //生成内存流对象
            var ms = new MemoryStream();
            //将此图像以Png图像文件的格式保存到流中
            img.Save(ms, ImageFormat.Jpeg);  

            //回收资源  
            g.Dispose();
            img.Dispose();
            return ms;
        }
    }
}
