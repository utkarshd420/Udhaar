using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udhaar
{
    class Credit
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public String Id { set; get; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public String Name { set; get; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "phone_no")]
        public String Phone_no { set; get; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "amount")]
        public double Amount { set; get; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "personal_name")]
        public String Personal_name { set; get; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "personal_number")]
        public String Personal_number { set; get; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "valid")]
        public Boolean Valid { set; get; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "_updatedAt")]
        public DateTime date { set; get; }
    }
}
