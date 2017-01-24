using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Misana.Network
{
    public static class MessageHandleManager
    {
        private static int maxIndex = 0;

        private static Dictionary<Type,MessageIdPair> SystemPairs = new Dictionary<Type, MessageIdPair>();


        static MessageHandleManager()
        {
            var structs = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsValueType && !type.IsEnum
                let attribute = type.GetCustomAttribute<MessageDefinitionAttribute>(false)
                where attribute != null
                select new {Type = type,Attribute = attribute};

            foreach (var @struct in structs)
            {
                RegisterType(@struct.Type);
                CreateMessageHandle(@struct.Type).Initialize(@struct.Attribute);
            }
        }

        public static int RegisterType<T>()
            where T : struct
        {
            return RegisterType(typeof(T));
        }

        public static int RegisterType(Type type)
        {

            if (SystemPairs.ContainsKey(type))
                throw new ArgumentException("SystemId already");

            var systemId =  Interlocked.Increment(ref maxIndex) -1;

            SystemPairs.Add(type, new MessageIdPair(systemId,type));


            return systemId;
        }


        public static int RegisterType<T>(int index)
            where T : struct
        {
            return RegisterType(typeof(T),index);
        }

        public static int RegisterType(Type type,int index)
        {

            if (SystemPairs.ContainsKey(type))
                throw new ArgumentException("SystemId already");

            SystemPairs.Add(type, new MessageIdPair(index,type));

            if (maxIndex < index)
                maxIndex = index;

            return index;
        }

        public static int? GetId<T>()
            where T : struct
        {
            return GetId(typeof(T));
        }

        public static int? GetId(Type type)
        {
            if (SystemPairs.ContainsKey(type))
            {
                return SystemPairs[type].SystemId;
            }

            return null;
        }

        internal static MessageHandle CreateMessageHandle(Type type)
        {
            var generictype = typeof(MessageHandle<>).MakeGenericType(type);

            var instance = (MessageHandle) Activator.CreateInstance(generictype);

            return instance;
        }

        internal static MessageHandle[] CreateHandleArray()
        {
            MessageHandle[] array = new MessageHandle[SystemPairs.Count];
            int i = 0;
            foreach (var pair in SystemPairs)
            {
                array[i++] = CreateMessageHandle(pair.Key);
            }

            return array;
        }


    }
}