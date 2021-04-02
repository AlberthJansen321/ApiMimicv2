using ApiMimicv2.V1.Models;
using ApiMimicv2.V1.Models.DTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMimicv2.Herlpers
{
    public class DTOMapperProfile:Profile
    {
        public DTOMapperProfile()
        {
            CreateMap<Palavra, PalavraDTO>();
            CreateMap<PaginationList<Palavra>,PaginationList<PalavraDTO>>();
        }
    }
}
