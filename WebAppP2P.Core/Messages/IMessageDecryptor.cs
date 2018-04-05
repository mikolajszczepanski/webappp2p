namespace WebAppP2P.Core.Messages
{
    public interface IMessageDecryptor
    {
        DecryptedMessageDto Decrypt(EncryptedMessage message, string privateKey, bool privateKeyIsReciver);
    }
}