using System;
using System.ComponentModel;
using System.Collections;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using DevExpress.Data;
using System.Collections.Generic;
using System.Drawing;

using TruePosition.Test.DataLayer;


// TODO: Create Command XML templates for each device type
// TODO: Format field must match standard String.Format (see: String.Format Method in MSDN)

//    <Command Type="">
//      <Format></Format>
//      <Parameter Number= "1" Prompt="Command" Access="Show">
//        <Value></Value>
//      </Parameter>
//      ...
//      <Parameter Number= "n" Prompt="Command" Access="Show">
//        <Value></Value>
//      </Parameter>
//    </Command>

namespace HITUI
{
    public delegate object DynamicGetValue(object component);
    public delegate void DynamicSetValue(object component, object newValue);
    public class DynamicPropertyDescriptor : PropertyDescriptor
    {
        protected Type _componentType;
        protected Type _propertyType;
        protected DynamicGetValue _getDelegate;
        protected DynamicSetValue _setDelegate;

        public DynamicPropertyDescriptor(Type componentType, string name, Type propertyType, DynamicGetValue getDelegate, DynamicSetValue setDelegate)
            :
            base(name, null)
        {
            _componentType = componentType;
            _propertyType = propertyType;
            _getDelegate = getDelegate;
            _setDelegate = setDelegate;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return _componentType; }
        }

        public override object GetValue(object component)
        {
            return _getDelegate(component);
        }

        public override bool IsReadOnly
        {
            get { return _setDelegate == null; }
        }

        public override Type PropertyType
        {
            get { return _propertyType; }
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            _setDelegate(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }

    public abstract class DisplayWrapper<T> : IList, ITypedList
    {
        protected readonly XNamespace Namespace = "";
        protected PropertyDescriptor[] PropDescriptors = null;
        private List<T> List { get; set; }

        protected abstract PropertyDescriptor[] Descriptors { get; }

        public DisplayWrapper(List<T> list)
        {
            List = list;
        }

        #region ITypedList Members

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return new PropertyDescriptorCollection(Descriptors);
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return "";
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            List.Add((T)value);
            return List.Count;
        }

        public void Clear()
        {
            List.Clear();
        }

        public bool Contains(object value)
        {
            return List.Contains((T)value);
        }

        public int IndexOf(object value)
        {
            return List.IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            List.Insert(index, (T)value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            List.Remove((T)value);
        }

        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }

        public object this[int index]
        {
            get
            {
                return List[(int)index];
            }
            set
            {
                List[(int)index] = (T)value;
            }
        }
        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            List.CopyTo(array.Cast<T>().ToArray(), index);
        }                              

        public int Count
        {
            get { return List.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }

        #endregion
    }
    public class CommandWrapper : DisplayWrapper<Actor>
    {
        public Actor Actor { get; set; }
        public CommandWrapper(Actor actor)
            : base(new List<Actor>()) 
        {
            Actor = actor;
            Add(Actor); 
        }

        protected override PropertyDescriptor[] Descriptors
        {
            get
            {
                List<PropertyDescriptor> props = new List<PropertyDescriptor>();
                Dictionary<int, XElement> parameters = new Dictionary<int, XElement>();
                if (PropDescriptors == null)
                {
                    
                    // Command Type (not ActorType)
                    // TODO: UI to manage valid Command Types via config file or some other mechanism
                    props.Add(new DynamicPropertyDescriptor(typeof(Actor),
                                                            "Command Type",
                                                            typeof(string),
                                                            delegate(object a)
                                                            {
                                                                return ((Actor)a).Command.Attribute(Namespace + "Type").Value;
                                                            },
                                                            delegate(object a, object value)
                                                            {
                                                                ((Actor)a).Command.Attribute(Namespace + "Type").Value = (string)value;
                                                            }));
                    props.Add(new DynamicPropertyDescriptor(typeof(Actor),
                                                            "Format",
                                                            typeof(string),
                                                            delegate(object a)
                                                            {
                                                                return ((Actor)a).Command.Element(Namespace + "Format").Value;
                                                            },
                                                            delegate(object a, object value)
                                                            {
                                                                ((Actor)a).Command.Element(Namespace + "Format").Value = (string)value;
                                                            }));

                    int number = 0;
                    foreach (XElement node in Actor.Command.Elements(Namespace + "Parameter"))
                    {
                        parameters.Add(number, node);
                        XElement parameter = parameters[number];
                        props.Add(new DynamicPropertyDescriptor(typeof(XElement),
                                                                node.Attribute(Namespace + "Prompt").Value,
                                                                typeof(string),
                                                                delegate(object x)
                                                                {
                                                                    return parameter.Element(Namespace + "Value").Value;
                                                                },
                                                                delegate(object x, object value)
                                                                {
                                                                    parameter.Element(Namespace + "Value").Value = (string)value;
                                                                }));

                        number++;
                    }
                    PropDescriptors = props.ToArray();
                }
                return PropDescriptors;
            }
        }
    }
    public class ResponseWrapper : DisplayWrapper<Response>
    {
        private Response Response { get; set; }
        public ResponseWrapper(Response response)
            : base(new List<Response>())
        {
            Response = response;
            Add(response);
        }

