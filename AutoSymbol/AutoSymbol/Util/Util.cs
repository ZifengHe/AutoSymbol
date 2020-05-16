using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

using AutoSymbol.Core;

namespace System
{
    using AutoSymbol;
    using System.ArrayExtensions;
    using System.Runtime.CompilerServices;

    public static class d
    {
        public static SortedDictionary<string, SortedDictionary<int, TreeMessageNode>> MsgDict = new SortedDictionary<string, SortedDictionary<int, TreeMessageNode>>();
        public static string TrackingSig = "NA";
        public static ER TrackingER;
        public static void BreakOnCondition(bool b)
        {
            if (b)
                Debugger.Break();
        }
        public static void BreakOnSequence(int num)
        {
            if (OneTransform.GlobalSequenceNum >= num)
                Debugger.Break();
        }
        public static void Info(string msg)
        {
            Trace.WriteLine(msg);
        }

        public static void Info(string format, params string[] args)
        {
            Trace.WriteLine(string.Format(format, args));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LogTreeMessage(string msg, params object [] objs)
        {
            if (TreeMessageNode.Enabled)
            {
                string key = BuildStackNameKeys();
                TreeMessageNode tmd = new TreeMessageNode();
                tmd.Message = msg;
                tmd.Objs = objs;
                if (MsgDict.ContainsKey(key) == false)
                    MsgDict[key] = new SortedDictionary<int, TreeMessageNode>();
                MsgDict[key][tmd.Id] = tmd;
            }
        }

        public static void StopTreeMessage()
        {
            TreeMessageNode.Enabled = false;
            TreeMessageNode.LastId = 0;
            MsgDict.Clear();
        }       

        public static void StartTreeMessage(string msg, bool b, params object[] objs)
        {
            if (b)
            {
                TreeMessageNode.Enabled = true;
                TreeMessageNode.StartFrameIndex = new StackTrace()
                    .GetFrames().Where(x => x.GetMethod().ReflectedType.FullName.Contains("AutoSymbol")).Count();
                LogTreeMessage(msg, objs);
            }
            else
            {
                TreeMessageNode.Enabled = false;
            }
        }

        private static string BuildStackNameKeys()
        {
            var st = new StackTrace();
            StackFrame[] sfs = st.GetFrames().Where(x=>x.GetMethod().ReflectedType.FullName.Contains("AutoSymbol")).ToArray();
            StringBuilder sb = new StringBuilder();
            for (int i = sfs.Length - TreeMessageNode.StartFrameIndex; i>=0; i--)
            {
                sb.AppendFormat("{0}|", sfs[i].GetMethod().Name);
            }
            return sb.ToString();
        }
    }

    public class TreeMessageNode
    {        
        public static bool Enabled = false;
        public static int LastId = 0;
        public static int StartFrameIndex =0;
        public object [] Objs;
        public string Message;
        public int Id;

        public List<TreeMessageNode> Children = new List<TreeMessageNode>();
        public TreeMessageNode Parent;   
        
        public TreeMessageNode()
        {
            Id = LastId;
            LastId++;
        }

        public string ToDisplayMessage()
        {
            return string.Format("{0} {1}",
                Id,
                Message);
        }

        public object ObjOne
        {
            get
            {
                if(Objs!= null)
                {
                    if (Objs.Length >= 1)
                        return Objs[0];
                }

                return null;
            }
        }

        public object ObjTwo
        {
            get
            {
                if (Objs != null)
                {
                    if (Objs.Length >= 2)
                        return Objs[1];
                }

                return null;
            }
        }
    }

    public static class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        public static Object Copy(this Object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
        }
        private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }

            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
        public static T Copy<T>(this T original)
        {
            return (T)Copy((Object)original);
        }
    }

    public class ReferenceEqualityComparer : EqualityComparer<Object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }
        public override int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }

    namespace ArrayExtensions
    {
        public static class ArrayExtensions
        {
            public static void ForEach(this Array array, Action<Array, int[]> action)
            {
                if (array.LongLength == 0) return;
                ArrayTraverse walker = new ArrayTraverse(array);
                do action(array, walker.Position);
                while (walker.Step());
            }
        }

        internal class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
