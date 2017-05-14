using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Web.Script.Serialization;

namespace ScreepsApi
{
    public class Json
    {
        private JavaScriptSerializer dynamicJs;
        private JavaScriptSerializer js;
        public Json()
        {
            js = new JavaScriptSerializer();
            dynamicJs = new JavaScriptSerializer();
            dynamicJs.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });
        }
        public string Serialize(object obj)
        {
            if (obj is DynamicJsonObject)
                return js.Serialize( (obj as DynamicJsonObject).AsIDictionary() );
            return js.Serialize(obj);
        }
        public dynamic Deserialize(string json)
        {
            return dynamicJs.Deserialize(json, typeof(object)) as dynamic;
        }
    }

    public class DynamicJsonObject : DynamicObject
    {
        private IDictionary<string, object> Dictionary { get; set; }

        public DynamicJsonObject(IDictionary<string, object> dictionary)
        {
            this.Dictionary = dictionary;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            try
            {
                result = this.Dictionary[binder.Name];
            }
            catch (KeyNotFoundException knfe)
            {
                result = null;
            }

            if (result is IDictionary<string, object>)
            {
                result = new DynamicJsonObject(result as IDictionary<string, object>);
            }
            else if (result is ArrayList && (result as ArrayList) is IDictionary<string, object>)
            {
                result = new List<DynamicJsonObject>((result as ArrayList).ToArray().Select(x => new DynamicJsonObject(x as IDictionary<string, object>)));
            }
            else if (result is ArrayList)
            {
                result = new List<object>((result as ArrayList).ToArray());
            }

            return this.Dictionary.ContainsKey(binder.Name);
        }

        public IDictionary<string, object> AsIDictionary()
        {
            return Dictionary;
        }
    }

    public class DynamicJsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (type == typeof(object))
            {
                return new DynamicJsonObject(dictionary);
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(object) })); }
        }
    }
}
