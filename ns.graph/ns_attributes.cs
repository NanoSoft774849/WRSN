using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.graph
{
    public class ns_keyValuePair
    {
        public string key;
        public object value;
        public ns_keyValuePair(string _key , object _val)
        {
            this.key = _key;
            this.value = _val;
        }

        public int ParseInt()
        {
            return int.Parse(this.value.ToString());
        }
        public double ParseDouble()
        {
            return double.Parse(value.ToString());
        }
       
    
    };

    
   public class ns_list<T> : Iterator<T>
   {

       public ns_list<T> add(params T[] items)
       {
           append(items);
           return this;
       }       
   }
    public class ns_attributes:ns_list<ns_keyValuePair>
    {
        public ns_attributes add(string key, object value)
        {
            this.add(new ns_keyValuePair(key, value));
            return this;
        }
        public ns_attributes SetAttribute(string name , object value)
        {
            if (this.hasAttribute(name))
            {
                this[(item) => { return item.key == name; }].value = value;
                return this;
            }
            this.add(name, value);
            return this;
        }
        public ns_attributes SetAttributes(params object[] kvps)
        {
            int count = kvps.Length;
            for(int i=0;i<(count/2);i++)
            {
                string key = kvps[2 * i].ToString();
                object value = kvps[2 * i + 1];
                this.SetAttribute(key, value);
            }
            return this;
        }
        public ns_keyValuePair this[string key]
        {
            get
            {
                return this.get((item) => { return item.key == key; }, null);
            }
            set
            {
                int i = this.indexOf((item) => { return item.key == key; });
                if( i!=-1)
                {
                    this[i] = value;
                }
            }
        }
        public bool hasAttribute(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;
            key = key.Trim();
            return indexOf((item) => { return item.key == key; }) != -1;
        }
        public ns_attributes SelectMany(params string[] keys)
        {
            ns_attributes attr = new ns_attributes();
            foreach(string key in keys)
            {
                if(this.hasAttribute(key))
                {
                    attr.add(this[key]);
                }
            }
            return attr;
        }
        public object parse<T>(string name)
        {
            if(typeof(T)==typeof(int))
            {
                return this[name].value;
            }
            return 0;
        }
        public object getAttributeValue(string key)
        {
            if(!this.hasAttribute(key) || string.IsNullOrEmpty(key) )
            {
                return null;
            }
            int i = 0;
            for(i=0;i<this.Count;i++)
            {
                if(this[i].key.Trim()==key.Trim())
                {
                    return this[i].value;
                }
            }
            return null;
        }
    }
   
}
