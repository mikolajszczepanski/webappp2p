namespace WebAppP2P.Core.Messages
{
    public interface IHashCash
    {
        ulong GetNonce(string data);
        bool Validate(string data, ulong nonce);
    }
}