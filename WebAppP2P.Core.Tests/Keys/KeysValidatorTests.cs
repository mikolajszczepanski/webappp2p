using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WebAppP2P.Core.Keys;
using WepAppP2P.Core.Tests.Model.Helpers;
using Xunit;

namespace WepAppP2P.Core.Tests.Keys
{
    public class KeysValidatorTests
    {
        public static IEnumerable<object[]> GetValidKeys()
        {
            return KeysLoaderHelper.GetValidKeys();
        }


        public static IEnumerable<object[]> GetInvalidKeys()
        {
            return KeysLoaderHelper.GetInvalidKeys();
        }

        [Theory]
        [MemberData(nameof(GetValidKeys))]
        public void KeysValidator_Validate_Should_Return_True_When_Public_And_Private_Keys_Are_Valid(KeysPair keysPairTest)
        {
            var kv = new KeysValidator();

            var result = kv.VerifyKeys(keysPairTest);

            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(GetInvalidKeys))]
        public void KeysValidator_Validate_Should_Return_False_When_Public_Or_Private_Keys_Are_Invalid(KeysPair keysPairTest)
        {
            var kv = new KeysValidator();

            var result = kv.VerifyKeys(keysPairTest);

            Assert.False(result);
        }
    }
}
