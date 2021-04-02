using ApiMimicv2.DataBase;
using ApiMimicv2.Herlpers;
using ApiMimicv2.V1.Models;
using ApiMimicv2.V1.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMimicv2.V1.Repositories
{
    public class PalavraRepository : IPalavraRepository
    {

        private readonly MimicContext _banco;

        public PalavraRepository(MimicContext banco)
        {
            _banco = banco;
        }

        public PaginationList<Palavra> get(QueryPaginacao query)
        {
            
                var item = _banco.Palavras.AsNoTracking().AsQueryable();

                var lista = new PaginationList<Palavra>();

                if (query.Data.HasValue)
                {
                    item = item.Where(C => C.Criado > query.Data.Value || C.Atualizado > query.Data.Value);
                }

                if (query.Pagnumero.HasValue)
                {
                    var quantidadeRegistros = item.Count();
                    item = item.Skip((query.Pagnumero.Value - 1) * query.Pagregistro.Value).Take(query.Pagregistro.Value);

                    Paginacao paginacao = new Paginacao();
                    paginacao.NumeroPagina = query.Pagnumero.Value;
                    paginacao.Registroporpagina = query.Pagregistro.Value;
                    paginacao.TotalRegistros = quantidadeRegistros;
                    paginacao.TotalPaginas = (int)Math.Ceiling((double)quantidadeRegistros / query.Pagregistro.Value);
                    lista.Paginacao = paginacao;
                }

                lista.Results.AddRange(item.ToList());

                return lista;
           
        }
        public Palavra Obter(int id)
        {
            return _banco.Palavras.AsNoTracking().FirstOrDefault(P => P.Id == id);
        }
        public void Cadastrar(Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
        }
        public void Atualizar(Palavra palavra)
        {
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

        }
        public void Deletar(int id)
        {
            var palavra = Obter(id);
            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }
    }
}


