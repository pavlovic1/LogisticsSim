using System;

namespace LogisticsSystem
{
    public class Package
    {
        private static int nextId = 1;
        
        public int Id { get; }
        public string SourceStation { get; }
        public string DestinationStation { get; }
        public bool IsDelivered { get; set; }

        public Package(string sourceStation, string destinationStation)
        {
            Id = nextId++;
            SourceStation = sourceStation;
            DestinationStation = destinationStation;
            IsDelivered = false;
        }

        public override string ToString()
        {
            if (IsDelivered)
            {
                return $"Balík #{Id} -> {DestinationStation} (Doručeno)";
            }
            else
            {
                return $"Balík #{Id} -> {DestinationStation}";
            }
        }
    }
} 