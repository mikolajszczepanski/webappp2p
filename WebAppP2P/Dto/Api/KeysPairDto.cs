using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppP2P.Dto.Api
{
    public class KeysPairDto
    {
        [Required]
        public string PublicKey { get; set; }
        [Required]
        public string PrivateKey { get; set; }
    }
}
