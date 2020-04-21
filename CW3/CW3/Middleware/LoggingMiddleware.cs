using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
namespace CW3.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            if (context.Request != null)
            {
                string path = context.Request.Path;
                string method = context.Request.Method;
                string querryString = context.Request.QueryString.ToString();
                string bodyString = "";

                using(var reader=new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyString = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }
                
                String log = DateTime.Now.ToString()+"\nMETHOD: "+method+"\nPATH: "+path+"\nBODY: "+bodyString+"\nQUERRY: "+querryString+"\n///////////////////////////////////////\n";
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+ "\\requestsLog.txt", log);
            }


            if(_next!=null)
                await _next(context);
        }
    }
}