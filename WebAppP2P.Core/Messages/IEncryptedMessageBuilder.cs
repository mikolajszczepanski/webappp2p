namespace WebAppP2P.Core.Messages
{
    public interface IEncryptedMessageBuilder
    {
        IEncryptedMessageBuilder AddContent(string content);
        IEncryptedMessageBuilder AddReciever(string recieverPublicKey);
        IEncryptedMessageBuilder AddSender(string senderPublicKey);
        IEncryptedMessageBuilder AddTitle(string title);
        EncryptedMessage EncryptAndBuild(string senderPrivateKey);
    }
}