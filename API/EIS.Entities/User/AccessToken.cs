using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.User
{
    public class AccessToken
    {
        public int Id { get; set; }
        public string TokenName { get; set; }
        public string DeviceName { get; set; }
        public string IPAddress { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime Expiry { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }
    }
}