        protected override PropertyDescriptor[] Descriptors
        {
            get
            {
                List<PropertyDescriptor> props = new List<PropertyDescriptor>();
                if (PropDescriptors == null)
                {
                    props.Add(new DynamicPropertyDescriptor(typeof(Response),
                                                            "Key",
                                                            typeof(string),
                                                            delegate(object r)
                                                            {
                                                                return ((Response)r).Elements[0].Key.RawExpression;
                                                            },
                                                            delegate(object r, object value)
                                                            {
                                                                ((Response)r).Elements[0].Key = new Evaluator((string)value);
                                                            }));
                    props.Add(new DynamicPropertyDescriptor(typeof(Response),
                                                            "Expected",
                                                            typeof(string),
                                                            delegate(object r)
                                                            {
                                                                string rawExpression = null;
                                                                if (((Response)r).Elements[0].ExpectedResponses[0].Expressions.Count > 0)
                                                                    rawExpression = ((Response)r).Elements[0].ExpectedResponses[0].Expressions[0].Evaluator.RawExpression;
                                                                return rawExpression;
                                                            },
                                                            delegate(object r, object value)
                                                            {
                                                                if (((Response)r).Elements[0].ExpectedResponses[0].Expressions.Count == 0)
                                                                {
                                                                    ((Response)r).Elements[0].ExpectedResponses[0].Expressions.Add(new TestExpression((string)value));
                                                                }
                                                                else
                                                                {
                                                                    ((Response)r).Elements[0].ExpectedResponses[0].Expressions[0].Evaluator = new Evaluator((string)value);
                                                                }
                                                            }));
                    props.Add(new DynamicPropertyDescriptor(typeof(Response),
                                                            "Header",
                                                            typeof(string),
                                                            delegate(object r)
                                                            {
                                                                return ((Response)r).Header;
                                                            },
                                                            delegate(object r, object value)
                                                            {
                                                                ((Response)r).Header = (string)value;
                                                            }));
                    props.Add(new DynamicPropertyDescriptor(typeof(Response),
                                                            "Trailer",
                                                            typeof(string),
                                                            delegate(object r)
                                                            {
                                                                return ((Response)r).Trailer;
                                                            },
                                                            delegate(object r, object value)
                                                            {
                                                                ((Response)r).Trailer = (string)value;
                                                            }));
                    props.Add(new DynamicPropertyDescriptor(typeof(Response),
                                                            "Delimiter",
                                                            typeof(string),
                                                            delegate(object r)
                                                            {
                                                                return ((Response)r).Delimiter;
                                                            },
                                                            delegate(object r, object value)
                                                            {
                                                                ((Response)r).Delimiter = (string)value;
                                                            }));
                    props.Add(new DynamicPropertyDescriptor(typeof(Response),
                                                            "Trim",
                                                            typeof(bool),
                                                            delegate(object r)
                                                            {
                                                                return ((Response)r).Elements[0].ExpectedResponses[0].Trim;
                                                            },
                                                            delegate(object r, object value)
                                                            {
                                                                ((Response)r).Elements[0].ExpectedResponses[0].Trim = Convert.ToBoolean(value);
                                                            }));
                    props.Add(new DynamicPropertyDescriptor(typeof(Response),
                                                            "Failure Message",
                                                            typeof(string),
                                                            delegate(object r)
                                                            {
                                                                return ((Response)r).Elements[0].ExpectedResponses[0].FailureMessage;
                                                            },
                                                            delegate(object r, object value)
                                                            {
                                                                ((Response)r).Elements[0].ExpectedResponses[0].FailureMessage = (string)value;
                                                            }));

                    PropDescriptors = props.ToArray();
                }
                return PropDescriptors;
            }
        }
    }

