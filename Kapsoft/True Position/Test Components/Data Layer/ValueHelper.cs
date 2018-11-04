using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading;

namespace TruePosition.Test.DataLayer
{
    public class ValueHelper
    {
        protected object innerValue { get; set; }
        public ValueHelper(object innerValue)
        {
            this.innerValue = innerValue;
        }

        public virtual bool IsNull
        {
            get
            {
                return (innerValue == null) || ((innerValue is string) && (string.IsNullOrEmpty((string)innerValue)));
            }
        }

        public object Value { get { return GetValue(); } set { innerValue = value; } }
        public string GetBooleanString()
        {
            if (GetBoolean())
                return "Y";
            else
                return "N";
        }
        public bool GetBoolean()
        {
            if (innerValue.ToString().ToUpper() == "Y" || innerValue.ToString().ToUpper() == "YES")
                return true;
            if (innerValue.ToString().ToUpper() == "N" || innerValue.ToString().ToUpper() == "NO" || string.IsNullOrEmpty(innerValue.ToString()))
                return false;
            return Convert.ToBoolean(innerValue);
        }
        public bool GetIndicator()
        {
            if (innerValue.ToString() == "Y")
                return true;
            if (innerValue.ToString() == "N")
                return false;
            throw new Exception("Field not a valid indicator value.");
        }
        public byte GetByte()
        {
            return Convert.ToByte(innerValue);
        }
        public char GetChar()
        {
            return Convert.ToChar(innerValue);
        }
        public virtual DateTime GetDateTime()
        {
            DateTime dt;

            if (innerValue is DateTime)
                return (DateTime)innerValue;
            if (IsNull || innerValue.ToString() == "" || innerValue.ToString() == "00000000")
                return DateTime.MinValue; // MinValue is what convert returns anyway for null
            else if (DateTime.TryParse(innerValue.ToString(), out dt))
                return dt;
            else if (DateTime.TryParseExact(innerValue.ToString(), "yyyy", Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out dt))
                return dt;
            else if (DateTime.TryParseExact(innerValue.ToString(), "yyyyMMdd", Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out dt))
                return dt;
            else if (DateTime.TryParseExact(innerValue.ToString(), "yyyyMMddHHmm", Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out dt))
                return dt;
            else if (DateTime.TryParseExact(innerValue.ToString(), "yyyyMMddHHmmss", Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out dt))
                return dt;
            else if (DateTime.TryParseExact(innerValue.ToString(), "yyyyMMddHHmmsszzz", Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out dt))
                return dt;          //CJL 03.17.08 -- Added this type
            else
                throw new FormatException("Could not convert innerValue: " + innerValue.ToString() + " to a DateTime");
        }
        public string GetDateInyyyyMMdd()
        {
            return GetDateTime().ToString("yyyyMMdd");
        }
        public decimal GetDecimal()
        {
            return Convert.ToDecimal(innerValue);
        }
        public Guid GetSysGuid()
        {
            if (IsNull)
                return Guid.Empty;
            return new Guid(innerValue.ToString());
        }
        public double GetDoubleSafe()
        {
            if (IsNull)
                return 0.00;
            return GetDouble();
        }

        public double GetDouble()
        {
            if (innerValue is DateTime)
                return ((DateTime)(innerValue)).ToOADate();
            return Convert.ToDouble(innerValue);
        }
        public float GetFloat()
        {
            return (float)Convert.ToDouble(innerValue);
        }
        public Int16 GetInt16()
        {
            return Convert.ToInt16(innerValue);
        }
        public Int32 GetInt32()
        {
            return Convert.ToInt32(innerValue);
        }
        public Int32 GetInt32Safe()
        {
            if (IsNull)
                return 0;
            return Convert.ToInt32(innerValue);
        }
        public Int64 GetInt64()
        {
            return Convert.ToInt64(innerValue);
        }
        public byte[] GetBytes()
        {
            if (innerValue is System.Byte[])
                return (byte[])innerValue;
            else
                return null;
        }
        public string GetNumeric()
        {
            if (IsNull) return "";

            string finalString = "";
            string originalString = Convert.ToString(innerValue);

            for (int i = 0; i < originalString.Length; i++)
                if (Char.IsDigit(originalString[i]))
                    finalString += originalString[i];

            return finalString;
        }
        public override string ToString()
        {
            return GetString();
        }
        public virtual string GetString()
        {
            if (IsNull)
                return "";
            else
            {
                if (innerValue is System.Byte[])
                {
                    return System.Text.Encoding.ASCII.GetString((byte[])innerValue);
                }
                else
                {
                    return Convert.ToString(innerValue);
                }
            }
        }
        public object GetValue()
        {
            return innerValue;
        }
        public object ChangeType(Type conversionType)
        {
            if (conversionType == typeof(string))
            {
                return GetString();
            }
            else if (conversionType == typeof(int))
            {
                return GetInt32Safe();
            }
            else if (conversionType == typeof(long))
            {
                return GetInt64();
            }
            else if (conversionType == typeof(short))
            {
                return GetInt16();
            }
            else if (conversionType == typeof(DateTime))
            {
                return GetDateTime();
            }
            else if (conversionType == typeof(double))
            {
                return GetDoubleSafe();
            }
            else if (conversionType == typeof(bool))
            {
                return GetBoolean();
            }
            else if (conversionType == typeof(char))
            {
                return GetChar();
            }
            else if (conversionType == typeof(float))
            {
                return GetFloat();
            }
            else if (conversionType == typeof(decimal))
            {
                return GetDecimal();
            }
            else if (conversionType == typeof(byte))
            {
                return GetByte();
            }
            else if (conversionType == typeof(byte[]))
            {
                return GetBytes();
            }
            else
            {
                return GetValue();
            }
        }
    }
}
