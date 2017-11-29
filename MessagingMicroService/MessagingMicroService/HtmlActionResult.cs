using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace MessagingMicroService
{
    public class HtmlActionResult : IHttpActionResult
    {
        private const string ViewDirectory = @"views/Html";
        private readonly string _view;
        private readonly dynamic _model;

        public HtmlActionResult(string viewName, dynamic model)
        {
            _view = LoadView(viewName);
            _model = model;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
#pragma warning disable CS0618 // Type or member is obsolete
            var parsedView = RazorEngine.Razor.Parse(_view, _model);
#pragma warning restore CS0618 // Type or member is obsolete
            response.Content = new StringContent(parsedView);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return Task.FromResult(response);
        }

        private static string LoadView(string name)
        {
            var view = File.ReadAllText(Path.Combine(ViewDirectory, name + ".cshtml"));
            return view;
        }
    }
}
