using Exo.WebApi.Models;
using Exo.WebApi.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Exo.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepository;
        
        public UsuariosController(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
        
        // get -> /api/usuarios
        [HttpGet]
        public IActionResult Listar()
        {
            return Ok(_usuarioRepository.Listar());
        }
        // post -> /api/usuarios
        // [HttpPost]
        // public IActionResult Cadastrar(Usuario usuario)
        // {
        //     _usuarioRepository.Cadastrar(usuario);
        //     return StatusCode(201);
        // }

        // Novo método de cadastro de usuário
        [HttpPost]
        public IActionResult Cadastrar(Usuario usuario)
        {
            var usuarioBuscado = _usuarioRepository.Login(usuario.Email, usuario.Senha);

            if (usuarioBuscado == null)            
                return NotFound("E-mail ou senha inválidos!");
            
            // Se o e-mail e senha estiverem corretos, gera o token
            // Define os dados que serão fornecidos no token - Payload
            var claims = new[]
            {
                // Armazena na claim o e-mail e o id do usuário autenticado
                new Claim(JwtRegisteredClaimNames.Email, usuarioBuscado.Email),
                new Claim(JwtRegisteredClaimNames.Jti, usuarioBuscado.Id.ToString())
            };

            // Define a chave de acesso ao token
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("exoapi-chave-autenticacao"));

            // Define as credenciais do token - Header
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Gera o token
            var token = new JwtSecurityToken(
                issuer: "exo.webapi",                // emissor do token
                audience: "exo.webapi",              // destinatário do token
                claims: claims,                      // dados definidos acima
                expires: DateTime.Now.AddMinutes(30),// tempo de expiração
                signingCredentials: creds            // credenciais do token
            );

            // Retorna o token gerado
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        // get -> /api/usuarios/{id}
        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var usuarioBuscado = _usuarioRepository.BuscarPorId(id);
            
            if (usuarioBuscado == null)            
                return NotFound();            
            
            return Ok(usuarioBuscado);
        }

        // put -> /api/usuarios/{id}
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Usuario usuario)
        {            
            _usuarioRepository.Atualizar(id, usuario);
            return StatusCode(204);
        }

        // delete -> /api/usuarios/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            try
            {
                _usuarioRepository.Deletar(id);
                return StatusCode(204);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
            
}