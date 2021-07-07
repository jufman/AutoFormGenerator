using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFormGenerator.Object
{
    [FormClass(DisplayName = "Web Tab", WindthOveride = true)]
    public class TestClass
    {
        [FormField]
        public string Name { get; set; }
        [FormField(DisplayName = "Refresh On Screen Change")]
        public bool RefreshOnScreenChange { get; set; }
        [FormField(DisplayName = "Refresh On Record Load")]
        public bool RefreshOnRecordLoad { get; set; }
        [FormField(DisplayName = "Block Refresh After NavigateA way")]
        public bool BlockRefreshAfterNavigateAway { get; set; }
        [FormField(DisplayName = "Default URL", ControlWidth = 300)]
        public string DefaultURL { get; set; }

    }
}
