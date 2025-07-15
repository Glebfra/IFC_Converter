using System;

namespace IFC.Tools
{
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
        public static explicit operator ActionProperty<T>(T value) => new ActionProperty<T>(value);
    }
}