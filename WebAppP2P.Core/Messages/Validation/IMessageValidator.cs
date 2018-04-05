namespace WebAppP2P.Core.Messages
{
    public interface IMessageValidator
    {
        bool Validate(EncryptedMessage message);
    }
}