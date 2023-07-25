using System;
using System.Collections.Generic;

namespace FrostweepGames.Plugins.GoogleCloud
{
    public class NetworkingTranslation : IDisposable
    {
        public event Action<NetworkResponseTranslation> NetworkResponseEvent;

        private List<NetworkRequestTranslation> _networkRequests;
        private List<NetworkResponseTranslation> _networkResponses;

        private long _packetIndex = 0;


        public NetworkingTranslation()
        {
            _networkRequests = new List<NetworkRequestTranslation>();
            _networkResponses = new List<NetworkResponseTranslation>();
        }

        public void Update()
        {
            for(int i = 0; i < _networkRequests.Count; i++)
            {
                if (_networkRequests[i].request.isDone)
                {
                    NetworkResponseTranslation response = new NetworkResponseTranslation(_networkRequests[i].request.text,
                                                                   _networkRequests[i].request.error,
                                                                   _networkRequests[i].netPacketIndex, 
                                                                   _networkRequests[i].requestType,
                                                                   _networkRequests[i].parameters);

                    _networkResponses.Add(response);

                    if (NetworkResponseEvent != null)
                        NetworkResponseEvent(response);

                    _networkRequests.RemoveAt(i--);
                }
            }
        }

        public void Dispose()
        {
            _networkRequests.Clear();
            _networkResponses.Clear();
        }

        public long SendRequest(string uri, string data, NetworkEnumeratorsTranslation.RequestType requestType, object[] param = null, bool checkCertificates = false)
        {
            long netIndex = _packetIndex++;

            NetworkRequestTranslation netRequest = new NetworkRequestTranslation(uri, data, netIndex, requestType, param, checkCertificates);

            _networkRequests.Add(netRequest);

            netRequest.Send();

            return netIndex;
        }
    }
}