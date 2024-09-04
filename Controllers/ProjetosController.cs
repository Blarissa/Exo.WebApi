using Exo.WebApi.Models;
using Exo.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Exo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjetosController : ControllerBase
    {
        private readonly ProjetoRepository _projetoRepository;
        
        public ProjetosController(ProjetoRepository projetoRepository)
        {
            _projetoRepository = projetoRepository;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            return Ok(_projetoRepository.Listar());
        }

        [HttpPost]
        public IActionResult Cadastrar(Projeto projeto)
        {
            _projetoRepository.Cadastrar(projeto);
            return StatusCode(201);
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var projetoBuscado = _projetoRepository.BuscarPorId(id);

            if (projetoBuscado == null)
            {
                return NotFound();
            }

            return Ok(projetoBuscado);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Projeto projeto)
        {
            var projetoBuscado = _projetoRepository.BuscarPorId(id);

            if (projetoBuscado == null)
            {
                return NotFound();
            }

            _projetoRepository.Atualizar(id, projeto);
            return StatusCode(204);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var projetoBuscado = _projetoRepository.BuscarPorId(id);

            try 
            {
                _projetoRepository.Deletar(id);
                return StatusCode(204);
            } catch (Exception e) {
                return BadRequest(e);
            }            
        }
    }
}
