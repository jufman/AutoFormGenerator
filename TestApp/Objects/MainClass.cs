﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFormGenerator.Object;

namespace TestApp.Objects
{
    [FormClass]
    public class MainClass : ExtendClass
    {
        [FormField(Required = true)]
        public string TestString { get; set; } = "";

        [FormField]
        public bool TestBool { get; set; } = true;

        [FormField]
        public bool ShowNow { get; set; } = true;

        [FormField]
        public int TestInt { get; set; }
        [FormField]
        public double TestDouble { get; set; }
        [FormField]
        public float TestFloat { get; set; }

        [FormField(ObjectTypeName = ObjectTypes.Custom, CustomControl = typeof(Controls.CustomControl))]
        public float Slider { get; set; } = 0.5f;


        [FormField(ObjectTypeName = ObjectTypes.TimePicker)]
        public double TestDoubleTime { get; set; } = 300;

        [FormField(ObjectTypeName = ObjectTypes.Password)]
        [FieldCondition(Field = "TestBool", Operator = Operators.Equals, Value = true, IsOr = true)]
        [FieldCondition(Field = "ShowNow", Operator = Operators.Equals, Value = true, IsOr = true)]
        public string TestPassword { get; set; } = "";

        [FormField(ObjectTypeName = ObjectTypes.SpecialDropdown, Required = true)]
        public string TestDropDown { get; set; }

        [FormField]
        [FieldCondition(Field = "TestDropDown", Operator = Operators.Equals, Value = "Bus", IsOr = true)]
        [FieldCondition(Field = "TestDropDown", Operator = Operators.Equals, Value = "Train", IsOr = true)]
        public string HideMe { get; set; } = "";

        [FormField(ObjectTypeName = ObjectTypes.Password)]
        [FieldCondition(Field = "TestBool", Operator = Operators.Equals, Value = true)]
        [FieldCondition(Field = "ShowNow", Operator = Operators.Equals, Value = true)]
        public string AndDisplay { get; set; } = "";


        [FormField(ObjectTypeName = ObjectTypes.FolderBrowser)]
        public string FolderLocation { get; set; } = "";


        [FormField(IsVisible = false)]
        public float DontShowMe { get; set; }

        [FormField(Regex = "^([1-9]|1[0-6])$")]
        public int Only1Th16 { get; set; } = 1;

        [FormField(Regex = "^[^0-9]+$")]
        public string NoNumbers { get; set; }


        [FormField(Type = Types.NestedClass)]
        public NestedClass NestedClass { get; set; }


        [FormField(Type = Types.NestedClass)]
        public ShrinkClass ShrinkClass { get; set; }

        [FormField]
        public List<ExtendClass> ExtendClasses { get; set; } = new List<ExtendClass>();

        [FormField(Type = Types.ItemList, ControlWidth = 200)]
        public List<string> StringItems { get; set; } = new List<string>
        {
            "TEST",
            "Another Item"
        };

        [FormField(Type = Types.ItemList, DisplayName = "Int Numbers")]
        public List<int> IntItems { get; set; } = new List<int>
        {
            1,
            2,
            3
        };

        public void Thing()
        {

        }

    }
}
