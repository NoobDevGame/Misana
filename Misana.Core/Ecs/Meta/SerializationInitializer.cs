using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using Misana.Core.Network;
using Misana.Serialization;

namespace Misana.Core.Ecs.Meta
{
    public class SerializationInitializer
    {
        private static MethodInfo _writeInt32;
        private static MethodInfo _writeInt16;
        private static MethodInfo _writeInt64;
        private static MethodInfo _writeSingle;
        private static MethodInfo _writeDouble;
        private static MethodInfo _writeUInt32;
        private static MethodInfo _writeUInt16;
        private static MethodInfo _writeUInt64;
        private static MethodInfo _writeByte;
        private static MethodInfo _writeBoolean;
        private static MethodInfo _writeString;
        private static MethodInfo _readInt32;
        private static MethodInfo _readInt16;
        private static MethodInfo _readInt64;
        private static MethodInfo _readSingle;
        private static MethodInfo _readDouble;
        private static MethodInfo _readUInt32;
        private static MethodInfo _readUInt16;
        private static MethodInfo _readUInt64;
        private static MethodInfo _readByte;
        private static MethodInfo _readBoolean;
        private static MethodInfo _readString;
        private static Type _serializesGenericType;
        private static ParameterExpression _bytesArrParam;
        private static ParameterExpression _refBytesArrParam;
        private static ParameterExpression _refIntParam;
        private static Type _serializeGenericType;
        private static Type _deserializeGenericType;
        private static MethodInfo DebugLog;

        public static void Initialize(List<Type> concreteTypes, List<Type> componentTypes)
        {
            _serializeGenericType = typeof(Serialize<>);
            _deserializeGenericType = typeof(Deserialize<>);

            DebugLog = typeof(Debug).GetMethod("WriteLine", new Type[] { typeof(string)});


            _bytesArrParam = Expression.Parameter(typeof(byte[]));
            _refBytesArrParam = Expression.Parameter(typeof(byte[]).MakeByRefType());
            _refIntParam = Expression.Parameter(typeof(int).MakeByRefType());

            _writeInt32 = typeof(Serializer).GetMethod("WriteInt32", BindingFlags.Static | BindingFlags.Public);
            _writeInt16 = typeof(Serializer).GetMethod("WriteInt16", BindingFlags.Static | BindingFlags.Public);
            _writeInt64 = typeof(Serializer).GetMethod("WriteInt64", BindingFlags.Static | BindingFlags.Public);
            _writeSingle = typeof(Serializer).GetMethod("WriteSingle", BindingFlags.Static | BindingFlags.Public);
            _writeDouble = typeof(Serializer).GetMethod("WriteDouble", BindingFlags.Static | BindingFlags.Public);
            _writeUInt32 = typeof(Serializer).GetMethod("WriteUInt32", BindingFlags.Static | BindingFlags.Public);
            _writeUInt16 = typeof(Serializer).GetMethod("WriteUInt16", BindingFlags.Static | BindingFlags.Public);
            _writeUInt64 = typeof(Serializer).GetMethod("WriteUInt64", BindingFlags.Static | BindingFlags.Public);
            _writeByte = typeof(Serializer).GetMethod("WriteByte", BindingFlags.Static | BindingFlags.Public);
            _writeBoolean = typeof(Serializer).GetMethod("WriteBoolean", BindingFlags.Static | BindingFlags.Public);
            _writeString = typeof(Serializer).GetMethod("WriteString", BindingFlags.Static | BindingFlags.Public);

            _readInt32 = typeof(Deserializer).GetMethod("ReadInt32", BindingFlags.Static | BindingFlags.Public);
            _readInt16 = typeof(Deserializer).GetMethod("ReadInt16", BindingFlags.Static | BindingFlags.Public);
            _readInt64 = typeof(Deserializer).GetMethod("ReadInt64", BindingFlags.Static | BindingFlags.Public);
            _readSingle = typeof(Deserializer).GetMethod("ReadSingle", BindingFlags.Static | BindingFlags.Public);
            _readDouble = typeof(Deserializer).GetMethod("ReadDouble", BindingFlags.Static | BindingFlags.Public);
            _readUInt32 = typeof(Deserializer).GetMethod("ReadUInt32", BindingFlags.Static | BindingFlags.Public);
            _readUInt16 = typeof(Deserializer).GetMethod("ReadUInt16", BindingFlags.Static | BindingFlags.Public);
            _readUInt64 = typeof(Deserializer).GetMethod("ReadUInt64", BindingFlags.Static | BindingFlags.Public);
            _readByte = typeof(Deserializer).GetMethod("ReadByte", BindingFlags.Static | BindingFlags.Public);
            _readBoolean = typeof(Deserializer).GetMethod("ReadBoolean", BindingFlags.Static | BindingFlags.Public);
            _readString = typeof(Deserializer).GetMethod("ReadString", BindingFlags.Static | BindingFlags.Public);


            _serializesGenericType = typeof(Serializes<>);


            InitializeEntitySerializer(componentTypes);
            InitializeEntityBuilderSerializer(componentTypes);
            InitializeComponentSerializers(componentTypes);
            InitializeMessageTypeSerializers(concreteTypes);
        }

