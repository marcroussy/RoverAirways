using System;

namespace Common
{
    public class Flight
    {
        public Flight(
            int id, 
            string departing, 
            string arriving, 
            string equipment)
        {
            Id = id;
            Departing = departing;
            Arriving = arriving;
            Equipment = equipment;
        }

        public int Id { get; set; }
        public string Departing { get; set; }
        public string Arriving { get; set; }
        public string Equipment { get; set; }
        public DateTimeOffset Scheduled { get; set; }
        public DateTimeOffset Revised { get; set; }
    }
}
