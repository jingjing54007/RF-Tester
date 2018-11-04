using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
using System.Security.Permissions;

namespace TruePosition.Test.Core
{
    /// <summary>
    /// Selects the GenericSerializer as the surrogate for serialization during object sizing.
    /// </summary>
    class GenericSurrogateSelector : ISurrogateSelector
    {
        private bool BaseTypesOnly { get; set; }
        private ISurrogateSelector NextSelector { get; set; }

        public GenericSurrogateSelector(bool baseTypesOnly) 
        {
            BaseTypesOnly = baseTypesOnly;
        }

        #region ISurrogateSelector Members

        public void ChainSelector(ISurrogateSelector selector)
        {
            NextSelector = selector;
        }

        public ISurrogateSelector GetNextSelector()
        {
            return NextSelector;
        }

        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            selector = null;
            ISerializationSurrogate surrogate = null;
            if (type.IsSerializable)
            {
                selector = null;
                surrogate = null;
            }
            else if (type.IsClass || type.IsValueType)
            {
                selector = this;
                surrogate = FormatterServices.GetSurrogateForCyclicalReference(new GenericSerializer(BaseTypesOnly));
            }
            else
            {
                selector = null;
                surrogate = null;
            }

            return surrogate;
        }

        #endregion
    }
    /// <summary>
    /// A generic serializer/deserializer that does not require an object graph to support ISerializable or [Serializable] to be serialized.
    /// </summary>
    class GenericSerializer : ISerializationSurrogate
    {
        private bool BaseTypesOnly { get; set; }
        public GenericSerializer(bool baseTypesOnly)
        {
            BaseTypesOnly = baseTypesOnly;
        }
        private void Serialize(object obj, Type type, SerializationInfo info, StreamingContext context)
        {
            if (type == typeof(object))
                return;
            else if (type.BaseType != typeof(object))
                Serialize(obj, type.BaseType, info, context);

            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance |
                                                       BindingFlags.NonPublic |
                                                       BindingFlags.Public | 
                                                       BindingFlags.DeclaredOnly))
            {
                if ((!field.IsNotSerialized) && (field.FieldType.BaseType != typeof(MulticastDelegate)))
                    info.AddValue(field.DeclaringType.Name + "+" + field.Name, field.GetValue(obj), field.FieldType);
            }
        }
        private object Deserialize(object obj, Type type, SerializationInfo info, StreamingContext context)
        {
            if ((type == typeof(object)) || (info == null) || (info.MemberCount == 0))
                return obj;
            else
            {
                obj = Deserialize(obj, type.BaseType, info, context);

                if ((!BaseTypesOnly) || (type != obj.GetType()))
                {
                    foreach (FieldInfo field in type.GetFields(BindingFlags.Instance |
                                                               BindingFlags.NonPublic |
                                                               BindingFlags.Public |
                                                               BindingFlags.DeclaredOnly))
                    {
                        if ((!field.IsNotSerialized) && (field.FieldType.BaseType != typeof(MulticastDelegate)))
                            field.SetValue(obj, info.GetValue(field.DeclaringType.Name + "+" + field.Name, field.FieldType));
                    }
                }
                return obj;
            }
        }

        #region ISerializationSurrogate Members

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Serialize(obj, obj.GetType(), info, context);
        }
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            return Deserialize(obj, obj.GetType(), info, context);
        }

        #endregion
    }

    class Binder<TSource, TResult> : SerializationBinder
        where TSource : class
        where TResult : class
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeof(TSource).FullName.Equals(typeName))
                return typeof(TResult);
            else
                return Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
        }
    }

    /// <summary>
    /// Class that encapsulates properties representing result of field-level comparison between two objects
    /// </summary>
    public class CompareResult
    {
        public CompareResult(string fieldName, string oldValue, string newValue)
        {
            FieldName = fieldName;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }

    public static class ObjectExtensions
    {
        private static object Copy(object source)
        {
            object result = null;
            MemoryStream m_Stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter(new GenericSurrogateSelector(false),
                                                            new StreamingContext(StreamingContextStates.Clone));

            m_Stream.Position = 0;
            using (m_Stream)
            {
                formatter.Serialize(m_Stream, source);
                m_Stream.Position = 0;
                result = formatter.Deserialize(m_Stream);
            }

            return result;
        }
        private static TResult Map<TSource, TResult>(TSource source, bool baseTypesOnly)
            where TSource : class
            where TResult : class
        {
            TResult result = default(TResult);
            MemoryStream m_Stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter(new GenericSurrogateSelector(baseTypesOnly),
                                                            new StreamingContext(StreamingContextStates.Clone));
            formatter.Binder = new Binder<TSource, TResult>();

            m_Stream.Position = 0;
            using (m_Stream)
            {
                formatter.Serialize(m_Stream, source);
                m_Stream.Position = 0;
                result = (TResult)formatter.Deserialize(m_Stream);
            }

            return result;
        }

        /// <summary>
        /// Produces a complete, deep copy of an object graph.
        /// </summary>
        public static TSource Copy<TSource>(this TSource source)
            where TSource : class
        {
            return (TSource)Copy((object)source);
        }
        /// <summary>
        /// Produces a complete, deep copy of an object graph and allows the copy to be further modified.
        /// </summary>
        public static TSource Copy<TSource>(this TSource source, Func<TSource, TSource> modifier)
            where TSource : class
        {
            return (TSource)Copy((object)modifier((TSource)Copy((object)source)));
        }

        /// <summary>
        /// Produces a complete, deep copy of each element in a sequence.
        /// </summary>
        public static IEnumerable<TSource> CopyMany<TSource>(this IEnumerable<TSource> source)
            where TSource : class
        {
            foreach (TSource t in source)
                yield return (TSource)Copy((object)t);
        }
        /// <summary>
        /// Produces a complete, deep copy of each element in a sequence and allows each element to be further modified.
        /// </summary>
        public static IEnumerable<TSource> CopyMany<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource> modifier)
            where TSource : class
        {
            foreach (TSource t in source)
                yield return (TSource)Copy((object)modifier((TSource)Copy((object)t)));
        }

        /// <summary>
        /// Performs a complete, deep copy of an instance of type TBase to a new instance of type TDerived.
        /// </summary>
        public static TDerived MapDown<TBase, TDerived>(this TBase source)
            where TBase : class
            where TDerived : class, TBase
        {
            return Map<TBase, TDerived>(source, true);
        }
        /// <summary>
        /// Performs a complete, deep copy of an instance of type TBase to a new instance of type TDerived and allows the result to be further modified.
        /// </summary>
        public static TDerived MapDown<TBase, TDerived>(this TBase source, Func<TDerived, TDerived> modifier)
            where TBase : class
            where TDerived : class, TBase
        {
            return (TDerived)Copy((object)modifier(Map<TBase, TDerived>(source, true)));
        }

        /// <summary>
        /// Performs a complete, deep copy of each element in a sequence of type TBase to a new sequence of type TDerived.
        /// </summary>
        public static IEnumerable<TDerived> MapDownMany<TBase, TDerived>(this IEnumerable<TBase> source)
            where TBase : class
            where TDerived : class, TBase
        {
            foreach (TBase t in source)
                yield return Map<TBase, TDerived>(t, true);
        }
        /// <summary>
        /// Performs a complete, deep copy of each element in a sequence of type TBase to a new sequence of type TDerived and allows each element to be further modified.
        /// </summary>
        public static IEnumerable<TDerived> MapDownMany<TBase, TDerived>(this IEnumerable<TBase> source, Func<TDerived, TDerived> modifier)
            where TBase : class
            where TDerived : class, TBase
        {
            foreach (TBase t in source)
                yield return (TDerived)Copy((object)modifier(Map<TBase, TDerived>(t, true)));
        }

        /// <summary>
        /// Performs a complete, deep copy an instance of type TDerived to a new instance of type TBase.
        /// </summary>
        public static TBase MapUp<TDerived, TBase>(this TDerived source)
            where TBase : class
            where TDerived : class, TBase
        {
            return Map<TDerived, TBase>(source, false);
        }
        /// <summary>
        /// Performs a complete, deep copy an instance of type TDerived to a new instance of type TBase and allows the result to be further modified.
        /// </summary>
        public static TBase MapUp<TDerived, TBase>(this TDerived source, Func<TBase, TBase> modifier)
            where TBase : class
            where TDerived : class, TBase
        {
            return (TBase)Copy((object)modifier(Map<TDerived, TBase>(source, false)));
        }

        /// <summary>
        /// Performs a complete, deep copy of each element in a sequence of type TDerived to a new sequence of type TBase.
        /// </summary>
        public static IEnumerable<TBase> MapUpMany<TDerived, TBase>(this IEnumerable<TDerived> source)
            where TBase : class
            where TDerived : class, TBase
        {
            foreach (TDerived t in source)
                yield return Map<TDerived, TBase>(t, false);
        }
        /// <summary>
        /// Performs a complete, deep copy of each element in a sequence of type TDerived to a new sequence of type TBase and allows each element to be further modified.
        /// </summary>
        public static IEnumerable<TBase> MapUpMany<TDerived, TBase>(this IEnumerable<TDerived> source, Func<TBase, TBase> modifier)
            where TBase : class
            where TDerived : class, TBase
        {
            foreach (TDerived t in source)
                yield return (TBase)Copy((object)modifier(Map<TDerived, TBase>(t, false)));
        }

        /// <summary>
        /// Returns the size in bytes (generally, within 10%) of an object graph.
        /// </summary>
        private static long Size(object source)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter(new GenericSurrogateSelector(false),
                                                            new StreamingContext(StreamingContextStates.Clone));

            long size = 0;
            stream.Position = 0;
            using (stream)
            {
                formatter.Serialize(stream, source);
                size = stream.Length;
            }

            return size;
        }
        public static long Size<TSource>(this TSource source)
        {
            return Size((object)source);
        }

        /// <summary>
        /// Compares two DataContracts of the same type and returns true if all property values on both objects
        /// are identical, including all collections and base properties
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        public static bool DeepEquals<TSource>(this TSource source, TSource comparedTo)
        {
            return ObjectComparer.DeepEquals<TSource>(source, comparedTo);
        }

        /// <summary>
        /// Compares two objects of the same type, and returns a list of CompareResult objects, 
        /// that contain the changes (differences) between the two data contracts. 
        /// Returns empty list if the objects are identical
        public static IEnumerable<CompareResult> CompareTo<TSource>(this TSource source, TSource compareTo)
        {
            return ObjectComparer.CompareTo<TSource>(source, compareTo);
        }
    }
}
