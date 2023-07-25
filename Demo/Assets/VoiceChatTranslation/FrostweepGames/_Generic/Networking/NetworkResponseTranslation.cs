namespace FrostweepGames.Plugins.GoogleCloud
{
    public class NetworkResponseTranslation
    {
        public long netPacketIndex;
        public NetworkEnumeratorsTranslation.RequestType requestType;
        public object[] parameters;

        public string response;
        public string error;

        public NetworkResponseTranslation(string resp, string err, long index, NetworkEnumeratorsTranslation.RequestType type, object[] param)
        {
            requestType = type;
            netPacketIndex = index;
            response = resp;
            error = err;
            parameters = param;
        }
    }
}