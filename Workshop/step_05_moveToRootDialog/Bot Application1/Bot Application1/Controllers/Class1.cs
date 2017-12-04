//using Microsoft.Bot.Connector;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Http;

//namespace Bot_Application1
//{
//    [BotAuthentication]
//    [RoutePrefix("api/calling")]
//    public class CallingController : ApiController
//    {
//        public CallingController() : base()
//        {
//            CallingConversation.RegisterCallingBot(callingBotService => new IVRBot(callingBotService));
//        }

//        [Route("callback")]
//        public async Task<HttpResponseMessage> ProcessCallingEventAsync()
//        {
//            return await CallingConversation.SendAsync(this.Request, CallRequestType.CallingEvent);
//        }

//        [Route("call")]
//        public async Task<HttpResponseMessage> ProcessIncomingCallAsync()
//        {
//            return await CallingConversation.SendAsync(this.Request, CallRequestType.IncomingCall);
//        }
//    }
//}