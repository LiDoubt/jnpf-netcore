using JNPF.Dependency;

namespace JNPF.Expand.Thirdparty.Sms.Model
{
    [SuppressSniffer]
    public class SmsMessageModel
    {
        public string RequestId { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
