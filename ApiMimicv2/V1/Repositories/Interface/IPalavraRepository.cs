using ApiMimicv2.Herlpers;
using ApiMimicv2.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMimicv2.V1.Repositories.Interface
{
    public interface IPalavraRepository
    {
        PaginationList<Palavra> get(QueryPaginacao query);
        Palavra Obter(int id);
        void Cadastrar(Palavra palavra);
        void Atualizar(Palavra palavra);
        void Deletar(int id);

    }
}
