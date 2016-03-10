using System;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;

namespace OmniSharp.Stdio
{
    internal class StdioServer : IServer
    {
        public IFeatureCollection Features { get; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Start<TContext>(IHttpApplication<TContext> application)
        {
            throw new NotImplementedException();
        }
    }
}