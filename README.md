# MyLog
一些奇思妙想的东西，记录下来

用户自定义回调函数线程池回调,事件函数则以当前线程调用

```C#
private void InitHttpClient()
{
    var feedback = new FeedbackDto();
    feedback.UserName = "admin1";
    feedback.Application = "admin1";
    feedback.Email = "admin@admin.com1";
    feedback.IPAddress = "127.0.0.1";
    feedback.MacAddress = "131311313";
    feedback.Feedback = "这是来自PDA的一条测试反馈1";
    _httpClient = HttpClient.Create(option =>
    {
        option.BaseServerUrl = "http://localhost:5000/api/services/app/feedback/";
        option.RemoteMethod = "submitfeedback";
        option.ContentType = "application/json";
        option.RequestMethod = "POST";
        option.Timeout = 10000;
    });
    _httpClient.OnBeforRequestException += new EventHandler<HttpClientExceptionArgs>(httpClient_OnBeforRequestException);
    _httpClient.OnRequestException += new EventHandler<HttpClientExceptionArgs>(httpClient_OnRequestException);
    _httpClient.OnResponseException += new EventHandler<HttpClientExceptionArgs>(httpClient_OnResponseException);
    _httpClient.OnResolutionException += new EventHandler<HttpClientExceptionArgs>(httpClient_OnResolutionException);
    _httpClient.OnHttpFailedHandler += new EventHandler<HttpClientFailedArgs>(httpClient_OnHttpFailedHandler);
    _httpClient.OnHttpSuccessHandler += new EventHandler<HttpClientSuccessArgs>(httpClient_OnHttpSuccessHandler);
    //成功后调用自定义函数
    _httpClient.SendMessage(feedback, (response) => SuccessCallback(response));
}

//成功后调用此函数
private void SuccessCallback(object response)
{
    Console.WriteLine("请求成功,转入回调函数,响应信息：" + response.ToString());
}
//成功后引发事件
private void httpClient_OnHttpSuccessHandler(object sender, HttpClientSuccessArgs e)
{
    Console.WriteLine("请求成功,响应信息：" + e.ResponseString);
}

private void httpClient_OnHttpFailedHandler(object sender, HttpClientFailedArgs e)
{
    Console.WriteLine("请求失败,响应信息：" + e.FailedMessage + "状态码：" + e.HttpErrorCode);
}

private void httpClient_OnResolutionException(object sender, HttpClientExceptionArgs e)
{
    Console.WriteLine("请求解析时出现错误：" + e.HttpException.Message);
}

private void httpClient_OnResponseException(object sender, HttpClientExceptionArgs e)
{
    Console.WriteLine("响应时出现错误：" + e.HttpException.Message);
}

private void httpClient_OnRequestException(object sender, HttpClientExceptionArgs e)
{
    Console.WriteLine("请求时出现错误：" + e.HttpException.Message);
}

private void httpClient_OnBeforRequestException(object sender, HttpClientExceptionArgs e)
{
    Console.WriteLine("请求前出现错误：" + e.HttpException.Message);
}


private void TestSendPatchHttpMessage()
{
    _httpClient
            .UseHttpClient(option =>
            {
                option.BaseServerUrl = "http://localhost:5000/api/services/app/product/";
                option.RemoteMethod = "CancelAuditVouch";
                option.RemoteParamters.Add("vouchCode", "0000002655");
                option.ContentType = "application/json";
                option.RequestMethod = "PATCH";
                option.Timeout = 20000;
                option.Proxy = null;
            })
            .SendMessage(null, (response) => Console.WriteLine(response.ToString()));
}
```
