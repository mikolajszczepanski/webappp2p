using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace WebAppP2P.Tests
{
    public class UtilsTests
    {
        [Fact]
        public void UrlDecodeWithBase64_Should_Return_Valid_Url()
        {
            var url = "http://localhost:62216/api/messages/CW7acM6J2N6F+qestK5l0FUsD5qx9TSyCOUbw7SJPIT+NzaxS4pUlKLYrYGO+aDGF9CPb/bus376uTaW58oz3uCNYUbXOy7kdhr9kXxCPvkMb/MpZhV/EOAGwC/se6PWPqt44BoKt65QQwO8iNpFSxIy/cANYNwwp2A+NFMMVX3YvrriOkqBHPvCgaYptGNNjy39jw2rhrAnlCfG1DE0KcQ1bbCDq3YXu5B/kqyYyjscwOSqOxu7Zfth/BCACwaqjN8CdBSXqr66m7FyNAmbqH5KTYcdTysXvG5Pu68tpMpOZ9n2haetRFsf2Y/EJ0C5E29rI2eHVnOYb8+NBZEdgQ==";
            var test = Uri.EscapeDataString(url);

            var result = UrlUtils.UrlDecodeWithBase64(test);

            Assert.True(result == url);
        }
    }
}
