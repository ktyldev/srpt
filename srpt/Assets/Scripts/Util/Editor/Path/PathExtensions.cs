using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Ktyl.Util
{
    public static class PathExtensions
    {
        public static string MustExist(this string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }
        
        public static void ClearDirectory(this string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.LogWarning($"Tried to clear non-existent directory {path}");
                return;
            }

            var items = Directory.EnumerateFileSystemEntries(path);

            using (IEnumerator<string> iter = items.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    var current = iter.Current;
                    var attributes = File.GetAttributes(current);

                    if (attributes.HasFlag(FileAttributes.Directory))
                    {
                        Directory.Delete(current, true);
                    }
                    else
                    {
                        File.Delete(current);
                    }
                }
            }
        }
    }
}
