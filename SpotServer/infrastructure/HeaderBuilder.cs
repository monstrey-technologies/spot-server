using System;
using Bosdyn.Api;
using Google.Protobuf.WellKnownTypes;

namespace SpotServer.infrastructure
{
    public class HeaderBuilder
    {
        public static ResponseHeader Build(RequestHeader requestHeader, CommonError error)
        {
            ResponseHeader responseHeader = new ResponseHeader
            {
                RequestHeader = requestHeader,
                Error = error,
                ResponseTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                RequestReceivedTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
            };

            return responseHeader;
        }
    }
}