    /// <summary>
    /// Http请求客户端
    /// 不能继承此类
    /// </summary>
    public sealed class HttpClient
    {
        /// <summary>
        /// 请求前异处理程序
        /// </summary>
        public event EventHandler<HttpClientExceptionArgs> OnBeforRequestException;
        /// <summary>
        /// 响应时异常处理程序
        /// </summary>
        public event EventHandler<HttpClientExceptionArgs> OnResponseException;
        /// <summary>
        /// 解析时异常处理程序
        /// </summary>
        public event EventHandler<HttpClientExceptionArgs> OnResolutionException;
        /// <summary>
        /// 请求时异常处理程序
        /// </summary>
        public event EventHandler<HttpClientExceptionArgs> OnRequestException;
        /// <summary>
        /// 响应成功处理程序
        /// </summary>
        public event EventHandler<HttpClientSuccessArgs> OnHttpSuccessHandler;
        /// <summary>
        /// 请求失败处理程序
        /// </summary>
        public event EventHandler<HttpClientFailedArgs> OnHttpFailedHandler;

        private HttpClientData _httpClientData;
        private bool _isRequested;
        /// <summary>
        /// Http请求数据
        /// </summary>
        public HttpClientData HttpClientData
        {
            get
            {
                return _httpClientData;
            }
        }
        /// <summary>
        /// 是否上一个请求进行中
        /// </summary>
        public bool IsRequested
        {
            get
            {
                return _isRequested;
            }
        }

        private HttpClient()
            : this(null)
        {

        }

        private HttpClient(HttpWebRequest httpWebRequest)
        {
            _httpClientData = new HttpClientData();
            _httpClientData.WebRequest = httpWebRequest;
            _isRequested = false;
        }
        /// <summary>
        /// 根据给定的远程服务调用参数初始化Uri类的实例
        /// </summary>
        /// <param name="baseServerAddress">调用服务地址</param>
        /// <param name="baseServerMethod">调用服务方法</param>
        /// <param name="baseServerParamters">调用服务参数</param>
        /// <returns></returns>
        private Uri CreateRequestUri(string baseServerAddress, string baseServerMethod, IDictionary<string, string> baseServerParamters)
        {

            var requestUri = new StringBuilder();
            if (!baseServerAddress.EndsWith("/"))
            {
                baseServerAddress += "/";
            }
            requestUri.Append(baseServerAddress).Append(baseServerMethod);
            if (baseServerParamters.Count > 0)
            {
                requestUri.Append("?");
                foreach (KeyValuePair<string, string> kvalues in baseServerParamters)
                {
                    requestUri
                        .Append(kvalues.Key)
                        .Append("=")
                        .Append(kvalues.Value)
                        .Append("&");
                }
            }
            var requestUriString = requestUri.ToString();
            return new Uri(requestUriString.Substring(0, requestUriString.Length - 1));
        }
        /// <summary>
        /// 创建一个HttpClient客户端
        /// </summary>
        /// <param name="options">连接配置信息</param>
        /// <returns></returns>
        public static HttpClient Create(Action<HttpClientOption> options)
        {
            var option = new HttpClientOption();
            options.Invoke(option);
            if (!option.BaseServerUrl.EndsWith("/"))
            {
                option.BaseServerUrl += "/";
            }
            var uri = new Uri(option.BaseServerUrl + option.RemoteMethod);
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            var client = new HttpClient(httpWebRequest);
            client.SetWebHttpRequest(httpWebRequest, option);
            return client;
            
        }
        /// <summary>
        /// 修改请求信息
        /// </summary>
        /// <param name="requestData"></param>
        public HttpClient UseHttpClient(Action<HttpClientOption> options)
        {
            var option = new HttpClientOption();
            options.Invoke(option);
            if (!string.IsNullOrEmpty(option.RemoteMethod))
            {
                Uri uri;
                if (!string.IsNullOrEmpty(option.BaseServerUrl))
                {
                    uri = CreateRequestUri(option.BaseServerUrl, option.RemoteMethod, option.RemoteParamters);
                }
                else
                {
                    var path = _httpClientData.WebRequest.RequestUri.AbsoluteUri.Substring(0, _httpClientData.WebRequest.RequestUri.AbsoluteUri.LastIndexOf('/') + 1);
                    uri = CreateRequestUri(path, option.RemoteMethod, option.RemoteParamters);
                }
                _httpClientData.WebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            }
            SetWebHttpRequest(_httpClientData.WebRequest, option);
            return this;
        }
        /// <summary>
        /// 设置HttpWebRequest连接信息
        /// </summary>
        /// <param name="httpWebRequest"></param>
        /// <param name="options"></param>
        public void SetWebHttpRequest(HttpWebRequest httpWebRequest, Action<HttpClientOption> options)
        {
            var option = new HttpClientOption();
            options.Invoke(option);
            SetWebHttpRequest(httpWebRequest, option);
        }
        /// <summary>
        /// 设置HttpWebRequest连接信息
        /// </summary>
        /// <param name="httpWebRequest"></param>
        /// <param name="option"></param>
        public void SetWebHttpRequest(HttpWebRequest httpWebRequest, HttpClientOption option)
        {
            httpWebRequest.Headers.Clear();
            httpWebRequest.Method = option.RequestMethod ?? httpWebRequest.Method;
            httpWebRequest.Timeout = option.Timeout > 1000 ? option.Timeout : httpWebRequest.Timeout;
            httpWebRequest.ContentType = option.ContentType ?? httpWebRequest.ContentType;
            httpWebRequest.KeepAlive = option.KeepAlive;
            httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.0; Windows CE Compact Framework)";
            if (httpWebRequest.ServicePoint != null)
            {
                httpWebRequest.ServicePoint.Expect100Continue = false;
            }
        }
        /// <summary>
        /// 发送请求数据
        /// </summary>
        /// <param name="requestData"></param>
        public void SendMessage(object requestData)
        {
            SendMessage(requestData, null);
        }
        /// <summary>
        /// 发送请求数据,在请求成功后调用给定的回调函数
        /// </summary>
        /// <param name="requestData">请求参数</param>
        /// <param name="successCallback">请求成功回调函数(包含一个响应信息)</param>
        public void SendMessage(object requestData, Action<object> successCallback)
        {
            if (!_isRequested)
            {
                _isRequested = true;
                Debug.WriteLine("Begin request...");
                _httpClientData.SuccessCallback = successCallback;
                if (requestData != null)
                {
                    var requestJson = JsonConvert.SerializeObject(requestData);
                    byte[] postData = Encoding.UTF8.GetBytes(requestJson);
                    _httpClientData.RequestData = postData;
                    _httpClientData.WebRequest.ContentLength = postData.Length;
                    _httpClientData.WebRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), _httpClientData);
                }
                else
                {
                    _httpClientData.WebRequest.BeginGetResponse(new AsyncCallback(GetResponseStreamCallback), _httpClientData);
                }
            }
        }
        /// <summary>
        /// 开始异步请求回调
        /// </summary>
        /// <param name="result"></param>
        private void GetRequestStreamCallback(IAsyncResult result)
        {
            var requestData = result.AsyncState as HttpClientData;
            try
            {
                Debug.WriteLine("Write request stream...");
                if (requestData.RequestData.Length > 0)
                {
                    using (var postStream = requestData.WebRequest.EndGetRequestStream(result))
                    {
                        postStream.Write(requestData.RequestData, 0, requestData.RequestData.Length);
                    }
                }
                Debug.WriteLine("Begin get response...");
                requestData.WebRequest.BeginGetResponse(new AsyncCallback(GetResponseStreamCallback), requestData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BeforRequest exception...");
                if (OnBeforRequestException != null)
                {
                    OnBeforRequestException.Invoke(requestData.WebRequest, new HttpClientExceptionArgs(ex));
                }
                return;
            }

        }
        /// <summary>
        /// 开始异步解析回调
        /// </summary>
        /// <param name="result"></param>
        private void GetResponseStreamCallback(IAsyncResult result)
        {
            StreamReader reader = null;
            Stream responseStream = null;
            var requestData = result.AsyncState as HttpClientData;
            try
            {
                Debug.WriteLine("End get response...");
                using (var httpWebReponse = (HttpWebResponse)requestData.WebRequest.EndGetResponse(result))
                {
                    try
                    {
                        Debug.WriteLine("Http status code:" + httpWebReponse.StatusCode);
                        Debug.WriteLine("Begin get response stream...");
                        responseStream = httpWebReponse.GetResponseStream();
                        try
                        {
                            Debug.WriteLine("Begin read response...");
                            reader = new StreamReader(responseStream);
                            var reponseString = reader.ReadToEnd();
                            if (httpWebReponse.StatusCode == HttpStatusCode.OK)
                            {
                                if (OnHttpSuccessHandler != null)
                                {
                                    OnHttpSuccessHandler.Invoke(httpWebReponse, new HttpClientSuccessArgs(reponseString));
                                }
                                if (requestData.SuccessCallback != null)
                                {
                                    ThreadPool.QueueUserWorkItem(new WaitCallback(requestData.SuccessCallback), reponseString);
                                }
                            }
                            else
                            {
                                if (OnHttpFailedHandler != null)
                                {
                                    OnHttpFailedHandler.Invoke(httpWebReponse, new HttpClientFailedArgs(reponseString, httpWebReponse.StatusCode));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Resolution exception:" + ex.Message);
                            if (OnResolutionException != null)
                            {
                                OnResolutionException.Invoke(httpWebReponse, new HttpClientExceptionArgs(ex));
                            }
                            return;
                        }
                        finally
                        {
                            Debug.WriteLine("Disposing reponse stream...");
                            if (responseStream != null)
                            {
                                responseStream.Close();
                            }
                            if (reader != null)
                            {
                                reader.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Response exception:" + ex.Message);
                        if (OnResponseException != null)
                        {
                            OnResponseException.Invoke(httpWebReponse, new HttpClientExceptionArgs(ex));
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Request exception:" + ex.Message);
                if (OnRequestException != null)
                {
                    OnRequestException.Invoke(requestData.WebRequest, new HttpClientExceptionArgs(ex));
                }
                return;
            }
            finally
            {
                Debug.WriteLine("Disposing http request...");
                if (requestData.WebRequest != null)
                {
                    requestData.WebRequest.Abort();
                }
                _isRequested = false;
            }
        }
    }
    /// <summary>
    /// 创建HttpClient的配置信息
    /// </summary>
    public class HttpClientOption
    {
        /// <summary>
        /// 请求服务地址
        /// </summary>
        public string BaseServerUrl { get; set; }
        /// <summary>
        /// 远程服务调用方法
        /// </summary>
        public string RemoteMethod { get; set; }
        /// <summary>
        /// 远程服务调用参数列表
        /// </summary>
        public IDictionary<string, string> RemoteParamters { get; set; }
        /// <summary>
        /// 请求服务方式
        /// </summary>
        public string RequestMethod { get; set; }
        /// <summary>
        /// 数据传递类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 请求代理
        /// </summary>
        public IWebProxy Proxy { get; set; }
        /// <summary>
        /// 是否与Internet建立永久连接
        /// </summary>
        public bool KeepAlive { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; }
        public HttpClientOption()
        {
            Timeout = 10000;
            RequestMethod = "POST";
            RemoteMethod = "Index";
            RemoteParamters = new Dictionary<string, string>();
            ContentType = "application/json";
            Proxy = null;
            KeepAlive = false;
        }
    }
    /// <summary>
    /// 传递请求数据
    /// </summary>
    public class HttpClientData
    {
        /// <summary>
        /// 原始请求类
        /// </summary>
        public HttpWebRequest WebRequest { get; set; }
        /// <summary>
        /// 请求数据
        /// </summary>
        public byte[] RequestData { get; set; }
        /// <summary>
        /// 请求成功用户回调方法
        /// </summary>
        public Action<object> SuccessCallback { get; set; }
    }
    /// <summary>
    /// Http请求错误数据
    /// </summary>
    public sealed class HttpClientExceptionArgs : EventArgs
    {
        /// <summary>
        /// 引发的错误类型
        /// </summary>
        public Exception HttpException { get; set; }

        public HttpClientExceptionArgs(Exception ex)
        {
            HttpException = ex;
        }
    }
    /// <summary>
    /// Http请求成功响应信息
    /// </summary>
    public sealed class HttpClientSuccessArgs : EventArgs
    {
        /// <summary>
        /// 成功响应信息
        /// </summary>
        public string ResponseString { get; set; }

        public HttpClientSuccessArgs(string response)
        {
            ResponseString = response;
        }
    }
    /// <summary>
    /// Http请求失败响应信息
    /// </summary>
    public sealed class HttpClientFailedArgs : EventArgs
    {
        /// <summary>
        /// 失败响应信息
        /// </summary>
        public string FailedMessage { get; set; }
        /// <summary>
        /// Http请求状态码
        /// </summary>
        public HttpStatusCode HttpErrorCode { get; set; }

        public HttpClientFailedArgs(string response, HttpStatusCode status)
        {
            FailedMessage = response;
            HttpErrorCode = status;
        }
    }
