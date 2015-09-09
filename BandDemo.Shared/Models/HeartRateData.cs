using System;

namespace BandDemo.Models
{
    public class HeartRateData
    {

        public int Id { get; set; }

        public int Value { get; set; }

        public DateTime CreatedAt { get; set; }

        public String CreatedAtStr { get; set; }
    }
}