using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models.DTOs
{
    //EVAL: Following standards for organising Model, DTO in seperate folder
    public class AddToInventoryRequestDto
    {
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }
    }
}
