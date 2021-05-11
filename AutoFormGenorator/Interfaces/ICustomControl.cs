using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFormGenerator.Interfaces
{
    public interface ICustomControl
    {
        event Events.PropertyModified OnPropertyModified;
        event Events.PropertyFinishedEditing OnPropertyFinishedEditing;

        void SetValue(object Value);

    }
}
