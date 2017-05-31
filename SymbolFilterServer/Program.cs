﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SymbolFilterServer
{
    class SymbolFilterServer
    {
        private static readonly RedirectParser RedirectParser = new RedirectParser();

        // this holds the white list of DLLs we do not ignore
        static List<string> dllFilterList = new List<string>();

        // default port is 8080, config would be nice...
        const int port = 8080;

        static void Main(string[] args)
        {
            // load up the dlls
            InitializeDllFilters();

            // open the socket
            StartHttpListener();

            // all real work happens in the background, if you ever press enter we just exit
            Console.WriteLine("Listening on port {0}.  Press enter to exit", port);
            Console.ReadLine();
        }

        static void InitializeDllFilters()
        {
            try
            {
                // we're just going to throw if it fails...
                StreamReader sr = new FileInfo("symfilter.txt").OpenText();

                // read lines from the file
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    dllFilterList.Add(line.Trim().ToLowerInvariant());
                }

                sr.Dispose();

            }
            catch (Exception e)
            {
                // anything goes wrong and we're done here, there's no recovery from this
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }

        // Here we will just listen for connections on the loopback adapter, port 8080
        // this could really use some configuration options as well.  In fact running more than one of these
        // listening to different ports with different white lists would be very useful.
        public static void StartHttpListener()
        {
            new WebHostBuilder().UseKestrel().Configure(app => app.Run(handler =>
                Request(handler)
            )).Build().Run();
        }

        static async Task Request(HttpContext context)
        {
            await Task.Run(() =>
            {
                var request = context.Request;
                var req = request.Path;

                // avoid case sensitivity issues for matching the pattern
                var reqLower = Uri.UnescapeDataString(req).ToLowerInvariant();

                int i;
                for (i = 0; i < dllFilterList.Count; i++)
                {
                    if (reqLower.Contains(dllFilterList[i]))
                        break;
                }

                // if we didn't match, or it isn't a GET then serve up a 404
                if (i == dllFilterList.Count || request.Method != "GET") 
                {
                    // you don't match, fast exit, this is basically the whole point of this thing
                    Return404(context);
                }
                else
                {
                    // this is the real work
                    Console.WriteLine("Matched pattern: {0}", dllFilterList[i]);
                    RedirectRequest(context, req);
                }
            });
        }

        // cons up a minimal 404 error and return it
        static void Return404(HttpContext context)
        {
            // it doesn't get any simpler than this
            context.Response.StatusCode = 404;
        }

        static void Return302(HttpContext context, string url)
        {
            Console.WriteLine("302 Redirect {0}", url);

            // emit the redirect
            context.Response.Redirect(url, false);
        }

        static void RedirectRequest(HttpContext context, string req)
        {
            var result = RedirectParser.Parse(req);

            if (result.IsValid)
            {
                Return302(context, result.Redirect);
            }
            else
            {
                Return404(context);
            }
        }
    }
}