        private static void InitializeEntitySerializer(List<Type> componentTypes)
        {
            var serializeBody = new List<Expression>();
            var deserializeBody = new List<Expression>();

            var entityType = typeof(Entity);

            var serializeType = _serializeGenericType.MakeGenericType(entityType);
            var deserializeType = _deserializeGenericType.MakeGenericType(entityType);

            var serializesType = _serializesGenericType.MakeGenericType(entityType);

            var entityParam = Expression.Parameter(entityType);
            var entityVar = Expression.Variable(entityType);

            deserializeBody.Add(Expression.Assign(entityVar, Expression.New(entityType)));

            serializeBody.Add(Expression.Call(null, _writeInt32, Expression.Field(entityParam, entityType.GetField("Id")), _refBytesArrParam, _refIntParam));
            deserializeBody.Add(Expression.Assign( Expression.Field(entityVar, entityType.GetField("Id")), Expression.Call(null, _readInt32, _bytesArrParam, _refIntParam)));


            var cmpArrField = typeof(Entity).GetField("Components");


            var cmpArrParam = Expression.Field(entityParam, cmpArrField);
            var cmpArrVar = Expression.Field(entityVar, cmpArrField);

            for (var i = 0; i < componentTypes.Count; i++)
            {
                var cmpType = componentTypes[i];
                var cmpSerializesType = _serializesGenericType.MakeGenericType(cmpType);
                //var cmpDeserializeType = _serializesGenericType.MakeGenericType(cmpType);

                var desField = cmpSerializesType.GetField("Deserialize", BindingFlags.Static | BindingFlags.Public);
                var seField = cmpSerializesType.GetField("Serialize", BindingFlags.Static | BindingFlags.Public);

                serializeBody.Add(
                        Expression.IfThenElse(
                            Expression.Equal(Expression.ArrayIndex(cmpArrParam, Expression.Constant(i)), Expression.Constant(null, typeof(Component))),
                            Expression.Call(null, _writeBoolean, Expression.Constant(false), _refBytesArrParam, _refIntParam),
                            Expression.Block(
                                Expression.Call(null, _writeBoolean, Expression.Constant(true), _refBytesArrParam, _refIntParam),
                                Expression.Invoke(Expression.Field(null, seField), Expression.Convert(Expression.ArrayIndex(cmpArrParam, Expression.Constant(i)), cmpType), _refBytesArrParam, _refIntParam)
                            )
                        )
                );

                deserializeBody.Add(
                        Expression.IfThen(
                            Expression.IsTrue(Expression.Call(null, _readBoolean, _bytesArrParam, _refIntParam)),
                            Expression.Assign(Expression.ArrayAccess(cmpArrVar, Expression.Constant(i)), Expression.Invoke(Expression.Field(null, desField), _bytesArrParam, _refIntParam) )
                        )
                );
            }

            var serialize = Expression.Lambda(serializeType, serializeBody.Count == 0 ? (Expression) Expression.Empty() : Expression.Block(serializeBody.ToArray()), entityParam, _refBytesArrParam, _refIntParam);

            // TODO: Check length
            deserializeBody.Add(entityVar);
            var deserialize = Expression.Lambda(deserializeType, Expression.Block(new [] { entityVar }, deserializeBody.ToArray()), _bytesArrParam, _refIntParam);


            serializesType.GetField("Deserialize", BindingFlags.Static | BindingFlags.Public).SetValue(null, deserialize.Compile());
            serializesType.GetField("Serialize", BindingFlags.Static | BindingFlags.Public).SetValue(null, serialize.Compile());
        }


