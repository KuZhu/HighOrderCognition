using System.Collections;
using System.Collections.Generic;
using System;

using HocInternal;

namespace HocInternal
{
    public abstract class HocVariable
    {
        public abstract Type type
        {
            get;
        }

        public abstract object value
        {
            get;
            set;
        }

        public abstract void clear(); 
    }


    public class HocVariable<T> : HocVariable
    {
        private T _value;
        public HocVariable()
        {
            clear();
        }

        public HocVariable(T value)
        {
            _value = value;
        }


        public override Type type
        {
            get { return typeof(T); }
        }

        public override object value
        {
            get { return _value; }
            set { _value = (T)value; }
        }


        public override void clear()
        {
            _value = default(T);
        }
    }
}

