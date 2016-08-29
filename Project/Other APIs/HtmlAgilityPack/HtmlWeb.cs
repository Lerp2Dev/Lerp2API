namespace HtmlAgilityPack
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    public class HtmlWeb
    {
        private bool _autoDetectEncoding = true;
        private bool _cacheOnly;
        private string _cachePath;
        private Encoding _encoding;
        private bool _fromCache;
        private static Dictionary<string, string> _mimeTypes;
        private int _requestDuration;
        private Uri _responseUri;
        private HttpStatusCode _statusCode = HttpStatusCode.OK;
        private int _streamBufferSize = 0x400;
        private bool _useCookies;
        private string _userAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:x.x.x) Gecko/20041107 Firefox/x.x";
        private bool _usingCache;
        public PostResponseHandler PostResponse;
        public PreHandleDocumentHandler PreHandleDocument;
        public PreRequestHandler PreRequest;

        public object CreateInstance(string url, Type type)
        {
            return this.CreateInstance(url, null, null, type);
        }

        public object CreateInstance(string htmlUrl, string xsltUrl, XsltArgumentList xsltArgs, Type type)
        {
            return this.CreateInstance(htmlUrl, xsltUrl, xsltArgs, type, null);
        }

        public object CreateInstance(string htmlUrl, string xsltUrl, XsltArgumentList xsltArgs, Type type, string xmlPath)
        {
            object obj2;
            StringWriter w = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(w);
            if (xsltUrl == null)
            {
                this.LoadHtmlAsXml(htmlUrl, writer);
            }
            else if (xmlPath == null)
            {
                this.LoadHtmlAsXml(htmlUrl, xsltUrl, xsltArgs, writer);
            }
            else
            {
                this.LoadHtmlAsXml(htmlUrl, xsltUrl, xsltArgs, writer, xmlPath);
            }
            writer.Flush();
            StringReader input = new StringReader(w.ToString());
            XmlTextReader xmlReader = new XmlTextReader(input);
            XmlSerializer serializer = new XmlSerializer(type);
            try
            {
                obj2 = serializer.Deserialize(xmlReader);
            }
            catch (InvalidOperationException exception)
            {
                throw new Exception(exception + ", --- xml:" + w);
            }
            return obj2;
        }

        private static void FilePreparePath(string target)
        {
            if (System.IO.File.Exists(target))
            {
                FileAttributes attributes = System.IO.File.GetAttributes(target);
                System.IO.File.SetAttributes(target, attributes & ~FileAttributes.ReadOnly);
            }
            else
            {
                string directoryName = Path.GetDirectoryName(target);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }
        }

        public void Get(string url, string path)
        {
            this.Get(url, path, "GET");
        }

        public void Get(string url, string path, string method)
        {
            Uri uri = new Uri(url);
            if (!(uri.Scheme == Uri.UriSchemeHttps) && !(uri.Scheme == Uri.UriSchemeHttp))
            {
                throw new HtmlWebException("Unsupported uri scheme: '" + uri.Scheme + "'.");
            }
            this.Get(uri, method, path, null, null, null);
        }

        public void Get(string url, string path, WebProxy proxy, NetworkCredential credentials)
        {
            this.Get(url, path, proxy, credentials, "GET");
        }

        public void Get(string url, string path, WebProxy proxy, NetworkCredential credentials, string method)
        {
            Uri uri = new Uri(url);
            if (!(uri.Scheme == Uri.UriSchemeHttps) && !(uri.Scheme == Uri.UriSchemeHttp))
            {
                throw new HtmlWebException("Unsupported uri scheme: '" + uri.Scheme + "'.");
            }
            this.Get(uri, method, path, null, proxy, credentials);
        }

        private HttpStatusCode Get(Uri uri, string method, string path, HtmlDocument doc, IWebProxy proxy, ICredentials creds)
        {
            string cachePath = null;
            HttpWebResponse response;
            bool flag = false;
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = method;
            request.UserAgent = this.UserAgent;
            if (proxy != null)
            {
                if (creds != null)
                {
                    proxy.Credentials = creds;
                    request.Credentials = creds;
                }
                else
                {
                    proxy.Credentials = CredentialCache.DefaultCredentials;
                    request.Credentials = CredentialCache.DefaultCredentials;
                }
                request.Proxy = proxy;
            }
            this._fromCache = false;
            this._requestDuration = 0;
            int tickCount = Environment.TickCount;
            if (this.UsingCache)
            {
                cachePath = this.GetCachePath(request.RequestUri);
                if (System.IO.File.Exists(cachePath))
                {
                    request.IfModifiedSince = System.IO.File.GetLastAccessTime(cachePath);
                    flag = true;
                }
            }
            if (this._cacheOnly)
            {
                if (!System.IO.File.Exists(cachePath))
                {
                    throw new HtmlWebException("File was not found at cache path: '" + cachePath + "'");
                }
                if (path != null)
                {
                    IOLibrary.CopyAlways(cachePath, path);
                    if (cachePath != null)
                    {
                        System.IO.File.SetLastWriteTime(path, System.IO.File.GetLastWriteTime(cachePath));
                    }
                }
                this._fromCache = true;
                return HttpStatusCode.NotModified;
            }
            if (this._useCookies)
            {
                request.CookieContainer = new CookieContainer();
            }
            if ((this.PreRequest != null) && !this.PreRequest(request))
            {
                return HttpStatusCode.ResetContent;
            }
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException exception)
            {
                this._requestDuration = Environment.TickCount - tickCount;
                response = (HttpWebResponse)exception.Response;
                if (response == null)
                {
                    if (!flag)
                    {
                        throw;
                    }
                    if (path != null)
                    {
                        IOLibrary.CopyAlways(cachePath, path);
                        System.IO.File.SetLastWriteTime(path, System.IO.File.GetLastWriteTime(cachePath));
                    }
                    return HttpStatusCode.NotModified;
                }
            }
            catch (Exception)
            {
                this._requestDuration = Environment.TickCount - tickCount;
                throw;
            }
            if (this.PostResponse != null)
            {
                this.PostResponse(request, response);
            }
            this._requestDuration = Environment.TickCount - tickCount;
            this._responseUri = response.ResponseUri;
            bool flag2 = this.IsHtmlContent(response.ContentType);
            Encoding overrideEncoding = !string.IsNullOrEmpty(response.ContentEncoding) ? Encoding.GetEncoding(response.ContentEncoding) : null;
            if (this.OverrideEncoding != null)
            {
                overrideEncoding = this.OverrideEncoding;
            }
            if (response.StatusCode == HttpStatusCode.NotModified)
            {
                if (!this.UsingCache)
                {
                    throw new HtmlWebException("Server has send a NotModifed code, without cache enabled.");
                }
                this._fromCache = true;
                if (path != null)
                {
                    IOLibrary.CopyAlways(cachePath, path);
                    System.IO.File.SetLastWriteTime(path, System.IO.File.GetLastWriteTime(cachePath));
                }
                return response.StatusCode;
            }
            Stream responseStream = response.GetResponseStream();
            if (responseStream != null)
            {
                if (this.UsingCache)
                {
                    SaveStream(responseStream, cachePath, RemoveMilliseconds(response.LastModified), this._streamBufferSize);
                    this.SaveCacheHeaders(request.RequestUri, response);
                    if (path != null)
                    {
                        IOLibrary.CopyAlways(cachePath, path);
                        System.IO.File.SetLastWriteTime(path, System.IO.File.GetLastWriteTime(cachePath));
                    }
                }
                else if ((doc != null) && flag2)
                {
                    if (overrideEncoding == null)
                    {
                        doc.Load(responseStream, true);
                    }
                    else
                    {
                        doc.Load(responseStream, overrideEncoding);
                    }
                }
                response.Close();
            }
            return response.StatusCode;
        }

        private string GetCacheHeader(Uri requestUri, string name, string def)
        {
            XmlDocument document = new XmlDocument();
            document.Load(this.GetCacheHeadersPath(requestUri));
            XmlNode node = document.SelectSingleNode("//h[translate(@n, 'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ')='" + name.ToUpper() + "']");
            if (node == null)
            {
                return def;
            }
            return node.Attributes[name].Value;
        }

        private string GetCacheHeadersPath(Uri uri)
        {
            return (this.GetCachePath(uri) + ".h.xml");
        }

        public string GetCachePath(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            if (!this.UsingCache)
            {
                throw new HtmlWebException("Cache is not enabled. Set UsingCache to true first.");
            }
            if (uri.AbsolutePath == "/")
            {
                return Path.Combine(this._cachePath, ".htm");
            }
            return Path.Combine(this._cachePath, (uri.Host + uri.AbsolutePath).Replace('/', '\\'));
        }

        public static string GetContentTypeForExtension(string extension, string def)
        {
            PermissionHelper helper = new PermissionHelper();
            if (string.IsNullOrEmpty(extension))
            {
                return def;
            }
            string str = "";
            if (!helper.GetIsRegistryAvailable())
            {
                if (MimeTypes.ContainsKey(extension))
                {
                    str = MimeTypes[extension];
                }
                else
                {
                    str = def;
                }
            }
            if (!helper.GetIsDnsAvailable())
            {
                try
                {
                    RegistryKey key = Registry.ClassesRoot.OpenSubKey(extension, false);
                    if (key != null)
                    {
                        str = (string)key.GetValue("", def);
                    }
                }
                catch (Exception)
                {
                    str = def;
                }
            }
            return str;
        }

        public static string GetExtensionForContentType(string contentType, string def)
        {
            PermissionHelper helper = new PermissionHelper();
            if (string.IsNullOrEmpty(contentType))
            {
                return def;
            }
            string str = "";
            if (!helper.GetIsRegistryAvailable())
            {
                if (MimeTypes.ContainsValue(contentType))
                {
                    foreach (KeyValuePair<string, string> pair in MimeTypes)
                    {
                        if (pair.Value == contentType)
                        {
                            return pair.Value;
                        }
                    }
                }
                return def;
            }
            if (helper.GetIsRegistryAvailable())
            {
                try
                {
                    RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + contentType, false);
                    if (key != null)
                    {
                        str = (string)key.GetValue("Extension", def);
                    }
                }
                catch (Exception)
                {
                    str = def;
                }
            }
            return str;
        }

        private bool IsCacheHtmlContent(string path)
        {
            string contentTypeForExtension = GetContentTypeForExtension(Path.GetExtension(path), null);
            return this.IsHtmlContent(contentTypeForExtension);
        }

        private bool IsGZipEncoding(string contentEncoding)
        {
            return contentEncoding.ToLower().StartsWith("gzip");
        }

        private bool IsHtmlContent(string contentType)
        {
            return contentType.ToLower().StartsWith("text/html");
        }

        public HtmlDocument Load(string url)
        {
            return this.Load(url, "GET");
        }

        public HtmlDocument Load(string url, string method)
        {
            HtmlDocument document;
            Uri uri = new Uri(url);
            if ((uri.Scheme == Uri.UriSchemeHttps) || (uri.Scheme == Uri.UriSchemeHttp))
            {
                document = this.LoadUrl(uri, method, null, null);
            }
            else
            {
                if (!(uri.Scheme == Uri.UriSchemeFile))
                {
                    throw new HtmlWebException("Unsupported uri scheme: '" + uri.Scheme + "'.");
                }
                document = new HtmlDocument
                {
                    //OptionAutoCloseOnEnd = false,
                    OptionAutoCloseOnEnd = true
                };
                if (this.OverrideEncoding != null)
                {
                    document.Load(url, this.OverrideEncoding);
                }
                else
                {
                    document.DetectEncodingAndLoad(url, this._autoDetectEncoding);
                }
            }
            if (this.PreHandleDocument != null)
            {
                this.PreHandleDocument(document);
            }
            return document;
        }

        public HtmlDocument Load(string url, string method, WebProxy proxy, NetworkCredential credentials)
        {
            HtmlDocument document;
            Uri uri = new Uri(url);
            if ((uri.Scheme == Uri.UriSchemeHttps) || (uri.Scheme == Uri.UriSchemeHttp))
            {
                document = this.LoadUrl(uri, method, proxy, credentials);
            }
            else
            {
                if (uri.Scheme != Uri.UriSchemeFile)
                {
                    throw new HtmlWebException("Unsupported uri scheme: '" + uri.Scheme + "'.");
                }
                document = new HtmlDocument
                {
                    //OptionAutoCloseOnEnd = false,
                    OptionAutoCloseOnEnd = true
                };
                document.DetectEncodingAndLoad(url, this._autoDetectEncoding);
            }
            if (this.PreHandleDocument != null)
            {
                this.PreHandleDocument(document);
            }
            return document;
        }

        public HtmlDocument Load(string url, string proxyHost, int proxyPort, string userId, string password)
        {
            WebProxy proxy = new WebProxy(proxyHost, proxyPort)
            {
                BypassProxyOnLocal = true
            };
            NetworkCredential cred = null;
            if ((userId != null) && (password != null))
            {
                cred = new NetworkCredential(userId, password);
                CredentialCache cache = new CredentialCache();
                cache.Add(proxy.Address, "Basic", cred);
                cache.Add(proxy.Address, "Digest", cred);
            }
            return this.Load(url, "GET", proxy, cred);
        }

        public void LoadHtmlAsXml(string htmlUrl, XmlTextWriter writer)
        {
            this.Load(htmlUrl).Save(writer);
        }

        public void LoadHtmlAsXml(string htmlUrl, string xsltUrl, XsltArgumentList xsltArgs, XmlTextWriter writer)
        {
            this.LoadHtmlAsXml(htmlUrl, xsltUrl, xsltArgs, writer, null);
        }

        public void LoadHtmlAsXml(string htmlUrl, string xsltUrl, XsltArgumentList xsltArgs, XmlTextWriter writer, string xmlPath)
        {
            if (htmlUrl == null)
            {
                throw new ArgumentNullException("htmlUrl");
            }
            HtmlDocument document = this.Load(htmlUrl);
            if (xmlPath != null)
            {
                XmlTextWriter writer2 = new XmlTextWriter(xmlPath, document.Encoding);
                document.Save(writer2);
                writer2.Close();
            }
            if (xsltArgs == null)
            {
                xsltArgs = new XsltArgumentList();
            }
            xsltArgs.AddParam("url", "", htmlUrl);
            xsltArgs.AddParam("requestDuration", "", this.RequestDuration);
            xsltArgs.AddParam("fromCache", "", this.FromCache);
            XslCompiledTransform transform = new XslCompiledTransform();
            transform.Load(xsltUrl);
            transform.Transform((IXPathNavigable)document, xsltArgs, (XmlWriter)writer);
        }

        private HtmlDocument LoadUrl(Uri uri, string method, WebProxy proxy, NetworkCredential creds)
        {
            HtmlDocument doc = new HtmlDocument
            {
                OptionAutoCloseOnEnd = false,
                OptionFixNestedTags = true
            };
            this._statusCode = this.Get(uri, method, null, doc, proxy, creds);
            if (this._statusCode == HttpStatusCode.NotModified)
            {
                doc.DetectEncodingAndLoad(this.GetCachePath(uri));
            }
            return doc;
        }

        private static DateTime RemoveMilliseconds(DateTime t)
        {
            return new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, 0);
        }

        private void SaveCacheHeaders(Uri requestUri, HttpWebResponse resp)
        {
            string cacheHeadersPath = this.GetCacheHeadersPath(requestUri);
            XmlDocument document = new XmlDocument();
            document.LoadXml("<c></c>");
            XmlNode firstChild = document.FirstChild;
            foreach (string str2 in resp.Headers)
            {
                XmlNode newChild = document.CreateElement("h");
                XmlAttribute node = document.CreateAttribute("n");
                node.Value = str2;
                newChild.Attributes.Append(node);
                node = document.CreateAttribute("v");
                node.Value = resp.Headers[str2];
                newChild.Attributes.Append(node);
                firstChild.AppendChild(newChild);
            }
            document.Save(cacheHeadersPath);
        }

        private static long SaveStream(Stream stream, string path, DateTime touchDate, int streamBufferSize)
        {
            FilePreparePath(path);
            FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write);
            BinaryReader reader = null;
            BinaryWriter writer = null;
            long num = 0L;
            try
            {
                byte[] buffer;
                reader = new BinaryReader(stream);
                writer = new BinaryWriter(output);
                do
                {
                    buffer = reader.ReadBytes(streamBufferSize);
                    num += buffer.Length;
                    if (buffer.Length > 0)
                    {
                        writer.Write(buffer);
                    }
                }
                while (buffer.Length > 0);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                }
                if (output != null)
                {
                    output.Close();
                }
            }
            System.IO.File.SetLastWriteTime(path, touchDate);
            return num;
        }

        public bool AutoDetectEncoding
        {
            get
            {
                return this._autoDetectEncoding;
            }
            set
            {
                this._autoDetectEncoding = value;
            }
        }

        public bool CacheOnly
        {
            get
            {
                return this._cacheOnly;
            }
            set
            {
                if (value && !this.UsingCache)
                {
                    throw new HtmlWebException("Cache is not enabled. Set UsingCache to true first.");
                }
                this._cacheOnly = value;
            }
        }

        public string CachePath
        {
            get
            {
                return this._cachePath;
            }
            set
            {
                this._cachePath = value;
            }
        }

        public bool FromCache
        {
            get
            {
                return this._fromCache;
            }
        }

        internal static Dictionary<string, string> MimeTypes
        {
            get
            {
                if (_mimeTypes == null)
                {
                    _mimeTypes = new Dictionary<string, string>();
                    _mimeTypes.Add(".3dm", "x-world/x-3dmf");
                    _mimeTypes.Add(".3dmf", "x-world/x-3dmf");
                    _mimeTypes.Add(".a", "application/octet-stream");
                    _mimeTypes.Add(".aab", "application/x-authorware-bin");
                    _mimeTypes.Add(".aam", "application/x-authorware-map");
                    _mimeTypes.Add(".aas", "application/x-authorware-seg");
                    _mimeTypes.Add(".abc", "text/vnd.abc");
                    _mimeTypes.Add(".acgi", "text/html");
                    _mimeTypes.Add(".afl", "video/animaflex");
                    _mimeTypes.Add(".ai", "application/postscript");
                    _mimeTypes.Add(".aif", "audio/aiff");
                    _mimeTypes.Add(".aif", "audio/x-aiff");
                    _mimeTypes.Add(".aifc", "audio/aiff");
                    _mimeTypes.Add(".aifc", "audio/x-aiff");
                    _mimeTypes.Add(".aiff", "audio/aiff");
                    _mimeTypes.Add(".aiff", "audio/x-aiff");
                    _mimeTypes.Add(".aim", "application/x-aim");
                    _mimeTypes.Add(".aip", "text/x-audiosoft-intra");
                    _mimeTypes.Add(".ani", "application/x-navi-animation");
                    _mimeTypes.Add(".aos", "application/x-nokia-9000-communicator-add-on-software");
                    _mimeTypes.Add(".aps", "application/mime");
                    _mimeTypes.Add(".arc", "application/octet-stream");
                    _mimeTypes.Add(".arj", "application/arj");
                    _mimeTypes.Add(".arj", "application/octet-stream");
                    _mimeTypes.Add(".art", "image/x-jg");
                    _mimeTypes.Add(".asf", "video/x-ms-asf");
                    _mimeTypes.Add(".asm", "text/x-asm");
                    _mimeTypes.Add(".asp", "text/asp");
                    _mimeTypes.Add(".asx", "application/x-mplayer2");
                    _mimeTypes.Add(".asx", "video/x-ms-asf");
                    _mimeTypes.Add(".asx", "video/x-ms-asf-plugin");
                    _mimeTypes.Add(".au", "audio/basic");
                    _mimeTypes.Add(".au", "audio/x-au");
                    _mimeTypes.Add(".avi", "application/x-troff-msvideo");
                    _mimeTypes.Add(".avi", "video/avi");
                    _mimeTypes.Add(".avi", "video/msvideo");
                    _mimeTypes.Add(".avi", "video/x-msvideo");
                    _mimeTypes.Add(".avs", "video/avs-video");
                    _mimeTypes.Add(".bcpio", "application/x-bcpio");
                    _mimeTypes.Add(".bin", "application/mac-binary");
                    _mimeTypes.Add(".bin", "application/macbinary");
                    _mimeTypes.Add(".bin", "application/octet-stream");
                    _mimeTypes.Add(".bin", "application/x-binary");
                    _mimeTypes.Add(".bin", "application/x-macbinary");
                    _mimeTypes.Add(".bm", "image/bmp");
                    _mimeTypes.Add(".bmp", "image/bmp");
                    _mimeTypes.Add(".bmp", "image/x-windows-bmp");
                    _mimeTypes.Add(".boo", "application/book");
                    _mimeTypes.Add(".book", "application/book");
                    _mimeTypes.Add(".boz", "application/x-bzip2");
                    _mimeTypes.Add(".bsh", "application/x-bsh");
                    _mimeTypes.Add(".bz", "application/x-bzip");
                    _mimeTypes.Add(".bz2", "application/x-bzip2");
                    _mimeTypes.Add(".c", "text/plain");
                    _mimeTypes.Add(".c", "text/x-c");
                    _mimeTypes.Add(".c++", "text/plain");
                    _mimeTypes.Add(".cat", "application/vnd.ms-pki.seccat");
                    _mimeTypes.Add(".cc", "text/plain");
                    _mimeTypes.Add(".cc", "text/x-c");
                    _mimeTypes.Add(".ccad", "application/clariscad");
                    _mimeTypes.Add(".cco", "application/x-cocoa");
                    _mimeTypes.Add(".cdf", "application/cdf");
                    _mimeTypes.Add(".cdf", "application/x-cdf");
                    _mimeTypes.Add(".cdf", "application/x-netcdf");
                    _mimeTypes.Add(".cer", "application/pkix-cert");
                    _mimeTypes.Add(".cer", "application/x-x509-ca-cert");
                    _mimeTypes.Add(".cha", "application/x-chat");
                    _mimeTypes.Add(".chat", "application/x-chat");
                    _mimeTypes.Add(".class", "application/java");
                    _mimeTypes.Add(".class", "application/java-byte-code");
                    _mimeTypes.Add(".class", "application/x-java-class");
                    _mimeTypes.Add(".com", "application/octet-stream");
                    _mimeTypes.Add(".com", "text/plain");
                    _mimeTypes.Add(".conf", "text/plain");
                    _mimeTypes.Add(".cpio", "application/x-cpio");
                    _mimeTypes.Add(".cpp", "text/x-c");
                    _mimeTypes.Add(".cpt", "application/mac-compactpro");
                    _mimeTypes.Add(".cpt", "application/x-compactpro");
                    _mimeTypes.Add(".cpt", "application/x-cpt");
                    _mimeTypes.Add(".crl", "application/pkcs-crl");
                    _mimeTypes.Add(".crl", "application/pkix-crl");
                    _mimeTypes.Add(".crt", "application/pkix-cert");
                    _mimeTypes.Add(".crt", "application/x-x509-ca-cert");
                    _mimeTypes.Add(".crt", "application/x-x509-user-cert");
                    _mimeTypes.Add(".csh", "application/x-csh");
                    _mimeTypes.Add(".csh", "text/x-script.csh");
                    _mimeTypes.Add(".css", "application/x-pointplus");
                    _mimeTypes.Add(".css", "text/css");
                    _mimeTypes.Add(".cxx", "text/plain");
                    _mimeTypes.Add(".dcr", "application/x-director");
                    _mimeTypes.Add(".deepv", "application/x-deepv");
                    _mimeTypes.Add(".def", "text/plain");
                    _mimeTypes.Add(".der", "application/x-x509-ca-cert");
                    _mimeTypes.Add(".dif", "video/x-dv");
                    _mimeTypes.Add(".dir", "application/x-director");
                    _mimeTypes.Add(".dl", "video/dl");
                    _mimeTypes.Add(".dl", "video/x-dl");
                    _mimeTypes.Add(".doc", "application/msword");
                    _mimeTypes.Add(".dot", "application/msword");
                    _mimeTypes.Add(".dp", "application/commonground");
                    _mimeTypes.Add(".drw", "application/drafting");
                    _mimeTypes.Add(".dump", "application/octet-stream");
                    _mimeTypes.Add(".dv", "video/x-dv");
                    _mimeTypes.Add(".dvi", "application/x-dvi");
                    _mimeTypes.Add(".dwf", "model/vnd.dwf");
                    _mimeTypes.Add(".dwg", "application/acad");
                    _mimeTypes.Add(".dwg", "image/vnd.dwg");
                    _mimeTypes.Add(".dwg", "image/x-dwg");
                    _mimeTypes.Add(".dxf", "application/dxf");
                    _mimeTypes.Add(".dxf", "image/vnd.dwg");
                    _mimeTypes.Add(".dxf", "image/x-dwg");
                    _mimeTypes.Add(".dxr", "application/x-director");
                    _mimeTypes.Add(".el", "text/x-script.elisp");
                    _mimeTypes.Add(".elc", "application/x-bytecode.elisp");
                    _mimeTypes.Add(".elc", "application/x-elc");
                    _mimeTypes.Add(".env", "application/x-envoy");
                    _mimeTypes.Add(".eps", "application/postscript");
                    _mimeTypes.Add(".es", "application/x-esrehber");
                    _mimeTypes.Add(".etx", "text/x-setext");
                    _mimeTypes.Add(".evy", "application/envoy");
                    _mimeTypes.Add(".evy", "application/x-envoy");
                    _mimeTypes.Add(".exe", "application/octet-stream");
                    _mimeTypes.Add(".f", "text/plain");
                    _mimeTypes.Add(".f", "text/x-fortran");
                    _mimeTypes.Add(".f77", "text/x-fortran");
                    _mimeTypes.Add(".f90", "text/plain");
                    _mimeTypes.Add(".f90", "text/x-fortran");
                    _mimeTypes.Add(".fdf", "application/vnd.fdf");
                    _mimeTypes.Add(".fif", "application/fractals");
                    _mimeTypes.Add(".fif", "image/fif");
                    _mimeTypes.Add(".fli", "video/fli");
                    _mimeTypes.Add(".fli", "video/x-fli");
                    _mimeTypes.Add(".flo", "image/florian");
                    _mimeTypes.Add(".flx", "text/vnd.fmi.flexstor");
                    _mimeTypes.Add(".fmf", "video/x-atomic3d-feature");
                    _mimeTypes.Add(".for", "text/plain");
                    _mimeTypes.Add(".for", "text/x-fortran");
                    _mimeTypes.Add(".fpx", "image/vnd.fpx");
                    _mimeTypes.Add(".fpx", "image/vnd.net-fpx");
                    _mimeTypes.Add(".frl", "application/freeloader");
                    _mimeTypes.Add(".funk", "audio/make");
                    _mimeTypes.Add(".g", "text/plain");
                    _mimeTypes.Add(".g3", "image/g3fax");
                    _mimeTypes.Add(".gif", "image/gif");
                    _mimeTypes.Add(".gl", "video/gl");
                    _mimeTypes.Add(".gl", "video/x-gl");
                    _mimeTypes.Add(".gsd", "audio/x-gsm");
                    _mimeTypes.Add(".gsm", "audio/x-gsm");
                    _mimeTypes.Add(".gsp", "application/x-gsp");
                    _mimeTypes.Add(".gss", "application/x-gss");
                    _mimeTypes.Add(".gtar", "application/x-gtar");
                    _mimeTypes.Add(".gz", "application/x-compressed");
                    _mimeTypes.Add(".gz", "application/x-gzip");
                    _mimeTypes.Add(".gzip", "application/x-gzip");
                    _mimeTypes.Add(".gzip", "multipart/x-gzip");
                    _mimeTypes.Add(".h", "text/plain");
                    _mimeTypes.Add(".h", "text/x-h");
                    _mimeTypes.Add(".hdf", "application/x-hdf");
                    _mimeTypes.Add(".help", "application/x-helpfile");
                    _mimeTypes.Add(".hgl", "application/vnd.hp-hpgl");
                    _mimeTypes.Add(".hh", "text/plain");
                    _mimeTypes.Add(".hh", "text/x-h");
                    _mimeTypes.Add(".hlb", "text/x-script");
                    _mimeTypes.Add(".hlp", "application/hlp");
                    _mimeTypes.Add(".hlp", "application/x-helpfile");
                    _mimeTypes.Add(".hlp", "application/x-winhelp");
                    _mimeTypes.Add(".hpg", "application/vnd.hp-hpgl");
                    _mimeTypes.Add(".hpgl", "application/vnd.hp-hpgl");
                    _mimeTypes.Add(".hqx", "application/binhex");
                    _mimeTypes.Add(".hqx", "application/binhex4");
                    _mimeTypes.Add(".hqx", "application/mac-binhex");
                    _mimeTypes.Add(".hqx", "application/mac-binhex40");
                    _mimeTypes.Add(".hqx", "application/x-binhex40");
                    _mimeTypes.Add(".hqx", "application/x-mac-binhex40");
                    _mimeTypes.Add(".hta", "application/hta");
                    _mimeTypes.Add(".htc", "text/x-component");
                    _mimeTypes.Add(".htm", "text/html");
                    _mimeTypes.Add(".html", "text/html");
                    _mimeTypes.Add(".htmls", "text/html");
                    _mimeTypes.Add(".htt", "text/webviewhtml");
                    _mimeTypes.Add(".htx", "text/html");
                    _mimeTypes.Add(".ice", "x-conference/x-cooltalk");
                    _mimeTypes.Add(".ico", "image/x-icon");
                    _mimeTypes.Add(".idc", "text/plain");
                    _mimeTypes.Add(".ief", "image/ief");
                    _mimeTypes.Add(".iefs", "image/ief");
                    _mimeTypes.Add(".iges", "application/iges");
                    _mimeTypes.Add(".iges", "model/iges");
                    _mimeTypes.Add(".igs", "application/iges");
                    _mimeTypes.Add(".igs", "model/iges");
                    _mimeTypes.Add(".ima", "application/x-ima");
                    _mimeTypes.Add(".imap", "application/x-httpd-imap");
                    _mimeTypes.Add(".inf", "application/inf");
                    _mimeTypes.Add(".ins", "application/x-internett-signup");
                    _mimeTypes.Add(".ip", "application/x-ip2");
                    _mimeTypes.Add(".isu", "video/x-isvideo");
                    _mimeTypes.Add(".it", "audio/it");
                    _mimeTypes.Add(".iv", "application/x-inventor");
                    _mimeTypes.Add(".ivr", "i-world/i-vrml");
                    _mimeTypes.Add(".ivy", "application/x-livescreen");
                    _mimeTypes.Add(".jam", "audio/x-jam");
                    _mimeTypes.Add(".jav", "text/plain");
                    _mimeTypes.Add(".jav", "text/x-java-source");
                    _mimeTypes.Add(".java", "text/plain");
                    _mimeTypes.Add(".java", "text/x-java-source");
                    _mimeTypes.Add(".jcm", "application/x-java-commerce");
                    _mimeTypes.Add(".jfif", "image/jpeg");
                    _mimeTypes.Add(".jfif", "image/pjpeg");
                    _mimeTypes.Add(".jfif-tbnl", "image/jpeg");
                    _mimeTypes.Add(".jpe", "image/jpeg");
                    _mimeTypes.Add(".jpe", "image/pjpeg");
                    _mimeTypes.Add(".jpeg", "image/jpeg");
                    _mimeTypes.Add(".jpeg", "image/pjpeg");
                    _mimeTypes.Add(".jpg", "image/jpeg");
                    _mimeTypes.Add(".jpg", "image/pjpeg");
                    _mimeTypes.Add(".jps", "image/x-jps");
                    _mimeTypes.Add(".js", "application/x-javascript");
                    _mimeTypes.Add(".js", "application/javascript");
                    _mimeTypes.Add(".js", "application/ecmascript");
                    _mimeTypes.Add(".js", "text/javascript");
                    _mimeTypes.Add(".js", "text/ecmascript");
                    _mimeTypes.Add(".jut", "image/jutvision");
                    _mimeTypes.Add(".kar", "audio/midi");
                    _mimeTypes.Add(".kar", "music/x-karaoke");
                    _mimeTypes.Add(".ksh", "application/x-ksh");
                    _mimeTypes.Add(".ksh", "text/x-script.ksh");
                    _mimeTypes.Add(".la", "audio/nspaudio");
                    _mimeTypes.Add(".la", "audio/x-nspaudio");
                    _mimeTypes.Add(".lam", "audio/x-liveaudio");
                    _mimeTypes.Add(".latex", "application/x-latex");
                    _mimeTypes.Add(".lha", "application/lha");
                    _mimeTypes.Add(".lha", "application/octet-stream");
                    _mimeTypes.Add(".lha", "application/x-lha");
                    _mimeTypes.Add(".lhx", "application/octet-stream");
                    _mimeTypes.Add(".list", "text/plain");
                    _mimeTypes.Add(".lma", "audio/nspaudio");
                    _mimeTypes.Add(".lma", "audio/x-nspaudio");
                    _mimeTypes.Add(".log", "text/plain");
                    _mimeTypes.Add(".lsp", "application/x-lisp");
                    _mimeTypes.Add(".lsp", "text/x-script.lisp");
                    _mimeTypes.Add(".lst", "text/plain");
                    _mimeTypes.Add(".lsx", "text/x-la-asf");
                    _mimeTypes.Add(".ltx", "application/x-latex");
                    _mimeTypes.Add(".lzh", "application/octet-stream");
                    _mimeTypes.Add(".lzh", "application/x-lzh");
                    _mimeTypes.Add(".lzx", "application/lzx");
                    _mimeTypes.Add(".lzx", "application/octet-stream");
                    _mimeTypes.Add(".lzx", "application/x-lzx");
                    _mimeTypes.Add(".m", "text/plain");
                    _mimeTypes.Add(".m", "text/x-m");
                    _mimeTypes.Add(".m1v", "video/mpeg");
                    _mimeTypes.Add(".m2a", "audio/mpeg");
                    _mimeTypes.Add(".m2v", "video/mpeg");
                    _mimeTypes.Add(".m3u", "audio/x-mpequrl");
                    _mimeTypes.Add(".man", "application/x-troff-man");
                    _mimeTypes.Add(".map", "application/x-navimap");
                    _mimeTypes.Add(".mar", "text/plain");
                    _mimeTypes.Add(".mbd", "application/mbedlet");
                    _mimeTypes.Add(".mc$", "application/x-magic-cap-package-1.0");
                    _mimeTypes.Add(".mcd", "application/mcad");
                    _mimeTypes.Add(".mcd", "application/x-mathcad");
                    _mimeTypes.Add(".mcf", "image/vasa");
                    _mimeTypes.Add(".mcf", "text/mcf");
                    _mimeTypes.Add(".mcp", "application/netmc");
                    _mimeTypes.Add(".me", "application/x-troff-me");
                    _mimeTypes.Add(".mht", "message/rfc822");
                    _mimeTypes.Add(".mhtml", "message/rfc822");
                    _mimeTypes.Add(".mid", "application/x-midi");
                    _mimeTypes.Add(".mid", "audio/midi");
                    _mimeTypes.Add(".mid", "audio/x-mid");
                    _mimeTypes.Add(".mid", "audio/x-midi");
                    _mimeTypes.Add(".mid", "music/crescendo");
                    _mimeTypes.Add(".mid", "x-music/x-midi");
                    _mimeTypes.Add(".midi", "application/x-midi");
                    _mimeTypes.Add(".midi", "audio/midi");
                    _mimeTypes.Add(".midi", "audio/x-mid");
                    _mimeTypes.Add(".midi", "audio/x-midi");
                    _mimeTypes.Add(".midi", "music/crescendo");
                    _mimeTypes.Add(".midi", "x-music/x-midi");
                    _mimeTypes.Add(".mif", "application/x-frame");
                    _mimeTypes.Add(".mif", "application/x-mif");
                    _mimeTypes.Add(".mime", "message/rfc822");
                    _mimeTypes.Add(".mime", "www/mime");
                    _mimeTypes.Add(".mjf", "audio/x-vnd.audioexplosion.mjuicemediafile");
                    _mimeTypes.Add(".mjpg", "video/x-motion-jpeg");
                    _mimeTypes.Add(".mm", "application/base64");
                    _mimeTypes.Add(".mm", "application/x-meme");
                    _mimeTypes.Add(".mme", "application/base64");
                    _mimeTypes.Add(".mod", "audio/mod");
                    _mimeTypes.Add(".mod", "audio/x-mod");
                    _mimeTypes.Add(".moov", "video/quicktime");
                    _mimeTypes.Add(".mov", "video/quicktime");
                    _mimeTypes.Add(".movie", "video/x-sgi-movie");
                    _mimeTypes.Add(".mp2", "audio/mpeg");
                    _mimeTypes.Add(".mp2", "audio/x-mpeg");
                    _mimeTypes.Add(".mp2", "video/mpeg");
                    _mimeTypes.Add(".mp2", "video/x-mpeg");
                    _mimeTypes.Add(".mp2", "video/x-mpeq2a");
                    _mimeTypes.Add(".mp3", "audio/mpeg3");
                    _mimeTypes.Add(".mp3", "audio/x-mpeg-3");
                    _mimeTypes.Add(".mp3", "video/mpeg");
                    _mimeTypes.Add(".mp3", "video/x-mpeg");
                    _mimeTypes.Add(".mpa", "audio/mpeg");
                    _mimeTypes.Add(".mpa", "video/mpeg");
                    _mimeTypes.Add(".mpc", "application/x-project");
                    _mimeTypes.Add(".mpe", "video/mpeg");
                    _mimeTypes.Add(".mpeg", "video/mpeg");
                    _mimeTypes.Add(".mpg", "audio/mpeg");
                    _mimeTypes.Add(".mpg", "video/mpeg");
                    _mimeTypes.Add(".mpga", "audio/mpeg");
                    _mimeTypes.Add(".mpp", "application/vnd.ms-project");
                    _mimeTypes.Add(".mpt", "application/x-project");
                    _mimeTypes.Add(".mpv", "application/x-project");
                    _mimeTypes.Add(".mpx", "application/x-project");
                    _mimeTypes.Add(".mrc", "application/marc");
                    _mimeTypes.Add(".ms", "application/x-troff-ms");
                    _mimeTypes.Add(".mv", "video/x-sgi-movie");
                    _mimeTypes.Add(".my", "audio/make");
                    _mimeTypes.Add(".mzz", "application/x-vnd.audioexplosion.mzz");
                    _mimeTypes.Add(".nap", "image/naplps");
                    _mimeTypes.Add(".naplps", "image/naplps");
                    _mimeTypes.Add(".nc", "application/x-netcdf");
                    _mimeTypes.Add(".ncm", "application/vnd.nokia.configuration-message");
                    _mimeTypes.Add(".nif", "image/x-niff");
                    _mimeTypes.Add(".niff", "image/x-niff");
                    _mimeTypes.Add(".nix", "application/x-mix-transfer");
                    _mimeTypes.Add(".nsc", "application/x-conference");
                    _mimeTypes.Add(".nvd", "application/x-navidoc");
                    _mimeTypes.Add(".o", "application/octet-stream");
                    _mimeTypes.Add(".oda", "application/oda");
                    _mimeTypes.Add(".omc", "application/x-omc");
                    _mimeTypes.Add(".omcd", "application/x-omcdatamaker");
                    _mimeTypes.Add(".omcr", "application/x-omcregerator");
                    _mimeTypes.Add(".p", "text/x-pascal");
                    _mimeTypes.Add(".p10", "application/pkcs10");
                    _mimeTypes.Add(".p10", "application/x-pkcs10");
                    _mimeTypes.Add(".p12", "application/pkcs-12");
                    _mimeTypes.Add(".p12", "application/x-pkcs12");
                    _mimeTypes.Add(".p7a", "application/x-pkcs7-signature");
                    _mimeTypes.Add(".p7c", "application/pkcs7-mime");
                    _mimeTypes.Add(".p7c", "application/x-pkcs7-mime");
                    _mimeTypes.Add(".p7m", "application/pkcs7-mime");
                    _mimeTypes.Add(".p7m", "application/x-pkcs7-mime");
                    _mimeTypes.Add(".p7r", "application/x-pkcs7-certreqresp");
                    _mimeTypes.Add(".p7s", "application/pkcs7-signature");
                    _mimeTypes.Add(".part", "application/pro_eng");
                    _mimeTypes.Add(".pas", "text/pascal");
                    _mimeTypes.Add(".pbm", "image/x-portable-bitmap");
                    _mimeTypes.Add(".pcl", "application/vnd.hp-pcl");
                    _mimeTypes.Add(".pcl", "application/x-pcl");
                    _mimeTypes.Add(".pct", "image/x-pict");
                    _mimeTypes.Add(".pcx", "image/x-pcx");
                    _mimeTypes.Add(".pdb", "chemical/x-pdb");
                    _mimeTypes.Add(".pdf", "application/pdf");
                    _mimeTypes.Add(".pfunk", "audio/make");
                    _mimeTypes.Add(".pfunk", "audio/make.my.funk");
                    _mimeTypes.Add(".pgm", "image/x-portable-graymap");
                    _mimeTypes.Add(".pgm", "image/x-portable-greymap");
                    _mimeTypes.Add(".pic", "image/pict");
                    _mimeTypes.Add(".pict", "image/pict");
                    _mimeTypes.Add(".pkg", "application/x-newton-compatible-pkg");
                    _mimeTypes.Add(".pko", "application/vnd.ms-pki.pko");
                    _mimeTypes.Add(".pl", "text/plain");
                    _mimeTypes.Add(".pl", "text/x-script.perl");
                    _mimeTypes.Add(".plx", "application/x-pixclscript");
                    _mimeTypes.Add(".pm", "image/x-xpixmap");
                    _mimeTypes.Add(".pm", "text/x-script.perl-module");
                    _mimeTypes.Add(".pm4", "application/x-pagemaker");
                    _mimeTypes.Add(".pm5", "application/x-pagemaker");
                    _mimeTypes.Add(".png", "image/png");
                    _mimeTypes.Add(".pnm", "application/x-portable-anymap");
                    _mimeTypes.Add(".pnm", "image/x-portable-anymap");
                    _mimeTypes.Add(".pot", "application/mspowerpoint");
                    _mimeTypes.Add(".pot", "application/vnd.ms-powerpoint");
                    _mimeTypes.Add(".pov", "model/x-pov");
                    _mimeTypes.Add(".ppa", "application/vnd.ms-powerpoint");
                    _mimeTypes.Add(".ppm", "image/x-portable-pixmap");
                    _mimeTypes.Add(".pps", "application/mspowerpoint");
                    _mimeTypes.Add(".pps", "application/vnd.ms-powerpoint");
                    _mimeTypes.Add(".ppt", "application/mspowerpoint");
                    _mimeTypes.Add(".ppt", "application/powerpoint");
                    _mimeTypes.Add(".ppt", "application/vnd.ms-powerpoint");
                    _mimeTypes.Add(".ppt", "application/x-mspowerpoint");
                    _mimeTypes.Add(".ppz", "application/mspowerpoint");
                    _mimeTypes.Add(".pre", "application/x-freelance");
                    _mimeTypes.Add(".prt", "application/pro_eng");
                    _mimeTypes.Add(".ps", "application/postscript");
                    _mimeTypes.Add(".psd", "application/octet-stream");
                    _mimeTypes.Add(".pvu", "paleovu/x-pv");
                    _mimeTypes.Add(".pwz", "application/vnd.ms-powerpoint");
                    _mimeTypes.Add(".py", "text/x-script.phyton");
                    _mimeTypes.Add(".pyc", "applicaiton/x-bytecode.python");
                    _mimeTypes.Add(".qcp", "audio/vnd.qcelp");
                    _mimeTypes.Add(".qd3", "x-world/x-3dmf");
                    _mimeTypes.Add(".qd3d", "x-world/x-3dmf");
                    _mimeTypes.Add(".qif", "image/x-quicktime");
                    _mimeTypes.Add(".qt", "video/quicktime");
                    _mimeTypes.Add(".qtc", "video/x-qtc");
                    _mimeTypes.Add(".qti", "image/x-quicktime");
                    _mimeTypes.Add(".qtif", "image/x-quicktime");
                    _mimeTypes.Add(".ra", "audio/x-pn-realaudio");
                    _mimeTypes.Add(".ra", "audio/x-pn-realaudio-plugin");
                    _mimeTypes.Add(".ra", "audio/x-realaudio");
                    _mimeTypes.Add(".ram", "audio/x-pn-realaudio");
                    _mimeTypes.Add(".ras", "application/x-cmu-raster");
                    _mimeTypes.Add(".ras", "image/cmu-raster");
                    _mimeTypes.Add(".ras", "image/x-cmu-raster");
                    _mimeTypes.Add(".rast", "image/cmu-raster");
                    _mimeTypes.Add(".rexx", "text/x-script.rexx");
                    _mimeTypes.Add(".rf", "image/vnd.rn-realflash");
                    _mimeTypes.Add(".rgb", "image/x-rgb");
                    _mimeTypes.Add(".rm", "application/vnd.rn-realmedia");
                    _mimeTypes.Add(".rm", "audio/x-pn-realaudio");
                    _mimeTypes.Add(".rmi", "audio/mid");
                    _mimeTypes.Add(".rmm", "audio/x-pn-realaudio");
                    _mimeTypes.Add(".rmp", "audio/x-pn-realaudio");
                    _mimeTypes.Add(".rmp", "audio/x-pn-realaudio-plugin");
                    _mimeTypes.Add(".rng", "application/ringing-tones");
                    _mimeTypes.Add(".rng", "application/vnd.nokia.ringing-tone");
                    _mimeTypes.Add(".rnx", "application/vnd.rn-realplayer");
                    _mimeTypes.Add(".roff", "application/x-troff");
                    _mimeTypes.Add(".rp", "image/vnd.rn-realpix");
                    _mimeTypes.Add(".rpm", "audio/x-pn-realaudio-plugin");
                    _mimeTypes.Add(".rt", "text/richtext");
                    _mimeTypes.Add(".rt", "text/vnd.rn-realtext");
                    _mimeTypes.Add(".rtf", "application/rtf");
                    _mimeTypes.Add(".rtf", "application/x-rtf");
                    _mimeTypes.Add(".rtf", "text/richtext");
                    _mimeTypes.Add(".rtx", "application/rtf");
                    _mimeTypes.Add(".rtx", "text/richtext");
                    _mimeTypes.Add(".rv", "video/vnd.rn-realvideo");
                    _mimeTypes.Add(".s", "text/x-asm");
                    _mimeTypes.Add(".s3m", "audio/s3m");
                    _mimeTypes.Add(".saveme", "application/octet-stream");
                    _mimeTypes.Add(".sbk", "application/x-tbook");
                    _mimeTypes.Add(".scm", "application/x-lotusscreencam");
                    _mimeTypes.Add(".scm", "text/x-script.guile");
                    _mimeTypes.Add(".scm", "text/x-script.scheme");
                    _mimeTypes.Add(".scm", "video/x-scm");
                    _mimeTypes.Add(".sdml", "text/plain");
                    _mimeTypes.Add(".sdp", "application/sdp");
                    _mimeTypes.Add(".sdp", "application/x-sdp");
                    _mimeTypes.Add(".sdr", "application/sounder");
                    _mimeTypes.Add(".sea", "application/sea");
                    _mimeTypes.Add(".sea", "application/x-sea");
                    _mimeTypes.Add(".set", "application/set");
                    _mimeTypes.Add(".sgm", "text/sgml");
                    _mimeTypes.Add(".sgm", "text/x-sgml");
                    _mimeTypes.Add(".sgml", "text/sgml");
                    _mimeTypes.Add(".sgml", "text/x-sgml");
                    _mimeTypes.Add(".sh", "application/x-bsh");
                    _mimeTypes.Add(".sh", "application/x-sh");
                    _mimeTypes.Add(".sh", "application/x-shar");
                    _mimeTypes.Add(".sh", "text/x-script.sh");
                    _mimeTypes.Add(".shar", "application/x-bsh");
                    _mimeTypes.Add(".shar", "application/x-shar");
                    _mimeTypes.Add(".shtml", "text/html");
                    _mimeTypes.Add(".shtml", "text/x-server-parsed-html");
                    _mimeTypes.Add(".sid", "audio/x-psid");
                    _mimeTypes.Add(".sit", "application/x-sit");
                    _mimeTypes.Add(".sit", "application/x-stuffit");
                    _mimeTypes.Add(".skd", "application/x-koan");
                    _mimeTypes.Add(".skm", "application/x-koan");
                    _mimeTypes.Add(".skp", "application/x-koan");
                    _mimeTypes.Add(".skt", "application/x-koan");
                    _mimeTypes.Add(".sl", "application/x-seelogo");
                    _mimeTypes.Add(".smi", "application/smil");
                    _mimeTypes.Add(".smil", "application/smil");
                    _mimeTypes.Add(".snd", "audio/basic");
                    _mimeTypes.Add(".snd", "audio/x-adpcm");
                    _mimeTypes.Add(".sol", "application/solids");
                    _mimeTypes.Add(".spc", "application/x-pkcs7-certificates");
                    _mimeTypes.Add(".spc", "text/x-speech");
                    _mimeTypes.Add(".spl", "application/futuresplash");
                    _mimeTypes.Add(".spr", "application/x-sprite");
                    _mimeTypes.Add(".sprite", "application/x-sprite");
                    _mimeTypes.Add(".src", "application/x-wais-source");
                    _mimeTypes.Add(".ssi", "text/x-server-parsed-html");
                    _mimeTypes.Add(".ssm", "application/streamingmedia");
                    _mimeTypes.Add(".sst", "application/vnd.ms-pki.certstore");
                    _mimeTypes.Add(".step", "application/step");
                    _mimeTypes.Add(".stl", "application/sla");
                    _mimeTypes.Add(".stl", "application/vnd.ms-pki.stl");
                    _mimeTypes.Add(".stl", "application/x-navistyle");
                    _mimeTypes.Add(".stp", "application/step");
                    _mimeTypes.Add(".sv4cpio", "application/x-sv4cpio");
                    _mimeTypes.Add(".sv4crc", "application/x-sv4crc");
                    _mimeTypes.Add(".svf", "image/vnd.dwg");
                    _mimeTypes.Add(".svf", "image/x-dwg");
                    _mimeTypes.Add(".svr", "application/x-world");
                    _mimeTypes.Add(".svr", "x-world/x-svr");
                    _mimeTypes.Add(".swf", "application/x-shockwave-flash");
                    _mimeTypes.Add(".t", "application/x-troff");
                    _mimeTypes.Add(".talk", "text/x-speech");
                    _mimeTypes.Add(".tar", "application/x-tar");
                    _mimeTypes.Add(".tbk", "application/toolbook");
                    _mimeTypes.Add(".tbk", "application/x-tbook");
                    _mimeTypes.Add(".tcl", "application/x-tcl");
                    _mimeTypes.Add(".tcl", "text/x-script.tcl");
                    _mimeTypes.Add(".tcsh", "text/x-script.tcsh");
                    _mimeTypes.Add(".tex", "application/x-tex");
                    _mimeTypes.Add(".texi", "application/x-texinfo");
                    _mimeTypes.Add(".texinfo", "application/x-texinfo");
                    _mimeTypes.Add(".text", "application/plain");
                    _mimeTypes.Add(".text", "text/plain");
                    _mimeTypes.Add(".tgz", "application/gnutar");
                    _mimeTypes.Add(".tgz", "application/x-compressed");
                    _mimeTypes.Add(".tif", "image/tiff");
                    _mimeTypes.Add(".tif", "image/x-tiff");
                    _mimeTypes.Add(".tiff", "image/tiff");
                    _mimeTypes.Add(".tiff", "image/x-tiff");
                    _mimeTypes.Add(".tr", "application/x-troff");
                    _mimeTypes.Add(".tsi", "audio/tsp-audio");
                    _mimeTypes.Add(".tsp", "application/dsptype");
                    _mimeTypes.Add(".tsp", "audio/tsplayer");
                    _mimeTypes.Add(".tsv", "text/tab-separated-values");
                    _mimeTypes.Add(".turbot", "image/florian");
                    _mimeTypes.Add(".txt", "text/plain");
                    _mimeTypes.Add(".uil", "text/x-uil");
                    _mimeTypes.Add(".uni", "text/uri-list");
                    _mimeTypes.Add(".unis", "text/uri-list");
                    _mimeTypes.Add(".unv", "application/i-deas");
                    _mimeTypes.Add(".uri", "text/uri-list");
                    _mimeTypes.Add(".uris", "text/uri-list");
                    _mimeTypes.Add(".ustar", "application/x-ustar");
                    _mimeTypes.Add(".ustar", "multipart/x-ustar");
                    _mimeTypes.Add(".uu", "application/octet-stream");
                    _mimeTypes.Add(".uu", "text/x-uuencode");
                    _mimeTypes.Add(".uue", "text/x-uuencode");
                    _mimeTypes.Add(".vcd", "application/x-cdlink");
                    _mimeTypes.Add(".vcs", "text/x-vcalendar");
                    _mimeTypes.Add(".vda", "application/vda");
                    _mimeTypes.Add(".vdo", "video/vdo");
                    _mimeTypes.Add(".vew", "application/groupwise");
                    _mimeTypes.Add(".viv", "video/vivo");
                    _mimeTypes.Add(".viv", "video/vnd.vivo");
                    _mimeTypes.Add(".vivo", "video/vivo");
                    _mimeTypes.Add(".vivo", "video/vnd.vivo");
                    _mimeTypes.Add(".vmd", "application/vocaltec-media-desc");
                    _mimeTypes.Add(".vmf", "application/vocaltec-media-file");
                    _mimeTypes.Add(".voc", "audio/voc");
                    _mimeTypes.Add(".voc", "audio/x-voc");
                    _mimeTypes.Add(".vos", "video/vosaic");
                    _mimeTypes.Add(".vox", "audio/voxware");
                    _mimeTypes.Add(".vqe", "audio/x-twinvq-plugin");
                    _mimeTypes.Add(".vqf", "audio/x-twinvq");
                    _mimeTypes.Add(".vql", "audio/x-twinvq-plugin");
                    _mimeTypes.Add(".vrml", "application/x-vrml");
                    _mimeTypes.Add(".vrml", "model/vrml");
                    _mimeTypes.Add(".vrml", "x-world/x-vrml");
                    _mimeTypes.Add(".vrt", "x-world/x-vrt");
                    _mimeTypes.Add(".vsd", "application/x-visio");
                    _mimeTypes.Add(".vst", "application/x-visio");
                    _mimeTypes.Add(".vsw", "application/x-visio");
                    _mimeTypes.Add(".w60", "application/wordperfect6.0");
                    _mimeTypes.Add(".w61", "application/wordperfect6.1");
                    _mimeTypes.Add(".w6w", "application/msword");
                    _mimeTypes.Add(".wav", "audio/wav");
                    _mimeTypes.Add(".wav", "audio/x-wav");
                    _mimeTypes.Add(".wb1", "application/x-qpro");
                    _mimeTypes.Add(".wbmp", "image/vnd.wap.wbmp");
                    _mimeTypes.Add(".web", "application/vnd.xara");
                    _mimeTypes.Add(".wiz", "application/msword");
                    _mimeTypes.Add(".wk1", "application/x-123");
                    _mimeTypes.Add(".wmf", "windows/metafile");
                    _mimeTypes.Add(".wml", "text/vnd.wap.wml");
                    _mimeTypes.Add(".wmlc", "application/vnd.wap.wmlc");
                    _mimeTypes.Add(".wmls", "text/vnd.wap.wmlscript");
                    _mimeTypes.Add(".wmlsc", "application/vnd.wap.wmlscriptc");
                    _mimeTypes.Add(".word", "application/msword");
                    _mimeTypes.Add(".wp", "application/wordperfect");
                    _mimeTypes.Add(".wp5", "application/wordperfect");
                    _mimeTypes.Add(".wp5", "application/wordperfect6.0");
                    _mimeTypes.Add(".wp6", "application/wordperfect");
                    _mimeTypes.Add(".wpd", "application/wordperfect");
                    _mimeTypes.Add(".wpd", "application/x-wpwin");
                    _mimeTypes.Add(".wq1", "application/x-lotus");
                    _mimeTypes.Add(".wri", "application/mswrite");
                    _mimeTypes.Add(".wri", "application/x-wri");
                    _mimeTypes.Add(".wrl", "application/x-world");
                    _mimeTypes.Add(".wrl", "model/vrml");
                    _mimeTypes.Add(".wrl", "x-world/x-vrml");
                    _mimeTypes.Add(".wrz", "model/vrml");
                    _mimeTypes.Add(".wrz", "x-world/x-vrml");
                    _mimeTypes.Add(".wsc", "text/scriplet");
                    _mimeTypes.Add(".wsrc", "application/x-wais-source");
                    _mimeTypes.Add(".wtk", "application/x-wintalk");
                    _mimeTypes.Add(".xbm", "image/x-xbitmap");
                    _mimeTypes.Add(".xbm", "image/x-xbm");
                    _mimeTypes.Add(".xbm", "image/xbm");
                    _mimeTypes.Add(".xdr", "video/x-amt-demorun");
                    _mimeTypes.Add(".xgz", "xgl/drawing");
                    _mimeTypes.Add(".xif", "image/vnd.xiff");
                    _mimeTypes.Add(".xl", "application/excel");
                    _mimeTypes.Add(".xla", "application/excel");
                    _mimeTypes.Add(".xla", "application/x-excel");
                    _mimeTypes.Add(".xla", "application/x-msexcel");
                    _mimeTypes.Add(".xlb", "application/excel");
                    _mimeTypes.Add(".xlb", "application/vnd.ms-excel");
                    _mimeTypes.Add(".xlb", "application/x-excel");
                    _mimeTypes.Add(".xlc", "application/excel");
                    _mimeTypes.Add(".xlc", "application/vnd.ms-excel");
                    _mimeTypes.Add(".xlc", "application/x-excel");
                    _mimeTypes.Add(".xld", "application/excel");
                    _mimeTypes.Add(".xld", "application/x-excel");
                    _mimeTypes.Add(".xlk", "application/excel");
                    _mimeTypes.Add(".xlk", "application/x-excel");
                    _mimeTypes.Add(".xll", "application/excel");
                    _mimeTypes.Add(".xll", "application/vnd.ms-excel");
                    _mimeTypes.Add(".xll", "application/x-excel");
                    _mimeTypes.Add(".xlm", "application/excel");
                    _mimeTypes.Add(".xlm", "application/vnd.ms-excel");
                    _mimeTypes.Add(".xlm", "application/x-excel");
                    _mimeTypes.Add(".xls", "application/excel");
                    _mimeTypes.Add(".xls", "application/vnd.ms-excel");
                    _mimeTypes.Add(".xls", "application/x-excel");
                    _mimeTypes.Add(".xls", "application/x-msexcel");
                    _mimeTypes.Add(".xlt", "application/excel");
                    _mimeTypes.Add(".xlt", "application/x-excel");
                    _mimeTypes.Add(".xlv", "application/excel");
                    _mimeTypes.Add(".xlv", "application/x-excel");
                    _mimeTypes.Add(".xlw", "application/excel");
                    _mimeTypes.Add(".xlw", "application/vnd.ms-excel");
                    _mimeTypes.Add(".xlw", "application/x-excel");
                    _mimeTypes.Add(".xlw", "application/x-msexcel");
                    _mimeTypes.Add(".xm", "audio/xm");
                    _mimeTypes.Add(".xml", "application/xml");
                    _mimeTypes.Add(".xml", "text/xml");
                    _mimeTypes.Add(".xmz", "xgl/movie");
                    _mimeTypes.Add(".xpix", "application/x-vnd.ls-xpix");
                    _mimeTypes.Add(".xpm", "image/x-xpixmap");
                    _mimeTypes.Add(".xpm", "image/xpm");
                    _mimeTypes.Add(".x-png", "image/png");
                    _mimeTypes.Add(".xsr", "video/x-amt-showrun");
                    _mimeTypes.Add(".xwd", "image/x-xwd");
                    _mimeTypes.Add(".xwd", "image/x-xwindowdump");
                    _mimeTypes.Add(".xyz", "chemical/x-pdb");
                    _mimeTypes.Add(".z", "application/x-compress");
                    _mimeTypes.Add(".z", "application/x-compressed");
                    _mimeTypes.Add(".zip", "application/x-compressed");
                    _mimeTypes.Add(".zip", "application/x-zip-compressed");
                    _mimeTypes.Add(".zip", "application/zip");
                    _mimeTypes.Add(".zip", "multipart/x-zip");
                    _mimeTypes.Add(".zoo", "application/octet-stream");
                    _mimeTypes.Add(".zsh", "text/x-script.zsh");
                }
                return _mimeTypes;
            }
        }

        public Encoding OverrideEncoding
        {
            get
            {
                return this._encoding;
            }
            set
            {
                this._encoding = value;
            }
        }

        public int RequestDuration
        {
            get
            {
                return this._requestDuration;
            }
        }

        public Uri ResponseUri
        {
            get
            {
                return this._responseUri;
            }
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return this._statusCode;
            }
        }

        public int StreamBufferSize
        {
            get
            {
                return this._streamBufferSize;
            }
            set
            {
                if (this._streamBufferSize <= 0)
                {
                    throw new ArgumentException("Size must be greater than zero.");
                }
                this._streamBufferSize = value;
            }
        }

        public bool UseCookies
        {
            get
            {
                return this._useCookies;
            }
            set
            {
                this._useCookies = value;
            }
        }

        public string UserAgent
        {
            get
            {
                return this._userAgent;
            }
            set
            {
                this._userAgent = value;
            }
        }

        public bool UsingCache
        {
            get
            {
                return ((this._cachePath != null) && this._usingCache);
            }
            set
            {
                if (value && (this._cachePath == null))
                {
                    throw new HtmlWebException("You need to define a CachePath first.");
                }
                this._usingCache = value;
            }
        }

        public delegate void PostResponseHandler(HttpWebRequest request, HttpWebResponse response);

        public delegate void PreHandleDocumentHandler(HtmlDocument document);

        public delegate bool PreRequestHandler(HttpWebRequest request);
    }
}