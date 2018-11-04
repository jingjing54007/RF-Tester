using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TruePosition.Test.Core
{
    internal class ObjectComparer
    {
        /// <summary>
        /// Retruns serialized xml of given object.
        /// Uses DataContractSerializer to serialize object
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="obj">DataContract that implements IExtensibleDataObject</param>
        /// <returns></returns>
        private static XElement GetDataContractXml<TSource>(TSource obj)
        {
            XElement xml = null;

            DataContractSerializer s = new DataContractSerializer(typeof(TSource));
            using (MemoryStream ms = new MemoryStream())
            {
                s.WriteObject(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                using (XmlTextReader xtr = new XmlTextReader(ms))
                {
                    xml = XElement.Load(xtr);
                    xtr.Close();
                }
                ms.Close();
            }
            return xml;
        }

        /// <summary>
        /// Retruns serialized xml of given object.
        /// Uses XmlSerializer to serialize object
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static XElement GetObjectXml<TSource>(TSource obj)
        {
            if (typeof(TSource).GetInterfaces() != null &&
                typeof(TSource).GetInterfaces().Contains(typeof(IExtensibleDataObject)))
            {
                //this is a data contract
                return GetDataContractXml<TSource>(obj);
            }
            XElement xml = null;

            XmlSerializer s = new XmlSerializer(typeof(TSource));
            using (MemoryStream ms = new MemoryStream())
            {
                s.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                using (XmlTextReader xtr = new XmlTextReader(ms))
                {
                    xml = XElement.Load(xtr);
                    xtr.Close();
                }
                ms.Close();
            }
            return xml;
        }

        /// <summary>
        /// Recursive method that returns fully-qualified name of XML element that includes names of all parent nodes. 
        /// The method uses notation "[Parent].[Child]"
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        private static string GetElementName(XElement xe)
        {
            if (xe.Parent != null)
            {
                return GetElementName(xe.Parent) + "." + xe.Name.LocalName;
            }
            return xe.Name.LocalName;
        }

        /// <summary>
        /// Compares two DataContracts of the same type and returns true if all property values on both objects
        /// are identical, including all collections and base properties
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        internal static bool DeepEquals<TSource>(TSource source, TSource comparedTo)
        {
            XElement elSource = GetObjectXml<TSource>(source);
            XElement elComparedTo = GetObjectXml<TSource>(comparedTo);

            return XElement.DeepEquals(elSource, elComparedTo);
        }

        /// <summary>
        /// Compares two objects of the same type, and returns a list of CompareResult objects, 
        /// that contain the changes (differences) between the two data contracts. 
        /// Returns empty list if the objects are identical
        internal static IEnumerable<CompareResult> CompareTo<TSource>(TSource source, TSource compareTo)
        {
            XElement elSource = GetObjectXml<TSource>(source);
            XElement elComparedTo = GetObjectXml<TSource>(compareTo);

            //DESIGN NOTE: Exclude XML Elements that contain child elements, 
            //since child elements are serialized as descendants
            //Use GetElementName() function to return fully-qualified name of XElement
            //because a complex type property's property may have same name as another parent property
            //and [XElement].Name.LocalName will not be sufficient

            //DESIGN NOTE: XmlSerializer WILL NOT serialize objects with value "null" by default 
            //(IsNullable attribute must be explicitly set to true)
            //DataContractSerializer WILL NOT serialize objects with value null if "EmitDefaultValue" of DataMember
            //is set to false (true by default)
            //Therefore, we need to incorporate different elements as well as different values for matching elements
            var diff = (from a1 in elSource.Descendants()
                        join a2 in elComparedTo.Descendants()
                        on GetElementName(a1) equals GetElementName(a2)
                        where !a1.HasElements && !a2.HasElements
                        select new CompareResult(GetElementName(a1), a1.Value, a2.Value)
                            into result
                            where result.NewValue != result.OldValue
                            select result).Union<CompareResult>
                           (from a1 in elSource.Descendants()
                            where !a1.HasElements &&
                                   elSource.Descendants(a1.Name).Count<XElement>() != elComparedTo.Descendants(a1.Name).Count<XElement>()
                            select new CompareResult(GetElementName(a1), a1.Value, string.Empty)).Union<CompareResult>
                            (from a2 in elComparedTo.Descendants()
                             where !a2.HasElements &&
                                    elSource.Descendants(a2.Name).Count<XElement>() != elComparedTo.Descendants(a2.Name).Count<XElement>()
                             select new CompareResult(GetElementName(a2), string.Empty, a2.Value));

            return diff.ToList<CompareResult>();
        }
    }
}
