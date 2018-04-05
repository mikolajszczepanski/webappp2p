using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Keys;
using WebAppP2P.Core.Messages;
using WepAppP2P.Core.Tests.Model.Helpers;
using Xunit;

namespace WepAppP2P.Core.Tests.Messages
{
    public class EncryptedMessageBuilderTests
    {

        public static IEnumerable<object[]> GetValidKeys()
        {
            return KeysLoaderHelper.GetValidKeys();
        }

        [Theory]
        [MemberData(nameof(GetValidKeys))]
        public void EncryptedMessageBuilder_Build_Should_Return_Encrypted_Message(KeysPair keyPairTest)
        {
            var title = "tit";
            var content = "content123";

            var toPublicKeyBase64 = "BgIAAACkAABSU0ExAAgAAAEAAQAF7p/n+6T+dUoRA0V+SQZBtK9C7KZYfFFM/ASeiSVk8zp6Y84OQ115boZcfLL5K/ySYcuvWs6PdjJoGc/y3fDt2ojEdiF33g0AlPkrI+3QjM6V0xtcIqsx/2itlwb8qLvMqsATIJ7wzQiRvta+jqpTtP7Y28FsFgag5djj+oFgcU5l2tbx83uHYncueLehjJYMtNK8T1WqRuEpBwWuDA6q+XX8sX+XdhVQ8EzJco+Rme5rXNTGqrTuFW9rFmzQwuDIwKPhoTpE6sJ4rsv4/LtM2lT6GjQg0PlIwJf48eYra4h+0KnEfK5+a/HmdLCk36I/LenvpHD7NVun0kOp9/mq";
            var toPrivateKeyBase64 = "BwIAAACkAABSU0EyAAgAAAEAAQAF7p/n+6T+dUoRA0V+SQZBtK9C7KZYfFFM/ASeiSVk8zp6Y84OQ115boZcfLL5K/ySYcuvWs6PdjJoGc/y3fDt2ojEdiF33g0AlPkrI+3QjM6V0xtcIqsx/2itlwb8qLvMqsATIJ7wzQiRvta+jqpTtP7Y28FsFgag5djj+oFgcU5l2tbx83uHYncueLehjJYMtNK8T1WqRuEpBwWuDA6q+XX8sX+XdhVQ8EzJco+Rme5rXNTGqrTuFW9rFmzQwuDIwKPhoTpE6sJ4rsv4/LtM2lT6GjQg0PlIwJf48eYra4h+0KnEfK5+a/HmdLCk36I/LenvpHD7NVun0kOp9/mqDxurj+u8E13h7bDdR14+nGD+A/wCAvxPMrhOnSsdR7FAZt+DKCS4M8AFtPePWtWh1MAO/hWRMVJ/+UwwpUOkO852CopqnqdaM0ixrLE+PIsgZhOoHVcELEbFis6oj5ivDMe2RGlkpUcaF2PzAT73KL0UmfCSqOudo1asHJDCiMirdZlflJboNTihNfhSx6pZ06q1lAnGWQG/mbIGgelb/qq6Tbosky4tnf2hhJrhI9QgvycQ6+JWTf/9S1Ovk0ww6vO2E9NeLJ92nTRk+tTF6v9YDImz5UFcNrWFLfFGaQXO5iyVC4HmMDglNj/FiQlPrG1HWU+6tREjrW0/tE5E2h0mXKa42wZKc6LGUvuOKWOoExsDzlW8KyWTXyfdJXvLhYsbBDzrc1h023G4MnY4fN8j5F7D8fhrPP9IhL0QZLvQUBXwl4h6IL+OC0tCATEm1xIZZXF0pkdJFevxwg6qgJ72UF4wqIwdoZgxuPdJaDATwVK4ymS/8ueNIE9mzKucEa1W79wO3HMVle5OUDlwOTzNmKwtk2zObHf1vbZPaEHAo9OeRo49iBX4RkldVoOSmTpiySN5tDyD0ftE3JnwYEGfzbCkaCmT36IG8HuI0e8qNInJGDG+pKMqqGXbpDZLfxkF7KesLmnQPimW0l3ih+9UpVRUPG4II+HhczxRRSk2jwtZdsT1hXpK0Gt7DKLc48rCxEEOsB9QjtfzBNL45Q9z4hC37/Qefz/pIqGLy7REqKqu9BNH1Rv7lUzoxj+0k6x33zgMJrDPktS29NQz1qfzyI3LDkVY2o+ttOQQNVJoomeIwQZGjy7YoG2ZMVWkJFJTHU3OE+J+xerhe9iusZk2ljFp2fAgYUl7ZAUM2q0sBkbhfHHwl9kDr9fuM5yoNTidJMJawmwVew/nAzzZLBptRi9fFqBpfuYPa2bfCSjFAelOnQTLlEqAb++PpAgALR/DJELSyOs90+Xry8vzF2WfvqOnwhI8tyeWhaCwETujoeCatJ9W20bSjGhmTU/pnMi4W8ow6kM+2yQ5ultj4ewQQ+B1NzHUR4mIOXBIQlgKSTlEPkJkhWL2EJVpLECmZSdmhXscp52IpQRYeXBUhwJXbX8QEO42Tf7MPfb97kWjzWZ2BdxVOAUyEJb67Vi3ZSX4f8qlZP5CvfcnJc4pqPPBYEKNT++90vPEBMra1Fg=";

            var mb = new EncryptedMessageBuilder(new HashCash());
            var msg = mb.AddContent(content)
                .AddTitle(title)
                .AddReciever(toPublicKeyBase64)
                .AddSender(Convert.ToBase64String(keyPairTest.PublicKey))
                .EncryptAndBuild(Convert.ToBase64String(keyPairTest.PrivateKey));

            Assert.True(!string.IsNullOrEmpty(msg.Id));
            Assert.True(!string.IsNullOrEmpty(msg.Title));
            Assert.True(title != msg.Title);
            Assert.True(!string.IsNullOrEmpty(msg.Content));
            Assert.True(content != msg.Content);
            Assert.True(toPublicKeyBase64 == msg.To);
            Assert.True(msg.Timestamp > 0);

        }

    }
}
