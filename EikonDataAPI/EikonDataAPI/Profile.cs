using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;

namespace EikonDataAPI
{
    public class Profile
    {
        public string AppId { get; set; }
        public uint Port {
            get {
                uint _port;
                _port = ForcePort?? GetScriptingProxyPort();
                ForcePort = _port;
                _logger?.LogInformation("Return port: {0}", _port);
                return _port;                                 
            }
        }
        private Uri _url = null;
        private Uri _streamUrl = null;
        public Uri Url {
            get{
                if (_url == null)
                {
                    _url = new Uri($"http://localhost:{Port}/api/v1/data");
                }
                return _url;
            }
        }
        public Uri StreamingUrl
        {
            get
            {
                if (_streamUrl == null)
                {
                    _streamUrl = new Uri($"ws://localhost:{Port}/?");
                }
                return _streamUrl;
            }
        }
        public int? Timeout { get; set; }
        
        public string FileName { get; set; }
        public string[] AppNames { get; set; }
        public string[] AppAuthor { get; set; }
        public uint? ForcePort { get; set; } = null;
        public string AppDataFolder { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        private ILogger _logger = null;
        public ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
       
        public Profile()
        {
            LoggerFactory = new LoggerFactory();
            _logger = LoggerFactory.CreateLogger<Profile>();
            AppNames = new string[3];
            AppNames[0] = "Data API Proxy";
            AppNames[1] = "Eikon API Proxy";
            AppNames[2] = "Eikon Scripting Proxy";
            AppAuthor = new string[2];
            AppAuthor[0] = "Refinitiv";
            AppAuthor[1] = "Thomson Reuters";
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            { 
                AppDataFolder = Environment.GetEnvironmentVariable("APPDATA");
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                AppDataFolder = Environment.GetEnvironmentVariable("HOME");
            }
            FileName = ".portInUse";

            //Port = GetScriptingProxyPort();

         //   Url = new Uri($"http://localhost:{Port}/api/v1/data");
         //   StreamingUrl = new Uri($"ws://localhost:{Port}/?");
            Timeout = 30;

        }
        //public void CreateLogger(ILoggerFactory factory)
        //{
        //    _logger = factory.CreateLogger<Profile>();
        //}
        private uint GetScriptingProxyPort()
        {
            //throw new NotImplementedException();
            uint port = 36036;
            foreach (string appAuthor in AppAuthor)
            {
                foreach (string appName in AppNames)
                {
                    string path = "";
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        path = Path.Combine(new string[] { AppDataFolder, appAuthor, appName, FileName });

                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        path = Path.Combine(new string[] { AppDataFolder, "Library", "Application Support", appName, FileName });

                    }
                    // string path = Path.Combine(new string[] { AppDataFolder, AppAuthor, appName, FileName });

                    if (File.Exists(path))
                    {
                        _logger?.LogInformation("Find port in a file: {0}", path);
                        try
                        {
                            port = Convert.ToUInt16(File.ReadAllText(path));
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogWarning("Can't convert port to a number: {0}",
                                JSONRequest.GetInnerMostException(ex).Message.ToString());
                            continue;
                            //Log Error
                        }
                        break;
                    }
                }
            }
            _logger?.LogInformation("use port: {0}", port);
            return port;
        }
    }
}
