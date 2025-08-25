using System;
using System.Diagnostics;

namespace IFC.Tools
{
    [DebuggerDisplay("Value = {Value.ToString()}")]
    public class ActionProperty<T>
    {
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnValueChange?.Invoke();
            }
        }

        public event Action? OnValueChange;
        
        private T _value;

        public ActionProperty(T value)
        {
            _value = value;
        }
        
        public static implicit operator T(ActionProperty<T> actionProperty) => actionProperty.Value;
        public static implicit operator ActionProperty<T>(T value) => new ActionProperty<T>(value);
    }
}