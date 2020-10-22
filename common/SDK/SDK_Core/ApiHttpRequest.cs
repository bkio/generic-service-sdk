/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SDK
{
    public class ApiHttpRequest
    {
        private readonly string RequestUrl;

        private readonly HttpClientHandler Handler = null;
        private readonly HttpClient Client = null;

        private EApiHttpRequestType ApiHttpRequestType = EApiHttpRequestType.NONE;

        private StringContent RequestContent = null;
        private Task<HttpResponseMessage> RequestTask = null;

        public static string TokenCached;
        public static Action<string> OnTokenIsChangedByServer;

        public ApiHttpRequest(string _BaseApiUrl, string _RequestPath)
        {
            RequestUrl = _BaseApiUrl + _RequestPath;

            Handler = new HttpClientHandler();
            Client = new HttpClient(Handler);
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", TokenCached);
        }
        ~ApiHttpRequest()
        {
            try { RequestTask?.Dispose(); } catch (Exception) { }
            try { RequestContent?.Dispose(); } catch (Exception) { }
            try { Client?.Dispose(); } catch (Exception) { }
            try { Handler?.Dispose(); } catch (Exception) { }
        }

        private enum EApiHttpRequestType
        {
            NONE, GET, POST, PUT, DELETE
        }

        public int Perform(out int _ResultHttpCode, out JObject _ResultJson)
        {
            if (ApiHttpRequestType == EApiHttpRequestType.NONE) throw new ArgumentException("Request type must be defined by calling Get(), Delete(), Post(), Put() calls.");

            _ResultHttpCode = -1;
            _ResultJson = null;

            string LastExceptionMessage = "";
            string LastHttpResponseContent = "";

            bool bMaintenanceRetry;
            bool bRetry;
            int RetryCount = 0;
            do
            {
                bRetry = false;
                bMaintenanceRetry = false;

                try
                {
                    switch (ApiHttpRequestType)
                    {
                        case EApiHttpRequestType.GET:
                            RequestTask = Client.GetAsync(RequestUrl);
                            break;
                        case EApiHttpRequestType.DELETE:
                            RequestTask = Client.DeleteAsync(RequestUrl);
                            break;
                        case EApiHttpRequestType.POST:
                            RequestTask = Client.PostAsync(RequestUrl, RequestContent);
                            break;
                        case EApiHttpRequestType.PUT:
                            RequestTask = Client.PutAsync(RequestUrl, RequestContent);
                            break;
                    }

                    RequestTask.Wait();

                    using (var Response = RequestTask.Result)
                    {
                        _ResultHttpCode = (int)Response.StatusCode;

                        if (Response.Headers.TryGetValues("x-bkio-sso-token-refreshed", out IEnumerable<string> IsRefreshedEnumerable) 
                            && IsRefreshedEnumerable != null
                            && Response.Headers.TryGetValues("x-bkio-sso-token-after-refresh", out IEnumerable<string> TokenAfterRefreshEnumerable)
                            && TokenAfterRefreshEnumerable != null)
                        {
                            var IsRefreshed = IsRefreshedEnumerable.FirstOrDefault();
                            var TokenAfterRefresh = TokenAfterRefreshEnumerable.FirstOrDefault();
                            if (IsRefreshed == "true" && TokenAfterRefresh != null && TokenAfterRefresh.Length > 0)
                            {
                                TokenCached = TokenAfterRefresh;
                                OnTokenIsChangedByServer?.Invoke(TokenCached);
                            }
                        }

                        using (var ResponseContent = Response.Content)
                        {
                            using (var ReadResponseTask = ResponseContent.ReadAsStringAsync())
                            {
                                ReadResponseTask.Wait();
                                LastHttpResponseContent = ReadResponseTask.Result;

                                try
                                {
                                    _ResultJson = JObject.Parse(LastHttpResponseContent);
                                }
                                catch (JsonReaderException)
                                {
                                    _ResultJson = null;
                                    bRetry = true;
                                }
                            }
                        }
                    }

                    //There is eventual consistency; rights might take some short amount of time to be granted for the user.
                    //Also in case it returns >=500; let's retry.
                    if (_ResultHttpCode == 403 || _ResultHttpCode >= 500)
                    {
                        bRetry = true;
                        bMaintenanceRetry = _ResultHttpCode == 503;
                    }
                }
                catch (Exception e)
                {
                    LastExceptionMessage = "Message: " + e.Message + ", Trace: " + e.StackTrace;
                    bRetry = true;
                }

            } while ((bMaintenanceRetry || bRetry && ++RetryCount < 60) && RetryCooldown());

            if (_ResultJson != null)
            {
                if (_ResultHttpCode >= 400)
                {
                    _ResultJson["errorCode"] = _ResultHttpCode;
                }
                return Utilities.SUCCESS;
            }
            return Utilities.Error("Api http request has failed. Response code: " + _ResultHttpCode + ", Response: " + LastHttpResponseContent + ", Exception if any: " + LastExceptionMessage);
        }
        private bool RetryCooldown() { Thread.Sleep(1000); return true; }

        public ApiHttpRequest Get()
        {
            ApiHttpRequestType = EApiHttpRequestType.GET;
            return this;
        }
        public ApiHttpRequest Delete()
        {
            ApiHttpRequestType = EApiHttpRequestType.DELETE;
            return this;
        }
        public ApiHttpRequest Post(JObject _Content)
        {
            RequestContent = new StringContent(_Content.ToString());
            ApiHttpRequestType = EApiHttpRequestType.POST;
            return this;
        }
        public ApiHttpRequest Post(JArray _Content)
        {
            RequestContent = new StringContent(_Content.ToString());
            ApiHttpRequestType = EApiHttpRequestType.POST;
            return this;
        }
        public ApiHttpRequest Put(JObject _Content)
        {
            RequestContent = new StringContent(_Content.ToString());
            ApiHttpRequestType = EApiHttpRequestType.PUT;
            return this;
        }
        public ApiHttpRequest Put(JArray _Content)
        {
            RequestContent = new StringContent(_Content.ToString());
            ApiHttpRequestType = EApiHttpRequestType.PUT;
            return this;
        }
    }
}