﻿using Ao.ObjectDesign.Designing;
using Ao.ObjectDesign.Designing.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Ao.ObjectDesign.Wpf.Designing
{
    [DesignFor(typeof(Cursor))]
    public class CursorDesigner : NotifyableObject
    {
        public static readonly IReadOnlyDictionary<string, Cursor> CursorMap = GetCursorMap();
        public static readonly IReadOnlyDictionary<Cursor, string> CursorRevMap = GetCursorRevMap();
        private static PropertyInfo[] GetCursorProperties()
        {
            return typeof(Cursors).GetProperties(BindingFlags.Static | BindingFlags.Public);
        }
        private static IReadOnlyDictionary<string, Cursor> GetCursorMap()
        {
            return GetCursorProperties().ToDictionary(x => x.Name, x => (Cursor)x.GetValue(null));
        }
        private static IReadOnlyDictionary<Cursor, string> GetCursorRevMap()
        {
            return GetCursorProperties().ToDictionary(x => (Cursor)x.GetValue(null), x => x.Name);
        }

        private string name;
        private CursorTypes cursorType;
        private Stream cursorStream;
        private string cursorFile;

        public virtual Stream CursorStream
        {
            get => cursorStream;
            set
            {
                Set(ref cursorStream, value);
                RaiseCursorChanged();
            }
        }

        public virtual string CursorFile
        {
            get => cursorFile;
            set
            {
                Set(ref cursorFile, value);
                RaiseCursorChanged();
            }
        }

        [DefaultValue(CursorTypes.None)]
        public CursorTypes CursorType
        {
            get => cursorType;
            set
            {
                Set(ref cursorType, value);
                RaiseCursorChanged();
            }
        }

        public virtual string Name
        {
            get => name;
            set
            {
                Set(ref name, value);
                RaiseCursorChanged();
            }
        }
        private Cursor cursor;
        [PlatformTargetProperty]
        public virtual Cursor Cursor
        {
            get
            {
                cursor?.Dispose();
                cursor = null;
                if (CursorType == CursorTypes.None)
                {
                    return null;
                }
                else if (CursorType == CursorTypes.SystemCursorName)
                {
                    if (!string.IsNullOrEmpty(name) && CursorMap.TryGetValue(name, out Cursor cur))
                    {
                        return cur;
                    }
                    return null;
                }
                else if (CursorType == CursorTypes.FilePath)
                {
                    if (string.IsNullOrEmpty(CursorFile))
                    {
                        return null;
                    }
                    return cursor = new Cursor(CursorFile);
                }
                else if (CursorType == CursorTypes.Stream)
                {
                    if (CursorStream is null)
                    {
                        return null;
                    }
                    return cursor = new Cursor(CursorStream);
                }
                return null;
            }
            set
            {
                if (value is null)
                {
                    CursorType = CursorTypes.None;
                }
                else if (CursorRevMap.TryGetValue(value, out string v))
                {
                    Name = v;
                    CursorType = CursorTypes.SystemCursorName;
                }
                else
                {
                    throw new NotSupportedException("Do not support not set null or system cursor!");
                }
            }
        }

        protected void RaiseCursorChanged()
        {
            RaisePropertyChanged(nameof(Cursor));
        }
    }
}
