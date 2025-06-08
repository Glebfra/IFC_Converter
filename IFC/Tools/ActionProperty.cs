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
    }
}