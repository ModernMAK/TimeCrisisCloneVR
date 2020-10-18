using System;

public static class PropertyHelpers
{
    //Returns true if the value was updated, false if the value did not change
    public static bool UpdateValue<T>(ref T field, T value) where T : IEquatable<T>
    {
        var didChange = !field.Equals(value);
        field = value;
        return didChange;
    }
}