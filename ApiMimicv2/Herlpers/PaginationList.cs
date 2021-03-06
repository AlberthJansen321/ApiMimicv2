using ApiMimicv2.V1.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMimicv2.Herlpers
{
    public class PaginationList<T>
    {
        public List<T> Results { get; set; } = new List<T>();
        public Paginacao Paginacao { get; set; }
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }
}
