using System.Runtime.InteropServices;
using PipeWireSharp.Native;
using PipeWireSharp.Spa.Enums;
using PipeWireSharp.Spa.Pods.Choice;
using PipeWireSharp.Utils;

namespace PipeWireSharp.Spa.Pods;

public unsafe class PodBuilder : IDisposable
{
    private spa_pod_builder _builder;
    private spa_pod_frame _frame;

    private readonly IntPtr _dataPtr;

    public PodBuilder(uint bufferSize)
    {
        _dataPtr = Marshal.AllocHGlobal((int)bufferSize);

        _builder = new spa_pod_builder();
        _frame = new spa_pod_frame();

        fixed (spa_pod_builder* builderPtr = &_builder)
        {
            Bindings.spa_pod_builder_init(builderPtr, (void*)_dataPtr, bufferSize);
        }
    }
    
    public PodBuilder PushObject(SpaType type, SpaParamType id) => PushObject((uint)type, (uint)id);

    public PodBuilder PushObject(uint type, uint id)
    {
        fixed (spa_pod_builder* builderPtr = &_builder)
        fixed (spa_pod_frame* framePtr = &_frame)
        {
            var result = Bindings.spa_pod_builder_push_object(builderPtr, framePtr, type, id);
            if (result < 0)
                throw new Exception($"Failed to push object! Error: {result}");
        }

        return this;
    }

    public PodBuilder AddProperty(uint key, PodValue value, uint flags = 0)
    {
        fixed (spa_pod_builder* builderPtr = &_builder)
        {
            var result = Bindings.spa_pod_builder_prop(builderPtr, key, flags);
            if (result < 0)
                throw new Exception($"Failed to add property! Error: {result}");
        }

        return PushValue(value);
    }

    public PodBuilder PushValue(PodValue value)
    {
        fixed (spa_pod_builder* builderPtr = &_builder)
        {
            var result = value.Kind switch
            {
                PodValueKind.None => Bindings.spa_pod_builder_none(builderPtr),
                PodValueKind.Bool => Bindings.spa_pod_builder_bool(builderPtr, value.GetValue<bool>()),
                PodValueKind.Id => Bindings.spa_pod_builder_id(builderPtr, value.GetValue<uint>()),
                PodValueKind.Int => Bindings.spa_pod_builder_int(builderPtr, value.GetValue<int>()),
                PodValueKind.Long => Bindings.spa_pod_builder_long(builderPtr, value.GetValue<long>()),
                PodValueKind.Float => Bindings.spa_pod_builder_float(builderPtr, value.GetValue<float>()),
                PodValueKind.Double => Bindings.spa_pod_builder_double(builderPtr, value.GetValue<double>()),
                PodValueKind.String => PushString(builderPtr, value.GetValue<string>()),
                PodValueKind.Bytes => PushBytes(builderPtr, value.GetValue<byte[]>()),
                PodValueKind.Rectangle => Bindings.spa_pod_builder_rectangle(builderPtr, value.GetValue<SpaRectangle>().Width, value.GetValue<SpaRectangle>().Height),
                PodValueKind.Fraction => Bindings.spa_pod_builder_fraction(builderPtr, value.GetValue<SpaFraction>().Numerator, value.GetValue<SpaFraction>().Denominator),
                PodValueKind.Bitmap => throw new NotImplementedException(),
                PodValueKind.Array => throw new NotImplementedException(),
                PodValueKind.Struct => throw new NotImplementedException(),
                PodValueKind.Object => throw new NotImplementedException(),
                PodValueKind.Sequence => throw new NotImplementedException(),
                PodValueKind.Pointer => throw new NotImplementedException(),
                PodValueKind.Fd => throw new NotImplementedException(),
                PodValueKind.Choice => PushChoice(builderPtr, value.GetValue<PodChoice>()),
                PodValueKind.Pod => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
            
            if (result < 0)
                throw new Exception($"Failed to add property value! Error: {result}");
        }

        return this;
    }

    public Pod Build()
    {
        fixed (spa_pod_builder* builderPtr = &_builder)
        fixed (spa_pod_frame* framePtr = &_frame)
        {
            var pod = (IntPtr)Bindings.spa_pod_builder_pop(builderPtr, framePtr);
            
            return new Pod(pod);
        }
    }

    private static int PushString(spa_pod_builder* builderPtr, string str)
    {
        var strPtr = str.AllocUtf8Ptr();
        
        var result = Bindings.spa_pod_builder_string(builderPtr, (byte*)strPtr);
        
        Marshal.FreeHGlobal(strPtr);

        return result;
    }

    private static int PushBytes(spa_pod_builder* builderPtr, byte[] bytes)
    {
        fixed (byte* bytesPtr = bytes)
        {
            var result = Bindings.spa_pod_builder_bytes(builderPtr, bytesPtr, (uint)bytes.Length);
            return result;
        }
    }

    private int PushChoice(spa_pod_builder* builderPtr, PodChoice choice)
    {
        var choiceFrame = new spa_pod_frame();

        var result = Bindings.spa_pod_builder_push_choice(builderPtr, &choiceFrame, (uint)choice.Kind, choice.ExtraFlags);
        if (result < 0)
            return result;
        
        foreach (var value in choice.Values)
            PushValue(value);
        
        Bindings.spa_pod_builder_pop(builderPtr, &choiceFrame);
        return 0;
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(_dataPtr);
    }
}