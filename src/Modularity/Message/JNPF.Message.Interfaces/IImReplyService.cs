namespace JNPF.Message.Interfaces
{
    public interface IImReplyService
    {
        /// <summary>
        /// 强制下线
        /// </summary>
        /// <param name="connectionId"></param>
        void ForcedOffline(string connectionId);
    }
}
