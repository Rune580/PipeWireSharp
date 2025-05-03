using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PipeWireSharp.Native;
using PipeWireSharp.Spa.Pods.Choice;
using PipeWireSharp.Utils;

namespace PipeWireSharp.Spa.Pods;

public class Pod
{
    internal readonly IntPtr Handle;
    internal unsafe spa_pod* RawHandle => (spa_pod*)Handle;
    
    public Pod(IntPtr handle)
    {
        Handle = handle;
    }

    internal unsafe Pod(spa_pod* rawHandle)
    {
        Handle = (IntPtr)rawHandle;
    }


    internal unsafe bool IsNone() => Bindings.spa_pod_is_none(RawHandle) != 0;
    internal unsafe bool IsBool() => Bindings.spa_pod_is_bool(RawHandle) != 0;
    internal unsafe bool IsId() => Bindings.spa_pod_is_id(RawHandle) != 0;
    internal unsafe bool IsInt() => Bindings.spa_pod_is_int(RawHandle) != 0;
    internal unsafe bool IsLong() => Bindings.spa_pod_is_long(RawHandle) != 0;
    internal unsafe bool IsFloat() => Bindings.spa_pod_is_float(RawHandle) != 0;
    internal unsafe bool IsDouble() => Bindings.spa_pod_is_double(RawHandle) != 0;
    internal unsafe bool IsString() => Bindings.spa_pod_is_string(RawHandle) != 0;
    internal unsafe bool IsBytes() => Bindings.spa_pod_is_bytes(RawHandle) != 0;
    internal unsafe bool IsRectangle() => Bindings.spa_pod_is_rectangle(RawHandle) != 0;
    internal unsafe bool IsFraction() => Bindings.spa_pod_is_fraction(RawHandle) != 0;
    internal unsafe bool IsBitmap() => Bindings.spa_pod_is_bitmap(RawHandle) != 0;
    internal unsafe bool IsArray() => Bindings.spa_pod_is_array(RawHandle) != 0;
    internal unsafe bool IsStruct() => Bindings.spa_pod_is_struct(RawHandle) != 0;
    internal unsafe bool IsObject() => Bindings.spa_pod_is_object(RawHandle) != 0;
    internal unsafe bool IsSequence() => Bindings.spa_pod_is_sequence(RawHandle) != 0;
    internal unsafe bool IsFd() => Bindings.spa_pod_is_fd(RawHandle) != 0;
    internal unsafe bool IsChoice() => Bindings.spa_pod_is_choice(RawHandle) != 0;

