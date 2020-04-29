using Capcha.Models;
using Capcha.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Capcha.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagemController : ControllerBase
    {
        private readonly IImagemServie _ImagemServie;
        public ImagemController(IImagemServie imagemServie)
        {
            _ImagemServie = imagemServie;
        }

        
        [Route("Texto")]
        [HttpPost]
        public IActionResult Texto(Arquivo arquivo)
        {
            try
            {
                return Ok(_ImagemServie.ExtrairTexto(arquivo.Base64));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }


    }
}