using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAppP2P.Dto;
using WebAppP2P.Core.Keys;
using WebAppP2P.Dto.Api;

namespace WebAppP2P.Api
{
    public class KeysApi : Controller
    {
        private readonly IKeysValidator _keysValidator;
        private readonly IKeysFactory _keysFactory;

        public KeysApi(IKeysValidator keysValidator, IKeysFactory keysFactory)
        {
            _keysValidator = keysValidator;
            _keysFactory = keysFactory;
        }

        [HttpGet("api/keys")]
        public IActionResult GenerateKeys()
        {
            var newKeys = _keysFactory.GenerateNewPair();
            return Ok(new KeysPairDto()
            {
                PublicKey = Convert.ToBase64String(newKeys.PublicKey),
                PrivateKey = Convert.ToBase64String(newKeys.PrivateKey)
            });
        }

        [HttpPost("api/keys/check")]
        public IActionResult CheckKeys([FromBody] KeysPairDto keys)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            KeysPair keysPair = new KeysPair();
            try
            {
                keysPair.PrivateKey = Convert.FromBase64String(keys.PrivateKey);
                keysPair.PublicKey = Convert.FromBase64String(keys.PublicKey);
            }
            catch(FormatException ex)
            {
                return BadRequest("Public or private key format is invalid.");
            }
            return Ok(_keysValidator.VerifyKeys(keysPair));
        }
    }
}
