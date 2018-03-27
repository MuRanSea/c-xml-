using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Net;
using System.Net.Sockets;

namespace xml序列化
{
    class Program
    {
        //反射获取属性值
        static void GetPor<T>(T t)
        {
            try
            {
                Type ty = t.GetType();
                var pros = ty.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                Console.WriteLine("以下是:" + ty.Name + "的属性值");
                foreach (var pr in pros)
                {
                    Console.WriteLine(string.Format("{0} {1} : {2}", pr.PropertyType,pr.Name,pr.GetValue(t)));
                }
                Console.WriteLine("输出完毕\n\n");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void Main(string[] args)
        {
            var rd = new Random();
            // GPS_TRACK_350M gis = new GPS_TRACK_350M() {ID = "123",GPSID = "25", Longitude = 25.25f,Latitude = 112.256f,Speed = 5,Direction = 52,Height = 156 };
            //string daxml = XmlUtility.SerializeToXml<GPS_TRACK_350M>(gis);
            // string da_xml = SerializeXML.XmlSerialize<GPS_TRACK_350M>(gis);

            YZ_ShortMsg MSG = new YZ_ShortMsg { Id = "123", SJHM = 3898081, FSNR = "里", FSRXM = "karry", JSRXM = "kris", FSRJH = "6659852",};
            string shortmsg_XML = XmlUtility.SerializeToXml<YZ_ShortMsg>(MSG);
            //WriteTolocal(shortmsg_XML,"msg");
            //TestXML();
            string das = ReadXML(@"..\..\data\arrayPeople.txt");
            List<PeopleI> list = DeSerialize<List<PeopleI>>(das);
            foreach (var pi in list)
            {
                GetPor<PeopleI>(pi);
            }

            Console.WriteLine("11");

            Console.ReadKey();

        }


        static void TestXML()
        {
            //文件路径的读取
            string t_xml = ReadXML(@"..\..\data\xml_test.txt");
            string g_xml = ReadXML(@"..\..\data\4g.txt");

            var obj = DeSerialize<GPS_TRACK_350M>(t_xml);
            if (obj == null)
            {
                Console.WriteLine("350m没能转换成功");
            }
            else
            {
                Console.WriteLine("成功装换350m");
                GetPor<GPS_TRACK_350M>(obj);

            }

            var gps_4g = DeSerialize<GPS_TRACK_4GGPS>(g_xml);
            if (gps_4g == null)
            {
                Console.WriteLine("4g没能转换成功");
            }
            else
            {
                Console.WriteLine("成功转换4ggps");
                GetPor<GPS_TRACK_4GGPS>(gps_4g);
            }
        }
        static string ReadXML(string path)
        {
            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path, Encoding.UTF8);
                string result = sr.ReadToEnd();
                sr.Dispose();
                sr.Close();
                return result;
            }
            Console.WriteLine("读取文件错误");
            return null;
        }

        static void WriteTolocal(string da,string filename)
        {
            string filePath = "e:/doc/"+filename+".txt";
            bool _exists = File.Exists(filePath);
            FileStream fs = new FileStream(filePath, _exists ? FileMode.Append : FileMode.Create, FileAccess.Write);
            byte[] _data = Encoding.UTF8.GetBytes(da);
            fs.Write(_data, 0, _data.Length);
            fs.Flush();
            fs.Close();
        }


        /// <summary>  
        /// 反序列化xml字符为对象，默认为Utf-8编码  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="xml"></param>  
        /// <returns></returns>  
        public static T DeSerialize<T>(string xml)
            where T : new()
        {
            return DeSerialize<T>(xml, Encoding.UTF8);
        }

