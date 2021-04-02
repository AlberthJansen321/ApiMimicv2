using ApiMimicv2.Herlpers;
using ApiMimicv2.V1.Models;
using ApiMimicv2.V1.Models.DTO;
using ApiMimicv2.V1.Repositories.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMimicv2.V1.Controllers
{
    [ApiController]
    [Route("api/v{Version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class PalavrasController : ControllerBase
    {
        public readonly IPalavraRepository _repository;
        public readonly IMapper _mapper;

        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        /// <summary>
        /// Operação que pegar do banco todas as palavras existentes.
        /// </summary>
        /// <param name="query">Filtro de Pesquisa</param>
        /// <returns>Listagem de Palavras</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [HttpGet("",Name = "ObterTodas")]
        public ActionResult get([FromQuery] QueryPaginacao query)
        {

           
                var item = _repository.get(query);


                if (item.Results.Count == 0)
                    return NotFound();

                PaginationList<PalavraDTO> lista = CriarLinksListPalavraDTO(query, item);

                return Ok(lista);
          

        }

        /// <summary>
        /// Operação que pegar uma unica palavra do banco de dados.
        /// </summary>
        /// <param name="id">Código Identificador</param>
        /// <returns>Um Objeto Palavra</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [HttpGet("{id}", Name = "IdObterPalavra")]
        public ActionResult Obter(int id)
        {
            var obj = _repository.Obter(id);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(obj);

            palavraDTO.Links.Add(new LinkDTO("self", Url.Link("IdObterPalavra",new { id = palavraDTO.Id}), "GET"));
            palavraDTO.Links.Add(new LinkDTO("update", Url.Link("IdAtualizarPalavra", new { id = palavraDTO.Id }), "PUT"));
            palavraDTO.Links.Add(new LinkDTO("delete", Url.Link("IdDeletarPalavra", new { id = palavraDTO.Id }), "DELETE"));

            if (palavraDTO == null)
                return NotFound();
            return Ok(palavraDTO);

        }
        /// <summary>
        /// Operação que realizar o cadastro da palavra.
        /// </summary>
        /// <param name="palavra">Um objeto Palavra</param>
        /// <returns>Objeto Palavra Cadastro</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody] Palavra palavra)
        {
            if (palavra == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            palavra.Ativo = true;
            palavra.Criado = DateTime.Now;
           
            _repository.Cadastrar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);
            palavraDTO.Links.Add(new LinkDTO("self", Url.Link("IdObterPalavra", new { id = palavraDTO.Id }), "GET"));

            return Created($"api/palavras/{palavraDTO.Id}", palavraDTO);
        }
        /// <summary>
        /// Operação que Atualizar um objeto palavra.
        /// </summary>
        /// <param name="id">Código Identificador</param>
        /// <param name="palavra">Objeto Palavra</param>
        /// <returns>Objeto Atualizado</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [HttpPut("{id}",Name = "IdAtualizarPalavra")]
        public ActionResult Alterar(int id, [FromBody] Palavra palavra)
        {
            if (palavra == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var obj = _repository.Obter(id);

            if (obj == null)
                return NotFound();

            palavra.Id = id;
            palavra.Criado = obj.Criado;
            palavra.Ativo = obj.Ativo;
            palavra.Atualizado = DateTime.Now;

            _repository.Atualizar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);
            palavraDTO.Links.Add(new LinkDTO("self", Url.Link("IdObterPalavra", new { id = palavraDTO.Id }), "GET"));

            return Ok(palavraDTO);
        }
        /// <summary>
        /// Desativar um objeto palavra do banco de dados.
        /// </summary>
        /// <param name="id">Código Idenficador</param>
        /// <returns></returns>
        [MapToApiVersion("1.1")]
        [HttpDelete("{id}",Name = "IdDeletarPalavra")]
        public ActionResult Deletar(int id)
        {

            var palavra = _repository.Obter(id);

            if (palavra == null)
                return NotFound();

            _repository.Deletar(id);

            return NoContent();
        }
        private PaginationList<PalavraDTO> CriarLinksListPalavraDTO(QueryPaginacao query, PaginationList<Palavra> item)
        {
            var lista = _mapper.Map<PaginationList<Palavra>, PaginationList<PalavraDTO>>(item);

            foreach (var palavraDTO in lista.Results)
            {
                palavraDTO.Links.Add(new LinkDTO("self", Url.Link("IdObterPalavra", new { id = palavraDTO.Id }), "GET"));
            }

            lista.Links.Add(new LinkDTO("self", Url.Link("ObterTodas", query), "GET"));


            if (item.Paginacao != null)
            {

                Response.Headers.Add("X-Pagination:", JsonConvert.SerializeObject(item.Paginacao));

                if (query.Pagnumero + 1 <= item.Paginacao.TotalPaginas)
                {
                    var querystring = new QueryPaginacao() { Pagnumero = query.Pagnumero + 1, Pagregistro = query.Pagregistro, Data = query.Data };
                    lista.Links.Add(new LinkDTO("next", Url.Link("ObterTodas", querystring), "GET"));
                }
                if (query.Pagnumero - 1 > 0)
                {
                    var querystring = new QueryPaginacao() { Pagnumero = query.Pagnumero - 1, Pagregistro = query.Pagregistro, Data = query.Data };
                    lista.Links.Add(new LinkDTO("prev", Url.Link("ObterTodas", querystring), "GET"));
                }
            }
            return lista;
        }
    }
}
