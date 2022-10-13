using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace ns.networking
{
    public  class MimeTypes:Dictionary<string,string>
    {
         
        
        public MimeTypes()
        {
            fx(".html", "text/html")
           .fx(".js", "application/javascript")
           .fx(".jpeg", "image/jpeg")
           .fx(".jpg", "image/jpeg")
           .fx(".css", "text/css")
           .fx(".png", "image/png")
           .fx(".json", "application/json")
           .fx(".gif", "image/gif");
        }
        public MimeTypes fx(string key,string value)
        {
            Add(key, value);
            return this;
        }
        public string getValue(string key)
        {
            if(!key.StartsWith(".")) key="."+key;

            //string s="";
            
            return this[key];
        }
    }
    public class ns_keyValue
    {
        public string key;
        public object value;
        public ns_keyValue(string key,object val)
        {
            this.key = key;
            this.value = val;
        }

    }
    public class ns_WebSockets
    {

        /// <summary>
        /// the Message sent by websocket clients is encoded 
        /// We can use this function to decode the bytes into string.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// 
        public static byte[] ConvertInt2ByteArray(UInt16 i)
        {
            return BitConverter.GetBytes(i);
        }

        public static byte[] EncodeMessage(string msg)
        {
            byte payloadlen = (byte)(Encoding.UTF8.GetByteCount(msg) & 0x7f);
             byte b1 = 0x81;
            UInt16 len =(UInt16)  Encoding.UTF8.GetByteCount(msg);
            if(len<126)
            {
                byte[] bufferx = new byte[payloadlen + 2];

                bufferx[0] = b1;
                bufferx[1] = (byte)(payloadlen ^ 0x00);//no mask
                Encoding.UTF8.GetBytes(msg).CopyTo(bufferx, 2);
                return bufferx;
            };

            byte[] lenx = ConvertInt2ByteArray(len);
            //fin=1 ,
          //fin=1, opcode=1 text msg
            //payload= ( %x00-7D )
            //payload                / ( %x7E frame-payload-length-16 )
            //payload                / ( %x7F frame-payload-length-63 )
            //payload                ; 7, 7+16, or 7+64 bits in length,
            //payload                ; respectively
            //ref :https://tools.ietf.org/html/rfc6455 
            // for WebSocket protocol
            byte[] buffer = new byte[len + 2+2]; 
            // why 4:
            // 1 -for fin ,rsv1, rsv2,rsv3 , opcode(3)
            // 1 for specify that the payload len is 16 bit or 64 bit
            // 2 if the payload length 16-bit
            // 8 if the payload len is 64-bit
           
            buffer[0] = b1;
            buffer[1] = (byte)(0x7E ^ 0x00);//no mask --payload len 16 bit
          //  buffer[1] = 0x7f if the 64 -bit paylen
            //buffer[1]=0x7E if payload len is 
            buffer[2] = lenx[1];//  in byte reversal manner 
            buffer[3] = lenx[0];
            //int i = 0;
            //for (i = 0; i < 8;i++)
            //{
            //    buffer[i + 2] = (byte) (lenx[7-i] & 0xff);//Extented Payload length
            //}
                Encoding.UTF8.GetBytes(msg).CopyTo(buffer, 2+2);
            return buffer;
        }
        public static string decodeMessage(byte[] bytes)
        {
            bool fin = (bytes[0] & 0x80) != 0, mask = (bytes[1] & 0x80) != 0;
            // must be true, "All messages from the client to the server have this bit set"

            int opcode = bytes[0] & 0x0f, msglen = bytes[1] - 128, offset = 2;// expecting 1 - text message
            // & 0111 1111


            if (msglen == 126)
            {
                // was ToUInt16(bytes, offset) but the result is incorrect
                msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                offset = 4;
            }
            else if (msglen == 127)
            {
                // Console.WriteLine("TODO: msglen == 127, needs qword to store msglen");
                // i don't really know the byte order, please edit this
                // msglen = BitConverter.ToUInt64(new byte[] { bytes[5], bytes[4], bytes[3], bytes[2], bytes[9], bytes[8], bytes[7], bytes[6] }, 0);
                // offset = 10;
            }

            if (msglen == 0)
                Console.WriteLine("msglen == 0");
            else if (mask)
            {
                byte[] decoded = new byte[msglen];
                byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                offset += 4;

                for (int i = 0; i < msglen; ++i)
                    decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);

                string text = Encoding.UTF8.GetString(decoded);
                return text;
            }
            return "";
        }
        public static string ComputeWebSocketAcceptKey(string web_socket_key)
        {
            string _ap = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            string fk = web_socket_key.Trim() + _ap;
            string hash_key = "";
            try
            {
                SHA1 sha1Hash = SHA1.Create();
                byte[] src = Encoding.UTF8.GetBytes(fk);
                byte[] hash_bytes = sha1Hash.ComputeHash(src);
                // hash_key = Encoding.UTF8.GetString(hash_bytes);

                hash_key = Convert.ToBase64String(hash_bytes);

                //string fmt = string.Format("key:{0} \t hash_key:{1}", fk, hash_key);

               

            }
            catch (Exception)
            {
                return "";
            }

            return hash_key;

        }
    }
    public class ns_HttpRequestHeader
    {
        private List<ns_keyValue> xmlist;
       // private string req;
        public delegate void _foreach(string key, string value);
        public ns_HttpRequestHeader(string reqx)
        {
            xmlist = getRequestHeaders(reqx);
        }
        public string this[string key]
        {
            get
            {
                return getValue(key);
            }
        }

        public void Foreach(_foreach _for)
        {
            int i = 0;
            int len = xmlist.Count;
            for (i = 0; i < len; _for(xmlist[i].key, xmlist[i].value.ToString()), i++) ;
        }
        public bool isExist(string key)
        {
            int len=this.xmlist.Count;
            int i=0;
            for(i=0;i<len;i++)
            {
                if(xmlist[i].key==key)
                {
                    // kv=xmlist[i];
                    return true;
                }
            }
           // kv=null;
            return false;

        }
        public string getValue(string key)
        {
             int len=this.xmlist.Count;
            int i=0;
            
           for(i=0;i<len;i++)
            {
                if(xmlist[i].key==key)
                {
                    return xmlist[i].value.ToString();
                   
                }
            }
            return "";
        }
        private List<ns_keyValue> getRequestHeaders(string req)
        {
            List<ns_keyValue> mlist = new List<ns_keyValue>();
            string[] reqs = req.Split(new char[] { '\r', '\n' });
            int i = 0;
            foreach (string s in reqs)
            {
                if (s.Contains(":"))
                {
                    string[] sp = s.Split(':');
                    string key = sp[0].Trim();
                    string value = sp[1].Trim();
                    ns_keyValue kv = new ns_keyValue(key, value);
                    mlist.Add(kv);
                }
                else if (i == 0 && !s.Contains(":"))
                {
                    string xk = "query";
                    string value = s;
                    ns_keyValue kv = new ns_keyValue(xk, value);
                    mlist.Add(kv);
                }
                i++;
            }

            return mlist;
        }
    }
    public class ns_httpReqHelper
    {
        public string httpVerb;
        public string httpGet;
        public string request;
        public bool is_web_socket;

        //public List<ns_keyValue> HttpRequestHeader;
        public ns_httpReqHelper(string http_request)
        {
            //GET /icons/add.png HTTP/1.1
            this.request = http_request;
            
        }
        
        public static string getRequest(string req)
        {
            return getQueryString(req, "GET", "HTTP/1.1");
        }
        public bool IsGet
        {
            get{
                return this.request.Trim().StartsWith("GET");
            }
            
        }
        public bool IsPost
        {
            get
            {
                return this.request.Trim().StartsWith("POST");
            }
        }
        public static string getQueryString(string str, string first,string last)
        {
            string s = "";

            int st = str.IndexOf(first);
            int lt = str.IndexOf(last);
            int slen = first.Length;
            int llen = last.Length;
            int len = 0;
            if(st!=-1 && lt!=-1)
            {
                int stx = st + slen;
                len = lt - (stx);
                if (len < 0) return "";
                s = str.Substring(stx, len);

            }
            

            return s;
        }
    }
    public class nsHttpApp
    {
        private Client mclient;
        private string webApp_dir;
        private string http_req;
        private MimeTypes mimeTypes;
        public delegate void _OnMessageHandler(string msg, string where);
        public _OnMessageHandler OnMessageHandler;
        public bool is_web_socket;
        public ns_HttpRequestHeader httpHeader;

        public nsHttpApp(Client client, string http_req, string web_app_path,bool is_web_sock)
        {
            this.http_req = http_req;
            this.mclient = client;
            this.webApp_dir = web_app_path;
            mimeTypes = new MimeTypes();
            this.is_web_socket = is_web_sock;
            this.httpHeader = new ns_HttpRequestHeader(http_req);
        }
        public nsHttpApp(Client client,string http_req,string web_app_path)
        {
            this.http_req=http_req;
            this.mclient=client;
            this.webApp_dir=web_app_path;
            mimeTypes=new MimeTypes();
            this.httpHeader = new ns_HttpRequestHeader(http_req);

            
        }
        public void LogAllHeader()
        {
            this.httpHeader.Foreach((key, value) =>
            {
                string fmt = string.Format("{0}:{1}", key, value);
                OnMessageHandler(fmt,"");
            });
        }
        public void SendResponse(_OnMessageHandler handler)
        {

            this.OnMessageHandler = handler;
            FormatResponse();
        }
        public static string FormatImagesIndirAsHtml(string dir)
        {
            string[] files=Directory.GetFiles(dir);
            int len = files.Length;
            string html = "";
            int i = 0;
            string fmt="<img src=\"{0}\" width=\"300\" height=\"300\" />\r\n";
            for(i=0;i<len;i++)
            {
                var fileifno = new FileInfo(files[i]);
                if (fileifno.Name.EndsWith(".jpg"))
                    html += string.Format(fmt, fileifno.Name);
            }
            return html;
        }
        public void sendDirAsResponse(string dir)
        {
            string _html = formatHtmlDoc(dir);

            SendHtml(_html);
        }
        public static string formatHtmlDoc(string dir)
        {
            string html=FormatImagesIndirAsHtml(dir);
            return formatHtml(html);
        }
        public static string formatHtml(string body)
        {
            string html = "";
            html += string.Format("<html><body>{0}</body></html>", body);

            return html;
        }
        private byte[] bytes(string s,string encoding)
        {
            encoding = encoding.ToLower();
            switch(encoding)
            {
                case "ascii":
                    return Encoding.ASCII.GetBytes(s);
                case"utf-8":
                    return Encoding.UTF8.GetBytes(s);
                default:
                    return Encoding.ASCII.GetBytes(s);
            }
        }
        public void SendHtml(string html)
        {
            int content_len = html.Length;
            try
            {
                string header = httpResponseHeader(".html", content_len.ToString());
                mclient.Send(bytes(header+"\r\n","utf-8"));
               // mclient.SendMsg("\r\n");
                //var bytes = Encoding.UTF8.GetBytes(html+"\r\n");
                mclient.Send(bytes(html+"\r\n","utf-8"));
                //mclient.SendMsg(html);
               // mclient.SendMsg("\r\n");
                mclient.Close();
            }
            catch(Exception ex)
            {
                this.OnMessageHandler(ex.StackTrace, "SendHtml");
            }
        }
        private void OnWebSocket()
        {
            try
            {
                string Sec_WebSocket_Key = httpHeader["Sec-WebSocket-Key"];
                OnMessageHandler("Sec_WebSocket_Key:" + Sec_WebSocket_Key, "OnWebSocket");
                string hashx = ComputeSHa1Hash(Sec_WebSocket_Key);
                string header = formatWebSocketResponseHeader(hashx);
                byte[] hed = Encoding.UTF8.GetBytes(header);
                mclient.Send(hed);
                //mclient.SendMsg("\r\n");
               //mclient.SendMsg("Hello,I am Abdulbary");
                OnMessageHandler("Inside OnWebSocket---> Sending Headers:", "OnWebSocket");
                OnMessageHandler(header,"HeaderX");

            }
            catch(Exception ex)
            {
                OnMessageHandler(ex.StackTrace, "OnWebSocket");
            }
        }
        private string ComputeSHa1Hash(string web_socket_sec_key)
        {
            string _ap = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            string fk = web_socket_sec_key.Trim() + _ap;
            string hash_key = "";
            try
            {
                SHA1 sha1Hash = SHA1.Create();
                byte[] src = Encoding.UTF8.GetBytes(fk);
                byte[] hash_bytes = sha1Hash.ComputeHash(src);
               // hash_key = Encoding.UTF8.GetString(hash_bytes);

                hash_key = Convert.ToBase64String(hash_bytes);

                string fmt = string.Format("key:{0} \t hash_key:{1}", fk, hash_key);

                OnMessageHandler(fmt, "ComputeSHa1Hash");

            }catch(Exception ex)
            {
                OnMessageHandler(ex.StackTrace, "ComputeSHa1Hash");
            }

            return hash_key;
        }
        private string formatWebSocketResponseHeader(string extr)
        {
            string header = "";
            /*
             * 101 Switching Protocols
               Upgrade: websocket
               Connection: Upgrade
               Sec-WebSocket-Accept: hsBlbuDTkk24srzEOTBUlZAlC2g=
             * 
             * */

            header += "HTTP/1.1 101 Switching Protocol\r\n";
           
            header += "Connection: Upgrade\r\n";

            header += "Upgrade: websocket\r\n";
            //header += "Sec-WebSocket-Accept: HSmrc0sMlYUkAGmm5OPpG2HaGWk=\r\n";
            //header += "Cache-Control: no-cache\r\n";
            header += string.Format("Sec-WebSocket-Accept: {0}\r\n\r\n", extr);
            //header += "";

            return header;
        }
        private void FormatResponse()
        {
            string where="FormatResponse @ nsHttp";

            if (this.is_web_socket)
            {
                this.OnWebSocket();
                return;
            }

            try
            {
                
                string app_path = this.webApp_dir;
                if (!Directory.Exists(app_path))
                {
                    mclient.Close();
                    OnMessageHandler("Path Not exists", where);
                    return;
                }
                string _req = ns_httpReqHelper.getRequest(this.http_req).Trim();
                if (_req == "/") _req = "/index.html";
                string _req_file = _req.Replace("/", "\\");
               // OnMessageHandler("Req:" + _req, where);
                string _file_path = string.Format("{0}{1}", app_path, _req_file);
               // OnMessageHandler("File Path:" + _file_path, where);
                if (File.Exists(_file_path))
                {
                   // var info = new FileInfo(_file_path);
                   //       string header = httpResponseHeader(info.Extension, string.Format("{0}", info.Length));
                   // 
                   //        mclient.Send(bytes(header + "\r\n", "utf-8"));
                   //
              // bool// x= await  Task.Factory.FromAsync(
                   //     new Func<string,AsyncCallback,object,IAsyncResult>(mclient.SendAsync),
                   //         
                   //     new Func<IAsyncResult, bool>(mclient.close), _file_path,null).ConfigureAwait(false);
                   //     


                    
                           var info = new FileInfo(_file_path);
                           string header = httpResponseHeader(info.Extension, string.Format("{0}", info.Length));
                    
                           mclient.Send(bytes(header + "\r\n", "utf-8"));

                           mclient.SendFile(_file_path, (msg) =>
                               {
                                   OnMessageHandler(msg, "SendFile--->" + where);
                               });

                      
                  // mclient.Close();


                }
                else
                {
                    if(_req.Contains("index.html"))
                      sendDirAsResponse(this.webApp_dir);
                    else
                    {
                        mclient.Close();
                    }
                    OnMessageHandler("INside ELSE ...Sending Response!" + _file_path, where);
                   // mclient.Close();
                    OnMessageHandler("File not Exist", where);

                }

            } 
            catch(Exception ex)
            {
                OnMessageHandler(ex.Message,where);
            }
            finally
            {

              mclient.Close();
            }

        }
        private string httpResponseHeader(string ext,string extra)
        {
            if (!ext.StartsWith(".")) ext = "." + ext;
            string head = "";//Continue
            head += "HTTP/1.1 200 OK\r\n";
            
            head += "Cache-Control: none\r\n";
            head += string.Format("Content-Type: {0}\r\n",mimeTypes[ext]);
            head += string.Format("Content-Length: {0}\r\n", extra);
            OnMessageHandler(head, "httpResponseHeader");
            return head;
        }
    }
}
