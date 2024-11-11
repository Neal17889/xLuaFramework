using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FileUtil
{
    /// <summary>
    /// 检测文件是否存在
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsExists(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        return fileInfo.Exists;
    }

    /// <summary>
    /// 写入文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    public static void WriteFile(string path, byte[] data)
    {
        path = PathUtil.GetStandardPath(path);
        //文件夹的路径
        string dir = path.Substring(0, path.LastIndexOf('/'));
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Exists )
        {
            fileInfo.Delete();
        }
        try
        {
            //using 会在块结束时自动调用 Dispose，确保资源被正确释放。即使发生异常，Dispose 也会被调用，避免资源泄漏。
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
                //所以fs.Close();是冗余的
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }
    }
}
