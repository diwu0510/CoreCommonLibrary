using System.Drawing;

namespace HZC.Utils.Image
{
    public class FileUploadOption
    {
        public string Path { get; set; } = "/Upload";

        public UploadDirectoryFormat UploadDirectoryFormat { get; set; } = UploadDirectoryFormat.YearMonth;

        public UploadFileNameFormat UploadFileNameFormat { get; set; } = UploadFileNameFormat.DateTime;

        public ImageUploadWaterType ImageUploadWaterType { get; set; } = ImageUploadWaterType.None;

        public ThumbOption Thumb { get; set; }

        public WaterFont Font { get; set; }

        public WaterImage Image { get; set; }
    }

    /// <summary>
    /// 自动生成子文件夹的格式
    /// </summary>
    public enum UploadDirectoryFormat
    {
        None,
        Year,
        YearMonth,
        YearMonthDate
    }

    /// <summary>
    /// 自动生成文件名的格式
    /// </summary>
    public enum UploadFileNameFormat
    {
        Orig,
        DateTime,
        Guid,
        Random
    }

    /// <summary>
    /// 水印类型
    /// </summary>
    public enum ImageUploadWaterType
    {
        None,
        Text,
        Image
    }

    /// <summary>
    /// 水印的位置
    /// </summary>
    public enum ImageUploadWaterPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        Bottom,
        BottomRight
    }

    /// <summary>
    /// 水印字体
    /// </summary>
    public class WaterFont
    {
        public string Text { get; set; }

        public int FontSize { get; set; } = 18;

        public int Opacity { get; set; } = 50;

        public Color Color { get; set; } = Color.Black;

        public int PadBottom { get; set; } = 0;

        public int PadLeft { get; set; } = 0;

        public int PadTop { get; set; } = 0;

        public int PadRight { get; set; } = 0;
    }

    /// <summary>
    /// 水印图片
    /// </summary>
    public class WaterImage
    {
        public string Path { get; set; }

        public int Opacity { get; set; }
    }

    /// <summary>
    /// 缩略图选项
    /// </summary>
    public class ThumbOption
    {
        public int Width { get; set; } = 400;

        public int Height { get; set; } = 400;

        public ThumbSizeType SizeType { get; set; } = ThumbSizeType.Clip;

        public ClipFrom ClipFrom { get; set; } = ClipFrom.MiddleCenter;
    }

    /// <summary>
    /// 缩略图尺寸类型
    /// </summary>
    public enum ThumbSizeType
    {
        None,
        Clip,
        FixedWith,
        FixedHeight
    }

    /// <summary>
    /// 裁剪选项起点
    /// </summary>
    public enum ClipFrom
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
}
