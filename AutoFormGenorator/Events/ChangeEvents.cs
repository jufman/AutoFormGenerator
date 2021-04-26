namespace AutoFormGenerator.Events
{
    public delegate void PropertyModified(string FieldName, object Value);
    public delegate void PropertyFinishedEditing(string FieldName, object Value);
}
