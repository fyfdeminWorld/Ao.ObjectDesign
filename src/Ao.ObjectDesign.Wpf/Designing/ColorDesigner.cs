﻿using Ao.ObjectDesign.Wpf.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ao.ObjectDesign.Wpf.Designing
{
    [DesignFor(typeof(Color))]
    public class ColorDesigner : NotifyableObject
    {
        private byte r;
        private byte g;
        private byte b;
        private byte a;

        public virtual byte A
        {
            get => a;
            set
            {
                Set(ref a, value);
                RaiseColorChanged();
            }
        }
        public virtual byte B
        {
            get => b;
            set
            {
                Set(ref b, value);
                RaiseColorChanged();
            }
        }

        public virtual byte G
        {
            get => g;
            set
            {
                Set(ref g, value);
                RaiseColorChanged();
            }
        }
        public virtual byte R
        {
            get => r;
            set
            {
                Set(ref r, value);
                RaiseColorChanged();
            }
        }

        [PlatformTargetProperty]
        public virtual Color Color
        {
            get => Color.FromArgb(a, r, g, b);
            set
            {
                A = value.A;
                R = value.R;
                G = value.G;
                B = value.B;
            }
        }

        protected void RaiseColorChanged()
        {
            RaisePropertyChanged(nameof(Color));
        }
    }
}