    /// <summary>
    /// Test Matrix UI Layout:
    /// Step Master:
    ///     Number | ActorName | ActorType | Retries | Timeout | ContinueOnError | StatusMessage
    /// Command Detail:
    ///     Type | Format | Param 1 | ... | Param n
    /// Response Detail:
    ///     KeyExpression | ExpectedExpression | Header | Trailer | Delimiter | Trim | FailureMessage        
    /// </summary>
    public class StepWrapper : DisplayWrapper<Step>
    {
        public StepWrapper(List<Step> list) : base(list) { }


        protected override PropertyDescriptor[] Descriptors
        {
            get
            {
                if (PropDescriptors == null)
                {
                    List<PropertyDescriptor> props = new List<PropertyDescriptor>();

                    props.Add(new DynamicPropertyDescriptor(typeof(Step),
                                                            "Number",
                                                            typeof(int),
                                                            delegate(object s)
                                                            {
                                                                return ((Step)s).Number;
                                                            },
                                                            null));

                    props.Add(new DynamicPropertyDescriptor(typeof(Actor),
                                                           "Name",
                                                           typeof(string),
                                                           delegate(object s)
                                                           {
                                                               return ((Step)s).Actor.Name;
                                                           },
                                                           null));

                    // TODO: Type should be dropdown of ActorType
                    props.Add(new DynamicPropertyDescriptor(typeof(Step),
                                                            "Type",
                                                            typeof(ActorType),
                                                            delegate(object s)
                                                            {
                                                                return ((Step)s).Actor.Type;
                                                            },
                                                            delegate(object s, object value)
                                                            {
                                                                Enum.Parse(typeof(ActorType), (string)value);
                                                            }));
                    props.Add(new DynamicPropertyDescriptor(typeof(Step),
                                                            "Continue On Error",
                                                            typeof(bool),
                                                            delegate(object s)
                                                            {
                                                                return ((Step)s).ContinueOnError;
                                                            },
                                                            delegate(object s, object value)
                                                            {
                                                                ((Step)s).ContinueOnError = Convert.ToBoolean(value);
                                                            }));
                    props.Add(new DynamicPropertyDescriptor(typeof(Step),
                                                            "Timeout",
                                                            typeof(int),
                                                            delegate(object s)
                                                            {
                                                                return ((Step)s).Timeout;
                                                            },
                                                            delegate(object s, object value)
                                                            {
                                                                ((Step)s).Timeout = Convert.ToInt32(value);
                                                            }));
                    props.Add(new DynamicPropertyDescriptor(typeof(Step),
                                                            "Status",
                                                            typeof(string),
                                                            delegate(object s)
                                                            {
                                                                return "";
                                                            },
                                                            null));

                    props.Add(new DynamicPropertyDescriptor(typeof(Step),
                                                            "Command",
                                                            typeof(CommandWrapper),
                                                            delegate(object s)
                                                            {            
                                                                CommandWrapper cw = new CommandWrapper(((Step)s).Actor);
                                                                return cw;
                                                            },
                                                            null));
                    props.Add(new DynamicPropertyDescriptor(typeof(Step),
                                                            "Response",
                                                            typeof(ResponseWrapper),
                                                            delegate(object s)
                                                            {
                                                                ResponseWrapper rw = null;
                                                                if (((Step)s).Response != null)
                                                                    rw = new ResponseWrapper(((Step)s).Response);
                                                                return rw;
                                                            },
                                                            null));

                    PropDescriptors = props.ToArray();
                }

                return PropDescriptors;
            }
        }
    }
}
