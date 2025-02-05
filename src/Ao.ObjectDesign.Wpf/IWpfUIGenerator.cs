﻿using System.Collections.Generic;
using System.Windows;
using Ao.ObjectDesign.Designing;

namespace Ao.ObjectDesign.Wpf
{
    public interface IWpfUIGenerator : IUIGenerator<FrameworkElement, WpfForViewBuildContext>
    {
        new IEnumerable<IWpfUISpirit> Generate(IEnumerable<IPropertyProxy> propertyProxies);
    }
}
