using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonablePeople.API.Models
{
    public class BadRequestOutDto
    {
        public BadRequestOutDto(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
