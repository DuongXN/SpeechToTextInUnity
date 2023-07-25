using UnityEngine.Networking;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using FrostweepGames.Plugins.Networking;

namespace FrostweepGames.Plugins.GoogleCloud
{
    public class NetworkRequestTranslation
    {
        public long netPacketIndex;
        public NetworkEnumeratorsTranslation.RequestType requestType;
        public object[] parameters;

        public NetworkMethod request;

        public NetworkRequestTranslation(string uri, string data, long index, NetworkEnumeratorsTranslation.RequestType type, object[] param = null, bool checkCeritifcates = false)
        {
            requestType = type;
            netPacketIndex = index;
            parameters = param;

            request = new NetworkMethod(uri, data, checkCeritifcates, type, NetworkConstantsTranslation.NETWORK_METHOD);
        }

        public void Send()
        {
            request.Send();
        }
    }


    public class NetworkMethod
    {
        private string _uri,
                       _data;

        private bool _checkCertificate;

        private NetworkEnumeratorsTranslation.RequestType _requestType;

        private WWW _wwwRequest;
        private UnityWebRequest _webRequest;

        private NetworkEnumeratorsTranslation.NetworkMethod _method;

        public bool isDone
        {
            get
            {
                switch(_method)
                {
                    case NetworkEnumeratorsTranslation.NetworkMethod.WWW:
                        return _wwwRequest.isDone;
                    case NetworkEnumeratorsTranslation.NetworkMethod.WEB_REQUEST:
                        return _webRequest.isDone;
                    default: break;
                }

                return false;
            }
        }

        public string text
        {
            get
            {
                switch (_method)
                {
                    case NetworkEnumeratorsTranslation.NetworkMethod.WWW:
                        return _wwwRequest.text;
                    case NetworkEnumeratorsTranslation.NetworkMethod.WEB_REQUEST:
                        return _webRequest.downloadHandler.text;
                    default: break;
                }

                return string.Empty;
            }
        }

        public string error
        {
            get
            {
                switch (_method)
                {
                    case NetworkEnumeratorsTranslation.NetworkMethod.WWW:
                        return _wwwRequest.error;
                    case NetworkEnumeratorsTranslation.NetworkMethod.WEB_REQUEST:
                        return _webRequest.error;
                    default: break;
                }

                return string.Empty;
            }
        }

        public NetworkMethod(string uri, string data, bool checkCeritifcates, NetworkEnumeratorsTranslation.RequestType type, NetworkEnumeratorsTranslation.NetworkMethod method)
        {
            _uri = uri;
            _data = data;
            _requestType = type;
            _checkCertificate = checkCeritifcates;
            _method = method;

            switch (method)
            {
                case NetworkEnumeratorsTranslation.NetworkMethod.WEB_REQUEST:
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(_data);

                        if (_requestType == NetworkEnumeratorsTranslation.RequestType.GET)
                            _webRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);
                        else
                            _webRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);

                        if (!string.IsNullOrEmpty(data))
                            _webRequest.uploadHandler = new UploadHandlerRaw(bytes);

                        _webRequest.downloadHandler = new DownloadHandlerBuffer();
                        _webRequest.SetRequestHeader("Content-Type", "application/json");

                        if (checkCeritifcates)
                        {
#if UNITY_ANDROID
                            _webRequest.SetRequestHeader("X-Android-Package", NetworkConstants.PACKAGE_NAME);
                            _webRequest.SetRequestHeader("X-Android-Cert", NetworkConstants.KEY_SIGNATURE);
#elif UNITY_IOS
                            // need to check are they correct keys
                           // _webRequest.SetRequestHeader("X-IOS-Package", NetworkConstants.PACKAGE_NAME);
                          //  _webRequest.SetRequestHeader("X-IOS-Cert", NetworkConstants.KEY_SIGNATURE);
#endif
                        }
                    }
                    break;
                default: break;
            }
        }

        public void Send()
        {
            switch (_method)
            {
                case NetworkEnumeratorsTranslation.NetworkMethod.WEB_REQUEST:
                    // _webRequest.SendWebRequest(); // use it in the new Unity versions instead of:
                    _webRequest.Send();
                    break;
                case NetworkEnumeratorsTranslation.NetworkMethod.WWW:
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(_data);

                        var headers = new Dictionary<string, string>();
                        headers.Add("Content-Type", "application/json");

                        if (_checkCertificate)
                        {
#if UNITY_ANDROID
                            headers.Add("X-Android-Package", NetworkConstants.PACKAGE_NAME);
                            headers.Add("X-Android-Cert", NetworkConstants.KEY_SIGNATURE);
#elif UNITY_IOS
                            //need to check are they correct keys
                            //headers.Add("X-IOS-Package", NetworkConstants.PACKAGE_NAME);
                            //headers.Add("X-IOS-Cert", NetworkConstants.KEY_SIGNATURE);
#endif
                        }

                        _wwwRequest = new WWW(_uri, bytes, headers);
                    }
                    break;
                default: break;
            }
        }
    }
}