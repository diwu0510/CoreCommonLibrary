using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using YunPian.Models;
using YunPian.Services;

namespace HZC.Utils.YunPian
{
    /// <summary>
    /// 云片网短信服务
    /// </summary>
    public class YunPianService
    {
        private readonly ISmsService _yunPianService;
        private readonly YunPianSettings _setting;


        public YunPianService(ISmsService service, IOptions<YunPianSettings> options)
        {
            _yunPianService = service;
            _setting = options.Value;
        }

        public async Task<Result<SmsSingleSend>> Send(string mobile, string message)
        {
            return await _yunPianService.SingleSendAsync(message, mobile);
        }

        public async Task<Result<SmsSingleSend>> SendVCode(string mobile, string code)
        {
            var message = $"【{_setting.Sign}】您的验证码是{code}。如非本人操作，请忽略本短信";
            return await Send(mobile, message);
        }

        public async Task<Result<SmsBatchSend>> SendMulti(string[] mobiles, string message)
        {
            return await _yunPianService.MultiSendAsync(string.Join(",", mobiles), message);
        }
    }
}