        private static void InitializeEntityBuilderSerializer(List<Type> componentTypes)
        {
            var serializeBody = new List<Expression>();
            var deserializeBody = new List<Expression>();

            var builderType = typeof(EntityBuilder);

            var serializeType = _serializeGenericType.MakeGenericType(builderType);
            var deserializeType = _deserializeGenericType.MakeGenericType(builderType);

            var serializesType = _serializesGenericType.MakeGenericType(builderType);

            var entityParam = Expression.Parameter(builderType);
            var entityVar = Expression.Variable(builderType);

            var cmpArrField = typeof(EntityBuilder).GetField("Components");
            deserializeBody.Add(Expression.Assign(entityVar, Expression.New(builderType)));



            var cmpArrParam = Expression.Field(entityParam, cmpArrField);
            var cmpArrVar = Expression.Field(entityVar, cmpArrField);

            for (var i = 0; i < componentTypes.Count; i++)
            {
                var cmpType = componentTypes[i];
                var cmpSerializesType = _serializesGenericType.MakeGenericType(cmpType);
                //var cmpDeserializeType = _serializesGenericType.MakeGenericType(cmpType);

                var desField = cmpSerializesType.GetField("Deserialize", BindingFlags.Static | BindingFlags.Public);
                var seField = cmpSerializesType.GetField("Serialize", BindingFlags.Static | BindingFlags.Public);

                serializeBody.Add(
                    Expression.IfThenElse(
                        Expression.Equal(Expression.ArrayIndex(cmpArrParam, Expression.Constant(i)), Expression.Constant(null, typeof(Component))),
                        Expression.Call(null, _writeBoolean, Expression.Constant(false), _refBytesArrParam, _refIntParam),
                        Expression.Block(
                            Expression.Call(null, _writeBoolean, Expression.Constant(true), _refBytesArrParam, _refIntParam),
                            Expression.Invoke(Expression.Field(null, seField), Expression.Convert(Expression.ArrayIndex(cmpArrParam, Expression.Constant(i)), cmpType), _refBytesArrParam, _refIntParam)
                        )
                    )
                );

                deserializeBody.Add(
                    Expression.IfThen(
                        Expression.IsTrue(Expression.Call(null, _readBoolean, _bytesArrParam, _refIntParam)),
                        Expression.Assign(Expression.ArrayAccess(cmpArrVar, Expression.Constant(i)), Expression.Invoke(Expression.Field(null, desField), _bytesArrParam, _refIntParam) )
                    )
                );
            }

            var attachedEntitiesField = builderType.GetField("AttachedEntities");
            var listCount = typeof(List<AttachedEntity>).GetProperty("Count");
            var listAdd = typeof(List<AttachedEntity>).GetMethod("Add");
            var countOfAttachedEntityExpr = Expression.Property(Expression.Field(entityParam, attachedEntitiesField), listCount);


            var serializeAttachedEntity = typeof(Serializes<AttachedEntity>).GetField("Serialize", BindingFlags.Static | BindingFlags.Public);
            var deserializeAttachedEntity = typeof(Serializes<AttachedEntity>).GetField("Deserialize", BindingFlags.Static | BindingFlags.Public);

            serializeBody.Add(Expression.Call(null, _writeInt32, countOfAttachedEntityExpr,_refBytesArrParam, _refIntParam));
            var aeVar = Expression.Variable(typeof(AttachedEntity));
            ComponentInitializer.ForEach(
                Expression.Field(entityParam, attachedEntitiesField),
                aeVar,
                Expression.Invoke(Expression.Field(null, serializeAttachedEntity), aeVar, _refBytesArrParam, _refIntParam)
            );

            var cntVar = Expression.Variable(typeof(int));
            var lbl = Expression.Label();
            deserializeBody.Add(
                    Expression.Block(new [] { cntVar },
                        Expression.Assign(cntVar, Expression.Call(null, _readInt32, _bytesArrParam, _refIntParam)),
                        Expression.Loop(
                            Expression.IfThenElse(
                                Expression.GreaterThanOrEqual(cntVar, Expression.Constant(1)),
                                Expression.Block(
                                    Expression.Call(Expression.Field(entityVar, attachedEntitiesField), listAdd, Expression.Invoke(Expression.Field(null, deserializeAttachedEntity), _bytesArrParam, _refIntParam)),
                                    Expression.Assign(cntVar, Expression.Decrement(cntVar))
                                ),
                                Expression.Break(lbl)
                            ), lbl
                        )
                    )
            );


            var serialize = Expression.Lambda(serializeType, serializeBody.Count == 0 ? (Expression) Expression.Empty() : Expression.Block(serializeBody.ToArray()), entityParam, _refBytesArrParam, _refIntParam);

            // TODO: Check length
            deserializeBody.Add(entityVar);
            var deserialize = Expression.Lambda(deserializeType, Expression.Block(new [] { entityVar }, deserializeBody.ToArray()), _bytesArrParam, _refIntParam);


            serializesType.GetField("Deserialize", BindingFlags.Static | BindingFlags.Public).SetValue(null, deserialize.Compile());
            serializesType.GetField("Serialize", BindingFlags.Static | BindingFlags.Public).SetValue(null, serialize.Compile());
        }

