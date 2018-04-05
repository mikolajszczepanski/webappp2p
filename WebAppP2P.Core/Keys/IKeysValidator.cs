namespace WebAppP2P.Core.Keys
{
    public interface IKeysValidator
    {
        bool VerifyKeys(KeysPair keys);
    }
}