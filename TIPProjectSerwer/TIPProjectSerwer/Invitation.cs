using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIPProjectSerwer
{
    public class Invitation
    {
        public string CallerUsername { get; private set; }
        public string CallerIP { get; private set; }
        public string CalleeUsername { get; private set; }
        public string CalleeIP { get; private set; }
        public bool UserRejected { get; private set; }
        public bool UserAccepted { get; private set; }

        public Invitation(string callerUsername, string callerIP, string calleeUsername, string calleeIP)
        {
            CallerUsername = callerUsername;
            CallerIP = callerIP;
            CalleeUsername = calleeUsername;
            CalleeIP = calleeIP;

            UserRejected = false;
            UserAccepted = false;
        }

        public bool IsMe(string username, string ip)
        {
            if(username == CalleeUsername && ip == CalleeIP)
            {
                return true;
            }
            return false;
        }

        public void Accept()
        {
            UserAccepted = true;
            UserRejected = false;
        }

        public void Reject()
        {
            UserRejected = true;
            UserAccepted = true;
        }
    }
}
