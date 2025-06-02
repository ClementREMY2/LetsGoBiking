using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RoutingServer
{


    [DataContract]
    public class Itineraries
    {
        [DataMember(Name = "itineraries")] public List<Itinerary> ItineraryList { get; set; }
    }

    [DataContract]
    public class Itinerary
    {
        [DataMember(Name = "segments")] public List<Segment> Segments { get; set; }
        [DataMember(Name = "geometry")] public Geometry Geometry { get; set; }


        
    }

    [DataContract]
    public class Geometry
    {
        [DataMember(Name = "coordinates")]
        public List<double[]> Coordinates { get; set; } 

        
    }

    [DataContract]
    public class Segment
    {
        [DataMember(Name = "steps")] public List<Step> Steps { get; set; }

        [DataMember(Name = "distance")] public double Distance { get; set; }

        [DataMember(Name = "duration")] public double Duration { get; set; }

       
    }

    [DataContract]
    public class Step
    {
        [DataMember(Name = "distance")] public double Distance { get; set; }

        [DataMember(Name = "duration")] public double Duration { get; set; }

        [DataMember(Name = "instruction")] public string Instruction { get; set; }

    }
}