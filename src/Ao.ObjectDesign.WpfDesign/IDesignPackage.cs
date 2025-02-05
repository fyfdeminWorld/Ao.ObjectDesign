﻿using Ao.ObjectDesign.Designing;
using Ao.ObjectDesign.Designing.Level;
using Ao.ObjectDesign.Wpf.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Ao.ObjectDesign.WpfDesign
{
    public interface IDesignPackage<TDesignObject>
    {
        UIDesignMap UIDesinMap { get; }

        IEnumerable<IWithSourceBindingScope> CreateBindingScopes(IDesignPair<UIElement, TDesignObject> unit);

        IBindingCreatorState CreateBindingCreatorState(IDesignPair<UIElement, TDesignObject> unit);
    }
}
