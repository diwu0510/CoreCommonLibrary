namespace HZC.Framework
{
    public class Result
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public Result()
        { }

        public Result(int code, string message = "")
        {
            Code = code;
            Message = message;
        }
    }
}
