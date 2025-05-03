using PipeWireSharp.Spa.Enums;

namespace PipeWireSharp.Utils;

internal static class SpaEnumUtils
{
    public static string GetNameFromType(uint spaType) => GetNameFromType((SpaType)spaType);
    
    public static string GetNameFromType(SpaType spaType)
    {
        return spaType.ToString();
    }

    public static string GetNameFromTypeKey(uint type, uint key) => GetNameFromTypeKey((SpaType)type, key);
    
    public static string GetNameFromTypeKey(SpaType type, uint key)
    {
        return type switch
        {
            SpaType.ObjectProps => ((SpaProperty)key).ToString(),
            SpaType.ObjectFormat => ((SpaFormat)key).ToString(),
            _ => $"{key}"
        };
    }

    public static string GetNameFromParamType(uint paramType) => GetNameFromParamType((SpaParamType)paramType);
    
    public static string GetNameFromParamType(SpaParamType paramType)
    {
        return paramType.ToString();
    }
}