        private static void InitializeComponentSerializers(List<Type> componentTypes)
        {
            for (var i = 0; i < componentTypes.Count; i++)
            {
                var cmpType = componentTypes[i];

                var fields = cmpType.GetFields()
                    .Where(f => f.GetCustomAttribute<CopyAttribute>() != null)
                    .OrderBy(f => f.FieldType.IsPrimitive).ThenBy(f => f.Name).ToList();

                var serializeBody = new List<Expression>();
                var deserializeBody = new List<Expression>();


                var serializesType = _serializesGenericType.MakeGenericType(cmpType);
                var serializeType = _serializeGenericType.MakeGenericType(cmpType);
                var deserializeType = _deserializeGenericType.MakeGenericType(cmpType);


                var cmpParam = Expression.Parameter(cmpType);
                var cmpVar = Expression.Variable(cmpType);

                var registryType = typeof(ComponentRegistry<>).MakeGenericType(cmpType);

                deserializeBody.Add(Expression.Assign(cmpVar, Expression.Call(null, registryType.GetMethod("Take", BindingFlags.Static | BindingFlags.Public))));
                GenerateFieldExpressions(fields, serializeBody, cmpParam, deserializeBody, cmpVar);

                var serialize = Expression.Lambda(serializeType, serializeBody.Count == 0 ? (Expression) Expression.Empty() : Expression.Block(serializeBody.ToArray()), cmpParam, _refBytesArrParam, _refIntParam);

                // TODO: Check length
                deserializeBody.Add(cmpVar);
                var deserialize = Expression.Lambda(deserializeType, Expression.Block(new [] { cmpVar }, deserializeBody.ToArray()), _bytesArrParam, _refIntParam);


                serializesType.GetField("Deserialize", BindingFlags.Static | BindingFlags.Public).SetValue(null, deserialize.Compile());
                serializesType.GetField("Serialize", BindingFlags.Static | BindingFlags.Public).SetValue(null, serialize.Compile());
            }
        }

        private static void InitializeMessageTypeSerializers(List<Type> concreteTypes)
        {
            var networkMessageType = typeof(NetworkMessage);
            var udpBaseType = typeof(UdpMessage);
            var rpcBaseType = typeof(IRpcMessage);
            var msgTypes = concreteTypes.Where(
                t => networkMessageType.IsAssignableFrom(t)
            ).ToList();

            var genericMsgInfo = typeof(MessageInfo<>);
            var param = Expression.Parameter(networkMessageType);

            var baseMsgInfo = typeof(MessageInfo);

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


                var serializeType = _serializeGenericType.MakeGenericType(msgType);
                var deserializeType = _deserializeGenericType.MakeGenericType(msgType);


                var msgParam = Expression.Parameter(msgType);
                var msgVar = Expression.Variable(msgType);

                var fields = msgType.GetFields().OrderBy(f => f.FieldType.IsPrimitive).ThenBy(f => f.Name).ToList();
                deserializeBody.Add(Expression.Assign(msgVar, Expression.New(msgType)));

                GenerateFieldExpressions(fields, serializeBody, msgParam, deserializeBody, msgVar);


                var serialize = Expression.Lambda(serializeType, serializeBody.Count == 0 ? (Expression) Expression.Empty() : Expression.Block(serializeBody.ToArray()), msgParam, _refBytesArrParam, _refIntParam);

                // TODO: Check length
                deserializeBody.Add(msgVar);
                var deserialize = Expression.Lambda(deserializeType, Expression.Block(new [] { msgVar }, deserializeBody.ToArray()), _bytesArrParam, _refIntParam);


                msgInfoType.GetField("Deserialize", BindingFlags.Static | BindingFlags.Public).SetValue(null, deserialize.Compile());
                msgInfoType.GetField("Serialize", BindingFlags.Static | BindingFlags.Public).SetValue(null, serialize.Compile());



                var serializeBase = Expression
                    .Lambda<Serialize<NetworkMessage>>(Expression.Invoke(serialize, Expression.Convert(param, msgType), _refBytesArrParam, _refIntParam), param, _refBytesArrParam, _refIntParam)
                    .Compile();

                msgInfoType.GetField("SerializeBase", BindingFlags.Static | BindingFlags.Public)
                    .SetValue(null, serializeBase);


                var deserializeBase = Expression.Lambda<Deserialize<NetworkMessage>>(Expression.Convert(Expression.Invoke(deserialize, _bytesArrParam, _refIntParam), msgType), _bytesArrParam, _refIntParam).Compile();
                msgInfoType.GetField("DeserializeBase", BindingFlags.Static | BindingFlags.Public)
                    .SetValue(null, deserializeBase);

                MessageInfo.Deserializers[i] = deserializeBase;
                MessageInfo.Serializers[i] = serializeBase;
                MessageInfo.IsUdp[i] = isUdp;
                MessageInfo.IsRpcMessage[i] = isRpc;
            }
        }

