using Google.Protobuf;
using Google.Protobuf.Reflection;
using System.Text.Json;

namespace EasyDapr.Core.Grpc
{
    public class DynamicMessageWrapper : IMessage<DynamicMessageWrapper>
    {
        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly object _data;

        public DynamicMessageWrapper(object data)
        {
            _data = data;
        }

        // 序列化到 CodedOutputStream
        public void WriteTo(CodedOutputStream output)
        {
            var json = JsonSerializer.Serialize(_data, _serializerOptions);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            output.WriteBytes(ByteString.CopyFrom(bytes));
        }

        // 计算序列化大小
        public int CalculateSize()
        {
            var json = JsonSerializer.Serialize(_data, _serializerOptions);
            return System.Text.Encoding.UTF8.GetByteCount(json);
        }

        // 合并数据（不适用，抛出异常）
        public void MergeFrom(CodedInputStream input)
        {
            throw new NotImplementedException("Deserialization is not supported in this wrapper.");
        }

        // 合并消息（不适用，抛出异常）
        public void MergeFrom(DynamicMessageWrapper message)
        {
            throw new NotImplementedException("Deserialization is not supported in this wrapper.");
        }

        // 克隆数据
        public DynamicMessageWrapper Clone()
        {
            return new DynamicMessageWrapper(_data);
        }

        // 描述符（必需实现，但可以返回 null 或自定义值）
        public MessageDescriptor Descriptor => null;

        // 比较相等性
        public bool Equals(DynamicMessageWrapper other)
        {
            if (other == null) return false;
            return JsonSerializer.Serialize(_data) == JsonSerializer.Serialize(other._data);
        }
    }
}