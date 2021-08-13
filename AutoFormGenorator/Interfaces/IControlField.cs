using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFormGenerator.Object;

namespace AutoFormGenerator.Interfaces
{
    public interface IControlField
    {
        event Events.ControlModified OnControlModified;
        event Events.ControlFinishedEditing OnControlFinishedEditing;

        object GetValue();
        void BuildDisplay(FormControlSettings formControlSettings);
        bool Validate();
        void SetVisibility(bool IsVisible);
    }
}
