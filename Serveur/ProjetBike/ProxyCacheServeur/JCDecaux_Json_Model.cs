using System.Collections.Generic;
using System.Runtime.Serialization;

//Edition -> collage spécial -> coller le code JSON en tant que classe (en ayant, au préalable, copier coller le code JSON correspondant au sein de la classe

namespace ProxyCacheServeur
{
    public class JCDecaux_Json_Model
    {

        [DataContract]
        public class BikeStation
        {
            public int number { get; set; }
            [DataMember] public string contract_name { get; set; }
            [DataMember] public string name { get; set; }
            [DataMember] public string address { get; set; }
            [DataMember] public Position position { get; set; }
            public bool banking { get; set; }
            public string status { get; set; }
            [DataMember] public int available_bikes { get; set; }
            [DataMember] public int available_bike_stands { get; set; }

            // add a getAwaiter() method to the class

        }

        [DataContract]
        public class Position
        {
            [DataMember] public float lat { get; set; }
            [DataMember] public float lng { get; set; }
        }

        [DataContract]
        public class Contract
        {
            [DataMember] public string name { get; set; }
            [DataMember] public string commercial_name { get; set; }
            [DataMember] public List<string> cities { get; set; }
            [DataMember] public string country_code { get; set; }

        }

    }
}

