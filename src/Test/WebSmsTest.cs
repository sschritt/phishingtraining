using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using AngleSharp;
using Newtonsoft.Json;
using PhishingTraining.Web.Entities;
using Xunit;
using Xunit.Abstractions;

namespace Test
{
    public class WebSmsTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public WebSmsTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        //See https://developer.websms.com/web-api/
        public const string BasicAuthenticationUser = "charles.lioe@sba-research.org";
        public const string BasicAuthenticationPassword = "WebSmsTest";
        
        public const string AccessToken = "";


        public const string BaseUrl = "https://api.websms.com/rest";



        public const string TestBasicAuthenticationEndPoint = "/smsmessaging/text";
        [Fact]
        public void TestBasicAuthenticationPost()
        {
            var requestjson = JsonConvert.SerializeObject(new WebSmsRequestJson()
            {
                recipientAddressList = new []{ "+4369911096374"},
                contentCategory = "informational", // "advertisement"
                //senderAddress ="+4369123456", //
                //senderAddressType = "international", //did not work with alphanumeric or shortcode
                clientMessageId = "+43 123123",
                test = false,
                messageContent = "This is a Test Message",
                maxSmsPerMessage = 1,
                validityPeriode = 300
            });
            _testOutputHelper.WriteLine(requestjson);

            var endPoint = new Uri(BaseUrl + TestBasicAuthenticationEndPoint);
            var client = new WebClient() {Credentials = new NetworkCredential(BasicAuthenticationUser, BasicAuthenticationPassword)};
            client.Headers.Add("Content-Type","application/json");
            var result = client.UploadString(endPoint.ToString(), requestjson);
            _testOutputHelper.WriteLine(result);
        }


        [Fact]
        public void TestAccessToken()
        {
            //Todo test via AccessToken? Token needs to be generated on website
        }
    }

    public class WebSmsRequestJson
    {
        public string[] recipientAddressList { get; set; }
        public string contentCategory { get; set; } 
        public string senderAddress { get; set; }
        public string senderAddressType { get; set; }
        public string clientMessageId { get; set; }
        public bool test { get; set; }
        public string messageContent { get; set; }
        public int maxSmsPerMessage { get; set; }
        public int validityPeriode { get; set; }
    }
}