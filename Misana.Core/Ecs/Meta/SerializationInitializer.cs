using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Misana.Core.Network;
using Misana.Serialization;

namespace Misana.Core.Ecs.Meta
{
    public class SerializationInitializer
    {
        public static void Initialize(List<Type> concreteTypes, List<Type> componentTypes)
        {
            var networkMessageType = typeof(NetworkMessage);
            var udpBaseType = typeof(UdpMessage);
            var rpcBaseType = typeof(IRpcMessage);
            var serializeGenericType = typeof(Serialize<>);
            var deserializeGenericType = typeof(Deserialize<>);

            var msgTypes = concreteTypes.Where(
                t => networkMessageType.IsAssignableFrom(t)
            ).ToList();

            var genericMsgInfo = typeof(MessageInfo<>);
            var bytesArrType =  Expression.Parameter(typeof(byte[]));
            var refBytes = Expression.Parameter(typeof(byte[]).MakeByRefType());
            var refInt = Expression.Parameter(typeof(int).MakeByRefType());
            var param = Expression.Parameter(networkMessageType);



            var writeInt32 = typeof(Serializer).GetMethod("WriteInt32", BindingFlags.Static | BindingFlags.Public);
            var writeInt16 = typeof(Serializer).GetMethod("WriteInt16", BindingFlags.Static | BindingFlags.Public);
            var writeInt64 = typeof(Serializer).GetMethod("WriteInt64", BindingFlags.Static | BindingFlags.Public);
            var writeSingle = typeof(Serializer).GetMethod("WriteSingle", BindingFlags.Static | BindingFlags.Public);
            var writeDouble = typeof(Serializer).GetMethod("WriteDouble", BindingFlags.Static | BindingFlags.Public);
            var writeUInt32 = typeof(Serializer).GetMethod("WriteUInt32", BindingFlags.Static | BindingFlags.Public);
            var writeUInt16 = typeof(Serializer).GetMethod("WriteUInt16", BindingFlags.Static | BindingFlags.Public);
            var writeUInt64 = typeof(Serializer).GetMethod("WriteUInt64", BindingFlags.Static | BindingFlags.Public);
            var writeByte = typeof(Serializer).GetMethod("WriteByte", BindingFlags.Static | BindingFlags.Public);
            var writeString = typeof(Serializer).GetMethod("WriteString", BindingFlags.Static | BindingFlags.Public);
            
            var readInt32 = typeof(Deserializer).GetMethod("ReadInt32", BindingFlags.Static | BindingFlags.Public);
            var readInt16 = typeof(Deserializer).GetMethod("ReadInt16", BindingFlags.Static | BindingFlags.Public);
            var readInt64 = typeof(Deserializer).GetMethod("ReadInt64", BindingFlags.Static | BindingFlags.Public);
            var readSingle = typeof(Deserializer).GetMethod("ReadSingle", BindingFlags.Static | BindingFlags.Public);
            var readDouble = typeof(Deserializer).GetMethod("ReadDouble", BindingFlags.Static | BindingFlags.Public);
            var readUInt32 = typeof(Deserializer).GetMethod("ReadUInt32", BindingFlags.Static | BindingFlags.Public);
            var readUInt16 = typeof(Deserializer).GetMethod("ReadUInt16", BindingFlags.Static | BindingFlags.Public);
            var readUInt64 = typeof(Deserializer).GetMethod("ReadUInt64", BindingFlags.Static | BindingFlags.Public);
            var readByte = typeof(Deserializer).GetMethod("ReadByte", BindingFlags.Static | BindingFlags.Public);
            var readString = typeof(Deserializer).GetMethod("ReadString", BindingFlags.Static | BindingFlags.Public);


            var baseMsgInfo = typeof(MessageInfo);

//            var baseSerialize = baseMsgInfo.GetField("Serializers", BindingFlags.Static | BindingFlags.Public);
//            var baseDeserialize = baseMsgInfo.GetField("Derializers", BindingFlags.Static | BindingFlags.Public);
//            var baseIsUdp = baseMsgInfo.GetField("IsUdp", BindingFlags.Static | BindingFlags.Public);
//            var baseIsRpc = baseMsgInfo.GetField("IsRpcMessage", BindingFlags.Static | BindingFlags.Public);

            MessageInfo.Serializers = new Serialize<NetworkMessage>[msgTypes.Count];
            MessageInfo.Deserializers = new Deserialize<NetworkMessage>[msgTypes.Count];
            MessageInfo.IsRpcMessage = new bool[msgTypes.Count];
            MessageInfo.IsUdp = new bool[msgTypes.Count];

            for (int i = 0; i < msgTypes.Count; i++)
            {
                var msgType = msgTypes[i];
                var msgInfoType = genericMsgInfo.MakeGenericType(msgType);
                var isUdp = udpBaseType.IsAssignableFrom(msgType);
                var isRpc = rpcBaseType.IsAssignableFrom(msgType);
                msgInfoType.GetField("Index", BindingFlags.Static | BindingFlags.Public).SetValue(null, i);
                msgInfoType.GetField("IsUdp", BindingFlags.Static | BindingFlags.Public).SetValue(null, isUdp);
                msgInfoType.GetField("IsRpcMessage", BindingFlags.Static | BindingFlags.Public).SetValue(null, isRpc);
                MessageInfo.IsUdp[i] = isUdp;
                MessageInfo.IsRpcMessage[i] = isRpc;


                var serializeBody = new List<Expression>();
                var deserializeBody = new List<Expression>();

                var serializesGenericType = typeof(Serializes<>);

                var serializeType = serializeGenericType.MakeGenericType(msgType);
                var deserializeType = deserializeGenericType.MakeGenericType(msgType);


                var msgParam = Expression.Parameter(msgType);
                var msgVar = Expression.Variable(msgType);

                var fields = msgType.GetFields().OrderBy(f => f.FieldType.IsPrimitive).ThenBy(f => f.Name).ToList();
                deserializeBody.Add(Expression.Assign(msgVar, Expression.New(msgType)));

                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(byte))
                    {
                        serializeBody.Add(Expression.Call(null, writeByte, Expression.Field(msgParam, field), refBytes, refInt));
                        deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, readByte, bytesArrType, refInt)));
                    }
                    else if (field.FieldType == typeof(short))
                    {
                        serializeBody.Add(Expression.Call(null, writeInt16, Expression.Field(msgParam, field), refBytes, refInt));
                        deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, readInt16, bytesArrType, refInt)));
                    }
                    else if (field.FieldType == typeof(int))
                    {
                        serializeBody.Add(Expression.Call(null, writeInt32, Expression.Field(msgParam, field), refBytes, refInt));
                        deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, readInt32, bytesArrType, refInt)));
                    }
                    else if (field.FieldType == typeof(long))
                    {
                        serializeBody.Add(Expression.Call(null, writeInt64, Expression.Field(msgParam, field), refBytes, refInt));
                        deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, readInt64, bytesArrType, refInt)));
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        serializeBody.Add(Expression.Call(null, writeSingle, Expression.Field(msgParam, field), refBytes, refInt));
                        deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, readSingle, bytesArrType, refInt)));
                    }
                    else if (field.FieldType == typeof(double))
                    {
                        serializeBody.Add(Expression.Call(null, writeDouble, Expression.Field(msgParam, field), refBytes, refInt));
                        deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, readDouble, bytesArrType, refInt)));
                    }
                    else if (field.FieldType == typeof(ushort))
                    {
                        serializeBody.Add(Expression.Call(null, writeUInt16, Expression.Field(msgParam, field), refBytes, refInt));
                        deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, readUInt16, bytesArrType, refInt)));
                    }
                    else if (field.FieldType == typeof(uint))
                    {
                        serializeBody.Add(Expression.Call(null, writeUInt32, Expression.Field(msgParam, field), refBytes, refInt));
                        deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, readUInt32, bytesArrType, refInt)));
                    }
                    else if (field.FieldType == typeof(ulong))
                    {
                        serializeBody.Add(Expression.Call(null, writeUInt64, Expression.Field(msgParam, field), refBytes, refInt));
                        deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, readUInt64, bytesArrType, refInt)));
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        serializeBody.Add(Expression.Call(null, writeString, Expression.Field(msgParam, field), refBytes, refInt));
                        deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, readString, bytesArrType, refInt)));
                    }
                    else
                    {
                        if (field.FieldType.IsConstructedGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            throw new NotImplementedException("List Serialization :(");
                        }
                        else if (field.FieldType.IsArray)
                        {
                            throw new NotImplementedException("Array Deserialization :(");
                        }
                        else
                        {
                            var serializes = serializesGenericType.MakeGenericType(field.FieldType);
                            serializeBody.Add(Expression.Invoke(
                                Expression.Field(null, serializes.GetField("Serialize", BindingFlags.Static | BindingFlags.Public)),
                                Expression.Field(msgParam, field), refBytes, refInt)
                            );

                            deserializeBody.Add(Expression.Assign(
                                Expression.Field(msgVar, field),
                                    Expression.Invoke(
                                        Expression.Field(null, serializes.GetField("Deserialize", BindingFlags.Static | BindingFlags.Public)),
                                       bytesArrType,refInt)
                                ));
                        }
                    }


                }


                var serialize = Expression.Lambda(serializeType, serializeBody.Count == 0 ? (Expression) Expression.Empty() : Expression.Block(serializeBody.ToArray()), msgParam, refBytes, refInt);

                // TODO: Check length
                deserializeBody.Add(msgVar);
                var deserialize = Expression.Lambda(deserializeType, Expression.Block(new [] { msgVar }, deserializeBody.ToArray()), bytesArrType, refInt);


                msgInfoType.GetField("Deserialize", BindingFlags.Static | BindingFlags.Public).SetValue(null, deserialize.Compile());
                msgInfoType.GetField("Serialize", BindingFlags.Static | BindingFlags.Public).SetValue(null, serialize.Compile());



                var serializeBase = Expression
                    .Lambda<Serialize<NetworkMessage>>(Expression.Invoke(serialize, Expression.Convert(param, msgType), refBytes, refInt), param, refBytes, refInt)
                    .Compile();

                msgInfoType.GetField("SerializeBase", BindingFlags.Static | BindingFlags.Public)
                    .SetValue(null, serializeBase);


                var deserializeBase = Expression.Lambda<Deserialize<NetworkMessage>>(Expression.Convert(Expression.Invoke(deserialize, bytesArrType, refInt), msgType), bytesArrType, refInt).Compile();
                msgInfoType.GetField("DeserializeBase", BindingFlags.Static | BindingFlags.Public)
                    .SetValue(null, deserializeBase);

                MessageInfo.Deserializers[i] = deserializeBase;
                MessageInfo.Serializers[i] = serializeBase;
                MessageInfo.IsUdp[i] = isUdp;
                MessageInfo.IsRpcMessage[i] = isRpc;
            }


        }
    }
}