        private static void GenerateFieldExpressions(List<FieldInfo> fields,
            List<Expression> serializeBody,
            ParameterExpression msgParam,
            List<Expression> deserializeBody,
            ParameterExpression msgVar)
        {
            foreach (var field in fields)
            {
              //  serializeBody.Add(Expression.Call(null, DebugLog, Expression.Constant($"S: {field.Name}")));
              //  deserializeBody.Add(Expression.Call(null, DebugLog, Expression.Constant($"D: {field.Name}")));
                if (field.FieldType == typeof(byte))
                {
                    serializeBody.Add(Expression.Call(null, _writeByte, Expression.Field(msgParam, field), _refBytesArrParam, _refIntParam));
                    deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, _readByte, _bytesArrParam, _refIntParam)));
                }
                if (field.FieldType == typeof(bool))
                {
                    serializeBody.Add(Expression.Call(null, _writeBoolean, Expression.Field(msgParam, field), _refBytesArrParam, _refIntParam));
                    deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, _readBoolean, _bytesArrParam, _refIntParam)));
                }
                else if (field.FieldType == typeof(short))
                {
                    serializeBody.Add(Expression.Call(null, _writeInt16, Expression.Field(msgParam, field), _refBytesArrParam, _refIntParam));
                    deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, _readInt16, _bytesArrParam, _refIntParam)));
                }
                else if (field.FieldType == typeof(int))
                {
                    serializeBody.Add(Expression.Call(null, _writeInt32, Expression.Field(msgParam, field), _refBytesArrParam, _refIntParam));
                    deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, _readInt32, _bytesArrParam, _refIntParam)));
                }
                else if (field.FieldType == typeof(long))
                {
                    serializeBody.Add(Expression.Call(null, _writeInt64, Expression.Field(msgParam, field), _refBytesArrParam, _refIntParam));
                    deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, _readInt64, _bytesArrParam, _refIntParam)));
                }
                else if (field.FieldType == typeof(float))
                {
                    serializeBody.Add(Expression.Call(null, _writeSingle, Expression.Field(msgParam, field), _refBytesArrParam, _refIntParam));
                    deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, _readSingle, _bytesArrParam, _refIntParam)));
                }
                else if (field.FieldType == typeof(double))
                {
                    serializeBody.Add(Expression.Call(null, _writeDouble, Expression.Field(msgParam, field), _refBytesArrParam, _refIntParam));
                    deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, _readDouble, _bytesArrParam, _refIntParam)));
                }
                else if (field.FieldType == typeof(ushort))
                {
                    serializeBody.Add(Expression.Call(null, _writeUInt16, Expression.Field(msgParam, field), _refBytesArrParam, _refIntParam));
                    deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, _readUInt16, _bytesArrParam, _refIntParam)));
                }
                else if (field.FieldType == typeof(uint))
                {
                    serializeBody.Add(Expression.Call(null, _writeUInt32, Expression.Field(msgParam, field), _refBytesArrParam, _refIntParam));
                    deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, _readUInt32, _bytesArrParam, _refIntParam)));
                }
                else if (field.FieldType == typeof(ulong))
                {
                    serializeBody.Add(Expression.Call(null, _writeUInt64, Expression.Field(msgParam, field), _refBytesArrParam, _refIntParam));
                    deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, _readUInt64, _bytesArrParam, _refIntParam)));
                }
                else if (field.FieldType == typeof(string))
                {
                    serializeBody.Add(Expression.Call(null, _writeString, Expression.Field(msgParam, field), _refBytesArrParam, _refIntParam));
                    deserializeBody.Add(Expression.Assign(Expression.Field(msgVar, field), Expression.Call(null, _readString, _bytesArrParam, _refIntParam)));
                }
                else
                {

                    if (field.FieldType.IsConstructedGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var serializesList = _serializesGenericType.MakeGenericType(field.FieldType);
                        var listSerialize = Expression.Field(null, serializesList.GetField("Serialize", BindingFlags.Static | BindingFlags.Public));
                        var listDeserialize = Expression.Field(null, serializesList.GetField("Deserialize", BindingFlags.Static | BindingFlags.Public));

                        var loopItemVar = Expression.Variable(field.FieldType.GenericTypeArguments[0]);
                        var itemSerialize = _serializesGenericType.MakeGenericType(field.FieldType.GenericTypeArguments[0])
                            .GetField("Serialize", BindingFlags.Static | BindingFlags.Public);
                        var itemDeserialize = _serializesGenericType.MakeGenericType(field.FieldType.GenericTypeArguments[0])
                            .GetField("Deserialize", BindingFlags.Static | BindingFlags.Public);


                        var listAdd = field.FieldType.GetMethod("Add");

                        serializeBody.Add(Expression.IfThenElse(
                                Expression.Equal(listSerialize, Expression.Constant(null, _serializeGenericType.MakeGenericType(field.FieldType))),
                                Expression.Block(
                         //           Expression.Call(null, DebugLog, Expression.Call(Expression.Property(Expression.Field(msgParam, field), "Count"), typeof(int).GetMethod("ToString", Type.EmptyTypes))),
                                    Expression.Call(null, _writeInt32, Expression.Property(Expression.Field(msgParam, field), "Count"), _refBytesArrParam, _refIntParam),
                                    ComponentInitializer.ForEach(Expression.Field(msgParam, field), loopItemVar,
                                        Expression.Invoke(Expression.Field(null, itemSerialize), loopItemVar, _refBytesArrParam, _refIntParam))
                                ),
                                Expression.Invoke(listSerialize, Expression.Field(msgParam, field), _refBytesArrParam, _refIntParam)
                        ));
                        var cntVar = Expression.Variable(typeof(int));
                        var listVar = Expression.Variable(field.FieldType);
                        var lbl = Expression.Label();
                        deserializeBody.Add(
                            Expression.IfThenElse(
                                Expression.Equal(listDeserialize, Expression.Constant(null, _deserializeGenericType.MakeGenericType(field.FieldType))),
                                Expression.Block( new [] { cntVar, listVar},
                                    Expression.Assign(listVar, Expression.New(field.FieldType)),
                                    Expression.Assign(cntVar, Expression.Call(null, _readInt32, _bytesArrParam, _refIntParam)),
                            //        Expression.Call(null, DebugLog, Expression.Call(cntVar, typeof(int).GetMethod("ToString", Type.EmptyTypes))),
                                    Expression.Loop(
                                            Expression.IfThenElse(
                                                Expression.GreaterThanOrEqual(cntVar, Expression.Constant(1)),
                                                Expression.Block(
                                                    Expression.Call(listVar, listAdd, Expression.Invoke(Expression.Field(null, itemDeserialize), _bytesArrParam, _refIntParam)),
                                                    Expression.Assign(cntVar, Expression.Decrement(cntVar))
                                                ),
                                                Expression.Break(lbl)
                                            ), lbl
                                    ),
                                    Expression.Assign(Expression.Field(msgVar, field), listVar)
                                ),
                                Expression.Assign(Expression.Field(msgVar, field), Expression.Invoke(listDeserialize,  _bytesArrParam, _refIntParam))
                        ));
                    }
                    else if (field.FieldType.IsArray)
                    {
                        throw new NotImplementedException("Array Deserialization :(");
                    }
                    else
                    {
                        var serializes = _serializesGenericType.MakeGenericType(field.FieldType);
                        serializeBody.Add(
                            Expression.Invoke(
                                Expression.Field(null, serializes.GetField("Serialize", BindingFlags.Static | BindingFlags.Public)),
                                Expression.Field(msgParam, field),
                                _refBytesArrParam,
                                _refIntParam)
                        );

                        deserializeBody.Add(
                            Expression.Assign(
                                Expression.Field(msgVar, field),
                                Expression.Invoke(
                                    Expression.Field(null, serializes.GetField("Deserialize", BindingFlags.Static | BindingFlags.Public)),
                                    _bytesArrParam,
                                    _refIntParam)
                            ));
                    }
                }
            }
        }
    }
}