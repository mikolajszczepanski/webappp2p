namespace WebAppP2P.Core.Keys
{
    public interface IKeysFactory
    {
        KeysPair GenerateNewPair();
    }
}