    public unsafe PodValue ParsePod()
    {
        if (IsNone())
            return PodValue.None();

        if (IsBool())
        {
            var value = false;
            var result = Bindings.spa_pod_get_bool(RawHandle, &value);
            if (result < 0)
                throw new Exception("Failed to get bool!");
            return value;
        }
        
        if (IsId())
        {
            uint value = 0;
            var result = Bindings.spa_pod_get_id(RawHandle, &value);
            if (result < 0)
                throw new Exception("Failed to get id!");
            return PodValue.Id(value);
        }
        
        if (IsInt())
        {
            int value = 0;
            var result = Bindings.spa_pod_get_int(RawHandle, &value);
            if (result < 0)
                throw new Exception("Failed to get int!");
            return value;
        }
        
        if (IsLong())
        {
            long value = 0;
            var result = Bindings.spa_pod_get_long(RawHandle, &value);
            if (result < 0)
                throw new Exception("Failed to get long!");
            return value;
        }
        
        if (IsFloat())
        {
            float value = 0;
            var result = Bindings.spa_pod_get_float(RawHandle, &value);
            if (result < 0)
                throw new Exception("Failed to get float!");
            return value;
        }
        
        if (IsDouble())
        {
            double value = 0;
            var result = Bindings.spa_pod_get_double(RawHandle, &value);
            if (result < 0)
                throw new Exception("Failed to get double!");
            return value;
        }
        
        if (IsString())
        {
            var buffer = Marshal.AllocHGlobal(1024);
            var result = Bindings.spa_pod_get_string(RawHandle, (byte**)buffer); // Todo: verify this.
            var value = Marshal.PtrToStringAuto(buffer);
            
            if (result < 0 || value is null)
                throw new Exception("Failed to get string!");
            return value;
        }
        
        if (IsBytes())
        {
            var buffer = Marshal.AllocHGlobal(8096);

            uint len = 0; 
            
            var result = Bindings.spa_pod_get_bytes(RawHandle, (void**)buffer, &len); // Todo: verify this.
            if (result < 0)
                throw new Exception("Failed to get bytes!");

            var bytes = new byte[len];
            
            Marshal.Copy(buffer, bytes, 0, (int)len);
            
            return bytes;
        }
        
        if (IsRectangle())
        {
            var value = new spa_rectangle();
            var result = Bindings.spa_pod_get_rectangle(RawHandle, &value);
            if (result < 0)
                throw new Exception("Failed to get spa_rectangle!");
            return new SpaRectangle(value);
        }
        
        if (IsFraction())
        {
            var value = new spa_fraction();
            var result = Bindings.spa_pod_get_fraction(RawHandle, &value);
            if (result < 0)
                throw new Exception("Failed to get spa_fraction!");
            return new SpaFraction(value);
        }

        if (IsArray())
        {
            uint valueCount = 0;
            var arrayPtr = (IntPtr)Bindings.spa_pod_get_array(RawHandle, &valueCount);

            var podArray = Marshal.PtrToStructure<spa_pod_array>(Handle);
            var childValueKind = (PodValueKind)podArray.child_type;

            var children = new List<PodValue>();
            
            for (int i = 0; i < valueCount; i++)
            {
                var memSize = (int)(podArray.child_size + childValueKind.PodHeaderByteSize());
                var childPtr = Marshal.AllocHGlobal(memSize);

                Buffer.MemoryCopy(&podArray.child_size, (void*)childPtr, memSize, 4);

                Buffer.MemoryCopy(&podArray.child_type, (void*)(childPtr + 4), memSize, 4);
                
                Unsafe.CopyBlock((void*)(childPtr + 8), (void*)arrayPtr, podArray.child_size);
                
                var childPod = (spa_pod*)childPtr;
                children.Add(new Pod(childPod).ParsePod());
                
                arrayPtr = IntPtr.Add(arrayPtr, (int)podArray.child_size);
                
                Marshal.FreeHGlobal(childPtr);
            }
            
            return new PodArray(children.ToArray());
        }
        
        if (IsObject())
        {
            var spaPodObject = (spa_pod_object*)RawHandle;

            var podObject = new PodObject(spaPodObject->body.type_, spaPodObject->body.id);

            var iter = Bindings.spa_pod_prop_first(&spaPodObject->body);

            while (iter != null && Bindings.spa_pod_prop_is_inside(&spaPodObject->body, spaPodObject->pod.size, iter))
            {
                var key = iter->key;
                var flags = iter->flags;
                
                var iterPtr = IntPtr.Add(new IntPtr(iter), sizeof(uint) * 2);

                podObject.Properties[key] = new Pod(iterPtr).ParsePod();
            
                iter = Bindings.spa_pod_prop_next(iter);
            }

            return podObject;
        }

        if (IsChoice())
        {
            uint numValues, choice;
            var pod = Bindings.spa_pod_get_values(RawHandle, &numValues, &choice);
            
            IntPtr ptr;
            PodValue[]? values;
            switch ((ChoiceKind)choice)
            {
                case ChoiceKind.None:
                    return PodChoice.None(new Pod(pod).ParsePod());
                case ChoiceKind.Range:
                    ptr = (IntPtr)pod;

                    values = new PodValue[numValues];
                    
                    for (int i = 0; i < numValues; i++)
                    {
                        var childPod = (spa_pod*)ptr;
                        values[i] = new Pod(childPod).ParsePod();

                        ptr = IntPtr.Add(ptr, (int)childPod->size);
                    }

                    return PodChoice.Range(values[0], values[1], values[2]);
                case ChoiceKind.Step:
                    ptr = (IntPtr)pod;
                    values = new PodValue[numValues];
                    
                    for (int i = 0; i < numValues; i++)
                    {
                        var childPod = (spa_pod*)ptr;
                        values[i] = new Pod(childPod).ParsePod();

                        ptr = IntPtr.Add(ptr, (int)childPod->size);
                    }

                    return PodChoice.Step(values[0], values[1], values[2], values[3]);
                case ChoiceKind.Enum:
                    ptr = (IntPtr)pod;
                    values = new PodValue[numValues];
                    
                    for (int i = 0; i < numValues; i++)
                    {
                        var childPod = (spa_pod*)ptr;
                        values[i] = new Pod(childPod).ParsePod();

                        ptr = IntPtr.Add(ptr, (int)childPod->size);
                    }

                    return PodChoice.Enum(values[0], values.Skip(1).ToArray());
                case ChoiceKind.Flags:
                    ptr = (IntPtr)pod;
                    values = new PodValue[numValues];
                    
                    for (int i = 0; i < numValues; i++)
                    {
                        var childPod = (spa_pod*)ptr;
                        values[i] = new Pod(childPod).ParsePod();

                        ptr = IntPtr.Add(ptr, (int)childPod->size);
                    }

                    return PodChoice.Flags(values[0], values.Skip(1).ToArray());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        Console.WriteLine($"Encountered Unhandled Pod Type: {RawHandle->type_}!");
        
        return PodValue.None();
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct spa_pod_array
    {
        public uint size;
        public uint type_;
        public uint child_size;
        public uint child_type;
    }
}