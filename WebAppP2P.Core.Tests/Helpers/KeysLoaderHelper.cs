using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using WebAppP2P.Core.Keys;

namespace WepAppP2P.Core.Tests.Model.Helpers
{
    public class KeysLoaderHelper
    {
        public static IEnumerable<object[]> GetValidKeys()
        {
            var klh = new KeysLoaderHelper();
            foreach (var pairKey in klh.LoadFromFile(Path.Combine("Helpers","ValidKeys.xml")))
            {
                yield return new object[] { pairKey };
            }
        }

        public static IEnumerable<object[]> GetInvalidKeys()
        {
            var klh = new KeysLoaderHelper();
            foreach (var pairKey in klh.LoadFromFile(Path.Combine("Helpers","InvalidKeys.xml")))
            {
                yield return new object[] { pairKey };
            }
        }

        public IList<KeysPair> LoadFromFile(string filename)
        {
            var xmlDocument = new XmlDocument();
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),filename);
            xmlDocument.Load(path);
            var rootNode = xmlDocument.DocumentElement.SelectNodes("/keysPairs/keyPair");
            var keysPairsList = new List<KeysPair>();
            foreach (XmlNode node in rootNode)
            {
                var publicKey = Convert.FromBase64String(node.SelectSingleNode("publicKey").InnerText);
                var privateKey = Convert.FromBase64String(node.SelectSingleNode("privateKey").InnerText);
                keysPairsList.Add(new KeysPair()
                {
                    PublicKey = publicKey,
                    PrivateKey = privateKey
                });
            }
            return keysPairsList;
        }
    }
}