        /// <summary>  
        /// 反序列化xml字符为对象  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="xml"></param>  
        /// <param name="encoding"></param>  
        /// <returns></returns>  
        public static T DeSerialize<T>(string xml, Encoding encoding )
            where T : new()
        {
            try
            {
                var mySerializer = new XmlSerializer(typeof(T));
                using (var ms = new MemoryStream(encoding.GetBytes(xml)))
                {
                    using (var sr = new StreamReader(ms, encoding))
                    {
                        return (T)mySerializer.Deserialize(sr);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return default(T);
            }
        }
    }

    

    public class XmlUtility
    {
        /// <summary>
        /// 将自定义对象序列化为XML字符串
        /// </summary>
        /// <param name="myObject">自定义对象实体</param>
        /// <returns>序列化后的XML字符串</returns>
        public static string SerializeToXml<T>(T myObject)
        {

            if (myObject != null)
            {
                //Create our own namespaces for the output
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                //Add an empty namespace and empty value
                ns.Add("", "");
                XmlSerializer xs = new XmlSerializer(typeof(T));

                MemoryStream stream = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
                writer.Formatting = Formatting.None;//缩进
                xs.Serialize(writer, myObject,ns);
                string str = Encoding.UTF8.GetString(stream.GetBuffer());
                //stream.Position = 0;
                //StringBuilder sb = new StringBuilder();
                //using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                //{
                //    string line;
                //    while ((line = reader.ReadLine()) != null)
                //    {
                //        sb.Append(line);
                //    }
                //    reader.Close();
                //}
                writer.Close();
                //return sb.ToString
                return str;
            }
            return string.Empty;
        }

        /// <summary>
        /// 将XML字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="xml">XML字符</param>
        /// <returns></returns>
        public static T DeserializeToObject<T>(string xml)
        {
            T myObject;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(xml);
            myObject = (T)serializer.Deserialize(reader);
            reader.Close();
            return myObject;
        }
    }


    class SerializeXML
    {
        public static string XmlSerialize<T>(T obj)
        {
            using (StringWriter sw = new StringWriter())
            {
                Type t = obj.GetType();
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(sw, obj);
                sw.Close();
                return sw.ToString();
            }
        }
    }




    /// <summary>
    /// 永州350M
    /// </summary>
    [XmlRoot("GPS_TRACK")]
    public class GPS_TRACK_350M
    {
        public string ID { get; set; }
        public string GPSID { get; set; }
        [XmlElement("GPSTIME")]
        public string _time { get; set; }
        [XmlIgnore]
        public DateTime? GPSTIME
        {
            get
            {
                DateTime t;
                if (DateTime.TryParse(_time, out t))
                {
                    return t;
                }
                return null;
            }
        }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Speed { get; set; }
        public double Direction { get; set; }
        public double Height { get; set; }
        public string Ywxtly { get; set; }
    }
    /// <summary>
    /// 永州4G无线图传
    /// </summary>
    [XmlRoot("GPS_TRACK")]
    public class GPS_TRACK_4GGPS
    {
        /// <summary>
        /// 唯一标识ID GUID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 设备号
        /// </summary>
        public string GPSID { get; set; }
        /// <summary>
        ///定位时间
        /// </summary>
        [XmlElement("GPSTIME")]
        public string _gpstime { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 维度
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public double Speed { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public double Direction { get; set; }
        /// <summary>
        /// 高程
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 天线
        /// </summary>
        public string Antenna { get; set; }
        /// <summary>
        /// 星数
        /// </summary>
        public string Stars { get; set; }
        /// <summary>
        /// 角度
        /// </summary>
        public double Angle { get; set; }
        /// <summary>
        /// 业务系统类型
        /// </summary>
        public string Ywxtly { get; set; }
        /// <summary>
        /// 定位时间DateTime?类型
        /// </summary>
        [XmlIgnore]
        public DateTime? GPSTIME
        {
            get
            {
                DateTime t;
                if (DateTime.TryParse(_gpstime, out t))
                {
                    return t;
                }
                return null;
            }
        }
    }

    /// <summary>
    /// 永州短信,new 的时候不需要对FSSJ,IP地址赋值。（new 就赋值了）
    /// </summary>
    [XmlRoot("ZD_DTT_SYNCMSGINFORMATION")]
    public class YZ_ShortMsg
    {
        /// <summary>
        /// 短信标识，采用32位的guid。
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public string FSSJ { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public int SJHM { get; set; }
        /// <summary>
        /// 发送内容注：不要添加特殊字符，比如>,<,'
        /// </summary>
        public string FSNR { get; set; }
        /// <summary>
        /// 发送人姓名
        /// </summary>
        public string FSRXM { get; set; }
        /// <summary>
        /// 接受人姓名
        /// </summary>
        public string JSRXM { get; set; }
        /// <summary>
        /// 发送人警号
        /// </summary>
        public string FSRJH { get; set; }
        /// <summary>
        /// 发送的电脑的ip地址
        /// </summary>
        public string FSIP { get; set; }
        public YZ_ShortMsg()
        {
            FSSJ = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            FSIP = GetLocalIP();
        }
        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <returns></returns>
        private string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                //这是我自己写的
                //var ps = Dns.GetHostAddresses(Dns.GetHostName());
                //foreach (var p in ps)
                //{
                //    if (p.AddressFamily == AddressFamily.InterNetwork)
                //    {
                //       return p.ToString();
                //    }
                //}
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取本机IP出错:" + ex.Message);
                return "";
            }
        }
    }
    [Serializable]
    [XmlRoot("测试数组数据")]
    public class PeopleI
    {
        public string Pname { get; set; }
        public int Page { get; set; }
        public int Psex { get; set; }
    }
}
