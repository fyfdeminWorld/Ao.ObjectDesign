﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Ao.ObjectDesign.Data
{
    public class DynamicVarTable : DynamicVarTable<string>
    {
        public DynamicVarTable(NotifyableMap<string, IVarValue> dataMap)
            : base(dataMap)
        {
        }

        public DynamicVarTable(NotifyableMap<string, IVarValue> dataMap, IReadOnlyHashSet<string> acceptKeys)
            : base(dataMap, acceptKeys)
        {
        }

        protected override string ToKey(string name)
        {
            return name;
        }

        protected override string ToName(string key)
        {
            return key;
        }
    }

    public abstract class DynamicVarTable<TKey> : DynamicObject, INotifyPropertyChanged, IDisposable
    {
        protected DynamicVarTable(NotifyableMap<TKey, IVarValue> dataMap)
            : this(dataMap, ReadOnlyHashSet<TKey>.Empty)
        {
            AllAccept = true;
        }
        protected DynamicVarTable(NotifyableMap<TKey, IVarValue> dataMap, IReadOnlyHashSet<TKey> acceptKeys)
        {
            DataMap = dataMap ?? throw new ArgumentNullException(nameof(dataMap));
            AcceptKeys = acceptKeys ?? throw new ArgumentNullException(nameof(acceptKeys));

            NoneValue = VarValue.NullValue;
        }
        private bool isListening;

        public bool IsListening => isListening;

        public bool AllAccept { get; }

        public IReadOnlyHashSet<TKey> AcceptKeys { get; }

        public NotifyableMap<TKey, IVarValue> DataMap { get; }

        public IVarValue NoneValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Listen()
        {
            if (isListening)
            {
                return;
            }
            DataMap.DataChanged += OnDataChanged;
            isListening = true;
            OnListened();
        }
        protected virtual void OnListened()
        {

        }
        public void UnListen()
        {
            if (!isListening)
            {
                return;
            }
            DataMap.DataChanged -= OnDataChanged;
            isListening = false;
            OnUnListen();
        }
        protected virtual void OnUnListen()
        {

        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (AllAccept)
            {
                return DataMap.Keys.Select(x => ToName(x));
            }
            return AcceptKeys.Select(x => ToName(x));
        }
        public bool SetValue(string name, object value)
        {
            var var = ToVarValue(value);
            return SetValue(name, var);
        }
        public bool SetValue(string name, IVarValue value)
        {
            var key = ToKey(name);
            return SetValue(key, value);
        }
        public bool SetValue(TKey key, object value)
        {
            var var = ToVarValue(value);
            return SetValue(key, var);
        }
        public bool SetValue(TKey key, IVarValue value)
        {
            if (IsAccept(key))
            {
                var v = ToVarValue(value);
                DataMap.AddOrUpdate(key, v);
                var name = ToName(key);
                RaiseValueChanged(name, value);
                return true;
            }
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return SetValue(binder.Name, value);
        }
        public bool TryGetValue(string name, out IVarValue result)
        {
            var key = ToKey(name);
            return TryGetValue(key, out result);
        }
        public bool TryGetValue(TKey key, out IVarValue result)
        {
            result = null;
            if (IsAccept(key))
            {
                DataMap.TryGetValue(key, out result);
                return true;
            }
            return false;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            if (TryGetValue(binder.Name, out var var))
            {
                result = VisitValue(var);
                return true;
            }
            return false;
        }
        protected virtual object VisitValue(IVarValue value)
        {
            return value?.Value ?? NoneValue;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length == 1 && indexes[0] is string str && IsAccept(ToKey(str)))
            {
                SetValue(str, value);
                return true;
            }
            return base.TrySetIndex(binder, indexes, value);
        }
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length == 1)
            {
                var index = indexes[0];
                string name = null;
                TKey key = default;
                bool succeed = false;
                if (index is TKey)
                {
                    key = (TKey)index;
                    name = ToName(key);
                    succeed = true;
                }
                else if (index is string)
                {
                    name = (string)index;
                    key = ToKey(name);
                    succeed = true;
                }
                if (succeed &&
                    IsAccept(key) &&
                    TryGetValue(name, out var val))
                {
                    result = VisitValue(val);
                    return true;
                }
            }
            return base.TryGetIndex(binder, indexes, out result);
        }
        protected void OnDataChanged(object sender, DataChangedEventArgs<TKey, IVarValue> e)
        {
            if (IsAccept(e.Key))
            {
                var name = ToName(e.Key);
                RaisePropertyChanged(name);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool IsAccept(TKey key)
        {
            return AllAccept || AcceptKeys.Contains(key);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void RaiseValueChanged(string name, object value)
        {
            RaisePropertyChanged(name);
        }
        protected abstract string ToName(TKey key);
        protected abstract TKey ToKey(string name);

        protected virtual IVarValue ToVarValue(object value)
        {
            return VarValue.FromObject(value);
        }

        public void Dispose()
        {
            UnListen();
            OnDispose();
        }
        protected virtual void OnDispose()
        {

        }
    }
}
