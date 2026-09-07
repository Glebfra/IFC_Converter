using System;
using Xbim.Common;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcProject : IDisposable
    {
        IModel Model { get; }

        void AddEntityRaw(IfcProduct product);
        void SaveAs(string filepath);
    }
}