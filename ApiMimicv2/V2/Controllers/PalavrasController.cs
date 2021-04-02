using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMimicv2.V2.Controllers
{
    //api/v2.0/palavras
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
#pragma warning disable CS1591 // O comentário XML ausente não foi encontrado para o tipo ou membro visível publicamente
    public class PalavrasController : ControllerBase
#pragma warning restore CS1591 // O comentário XML ausente não foi encontrado para o tipo ou membro visível publicamente
    {
        /// <summary>
        /// Operação que traz todas as palavras do banco de dados
        /// </summary>
        /// <returns>Version 2.0</returns>
        [MapToApiVersion("2.0")]
        [HttpGet("", Name = "gettodasOutra")]
        public string Get()
        {
            return "Version 2.0";
        }
    }
}
