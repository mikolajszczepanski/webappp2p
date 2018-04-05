using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Keys;
using WebAppP2P.Core.Messages;

namespace WebAppP2P.Core.Tests.Helpers
{
    public class MessageHelper
    {
        public static EncryptedMessage GetTestEncryptedMessage(string title = "tit", string content = "content123")
        {
            var keyPairTest = new KeysPair()
            {
                PublicKey = Convert.FromBase64String("BgIAAACkAABSU0ExAAgAAAEAAQDFz6K8OaO4J/HZYxt1Zi73N95LAvgR+7lMmBW/CpOC0FSswtAaNf+He1P8iVxwqsGQPEZIV6wuqRk29fUycUPrHBM6j3plKtE9Y6IMBoX4g/mu44XXChMbaDkl5EUcX+SY/8DzTrsqnghg7tpI0TffHh5fO6/snBi1HxqGH/OTsvu3J/vP86FRGBMzxPSQcq0sx66HBmQUC7i4WPJqnsRITiBu4z0vZr1rSYLr26OivEmwpHKgos78rbn+I3PjWe2CtNydhvJ2Qd4Lq09/dK72uWwfR9zc8J2LGdOVxAovGyva+EW/lQNa//dnTz6tOrla+H1v3tFEh0X89wz9zx/P"),
                PrivateKey = Convert.FromBase64String("BwIAAACkAABSU0EyAAgAAAEAAQDFz6K8OaO4J/HZYxt1Zi73N95LAvgR+7lMmBW/CpOC0FSswtAaNf+He1P8iVxwqsGQPEZIV6wuqRk29fUycUPrHBM6j3plKtE9Y6IMBoX4g/mu44XXChMbaDkl5EUcX+SY/8DzTrsqnghg7tpI0TffHh5fO6/snBi1HxqGH/OTsvu3J/vP86FRGBMzxPSQcq0sx66HBmQUC7i4WPJqnsRITiBu4z0vZr1rSYLr26OivEmwpHKgos78rbn+I3PjWe2CtNydhvJ2Qd4Lq09/dK72uWwfR9zc8J2LGdOVxAovGyva+EW/lQNa//dnTz6tOrla+H1v3tFEh0X89wz9zx/PTwh97VHN4SpiUwqQ63PC8Yw1kUKiWb5AxS2wfQ0BwrP0rFRtTbb1AAJyuUaYFiuj7prXxL+tC3iLr4aCVx59HtprbZU49ovWWuDiwYCjmdXGlN4LEAzf010z8eW23JuiJ2f0nWMbqQYYiC6sZA11ua9gA0PWQgF+Z3Wuu9Yyxd6rzVm5BqvcfkFy1FVeMv1mVBmHENZNiF6kXy+dlkWTwxXnFpLC7foqcrzLYvUR8s7044wmilyglQuhuAIl+QPxAl/jEm/chfB4gK+QJiJ2ehSqnFftmy2gVQ84IUGZQjj6q+BHPgVIwpeXlfPxV7q2HTDullOdF7cefTO3xiUF7iFtIyHfcDWskYu6SADj3CkLy8Duzq2O6SNt1Rj310zqHBXHgfTUVkrkU9MR5wbKw9gbLJnjtNsf+ajAKTdqORx1F7osMG9vGIGXkntUFL6VGOcz9DI/VkhsU5DPVUTUaAna9/hrSCwOzFrMJs5e2Xn9sK1fzPXsoV1vTJPt7pxSr8s8vm8r3r/2LyYcEJtXMXhArvgMvxjli6Y/SpwE4Td9RZnEu5gdmjvpAlRhB+wL+cFdjw2QHMyOG6cUwPfQbWsDXkA4OGGpKlDG+IF93WhoRGhqZ/aFivEN8Re9y7AiYP2XIVhsWAx5jQmPib+HQ6UCSyD9GnbE+sjD3Hql7rk5gzeJWOUKc9IcRPz/JQFMv9sNw6yf3tDPUtuhn/308u8O3l9aafihyEI616oZD36PNqq811JXnKmuNOALrIUSeeopTcwB+FvfwrWshaY2ss62fvucYFhJwZg3XuV8KDoCqGz5Srivmr/oC5RqKFKyGZ8CAjwmrGuKmI3EaBh92blnANSi4d2o00WBjpPE1xhqdFHFprc1mkootEzmg55x0t7nMGpA6HhVxtiyHvswRVtHY25+IJD8RyClYevjDcEbrtRmsbxKmnlitFAoVaP5VdlsW1+QZMJxDiqKtfVhjBITUev3qCzREeMhkAuKR2gyGqALXxAZRpU9Mx3hyccKO7dzmo1BjQJ01EX32UaS0ZZp1xikQ807BfjdPXUsdsDR6KdPvpCJLotccKe1xMIO4ExN1MVxXza+XR94/PpEuUbeTvN00DMiAFtCEDGz/x9kmAUaHxd6tZh8iFXaxRqDBOOoHgzv7wMtwTt4svlAWapCtWEi1epx8uvW/PQ8YIU=")
            };
            var toPublicKeyBase64 = "BgIAAACkAABSU0ExAAgAAAEAAQAF7p/n+6T+dUoRA0V+SQZBtK9C7KZYfFFM/ASeiSVk8zp6Y84OQ115boZcfLL5K/ySYcuvWs6PdjJoGc/y3fDt2ojEdiF33g0AlPkrI+3QjM6V0xtcIqsx/2itlwb8qLvMqsATIJ7wzQiRvta+jqpTtP7Y28FsFgag5djj+oFgcU5l2tbx83uHYncueLehjJYMtNK8T1WqRuEpBwWuDA6q+XX8sX+XdhVQ8EzJco+Rme5rXNTGqrTuFW9rFmzQwuDIwKPhoTpE6sJ4rsv4/LtM2lT6GjQg0PlIwJf48eYra4h+0KnEfK5+a/HmdLCk36I/LenvpHD7NVun0kOp9/mq";
            var toPrivateKeyBase64 = "BwIAAACkAABSU0EyAAgAAAEAAQAF7p/n+6T+dUoRA0V+SQZBtK9C7KZYfFFM/ASeiSVk8zp6Y84OQ115boZcfLL5K/ySYcuvWs6PdjJoGc/y3fDt2ojEdiF33g0AlPkrI+3QjM6V0xtcIqsx/2itlwb8qLvMqsATIJ7wzQiRvta+jqpTtP7Y28FsFgag5djj+oFgcU5l2tbx83uHYncueLehjJYMtNK8T1WqRuEpBwWuDA6q+XX8sX+XdhVQ8EzJco+Rme5rXNTGqrTuFW9rFmzQwuDIwKPhoTpE6sJ4rsv4/LtM2lT6GjQg0PlIwJf48eYra4h+0KnEfK5+a/HmdLCk36I/LenvpHD7NVun0kOp9/mqDxurj+u8E13h7bDdR14+nGD+A/wCAvxPMrhOnSsdR7FAZt+DKCS4M8AFtPePWtWh1MAO/hWRMVJ/+UwwpUOkO852CopqnqdaM0ixrLE+PIsgZhOoHVcELEbFis6oj5ivDMe2RGlkpUcaF2PzAT73KL0UmfCSqOudo1asHJDCiMirdZlflJboNTihNfhSx6pZ06q1lAnGWQG/mbIGgelb/qq6Tbosky4tnf2hhJrhI9QgvycQ6+JWTf/9S1Ovk0ww6vO2E9NeLJ92nTRk+tTF6v9YDImz5UFcNrWFLfFGaQXO5iyVC4HmMDglNj/FiQlPrG1HWU+6tREjrW0/tE5E2h0mXKa42wZKc6LGUvuOKWOoExsDzlW8KyWTXyfdJXvLhYsbBDzrc1h023G4MnY4fN8j5F7D8fhrPP9IhL0QZLvQUBXwl4h6IL+OC0tCATEm1xIZZXF0pkdJFevxwg6qgJ72UF4wqIwdoZgxuPdJaDATwVK4ymS/8ueNIE9mzKucEa1W79wO3HMVle5OUDlwOTzNmKwtk2zObHf1vbZPaEHAo9OeRo49iBX4RkldVoOSmTpiySN5tDyD0ftE3JnwYEGfzbCkaCmT36IG8HuI0e8qNInJGDG+pKMqqGXbpDZLfxkF7KesLmnQPimW0l3ih+9UpVRUPG4II+HhczxRRSk2jwtZdsT1hXpK0Gt7DKLc48rCxEEOsB9QjtfzBNL45Q9z4hC37/Qefz/pIqGLy7REqKqu9BNH1Rv7lUzoxj+0k6x33zgMJrDPktS29NQz1qfzyI3LDkVY2o+ttOQQNVJoomeIwQZGjy7YoG2ZMVWkJFJTHU3OE+J+xerhe9iusZk2ljFp2fAgYUl7ZAUM2q0sBkbhfHHwl9kDr9fuM5yoNTidJMJawmwVew/nAzzZLBptRi9fFqBpfuYPa2bfCSjFAelOnQTLlEqAb++PpAgALR/DJELSyOs90+Xry8vzF2WfvqOnwhI8tyeWhaCwETujoeCatJ9W20bSjGhmTU/pnMi4W8ow6kM+2yQ5ultj4ewQQ+B1NzHUR4mIOXBIQlgKSTlEPkJkhWL2EJVpLECmZSdmhXscp52IpQRYeXBUhwJXbX8QEO42Tf7MPfb97kWjzWZ2BdxVOAUyEJb67Vi3ZSX4f8qlZP5CvfcnJc4pqPPBYEKNT++90vPEBMra1Fg=";

            var mb = new EncryptedMessageBuilder(new HashCash());
            var msg = mb.AddContent(content)
                .AddTitle(title)
                .AddReciever(toPublicKeyBase64)
                .AddSender(Convert.ToBase64String(keyPairTest.PublicKey))
                .EncryptAndBuild(Convert.ToBase64String(keyPairTest.PrivateKey));
            return msg;
        }

        public static string GetInvalidData()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes("INVALID"));
        }
    }
}
