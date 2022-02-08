using System;
using System.Collections.Generic;
using System.Text;

namespace BotCore.Dto
{
    public class ProxyResponseDto
    {
        public class Security
        {
            public bool is_vpn { get; set; }
        }

        public class Timezone
        {
            public string name { get; set; }
            public string abbreviation { get; set; }
            public int gmt_offset { get; set; }
            public string current_time { get; set; }
            public bool is_dst { get; set; }
        }

        public class Flag
        {
            public string emoji { get; set; }
            public string unicode { get; set; }
            public string png { get; set; }
            public string svg { get; set; }
        }

        public class Currency
        {
            public string currency_name { get; set; }
            public string currency_code { get; set; }
        }

        public class Connection
        {
            public int autonomous_system_number { get; set; }
            public string autonomous_system_organization { get; set; }
            public string connection_type { get; set; }
            public string isp_name { get; set; }
            public string organization_name { get; set; }
        }

        public class Root
        {
            public string ip_address { get; set; }
            public string city { get; set; }
            public int city_geoname_id { get; set; }
            public string region { get; set; }
            public string region_iso_code { get; set; }
            public int region_geoname_id { get; set; }
            public string postal_code { get; set; }
            public string country { get; set; }
            public string country_code { get; set; }
            public int country_geoname_id { get; set; }
            public bool country_is_eu { get; set; }
            public string continent { get; set; }
            public string continent_code { get; set; }
            public int continent_geoname_id { get; set; }
            public double longitude { get; set; }
            public double latitude { get; set; }
            public Security security { get; set; }
            public Timezone timezone { get; set; }
            public Flag flag { get; set; }
            public Currency currency { get; set; }
            public Connection connection { get; set; }
        }
    }
}
