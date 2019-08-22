using System;

namespace Common
{
    public class Flight
    {
        public Flight()
        {

        }

        public Flight(
            int id,
            string departing,
            string arriving,
            long scheduled,
            long revised)
        {
            FlightNo = id;
            Departing = departing;
            Arriving = arriving;
            Scheduled = scheduled;
            Revised = revised;
        }

        public int FlightNo { get; set; }
        public string Departing { get; set; }
        public string Arriving { get; set; }
        public string TailNumber { get; set; }
        public long Scheduled { get; set; }
        public long Revised { get; set; }
        public DateTimeOffset ScheduledDate
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(this.Scheduled);
            }
        }
        public DateTimeOffset RevisedDate
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(this.Revised);
            }
        }

    }
}
