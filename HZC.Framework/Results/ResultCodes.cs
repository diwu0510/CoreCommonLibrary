namespace HZC.Framework
{
    public class ResultCodes
    {

        public static int Ok => 200;

        public static int NotFound => 404;

        public static int BadRequest => 400;

        public static int UnAuthenticate => 401;

        public static int UnAuthorize => 403;

        public static int Exception => 500;

        public static int DbFail => 510;

        public static int Fail => 511;
    }
}
